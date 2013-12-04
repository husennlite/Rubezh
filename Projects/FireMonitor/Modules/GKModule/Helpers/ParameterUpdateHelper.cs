﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XFiresecAPI;
using GKProcessor;
using System.Threading;
using GKModule.ViewModels;

namespace GKModule
{
	public static class ParameterUpdateHelper
	{
		public static event Action<AUParameterValue> NewAUParameterValue;
		static void OnNewAUParameterValue(AUParameterValue auParameterValue)
		{
			if (NewAUParameterValue != null)
				NewAUParameterValue(auParameterValue);
		}

		public static void UpdateDevice(XDevice device)
		{
			if (device.KauDatabaseParent != null && device.KauDatabaseParent.DriverType == XDriverType.KAU)
			{
				foreach (var auParameter in device.Driver.AUParameters)
				{
					var bytes = new List<byte>();
					bytes.Add((byte)device.Driver.DriverTypeNo);
					bytes.Add(device.IntAddress);
					bytes.Add((byte)(device.ShleifNo - 1));
					bytes.Add(auParameter.No);
					var result = SendManager.Send(device.KauDatabaseParent, 4, 131, 2, bytes);
					if (!result.HasError)
					{
						for (int i = 0; i < 1000; i++)
						{
							var no = device.GKDescriptorNo;
							result = SendManager.Send(device.GkDatabaseParent, 2, 12, 68, BytesHelper.ShortToBytes(no));
							if (result.Bytes.Count > 0)
							{
								var resievedParameterNo = result.Bytes[63];
								if (resievedParameterNo == auParameter.No)
								{
									var ushortValue = BytesHelper.SubstructShort(result.Bytes, 64);
									if (auParameter.IsHighByte)
									{
										ushortValue = (ushort)(ushortValue / 256);
									}
									else if (auParameter.IsLowByte)
									{
										ushortValue = (ushort)(ushortValue << 8);
										ushortValue = (ushort)(ushortValue >> 8);
									}
									var parameterValue = (double)ushortValue;
									if (auParameter.Multiplier != null)
									{
										parameterValue /= (double)auParameter.Multiplier;
									}
									var	stringValue = ushortValue.ToString();
									if (auParameter.Name == "Дата последнего обслуживания")
									{
										stringValue = (ushortValue / 256).ToString() + "." + (ushortValue % 256).ToString();
									}
									if ((device.DriverType == XDriverType.Valve || device.DriverType == XDriverType.Pump)
										&& auParameter.Name == "Режим работы")
									{
										stringValue = "Неизвестно";
										switch (ushortValue & 3)
										{
											case 0:
												stringValue = "Автоматический";
												break;

											case 1:
												stringValue = "Ручной";
												break;

											case 2:
												stringValue = "Отключено";
												break;
										}
									}
									var auParameterValue = new AUParameterValue()
									{
										Device = device,
										DriverParameter = auParameter,
										Name = auParameter.Name,
										Value = parameterValue,
										StringValue = stringValue
									};
									OnNewAUParameterValue(auParameterValue);

									break;
								}
								Thread.Sleep(100);
							}
						}
					}
				}
			}
			else if (device.KauDatabaseParent != null && device.KauDatabaseParent.DriverType == XDriverType.RSR2_KAU)
			{
				var no = device.GKDescriptorNo;
				var result = SendManager.Send(device.GkDatabaseParent, 2, 12, 68, BytesHelper.ShortToBytes(no));
				if (!result.HasError)
				{
					if (result.Bytes.Count > 0)
					{
						for (int i = 0; i < device.Driver.AUParameters.Count; i++)
						{
							var auParameter = device.Driver.AUParameters[i];
							var parameterValue = BytesHelper.SubstructShort(result.Bytes, 48 + i * 2);
							var stringValue = parameterValue.ToString();
							if (auParameter.Name == "Дата последнего обслуживания")
							{
								stringValue = (parameterValue / 256).ToString() + "." + (parameterValue % 256).ToString();
							}

							var auParameterValue = new AUParameterValue()
							{
								Device = device,
								DriverParameter = auParameter,
								Name = auParameter.Name,
								Value = parameterValue,
								StringValue = stringValue
							};
							OnNewAUParameterValue(auParameterValue);
						}
					}
				}
			}
		}
	}
}