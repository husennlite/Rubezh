﻿using Firesec.Imitator;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;
using FiresecClient;
using System;
using FiresecAPI;
using System.Diagnostics;
using System.Text;
using FiresecAPI.Models;
using System.Threading;
using System.Collections.Generic;
using Firesec;
using Infrastructure;
using Infrastructure.Events;

namespace DiagnosticsModule.ViewModels
{
    public class DiagnosticsViewModel : ViewPartViewModel
    {
        public DiagnosticsViewModel()
        {
            Test1Command = new RelayCommand(OnTest1);
            Test2Command = new RelayCommand(OnTest2);
            Test3Command = new RelayCommand(OnTest3);
            Test4Command = new RelayCommand(OnTest4);
            Test5Command = new RelayCommand(OnTest5);
            Test6Command = new RelayCommand(OnTest6);
            Test7Command = new RelayCommand(OnTest7);
        }

        string _text;
        public string Text
        {
            get { return _text; }
            set
            {
                _text = value;
                OnPropertyChanged("Text");
            }
        }

        public RelayCommand Test1Command { get; private set; }
        void OnTest1()
        {
            var thread = new Thread(new ThreadStart(() =>
            {
                while (true)
                {
					if (NativeFiresecClient.TasksCount > 10)
						continue;
					Thread.Sleep(TimeSpan.FromMilliseconds(1000));

                    var zoneIndex = random.Next(0, FiresecManager.Zones.Count - 1);
                    var zone = FiresecManager.Zones[zoneIndex];
                    var devices = new List<Device>();
                    foreach (var device in zone.DevicesInZone)
                    {
                        if (device.Driver.CanDisable)
                        {
                            devices.Add(device);
                        }
                    }
                    FiresecManager.FiresecDriver.RemoveFromIgnoreList(devices);
                }
            }));
            thread.IsBackground = true;
            thread.Start();
        }

        public RelayCommand Test2Command { get; private set; }
        void OnTest2()
        {
            var thread = new Thread(new ThreadStart(() =>
            {
                while (true)
                {
					if (NativeFiresecClient.TasksCount > 10)
						continue;
                    Thread.Sleep(TimeSpan.FromMilliseconds(1000));

                    var zoneIndex = random.Next(0, FiresecManager.Zones.Count - 1);
                    var zone = FiresecManager.Zones[zoneIndex];
                    var devices = new List<Device>();
                    foreach (var device in zone.DevicesInZone)
                    {
                        if (device.Driver.CanDisable)
                        {
                            devices.Add(device);
                        }
                    }
                    FiresecManager.FiresecDriver.AddToIgnoreList(devices);
                }
            }));
            thread.IsBackground = true;
            thread.Start();
        }

		Random random = new Random(1);

        public RelayCommand Test3Command { get; private set; }
        void OnTest3()
        {
            var thread = new Thread(new ThreadStart(() =>
            {
                while (true)
                {
                    if (NativeFiresecClient.TasksCount > 10)
                        continue;
                    Thread.Sleep(TimeSpan.FromMilliseconds(1000));

                    FiresecManager.ResetAllStates();
                }
            }));
            thread.IsBackground = true;
            thread.Start();
        }

        public RelayCommand Test4Command { get; private set; }
        void OnTest4()
        {
            throw new Exception("Unknown exception");
        }

        public RelayCommand Test5Command { get; private set; }
        void OnTest5()
        {
            var deviceControls = new List<DeviceControl>();
            foreach (var device in FiresecManager.Devices)
            {
                foreach (var property in device.Driver.Properties)
                {
                    if (property.IsControl)
                    {
                        var deviceControl = new DeviceControl()
                        {
                            Device = device,
                            DriverProperty = property
                        };
                        deviceControls.Add(deviceControl);
                    }
                }
            }

            var thread = new Thread(new ThreadStart(() =>
            {
                while (true)
                {
                    if (NativeFiresecClient.TasksCount > 10)
                        continue;
                    Thread.Sleep(TimeSpan.FromMilliseconds(1000));

                    var deviceControlIndex = random.Next(0, deviceControls.Count - 1);
                    var deviceControl = deviceControls[deviceControlIndex];
                    FiresecManager.FiresecDriver.ExecuteCommand(deviceControl.Device, deviceControl.DriverProperty.Name);
                }
            }));
            thread.IsBackground = true;
            thread.Start();
        }

        bool IsGuardZoneInverse;

        public RelayCommand Test6Command { get; private set; }
        void OnTest6()
        {
            var guardZones = new List<Zone>();
            foreach (var zone in FiresecManager.Zones)
            {
                if (zone.ZoneType == ZoneType.Guard)
                {
                    guardZones.Add(zone);
                }
            }

            var thread = new Thread(new ThreadStart(() =>
            {
                while (true)
                {
                    if (NativeFiresecClient.TasksCount > 10)
                        continue;
                    Thread.Sleep(TimeSpan.FromMilliseconds(1000));

                    var zoneIndex = random.Next(0, guardZones.Count - 1);
                    var zone = guardZones[zoneIndex];

                    IsGuardZoneInverse = !IsGuardZoneInverse;
                    if(IsGuardZoneInverse)
                        FiresecManager.SetZoneGuard(zone);
                    else
                        FiresecManager.UnSetZoneGuard(zone);
                }
            }));
            thread.IsBackground = true;
            thread.Start();
        }

        public RelayCommand Test7Command { get; private set; }
        void OnTest7()
        {
            var thread = new Thread(new ThreadStart(() =>
            {
                while (true)
                {
                    Thread.Sleep(TimeSpan.FromMilliseconds(5000));

                    var eventIndex = random.Next(0, 10);
                    switch(eventIndex)
                    {
                        case 0:
                            ServiceFactory.Events.GetEvent<ShowAlarmsEvent>().Publish(null);
                            break;
                        case 1:
                            ServiceFactory.Events.GetEvent<ShowArchiveEvent>().Publish(null);
                            break;
                        case 2:
                            ServiceFactory.Events.GetEvent<ShowDeviceEvent>().Publish(Guid.Empty);
                            break;
                        case 3:
                            ServiceFactory.Events.GetEvent<ShowDiagnosticsEvent>().Publish(null);
                            break;
                        case 4:
                            ServiceFactory.Events.GetEvent<ShowGKEvent>().Publish(null);
                            break;
                        case 5:
                            ServiceFactory.Events.GetEvent<ShowJournalEvent>().Publish(null);
                            break;
                        case 6:
                            ServiceFactory.Events.GetEvent<ShowNothingEvent>().Publish(null);
                            break;
                        case 7:
                            ServiceFactory.Events.GetEvent<ShowPlansEvent>().Publish(null);
                            break;
                        case 8:
                            ServiceFactory.Events.GetEvent<ShowReportsEvent>().Publish(null);
                            break;
                        case 9:
                            ServiceFactory.Events.GetEvent<ShowZoneEvent>().Publish(Guid.Empty);
                            break;
                    }
                }
            }));
            thread.IsBackground = true;
            thread.Start();
        }
    }

    internal class DeviceControl
    {
        public Device Device { get; set; }
        public DriverProperty DriverProperty { get; set; }
    }
}