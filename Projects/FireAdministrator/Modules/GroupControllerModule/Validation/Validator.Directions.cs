﻿using System.Collections.Generic;
using FiresecClient;
using Infrastructure.Common.Validation;
using XFiresecAPI;

namespace GKModule.Validation
{
	public static partial class Validator
	{
		static void ValidateDirections()
		{
			ValidateDirectionNoEquality();

			foreach (var direction in XManager.DeviceConfiguration.Directions)
			{
				if (IsManyGK())
					ValidateDifferentGK(direction);
				if (MustValidate("В направлении отсутствуют входные устройства или зоны"))
					ValidateDirectionInputCount(direction);
				if (MustValidate("В направлении отсутствуют выходные устройства"))
					ValidateDirectionOutputCount(direction);
			}
		}

		static void ValidateDirectionNoEquality()
		{
			var directionNos = new HashSet<int>();
			foreach (var direction in XManager.DeviceConfiguration.Directions)
			{
				if (!directionNos.Add(direction.No))
					Errors.Add(new DirectionValidationError(direction, "Дублиреутся номер", ValidationErrorLevel.CannotWrite));
			}
		}

		static void ValidateDifferentGK(XDirection direction)
		{
			if (AreDevicesInSameGK(direction.InputDevices))
				Errors.Add(new DirectionValidationError(direction, "Направление содержит входные устройства разных ГК", ValidationErrorLevel.CannotWrite));

			if (AreDevicesInSameGK(direction.OutputDevices))
				Errors.Add(new DirectionValidationError(direction, "Направление содержит выходные устройств разных ГК", ValidationErrorLevel.CannotWrite));

			var devicesInZones = new List<XDevice>();
			foreach (var zone in direction.InputZones)
			{
				devicesInZones.AddRange(zone.Devices);
			}

			if (AreDevicesInSameGK(devicesInZones))
				Errors.Add(new DirectionValidationError(direction, "Направление содержит выходные зоны разных ГК", ValidationErrorLevel.CannotWrite));
		}

		static void ValidateDirectionInputCount(XDirection direction)
		{
			if (direction.InputDevices.Count + direction.InputZones.Count == 0)
				Errors.Add(new DirectionValidationError(direction, "В направлении отсутствуют входные устройства или зоны", ValidationErrorLevel.CannotWrite));
		}

		static void ValidateDirectionOutputCount(XDirection direction)
		{
			var pumpStationCount = 0;
			foreach (var device in XManager.DeviceConfiguration.Devices)
			{
				if (device.Driver.DriverType == XDriverType.PumpStation)
				{
					if (device.PumpStationProperty.DirectionUIDs.Contains(direction.UID))
						pumpStationCount++;
				}
			}
			if (direction.OutputDevices.Count == 0 && pumpStationCount == 0)
				Errors.Add(new DirectionValidationError(direction, "В направлении отсутствуют выходные устройства", ValidationErrorLevel.CannotWrite));
		}
	}
}