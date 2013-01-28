﻿using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using FiresecClient;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;
using XFiresecAPI;
using System;

namespace GKModule.ViewModels
{
    public class ZonesSelectationViewModel : SaveCancelDialogViewModel
    {
		public List<XZone> Zones { get; private set; }

		public ZonesSelectationViewModel(List<XZone> zones)
        {
            Title = "Выбор зон";
            AddCommand = new RelayCommand<object>(OnAdd, CanAdd);
            RemoveCommand = new RelayCommand<object>(OnRemove, CanRemove);
            AddAllCommand = new RelayCommand(OnAddAll, CanAddAll);
            RemoveAllCommand = new RelayCommand(OnRemoveAll, CanRemoveAll);

            Zones = zones;
            TargetZones = new ObservableCollection<XZone>();
            SourceZones = new ObservableCollection<XZone>();

			foreach (var zone in XManager.DeviceConfiguration.SortedZones)
            {
                if (Zones.Contains(zone))
                    TargetZones.Add(zone);
                else
                    SourceZones.Add(zone);
            }

			SelectedTargetZone = TargetZones.FirstOrDefault();
			SelectedSourceZone = SourceZones.FirstOrDefault();
        }

        public ObservableCollection<XZone> SourceZones { get; private set; }

        XZone _selectedSourceZone;
        public XZone SelectedSourceZone
        {
            get { return _selectedSourceZone; }
            set
            {
                _selectedSourceZone = value;
                OnPropertyChanged("SelectedSourceZone");
            }
        }

        public ObservableCollection<XZone> TargetZones { get; private set; }

        XZone _selectedTargetZone;
        public XZone SelectedTargetZone
        {
            get { return _selectedTargetZone; }
            set
            {
                _selectedTargetZone = value;
                OnPropertyChanged("SelectedTargetZone");
            }
        }

        public RelayCommand<object> AddCommand { get; private set; }
        public IList SelectedSourceZones;
        void OnAdd(object parameter)
        {
			var index = SourceZones.IndexOf(SelectedSourceZone);

            SelectedSourceZones = (IList)parameter;
            var zoneViewModels = new List<XZone>();
            foreach (var selectedZone in SelectedSourceZones)
            {
                var zoneViewModel = selectedZone as XZone;
                if (zoneViewModel != null)
                    zoneViewModels.Add(zoneViewModel);
            }
            foreach (var zoneViewModel in zoneViewModels)
            {
                TargetZones.Add(zoneViewModel);
                SourceZones.Remove(zoneViewModel);
            }
			SelectedTargetZone = TargetZones.LastOrDefault();
            OnPropertyChanged("SourceZones");

			index = Math.Min(index, SourceZones.Count - 1);
			if (index > -1)
				SelectedSourceZone = SourceZones[index];
        }

        public RelayCommand<object> RemoveCommand { get; private set; }
        public IList SelectedTargetZones;
        void OnRemove(object parameter)
        {
			var index = TargetZones.IndexOf(SelectedTargetZone);

            SelectedTargetZones = (IList)parameter;
            var zoneViewModels = new List<XZone>();
            foreach (var selectedZone in SelectedTargetZones)
            {
                var zoneViewModel = selectedZone as XZone;
                if (zoneViewModel != null)
                    zoneViewModels.Add(zoneViewModel);
            }
            foreach (var zoneViewModel in zoneViewModels)
            {
                SourceZones.Add(zoneViewModel);
                TargetZones.Remove(zoneViewModel);
            }
			SelectedSourceZone = SourceZones.LastOrDefault();
            OnPropertyChanged("TargetZones");

			index = Math.Min(index, TargetZones.Count - 1);
			if (index > -1)
				SelectedTargetZone = TargetZones[index];
        }

        public RelayCommand AddAllCommand { get; private set; }
        void OnAddAll()
        {
            foreach (var zoneViewModel in SourceZones)
            {
                TargetZones.Add(zoneViewModel);
            }
            SourceZones.Clear();
			SelectedTargetZone = TargetZones.FirstOrDefault();
        }

        public RelayCommand RemoveAllCommand { get; private set; }
        void OnRemoveAll()
        {
            foreach (var zoneViewModel in TargetZones)
            {
                SourceZones.Add(zoneViewModel);
            }
            TargetZones.Clear();
			SelectedSourceZone = SourceZones.FirstOrDefault();
        }

        public bool CanAdd(object parameter)
        {
            return SelectedSourceZone != null;
        }

        public bool CanRemove(object parameter)
        {
            return SelectedTargetZone != null;
        }

        public bool CanAddAll()
        {
            return (SourceZones.Count > 0);
        }

        public bool CanRemoveAll()
        {
            return (TargetZones.Count > 0);
        }

		protected override bool Save()
		{
			Zones = new List<XZone>(TargetZones);
			return base.Save();
		}
    }
}