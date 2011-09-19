﻿using FiresecAPI.Models;

namespace FiresecService.Converters
{
    public static class DeviceStatesConverter
    {
        public static void Convert()
        {
            FiresecManager.DeviceConfigurationStates = new DeviceConfigurationStates();

            if (FiresecManager.DeviceConfiguration.Devices != null)
            foreach (var device in FiresecManager.DeviceConfiguration.Devices)
            {
                var deviceState = new DeviceState()
                {
                    UID = device.UID,
                    PlaceInTree = device.PlaceInTree,
                    Device = device
                };

                foreach (var parameter in device.Driver.Parameters)
                {
                    deviceState.Parameters.Add(parameter.Copy());
                }

                FiresecManager.DeviceConfigurationStates.DeviceStates.Add(deviceState);
            }

            foreach (var zone in FiresecManager.DeviceConfiguration.Zones)
            {
                var zoneState = new ZoneState() { No = zone.No };
                FiresecManager.DeviceConfigurationStates.ZoneStates.Add(zoneState);
            }
        }
    }
}