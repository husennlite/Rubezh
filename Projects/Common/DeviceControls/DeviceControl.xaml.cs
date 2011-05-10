﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using DeviceLibrary;

namespace DeviceControls
{
    public partial class DeviceControl : UserControl, INotifyPropertyChanged
    {
        private List<string> _additionalStatesIds;
        private string _driverId;
        private ObservableCollection<Canvas> _stateCanvases;
        private string _stateId;
        private List<StateViewModel> StatesViewModels { get; set; }

        public DeviceControl()
        {
            InitializeComponent();
            DataContext = this;
            StateCanvases = new ObservableCollection<Canvas>();
            StatesViewModels = new List<StateViewModel>();
        }

        public static DispatcherTimer Timer { get; set; }
        public static event Action Tick;

        static DeviceControl()
        {
            Timer = new DispatcherTimer();
            Timer.Interval = TimeSpan.FromMilliseconds(100);
            Timer.Tick += new EventHandler(Timer_Tick);
            Timer.Start();
        }

        static void Timer_Tick(object sender, EventArgs e)
        {
            if (Tick != null)
                Tick();
        }

        public static Device CurrentDevice;
        public bool IsAdditional;
        public string DriverId
        {
            get { return _driverId; }
            set
            {
                _driverId = value;
                CurrentDevice = LibraryManager.Devices.FirstOrDefault(x => x.Id == value);
            }
        }
        public string StateId
        {
            get { return _stateId; }
            set
            {
                _stateId = value;
                Update();
            }
        }
        public List<string> AdditionalStatesIds
        {
            get { return _additionalStatesIds; }
            set
            {
                _additionalStatesIds = value;
                Update();
            }
        }
        public ObservableCollection<Canvas> StateCanvases
        {
            get { return _stateCanvases; }
            set
            {
                _stateCanvases = value;
                OnPropertyChanged("StateCanvases");
            }
        }
        private void Update()
        {
            StateCanvases = new ObservableCollection<Canvas>();
            var state = CurrentDevice.States.FirstOrDefault(x => (x.Id == StateId) && (x.IsAdditional == IsAdditional));
            if (StatesViewModels != null) StatesViewModels.Add(new StateViewModel(state, StateCanvases));
            if (IsAdditional) return;
            if (AdditionalStatesIds != null)
                foreach (var aState in AdditionalStatesIds.Select(additionalState => CurrentDevice.States.FirstOrDefault(x => (x.Id == additionalState) && (x.IsAdditional))).Where(aState => StatesViewModels != null))
                {
                    new StateViewModel(aState, StateCanvases);
                }
        }
        #region INotifyPropertyChanged Members
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion
        protected void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
        private void UserControlSizeChanged(object sender, SizeChangedEventArgs e)
        {
            _itemsControl.LayoutTransform = new ScaleTransform(ActualWidth/500, ActualHeight/500);
        }
    }
}