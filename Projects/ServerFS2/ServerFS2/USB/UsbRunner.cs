﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using FiresecAPI;
using LibUsbDotNet;
using LibUsbDotNet.Main;
using FS2Api;
using System.Diagnostics;
using ServerFS2.ConfigurationWriter;

namespace ServerFS2
{
	public class UsbRunner : UsbRunnerBase
	{
		UsbDevice UsbDevice;
		UsbEndpointReader Reader;
		UsbEndpointWriter Writer;

		public override bool Open()
		{
			var usbFinder = new UsbDeviceFinder(0xC251, 0x1303);
			UsbDevice = UsbDevice.OpenUsbDevice(usbFinder);
			if (UsbDevice == null)
			{
				throw new Exception("Device Not Found.");
			}

			var wholeUsbDevice = UsbDevice as IUsbDevice;
			if (wholeUsbDevice != null)
			{
				wholeUsbDevice.SetConfiguration(1);
				wholeUsbDevice.ClaimInterface(0);
			}
			Reader = UsbDevice.OpenEndpointReader(ReadEndpointID.Ep01);
			Reader.DataReceived += (OnDataRecieved);
			Reader.ReadBufferSize = 64;
			Reader.DataReceivedEnabled = true;
			Writer = UsbDevice.OpenEndpointWriter(WriteEndpointID.Ep01);
			return true;
		}

		public override void Close()
		{
			if (Reader != null)
			{
				Reader.DataReceivedEnabled = false;
				Reader.DataReceived -= (OnDataRecieved);
			}

			if (UsbDevice != null)
			{
				if (UsbDevice.IsOpen)
				{
					var wholeUsbDevice = UsbDevice as IUsbDevice;
					if (!ReferenceEquals(wholeUsbDevice, null))
					{
						wholeUsbDevice.ReleaseInterface(0);
					}
					UsbDevice.Close();
				}
				UsbDevice = null;
				UsbDevice.Exit();
			}
		}

		public override bool Send(List<byte> data)
		{
			Trace.WriteLine(BytesHelper.BytesToString(data));
			int bytesWrite;
			if (Writer == null)
				throw new FS2Exception("Драйвер USB отсутствует");
			Writer.Write(data.ToArray(), 2000, out bytesWrite);
			return bytesWrite == data.Count;
		}

		void OnDataRecieved(object sender, EndpointDataEventArgs e)
		{
			var buffer = e.Buffer.ToList();
			if (buffer.Count < 2)
				return;
			IsMs = buffer[0] != 0;
		    buffer.RemoveRange(0, ServerHelper.IsExtendedMode ? 2 : 1);
			foreach (var b in buffer)
			{
				if (LocalResult.Count > 0)
				{
					LocalResult.Add(b);
					if (b == 0x3E)
					{
						var bytes = CreateInputBytes(LocalResult);
						LocalResult = new List<byte>();

						var response = new Response
						{
							Data = bytes.ToList()
						};
						if (!IsUsbDevice)
						{
							response.Id = BytesHelper.ExtractInt(bytes.ToList(), 0);
						}
						OnResponseRecieved(response);
						AutoResetEvent.Set();
						OnNewResponse(response);
						return;
					}
				}
				if (b == 0x7E)
				{
					if (!ServerHelper.IsExtendedMode)
					{
						if (buffer.IndexOf(0x7e) == 0)
							ServerHelper.IsExtendedMode = false;
						if (buffer.IndexOf(0x7e) == 1)
							ServerHelper.IsExtendedMode = true;
					}
				    LocalResult = new List<byte> { b };
				}
				if (RequestCollection.Count() == 0)
					AautoWaitEvent.Set();
			}
		}

