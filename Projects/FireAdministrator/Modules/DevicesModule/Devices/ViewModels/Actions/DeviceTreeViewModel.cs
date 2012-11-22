﻿using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure.Common.Windows.ViewModels;

namespace DevicesModule.ViewModels
{
    public class DeviceTreeViewModel : BaseViewModel
    {
		bool IsRemote;
        Device RootDevice;
        DeviceConfiguration DeviceConfiguration;

        public DeviceTreeViewModel(Device rootDevice, DeviceConfiguration deviceConfiguration, bool isRemote)
        {
			IsRemote = isRemote;
            RootDevice = rootDevice;
            DeviceConfiguration = deviceConfiguration;
			DeviceConfiguration.Reorder();

            BuildTree(rootDevice);
            if (Devices.Count > 0)
            {
                //CollapseChild(Devices[0]);
                ExpandChild(Devices[0]);
                SelectedDevice = Devices[0];
            }
        }

        public ObservableCollection<DeviceViewModel> Devices { get; private set; }

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

        void BuildTree(Device rootDevice)
        {
            Devices = new ObservableCollection<DeviceViewModel>();
            AddDevice(rootDevice, null);
        }

        DeviceViewModel AddDevice(Device device, DeviceViewModel parentDeviceViewModel)
        {
            var deviceViewModel = new DeviceViewModel(device);
            if (IsRemote)
                deviceViewModel.XXXPresentationZone = DeviceConfiguration.GetPresentationZone(device);
            else
                deviceViewModel.XXXPresentationZone = deviceViewModel.PresentationZone;
            var indexOf = Devices.IndexOf(parentDeviceViewModel);
            Devices.Insert(indexOf + 1, deviceViewModel);

            foreach (var childDevice in device.Children)
            {
                var childDeviceViewModel = AddDevice(childDevice, deviceViewModel);
                deviceViewModel.Children.Add(childDeviceViewModel);
            }

            return deviceViewModel;
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

		void ClearExternalZones(Device device)
		{
			var zoneLogic = new ZoneLogic();
			if (device.ZoneLogic != null)
			{
				foreach (var clause in device.ZoneLogic.Clauses)
				{
					var newClause = new Clause();
					foreach (var zone in clause.Zones)
					{
						if (!HasExternalZone(zone, device))
						{
							newClause.ZoneUIDs.Add(zone.UID);
							newClause.Zones.Add(zone);
						}
					}
					zoneLogic.Clauses.Add(newClause);
				}
			}
			device.ZoneLogic = zoneLogic;
		}

		bool HasExternalZone(Zone zone, Device device)
		{
			foreach (var deviceInZone in zone.DevicesInZone)
			{
				if (device.ParentPanel.UID == deviceInZone.ParentPanel.UID)
				{
					return true;
				}
			}
			return false;
		}
    }
}