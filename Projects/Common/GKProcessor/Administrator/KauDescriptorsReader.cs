﻿using System;
using System.Collections.Generic;
using System.Linq;
using FiresecClient;
using Infrastructure.Common.Windows;
using XFiresecAPI;

namespace GKProcessor
{
	public class KauDescriptorsReaderBase : DescriptorReaderBase
	{
		static XDevice KauDevice { get; set; }
		List<int> descriptorAddresses;

		override public bool ReadConfiguration(XDevice kauDevice)
		{
			KauDevice = (XDevice) kauDevice.Clone();
			KauDevice.Children = new List<XDevice>();
			descriptorAddresses = new List<int>();
			for (int i = 0; i < 8; i++)
			{
				var shleif = new XDevice();
				shleif.Driver = KauDevice.DriverType == XDriverType.KAU ? XManager.Drivers.FirstOrDefault(x => x.DriverType == XDriverType.KAU_Shleif) : XManager.Drivers.FirstOrDefault(x => x.DriverType == XDriverType.RSR2_KAU_Shleif);
				shleif.DriverUID = shleif.Driver.UID;
				shleif.IntAddress = (byte)(i + 1);
				KauDevice.Children.Add(shleif);
			}
			DeviceConfiguration = new XDeviceConfiguration { RootDevice = KauDevice };
			LoadingService.Show("Чтение конфигурации", "Перевод КАУ в технологический режим");
			DeviceBytesHelper.GoToTechnologicalRegime(kauDevice);
			LoadingService.DoStep("Получение дескрипторов устройств");
			if (GetDescriptorAddresses(kauDevice))
			{
				LoadingService.Show("Чтение конфигурации " + kauDevice.PresentationDriverAndAddress, "", descriptorAddresses.Count + 1, true);
				for (int i = 1; i < descriptorAddresses.Count; i++)
				{
					if (LoadingService.IsCanceled)
					{
						Error = "Операция отменена";
						break;
					}
					LoadingService.SaveDoStep("Чтение базы данных объектов. " + i + " из " + descriptorAddresses.Count);
					if (!GetDescriptorInfo(kauDevice, descriptorAddresses[i]))
						break;
				}
			}
			LoadingService.SaveDoStep("Перевод КАУ в рабочий режим");
			DeviceBytesHelper.GoToWorkingRegime(kauDevice);
			DeviceConfiguration.Update();
			LoadingService.SaveClose();
			return String.IsNullOrEmpty(Error);
		}

		bool GetDescriptorInfo(XDevice kauDevice, int descriptorAdderss)
		{
			var descriptorAdderssesBytes = new List<byte>(BitConverter.GetBytes(descriptorAdderss));
			var data = new List<byte>(descriptorAdderssesBytes);
			var sendResult = SendManager.Send(kauDevice, 4, 31, 256, data);
			var bytes = sendResult.Bytes;
			if (bytes.Count != 256)
			{
				Error = "Длина дескриптора не соответствует нужному значению";
				return false;
			}
			var deviceType = BytesHelper.SubstructShort(bytes, 0);
			var address = BytesHelper.SubstructShort(bytes, 2);
			int shleifNo = (byte)(address / 256 + 1);
			var device = new XDevice();
			device.Driver = XManager.Drivers.FirstOrDefault(x => x.DriverTypeNo == deviceType);
			if ((1 <= shleifNo && shleifNo <= 8) && (address != 0))
			{
				device.DriverUID = device.Driver.UID;
				var shleif = KauDevice.Children.FirstOrDefault(x => (x.DriverType == XDriverType.KAU_Shleif || x.DriverType == XDriverType.RSR2_KAU_Shleif) && x.IntAddress == shleifNo);
				shleif.Children.Add(device);
				device.IntAddress = (byte)(address % 256);
				return true;
			}
			device.Driver = XManager.Drivers.FirstOrDefault(x => x.DriverType == XDriverType.KAUIndicator);
			device.DriverUID = device.Driver.UID;
			device.IntAddress = 1;
			KauDevice.Children.Add(device);
			return true;
		}

		bool GetDescriptorAddresses(XDevice device)
		{
			descriptorAddresses = new List<int>();
			var startaddress = 0x078000;
			while (true)
			{
				byte[] startAddressBytes = BitConverter.GetBytes(startaddress);
				startaddress += 256;

				var data = new List<byte>(startAddressBytes);
				var sendResult = SendManager.Send(device, 4, 31, 256, data);
				if (sendResult.Bytes.Count != 256)
				{
					Error = "Не удалось распознать дескриптор";
					return false;
				}
				for (int i = 0; i < 256 / 4; i++)
				{
					var descriptorAddress = BytesHelper.SubstructInt(sendResult.Bytes, i * 4);
					if (descriptorAddress == -1)
					{
						return true;
					}
					descriptorAddresses.Add(descriptorAddress);
				}
			}
		}
	}
}