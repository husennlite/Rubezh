﻿using System.Collections.Generic;
using System.Runtime.Serialization;
using System;
using System.Linq;

namespace FiresecAPI.Models
{
    [DataContract]
    public class DeviceConfiguration
    {
        public DeviceConfiguration()
        {
            Devices = new List<Device>();
            Zones = new List<Zone>();
            Directions = new List<Direction>();
            GuardUsers = new List<GuardUser>();
            GuardLevels = new List<GuardLevel>();
        }

        public List<Device> Devices { get; set; }

        [DataMember]
        public Device RootDevice { get; set; }

        [DataMember]
        public List<Zone> Zones { get; set; }

        [DataMember]
        public List<Direction> Directions { get; set; }

        [DataMember]
        public List<GuardUser> GuardUsers { get; set; }

        [DataMember]
        public List<GuardLevel> GuardLevels { get; set; }

        public void Update()
        {
            Devices = new List<Device>();
            if (RootDevice != null)
            {
                RootDevice.Parent = null;
                Devices.Add(RootDevice);
                AddChild(RootDevice);
            }
        }

        void AddChild(Device parentDevice)
        {
            foreach (var device in parentDevice.Children)
            {
                device.Parent = parentDevice;
                Devices.Add(device);
                AddChild(device);
            }
        }

        public DeviceConfiguration CopyOneBranch(Guid uid)
        {
            var deviceConfiguration = new DeviceConfiguration();

            var device = Devices.FirstOrDefault(x => x.UID == uid);
            Device currentDevice = device;
            Device copyChildDevice = null;

            while (true)
            {
                var copyDevice = new Device()
                {
                    UID = currentDevice.UID,
                    DriverUID = currentDevice.DriverUID,
                    IntAddress = currentDevice.IntAddress,
                    Description = currentDevice.Description,
                    ZoneNo = currentDevice.ZoneNo,
                    Properties = currentDevice.Properties
                };
                if (copyChildDevice != null)
                    copyDevice.Children.Add(copyChildDevice);

                if (currentDevice.Parent == null)
                {
                    currentDevice = copyDevice;
                    break;
                }
                copyChildDevice = copyDevice;
                currentDevice = currentDevice.Parent;
            }

            deviceConfiguration.RootDevice = currentDevice;
            return deviceConfiguration;
        }
    }
}