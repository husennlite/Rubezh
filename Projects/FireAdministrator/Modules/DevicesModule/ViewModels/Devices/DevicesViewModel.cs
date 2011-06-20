﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common;
using FiresecClient;
using System.Collections.ObjectModel;
using System.Xml.Serialization;
using System.IO;

namespace DevicesModule.ViewModels
{
    public class DevicesViewModel : RegionViewModel
    {
        public void Initialize()
        {
            BuildTree();
            CollapseChild(Devices[0]);
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
                OnPropertyChanged("SelectedDevice");
            }
        }

        void BuildTree()
        {
            Devices = new ObservableCollection<DeviceViewModel>();

            Device device = FiresecManager.Configuration.RootDevice;

            DeviceViewModel deviceViewModel = new DeviceViewModel();
            deviceViewModel.Parent = null;
            deviceViewModel.Initialize(device, Devices);
            Devices.Add(deviceViewModel);
            AddDevice(device, deviceViewModel);
        }

        void AddDevice(Device parentDevice, DeviceViewModel parentDeviceViewModel)
        {
            foreach (var device in parentDevice.Children)
            {
                DeviceViewModel deviceViewModel = new DeviceViewModel();
                deviceViewModel.Parent = parentDeviceViewModel;
                deviceViewModel.Initialize(device, Devices);
                parentDeviceViewModel.Children.Add(deviceViewModel);
                Devices.Add(deviceViewModel);
                AddDevice(device, deviceViewModel);
            }
        }

        void CollapseChild(DeviceViewModel parentDeviceViewModel)
        {
            parentDeviceViewModel.IsExpanded = true;
            foreach (var deviceViewModel in parentDeviceViewModel.Children)
            {
                deviceViewModel.IsExpanded = true;
                CollapseChild(deviceViewModel);
            }
        }

        public override void Dispose()
        {
        }
    }
}
