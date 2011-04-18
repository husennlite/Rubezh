﻿using System.Collections.ObjectModel;
using System.Windows;
using System.IO;
using System.Xml.Serialization;
using System.ComponentModel;
using Common;
using System.Xml;
using System.Windows.Controls;
using RubezhDevices;
using System.Windows.Markup;
using System.Collections.Generic;
using System;
using System.Windows.Input;
using System.Windows.Threading;
using Firesec.Metadata;
using System.Linq;
using System.Threading;

namespace DeviceEditor
{
    public class ViewModel : BaseViewModel
    {
        public ViewModel()
        {
            Current = this;
            LoadMetadata();
            Load();

            SaveCommand = new RelayCommand(OnSaveCommand);
            StartTimerCommand = new RelayCommand(OnStartTimerCommand);
            dispatcherTimer.Tick += new EventHandler(dispatcherTimer_Tick);
        }

        public List<drvType> Drivers;

        public static string metadata_xml = @"c:\Rubezh\Assad\Projects\Assad\DeviceModelManager\metadata.xml";
        public void LoadMetadata()
        {
            FileStream file_xml = new FileStream(metadata_xml, FileMode.Open, FileAccess.Read, FileShare.Read);
            XmlSerializer serializer = new XmlSerializer(typeof(config));
            config metadata = (config)serializer.Deserialize(file_xml);
            file_xml.Close();

            Drivers = new List<drvType>();
            foreach (drvType drivers in metadata.drv)
                try
                {
                    Drivers.Add(drivers);

                }
                catch { }
        }

        public static ViewModel Current { get; private set; }
        public static string deviceLibrary_xml = @"c:\Rubezh\Assad\Projects\ActivexDevices\Library\DeviceLibrary.xml";

        public RelayCommand StartTimerCommand { get; private set; }
        void OnStartTimerCommand(object obj)
        {
            if (SelectedStateViewModel == null)
                return;

            if (TimerButtonName == "Старт таймер")
            {
                if (SelectedStateViewModel.FrameViewModels.Count > 1)
                {
                    TimerButtonName = "Стоп таймер";
                    dispatcherTimer.Start();
                }
            }
            else
            {
                TimerButtonName = "Старт таймер";
                dispatcherTimer.Stop();
            }
        }

        string timerButtonName = "Старт таймер";
        public string TimerButtonName
        {
            get { return timerButtonName; }
            set
            {
                timerButtonName = value;
                OnPropertyChanged("TimerButtonName");
            }
        }

        public RelayCommand SaveCommand { get; private set; }
        public void OnSaveCommand(object obj)
        {
            DeviceManager deviceManager = new DeviceManager();
            var result = MessageBox.Show("Вы уверены что хотите сохранить все изменения на диск?", "Окно подтверждения", MessageBoxButton.OKCancel, MessageBoxImage.Question);
            if (result == MessageBoxResult.Cancel)
                return;
            deviceManager.Devices = new List<Device>();
            foreach (DeviceViewModel deviceViewModel in DeviceViewModels)
            {
                Device device = new Device();
                device.Id = deviceViewModel.Id;
                deviceManager.Devices.Add(device);
                device.States = new List<State>();
                foreach (StateViewModel stateViewModel in deviceViewModel.StateViewModels)
                {
                    State state = new State();
                    state.Id = stateViewModel.Id;
                    state.IsAdditional = stateViewModel.IsAdditional;
                    device.States.Add(state);
                    state.Frames = new List<Frame>();
                    foreach (FrameViewModel frameViewModel in stateViewModel.FrameViewModels)
                    {
                        Frame frame = new Frame();
                        frame.Id = frameViewModel.Id;
                        frame.Image = frameViewModel.Image;
                        frame.Duration = frameViewModel.Duration;
                        state.Frames.Add(frame);
                    }
                }
            }
            FileStream file_xml = new FileStream(ViewModel.deviceLibrary_xml, FileMode.Create, FileAccess.Write, FileShare.Write);
            XmlSerializer serializer = new XmlSerializer(typeof(DeviceManager));
            serializer.Serialize(file_xml, deviceManager);
            file_xml.Close();
        }

        ObservableCollection<DeviceViewModel> deviceViewModels;
        public ObservableCollection<DeviceViewModel> DeviceViewModels
        {
            get { return deviceViewModels; }
            set
            {
                deviceViewModels = value;
                OnPropertyChanged("DeviceViewModels");
            }
        }

        DeviceViewModel selectedDeviceViewModel;
        public DeviceViewModel SelectedDeviceViewModel
        {
            get { return selectedDeviceViewModel; }
            set
            {
                selectedDeviceViewModel = value;
                DeviceViewModel.Current.LoadStates();
                OnPropertyChanged("SelectedDeviceViewModel");
            }
        }

