﻿using System.Collections.Generic;
using System.Linq;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure.Common;

namespace DevicesModule.ViewModels
{
    public class AutoSearchDeviceViewModel : SaveCancelDialogContent
    {
        public Device Device { get; private set; }

        public AutoSearchDeviceViewModel(Device device)
        {
            Device = device;
            Children = new List<AutoSearchDeviceViewModel>();
        }

        bool _isSelected;
        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                _isSelected = value;
                OnPropertyChanged("IsSelected");

                Children.ForEach(x => x.IsSelected = value);
            }
        }

        public bool CanSelect
        {
            get
            {
                return !FiresecManager.DeviceConfiguration.Devices.Any(x => x.Id == Device.Id);
            }
        }

        public string Name
        {
            get
            {
                if (Device.Driver.HasAddress)
                    return Device.PresentationAddress + "-" + Device.Driver.Name;
                return Device.Driver.Name;
            }
        }

        public List<AutoSearchDeviceViewModel> Children { get; set; }
    }
}