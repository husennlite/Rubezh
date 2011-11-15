﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common;

namespace DevicesModule.ViewModels
{
    public class DevicesViewModel : RegionViewModel
    {
        public DevicesViewModel()
        {
            FiresecEventSubscriber.DeviceStateChangedEvent += OnDeviceStateChanged;

            BuildDeviceTree();
            if (Devices.Count > 0)
            {
                CollapseChild(Devices[0]);
                ExpandChild(Devices[0]);
                SelectedDevice = Devices[0];
            }

            FiresecEventSubscriber.DeviceStateChangedEvent += new Action<Guid>(OnDeviceStateChangedEvent);
        }

        ObservableCollection<DeviceViewModel> _devices;
        public ObservableCollection<DeviceViewModel> Devices
        {
            get { return _devices; }
            set
            {
                _devices = value;
                OnPropertyChanged("Devices");
            }
        }

        DeviceViewModel _selectedDevice;
        public DeviceViewModel SelectedDevice
        {
            get { return _selectedDevice; }
            set
            {
                _selectedDevice = value;
                if (value != null)
                    value.ExpantToThis();
                OnPropertyChanged("SelectedDevice");
            }
        }

        void CollapseChild(DeviceViewModel parentDeviceViewModel)
        {
            parentDeviceViewModel.IsExpanded = false;
            foreach (var deviceViewModel in parentDeviceViewModel.Children)
            {
                CollapseChild(deviceViewModel);
            }
        }

        void ExpandChild(DeviceViewModel parentDeviceViewModel)
        {
            if (parentDeviceViewModel.Device.Driver.Category != DeviceCategoryType.Device)
            {
                parentDeviceViewModel.IsExpanded = true;
                foreach (var deviceViewModel in parentDeviceViewModel.Children)
                {
                    ExpandChild(deviceViewModel);
                }
            }
        }

        void OnDeviceStateChanged(Guid deviceUID)
        {
            var deviceViewModel = AllDevices.FirstOrDefault(x => x.Device.UID == deviceUID);
            if (deviceViewModel != null)
                deviceViewModel.Update();

            if (deviceViewModel.Device.Driver.DriverType == DriverType.Valve)
            {
                var deviceDriverState = deviceViewModel.DeviceState.States.FirstOrDefault(x => x.Code == "Bolt_On");
                if (deviceDriverState != null)
                {
                    if (DateTime.Now > deviceDriverState.Time)
                    {
                        var timeSpan = deviceDriverState.Time - DateTime.Now;

                        var timeoutProperty = deviceViewModel.Device.Properties.FirstOrDefault(x => x.Name == "Timeout");
                        if (timeoutProperty != null)
                        {
                            var timeout = int.Parse(timeoutProperty.Value);

                            int secondsLeft = timeout - timeSpan.Value.Seconds;
                            if (secondsLeft > 0)
                            {
                                var deviceDetailsViewModel = new DeviceDetailsViewModel(deviceUID);
                                ServiceFactory.UserDialogs.ShowWindow(deviceDetailsViewModel);
                                deviceDetailsViewModel.StartValveTimer(secondsLeft);
                            }
                        }
                    }
                }
            }
        }

        void OnDeviceStateChangedEvent(Guid deviceUID)
        {
            DeviceViewModel deviceViewModel = Devices.FirstOrDefault(x => x.Device.UID == deviceUID);
            if (deviceViewModel == null)
                return;
            deviceViewModel.UpdateParameters();
        }

        void BuildDeviceTree()
        {
            Devices = new ObservableCollection<DeviceViewModel>();
            AllDevices = new List<DeviceViewModel>();
            AddDevice(FiresecManager.DeviceConfiguration.RootDevice, null);
        }

        DeviceViewModel AddDevice(Device device, DeviceViewModel parentDeviceViewModel)
        {
            var deviceViewModel = new DeviceViewModel(device, Devices) { Parent = parentDeviceViewModel };
            AllDevices.Add(deviceViewModel);

            Devices.Insert(Devices.IndexOf(parentDeviceViewModel) + 1, deviceViewModel);
            foreach (var childDevice in device.Children)
            {
                deviceViewModel.Children.Add(AddDevice(childDevice, deviceViewModel));
            }

            return deviceViewModel;
        }

        #region DeviceSelection

        public List<DeviceViewModel> AllDevices;

        public void Select(Guid deviceUID)
        {
            var deviceViewModel = AllDevices.FirstOrDefault(x => x.Device.UID == deviceUID);
            if (deviceViewModel != null)
            {
                deviceViewModel.ExpantToThis();
                SelectedDevice = deviceViewModel;
            }
        }

        #endregion
    }
}