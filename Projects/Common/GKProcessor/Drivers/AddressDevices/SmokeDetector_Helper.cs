﻿using System;
using XFiresecAPI;

namespace GKProcessor
{
	public class SmokeDetectorHelper
	{
		public static XDriver Create()
		{
			var driver = new XDriver()
			{
				DriverTypeNo = 0x61,
				DriverType = XDriverType.SmokeDetector,
				UID = new Guid("1e045ad6-66f9-4f0b-901c-68c46c89e8da"),
				Name = "Пожарный дымовой извещатель ИП 212-64",
				ShortName = "ИП-64",
				HasZone = true,
                IsPlaceable = true
			};
			GKDriversHelper.AddAvailableStateBits(driver, XStateBit.Test);
			GKDriversHelper.AddAvailableStateBits(driver, XStateBit.Fire1);
            GKDriversHelper.AddAvailableStateClasses(driver, XStateClass.Test);
            GKDriversHelper.AddAvailableStateClasses(driver, XStateClass.Fire1);
            GKDriversHelper.AddAvailableStateClasses(driver, XStateClass.Service);

			GKDriversHelper.AddIntProprety(driver, 0x84, "Порог срабатывания по дыму, 0.01*дБ/м", 18, 5, 20);

			driver.AUParameters.Add(new XAUParameter() { No = 0x82, Name = "Задымленность, 0.001*дБ/м", InternalName = "Smokiness" });
			driver.AUParameters.Add(new XAUParameter() { No = 0x91, Name = "Запыленность, 0.001*дБ/м", InternalName = "Dustinness" });
			driver.AUParameters.Add(new XAUParameter() { No = 0x93, Name = "Дата последнего обслуживания", InternalName = "LastServiceTime" });

			return driver;
		}
	}
}