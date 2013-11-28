﻿using System.IO;
using System.Linq;
using System.Text;
using FiresecAPI;
using Ionic.Zip;
using XFiresecAPI;


namespace Infrastructure.Common
{
	public static class ZipFileConfigurationHelper
	{
		public static void SaveToZipFile(string fileName, XDeviceConfiguration deviceConfiguration)
		{
			var folderName = AppDataFolderHelper.GetTempFolder();
			if (!Directory.Exists(folderName))
				Directory.CreateDirectory(folderName);

			if (File.Exists(fileName))
				File.Delete(fileName);

			deviceConfiguration.BeforeSave();
			deviceConfiguration.Version = new ConfigurationVersion() { MinorVersion = 1, MajorVersion = 1 };
			ZipSerializeHelper.Serialize(deviceConfiguration, Path.Combine(folderName, "XDeviceConfiguration.xml"));
			var zipFile = new ZipFile(fileName);
			zipFile.AddDirectory(folderName);
			zipFile.Save(fileName);
			zipFile.Dispose();
			if (Directory.Exists(folderName))
				Directory.Delete(folderName, true);
		}

		public static XDeviceConfiguration LoadFromZipFile(MemoryStream memoryStream)
		{
			var deviceConfiguration = new XDeviceConfiguration();
			var zipFile = ZipFile.Read(memoryStream);
			var dataMemory = new MemoryStream();
			var firstOrDefault = zipFile.FirstOrDefault();
			if (firstOrDefault != null) firstOrDefault.Extract(dataMemory);
			dataMemory.Position = 0;
			if (dataMemory.Length != 0)
			{ deviceConfiguration = ZipSerializeHelper.DeSerialize<XDeviceConfiguration>(dataMemory); }
			memoryStream.Close(); 
			dataMemory.Close(); 
			zipFile.Dispose();
			return deviceConfiguration;
		}
	}
}
