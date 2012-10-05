﻿
namespace FiresecAPI.Models
{
	public static class ConfigurationCash
	{
		public static DriversConfiguration DriversConfiguration { get; set; }
		public static DeviceConfiguration DeviceConfiguration { get; set; }
		public static PlansConfiguration PlansConfiguration { get; set; }
		static ConfigurationCash()
		{
			DriversConfiguration = new DriversConfiguration();
		}
	}
}