﻿using System;
using System.Collections.Generic;
using System.Linq;
using FiresecAPI.Models;
using FiresecClient;
using XFiresecAPI;
using Infrastructure.Common.Windows;
using GKModule.ViewModels;

namespace GKModule.Converter
{
    public class ConfigurationConverter
    {
        public void Convert()
        {
            var convertationViewModel = new ConvertationViewModel();
            if (DialogService.ShowModalWindow(convertationViewModel))
            {
                ConvertDdevices();
                ConvertZones();
                ConvertLogic();
            }
            XManager.UpdateConfiguration();
        }

		XDevice gkDevice;
		int shleifPairNo = 0;
		byte kauAddress = 1;

        public void ConvertDdevices()
        {
			XDevice curentKauDevice = null;
            AddRootDevices();

            foreach (var panelDevice in GetPanels())
            {
                shleifPairNo++;
                if (shleifPairNo == 5)
                    shleifPairNo = 1;

                if (shleifPairNo == 1)
                {
                    curentKauDevice = XManager.AddChild(gkDevice, XManager.DriversConfiguration.Drivers.FirstOrDefault(x => x.DriverType == XDriverType.KAU), 0, kauAddress);
                    curentKauDevice.UID = panelDevice.UID;
                    XManager.DeviceConfiguration.Devices.Add(curentKauDevice);
					kauAddress++;
                }

                foreach (var childDevice in panelDevice.Children)
                {
					AddDevice(curentKauDevice, childDevice);
                }
            }
        }

		public void AddDevice(XDevice parentDevice, Device fsDevice)
		{
			var driver = XManager.DriversConfiguration.Drivers.FirstOrDefault(x => x.UID == fsDevice.DriverUID);
			if (driver == null)
			{
				return;
			}
			var shleifNo = ((shleifPairNo - 1) * 2) + (fsDevice.IntAddress >> 8);

			var xDevice = new XDevice()
			{
				UID = fsDevice.UID,
				DriverUID = driver.UID,
				Driver = driver,
				ShleifNo = (byte)shleifNo,
				IntAddress = (byte)(fsDevice.IntAddress & 0xff),
				Description = fsDevice.Description
			};
			XManager.DeviceConfiguration.Devices.Add(xDevice);
			parentDevice.Children.Add(xDevice);
			xDevice.Parent = parentDevice;

			foreach (var fsChildDevice in fsDevice.Children)
			{
				AddDevice(xDevice, fsChildDevice);
			}
		}

        public void AddRootDevices()
        {
            XManager.DeviceConfiguration = new XDeviceConfiguration();

            var systemDevice = new XDevice()
            {
                UID = Guid.NewGuid(),
				DriverUID = XDriver.System_UID,
                Driver = XManager.DriversConfiguration.Drivers.FirstOrDefault(x => x.DriverType == XDriverType.System)
            };
            XManager.DeviceConfiguration.Devices.Add(systemDevice);
            XManager.DeviceConfiguration.RootDevice = systemDevice;

            var gkDriver = XManager.DriversConfiguration.Drivers.FirstOrDefault(x => x.DriverType == XDriverType.GK);
            gkDevice = XManager.AddChild(systemDevice, gkDriver, 0, 1);
            gkDevice.UID = Guid.NewGuid();
            XManager.DeviceConfiguration.Devices.Add(gkDevice);
        }

        IEnumerable<Device> GetPanels()
        {
            foreach (var device in FiresecManager.Devices)
            {
                if (device.Driver.IsPanel)
                    yield return device;
            }
        }

        void ConvertZones()
        {
            foreach (var zone in FiresecManager.Zones)
            {
                var xZone = new XZone()
                {
					UID = zone.UID,
                    No = (ushort)zone.No,
                    Name = zone.Name,
                    Description = zone.Description,
                    Fire1Count = (ushort)zone.DetectorCount,
                };
                XManager.DeviceConfiguration.Zones.Add(xZone);
            }

            foreach (var device in FiresecManager.Devices)
            {
                var xDevice = XManager.DeviceConfiguration.Devices.FirstOrDefault(x => x.UID == device.UID);
                if (xDevice != null)
                {
                    if ((device.Driver.IsZoneDevice) && (device.ZoneUID != Guid.Empty))
                    {
                        var zone = FiresecManager.Zones.FirstOrDefault(x => x.UID == device.ZoneUID);
                        var xZone = XManager.DeviceConfiguration.Zones.FirstOrDefault(x => x.No == (ushort)zone.No);
                        if (zone != null)
                        {
                            xDevice.ZoneUIDs.Add(xZone.UID);
                        }
                    }
                }
            }
        }

        void ConvertLogic()
        {
            foreach (var xDevice in XManager.DeviceConfiguration.Devices)
            {
                var device = FiresecManager.Devices.FirstOrDefault(x => x.UID == xDevice.UID);
                if (device != null)
                {
                    if ((device.Driver.IsZoneLogicDevice) && (device.ZoneLogic != null))
                    {
                        var xDeviceLogic = new XDeviceLogic();

                        foreach (var clause in device.ZoneLogic.Clauses)
                        {
                            var xClause = new XClause()
                            {
								ClauseOperationType = ClauseOperationType.AllZones
                            };
							if(clause.Operation.HasValue)
							switch(clause.Operation.Value)
							{
								case ZoneLogicOperation.All:
									xClause.ClauseOperationType = ClauseOperationType.AllZones;
									break;

								case ZoneLogicOperation.Any:
									xClause.ClauseOperationType = ClauseOperationType.AnyZone;
									break;
							}
                            switch (clause.State)
                            {
                                case FiresecAPI.Models.ZoneLogicState.Attention:
                                    xClause.StateType = XStateType.Attention;
                                    break;

                                case FiresecAPI.Models.ZoneLogicState.Fire:
                                    xClause.StateType = XStateType.Fire1;
                                    break;

                                case FiresecAPI.Models.ZoneLogicState.Failure:
                                    xClause.StateType = XStateType.Failure;
                                    break;

                                default:
                                    continue;
                            }
                            if ((clause.ZoneUIDs == null) || (clause.ZoneUIDs.Count == 0))
                                continue;

                            foreach (var zoneUID in clause.ZoneUIDs)
                            {
                                var xZone = XManager.DeviceConfiguration.Zones.FirstOrDefault(x => x.UID == zoneUID);
                                xClause.ZoneUIDs.Add(xZone.UID);
                            }

                            xDeviceLogic.Clauses.Add(xClause);
                        }

                        if (xDeviceLogic.Clauses.Count > 0)
                            xDevice.DeviceLogic = xDeviceLogic;
                    }
                }
            }
        }
    }
}