		List<byte> CreateOutputBytes(IEnumerable<byte> messageBytes)
		{
			var bytes = new List<byte>(0) { 0x7e };
			foreach (var b in messageBytes)
			{
				if (b == 0x7E)
				{ bytes.Add(0x7D); bytes.Add(0x5E); continue; }
				if (b == 0x7D)
				{ bytes.Add(0x7D); bytes.Add(0x5D); continue; }
				if (b == 0x3E)
				{ bytes.Add(0x3D); bytes.Add(0x1E); continue; }
				if (b == 0x3D)
				{ bytes.Add(0x3D); bytes.Add(0x1D); continue; }
				bytes.Add(b);
			}
			bytes.Add(0x3e);

			while (bytes.Count % 64 > 0)
			{
				bytes.Add(0);
			}
			return bytes;
		}

		List<byte> CreateInputBytes(List<byte> messageBytes)
		{
			var bytes = new List<byte>();
			var previousByte = new byte();
			messageBytes.RemoveRange(0, messageBytes.IndexOf(0x7E) + 1);
			messageBytes.RemoveRange(messageBytes.IndexOf(0x3E), messageBytes.Count - messageBytes.IndexOf(0x3E));
			foreach (var b in messageBytes)
			{
				if ((b == 0x7D) || (b == 0x3D))
				{ previousByte = b; continue; }
				if (previousByte == 0x7D)
				{
					previousByte = new byte();
					if (b == 0x5E)
					{ bytes.Add(0x7E); continue; }
					if (b == 0x5D)
					{ bytes.Add(0x7D); continue; }
				}
				if (previousByte == 0x3D)
				{
					previousByte = new byte();
					if (b == 0x1E)
					{ bytes.Add(0x3E); continue; }
					if (b == 0x1D)
					{ bytes.Add(0x3D); continue; }
				}
				bytes.Add(b);
			}
			return bytes;
		}

		void OnResponseRecieved(Response response)
		{
			Request request;
			if (IsUsbDevice)
			{
				request = RequestCollection.GetFirst();
				if (request != null)
				{
					Responses.Clear();
					Responses.Add(response);
					if (Responses.Count != 0)
						RequestCollection.Clear();
				}
			}
			else
			{
				//Trace.WriteLine("response.Id = " + response.Id);
				request = RequestCollection.GetById(response.Id);
				RequestCollection.RemoveById(response.Id);
				if (request != null)
				{
					Responses.Add(response);
				}
			}
		}

		public override OperationResult<List<Response>> AddRequest(int usbRequestNo, List<List<byte>> bytesList, int delay, int timeout, bool isSyncronuos)
		{
			Responses = new List<Response>();
			RequestCollection.Clear();
			foreach (var bytes in bytesList)
			{
				if (bytesList.Count > 1)
				{
					usbRequestNo++;
				}
				_stop = false;
				var request = new Request();
				if (!IsUsbDevice)
				{
					request.Id = usbRequestNo;
					if (usbRequestNo != -1)
					{
						bytes.InsertRange(0, BitConverter.GetBytes(usbRequestNo).Reverse());
					}
				}
				request.Bytes = CreateOutputBytes(bytes);
				RequestCollection.AddRequest(request);
				if (request.Bytes.Count > 64)
				{
					for (int i = 0; i < request.Bytes.Count / 64; i++)
						Send(request.Bytes.GetRange(i * 64, 64));
				}
				else
				{
					//Trace.WriteLine("request.Id = " + request.Id);
					Send(request.Bytes);
				}
				if (isSyncronuos)
				{
					AutoResetEvent.WaitOne(delay);
				}
			}

			if (isSyncronuos)
			{
				foreach (var request in new List<Request>(RequestCollection.Requests))
				{
					for (int i = 0; i < 15; i++)
					{
						//Trace.WriteLine("request.Id = " + request.Id);
						Send(request.Bytes);
						AautoWaitEvent.WaitOne(timeout);
						if (RequestCollection.Count() == 0)
							break;
					}
				}
				return new OperationResult<List<Response>> { Result = Responses };
			}
		    return null;
		}
	}
}