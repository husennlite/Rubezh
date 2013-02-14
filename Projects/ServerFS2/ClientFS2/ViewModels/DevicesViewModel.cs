﻿using FiresecAPI.Models;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;

namespace ClientFS2.ViewModels
{
    public class DevicesViewModel : DialogViewModel
    {
        public DevicesViewModel()
        {
            BuildTree();
            if (RootDevice != null)
            {
                RootDevice.IsExpanded = true;
                SelectedDevice = RootDevice;
            }
            OnPropertyChanged("RootDevices");
        }
        public DevicesViewModel(Device device)
        {
            BuildTree(device);
            if (RootDevice != null)
            {
                RootDevice.IsExpanded = true;
                SelectedDevice = RootDevice;
            }
            OnPropertyChanged("RootDevices");
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
        DeviceViewModel _rootDevice;
        public DeviceViewModel RootDevice
        {
            get { return _rootDevice; }
            private set
            {
                _rootDevice = value;
                OnPropertyChanged("RootDevice");
            }
        }
        public DeviceViewModel[] RootDevices
        {
            get { return new[] { RootDevice }; }
        }
        void BuildTree(Device device)
        {
            RootDevice = AddDeviceInternal(device, null);
        }
        void BuildTree()
        {
            RootDevice = AddDeviceInternal(ConfigurationManager.DeviceConfiguration.RootDevice, null);
        }
        private static DeviceViewModel AddDeviceInternal(Device device, TreeItemViewModel<DeviceViewModel> parentDeviceViewModel)
        {
            var deviceViewModel = new DeviceViewModel(device);
            if (parentDeviceViewModel != null)
                parentDeviceViewModel.Children.Add(deviceViewModel);

            foreach (var childDevice in device.Children)
                AddDeviceInternal(childDevice, deviceViewModel);
            return deviceViewModel;
        }
    }
}