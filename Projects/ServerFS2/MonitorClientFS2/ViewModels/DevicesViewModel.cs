﻿using FiresecAPI.Models;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;
using ServerFS2;
using Infrastructure.Common.TreeList;

namespace MonitorClientFS2.ViewModels
{
	public class DevicesViewModel : DialogViewModel
	{
		public DevicesViewModel()
		{
			SetIgnoreCommand = new RelayCommand(OnSetIgnore, CanSetIgnore);
			ResetIgnoreCommand = new RelayCommand(OnResetIgnore, CanResetIgnore);
			ReadRomRamCommand = new RelayCommand(OnReadRomRam, CanReadRomRam);
			GetParametersCommand = new RelayCommand(OnGetParameters, CanGetParameters);

			BuildTree();
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

		private void BuildTree()
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

		public RelayCommand SetIgnoreCommand { get; private set; }

		private void OnSetIgnore()
		{
		}

		private bool CanSetIgnore()
		{
			return SelectedDevice != null;
		}

		public RelayCommand ResetIgnoreCommand { get; private set; }

		private void OnResetIgnore()
		{
		}

		private bool CanResetIgnore()
		{
			return SelectedDevice != null;
		}

		public RelayCommand ReadRomRamCommand { get; private set; }

		private void OnReadRomRam()
		{
			var device = ServerHelper.GetDeviceConfig(SelectedDevice.Device);
		}

		private bool CanReadRomRam()
		{
			return ((SelectedDevice != null) && (SelectedDevice.Device.Driver.IsPanel));
		}

		public RelayCommand GetParametersCommand { get; private set; }

		private void OnGetParameters()
		{
			ServerHelper.GetDeviceParameters(SelectedDevice.Device);
			SelectedDevice.Device.OnAUParametersChanged();
		}

		private bool CanGetParameters()
		{
			return SelectedDevice != null;
		}
	}
}