        /// <summary>
        /// Выбранное состояние.
        /// </summary>
        StateViewModel selectedStateViewModel;
        public StateViewModel SelectedStateViewModel
        {
            get { return selectedStateViewModel; }
            set
            {
                selectedStateViewModel = value;
                if (SelectedStateViewModel == null)
                    return;
                SelectedStateViewModel.Parent.MainStatePicture.Clear();
                SelectedStateViewModel.SelectedFrameViewModel = selectedStateViewModel.FrameViewModels[0];
                string frameImage = Svg2Xaml.XSLT_Transform(SelectedStateViewModel.SelectedFrameViewModel.Image, RubezhDevices.RubezhDevice.svg2xaml_xsl);
                StringReader stringReader = new StringReader(frameImage);
                XmlReader xmlReader = XmlReader.Create(stringReader);
                SelectedStateViewModel.Parent.MainStatePicture.Add((Canvas)XamlReader.Load(xmlReader));
                if (selectedStateViewModel.FrameViewModels.Count > 1)
                {
                    TimerButtonName = "Стоп таймер";
                    SelectedStateViewModel.Parent.MainStatePicture.Clear();
                    dispatcherTimer.Start();
                }
                OnPropertyChanged("SelectedStateViewModel");
            }
        }

        ObservableCollection<StateViewModel> additionalStatesViewModel;
        public ObservableCollection<StateViewModel> AdditionalStatesViewModel
        {
            get { return additionalStatesViewModel; }
            set
            {
                additionalStatesViewModel = value;
                OnPropertyChanged("AdditionalStatesViewModel");
            }
        }

        public static DispatcherTimer dispatcherTimer = new DispatcherTimer();
        int tick = 0;

        public Canvas DynamicPicture;
        /****************Таймер*****************/
        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            CommandManager.InvalidateRequerySuggested();
            string frameImage;
            StringReader stringReader;
            XmlReader xmlReader;

            try
            {
                try
                {
                    SelectedStateViewModel.Parent.MainStatePicture.Remove(DynamicPicture);
                }
                catch { }

                dispatcherTimer.Interval = new TimeSpan(0, 0, 0, 0, SelectedStateViewModel.FrameViewModels[tick].Duration);
                frameImage = Svg2Xaml.XSLT_Transform(SelectedStateViewModel.FrameViewModels[tick].Image, RubezhDevices.RubezhDevice.svg2xaml_xsl);
                stringReader = new StringReader(frameImage);
                xmlReader = XmlReader.Create(stringReader);
                DynamicPicture = (Canvas)XamlReader.Load(xmlReader);
                SelectedStateViewModel.Parent.MainStatePicture.Add(DynamicPicture);
                tick = (tick + 1) % SelectedStateViewModel.FrameViewModels.Count;
            }
            catch
            { }

        }

        public void Load()
        {
            DeviceManager deviceManager = new DeviceManager();
            FileStream file_xml = new FileStream(deviceLibrary_xml, FileMode.Open, FileAccess.Read, FileShare.Read);
            XmlSerializer serializer = new XmlSerializer(typeof(DeviceManager));
            deviceManager = (DeviceManager)serializer.Deserialize(file_xml);
            file_xml.Close();

            DeviceViewModels = new ObservableCollection<DeviceViewModel>();
            AdditionalStatesViewModel = new ObservableCollection<StateViewModel>();
            foreach (Device device in deviceManager.Devices)
            {
                DeviceViewModel deviceViewModel = new DeviceViewModel();
                deviceViewModel.Id = device.Id;
                DeviceViewModels.Add(deviceViewModel);
                deviceViewModel.StateViewModels = new ObservableCollection<StateViewModel>();
                try
                {
                    deviceViewModel.IconPath = @"C:/Program Files/Firesec/Icons/" + Drivers.FirstOrDefault(x => x.name == deviceViewModel.Id).dev_icon + ".ico";
                }
                catch { }
                foreach (State state in device.States)
                {
                    StateViewModel stateViewModel = new StateViewModel();
                    stateViewModel.Id = state.Id;
                    stateViewModel.IsAdditional = state.IsAdditional;
                    deviceViewModel.StateViewModels.Add(stateViewModel);

                    if (stateViewModel.IsAdditional)
                        AdditionalStatesViewModel.Add(stateViewModel);
                    stateViewModel.FrameViewModels = new ObservableCollection<FrameViewModel>();
                    foreach (Frame frame in state.Frames)
                    {
                        FrameViewModel frameViewModel = new FrameViewModel();
                        frameViewModel.Id = frame.Id;
                        frameViewModel.Image = frame.Image;
                        frameViewModel.Duration = frame.Duration;
                        stateViewModel.FrameViewModels.Add(frameViewModel);
                    }
                }
            }
        }
    }
}
