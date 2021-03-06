﻿using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using XFiresecAPI;
using GKProcessor;
using System;

namespace GKModule.ViewModels
{
	public class DirectionDetailsViewModel : SaveCancelDialogViewModel
	{
		public XDirection Direction { get; private set; }

		public DirectionDetailsViewModel(XDirection direction = null)
		{
			WritePropertiesCommand = new RelayCommand(OnWriteProperties);
			ReadPropertiesCommand = new RelayCommand(OnReadProperties);
			ResetPropertiesCommand = new RelayCommand(OnResetProperties);
			DelayRegimes = Enum.GetValues(typeof(DelayRegime)).Cast<DelayRegime>().ToList();

			if (direction == null)
			{
				Title = "Создание новоого направления";

				Direction = new XDirection()
				{
					Name = "Новое направление",
					No = 1
				};
				if (XManager.Directions.Count != 0)
					Direction.No = (ushort)(XManager.Directions.Select(x => x.No).Max() + 1);
			}
			else
			{
				Title = string.Format("Свойства направления: {0}", direction.PresentationName);
				Direction = direction;
			}
			CopyProperties();

			var availableNames = new HashSet<string>();
			var availableDescription = new HashSet<string>();
			foreach (var existingDirection in XManager.Directions)
			{
				availableNames.Add(existingDirection.Name);
				availableDescription.Add(existingDirection.Description);
			}
			AvailableNames = new ObservableCollection<string>(availableNames);
			AvailableDescription = new ObservableCollection<string>(availableDescription);
		}

		void CopyProperties()
		{
			Name = Direction.Name;
			No = Direction.No;
			Delay = Direction.Delay;
			Hold = Direction.Hold;
			DelayRegime = Direction.DelayRegime;
			Description = Direction.Description;
		}

		string _name;
		public string Name
		{
			get { return _name; }
			set
			{
				_name = value;
				OnPropertyChanged("Name");
			}
		}

		ushort _no;
		public ushort No
		{
			get { return _no; }
			set
			{
				_no = value;
				OnPropertyChanged("No");
			}
		}

		ushort _delay;
		public ushort Delay
		{
			get { return _delay; }
			set
			{
				_delay = value;
				OnPropertyChanged("Delay");
			}
		}

		ushort _hold;
		public ushort Hold
		{
			get { return _hold; }
			set
			{
				_hold = value;
				OnPropertyChanged("Hold");
			}
		}

		public List<DelayRegime> DelayRegimes { get; private set; }

		DelayRegime _delayRegime;
		public DelayRegime DelayRegime
		{
			get { return _delayRegime; }
			set
			{
				_delayRegime = value;
				OnPropertyChanged("DelayRegime");
			}
		}

		string _description;
		public string Description
		{
			get { return _description; }
			set
			{
				_description = value;
				OnPropertyChanged("Description");
			}
		}

		public ObservableCollection<string> AvailableNames { get; private set; }
		public ObservableCollection<string> AvailableDescription { get; private set; }

		protected override bool Save()
		{
			if (Direction.No != No && XManager.Directions.Any(x => x.No == No))
			{
				MessageBoxService.Show("Направление с таким номером уже существует");
				return false;
			}

			Direction.Name = Name;
			Direction.No = No;
			Direction.Delay = Delay;
			Direction.Hold = Hold;
			Direction.DelayRegime = DelayRegime;
			Direction.Description = Description;
			return base.Save();
		}

		public RelayCommand ReadPropertiesCommand { get; private set; }
		void OnReadProperties()
		{
			DescriptorsManager.Create();
			if (!CompareLocalWithRemoteHashes())
				return;

			var result = FiresecManager.FiresecService.GKGetSingleParameter(Direction);
			if (!result.HasError && result.Result != null && result.Result.Count == 3)
			{
				Delay = result.Result[0].Value;
				Hold = result.Result[1].Value;
				DelayRegime = (DelayRegime)result.Result[2].Value;
				OnPropertyChanged("Delay");
				OnPropertyChanged("Hold");
				OnPropertyChanged("Regime");
			}
			else
			{
				MessageBoxService.ShowError(result.Error);
			}
			ServiceFactory.SaveService.GKChanged = true;
		}

		public RelayCommand WritePropertiesCommand { get; private set; }
		void OnWriteProperties()
		{
			Direction.Name = Name;
			Direction.No = No;
			Direction.Description = Description;
			Direction.Delay = Delay;
			Direction.Hold = Hold;
			Direction.DelayRegime = DelayRegime;

			DescriptorsManager.Create();
			if (!CompareLocalWithRemoteHashes())
				return;

			var baseDescriptor = ParametersHelper.GetBaseDescriptor(Direction);
			if (baseDescriptor != null)
			{
				var result = FiresecManager.FiresecService.GKSetSingleParameter(Direction, baseDescriptor.Parameters);
				if (result.HasError)
				{
					MessageBoxService.ShowError(result.Error);
				}
			}
			else
			{
				MessageBoxService.ShowError("Ошибка. Отсутствуют параметры");
			}
		}

		public RelayCommand ResetPropertiesCommand { get; private set; }
		void OnResetProperties()
		{
			Delay = 10;
			Hold = 10;
			DelayRegime = DelayRegime.Off;
		}

		bool CompareLocalWithRemoteHashes()
		{
			if(Direction.GkDatabaseParent == null)
			{
				MessageBoxService.ShowError("Направление не относится ни к одному ГК");
				return false;
			}

			var result = FiresecManager.FiresecService.GKGKHash(Direction.GkDatabaseParent);
			if (result.HasError)
			{
				MessageBoxService.ShowError("Ошибка при сравнении конфигураций. Операция запрещена");
				return false;
			}

			var localHash = GKFileInfo.CreateHash1(XManager.DeviceConfiguration, Direction.GkDatabaseParent);
			var remoteHash = result.Result;
			if (GKFileInfo.CompareHashes(localHash, remoteHash))
				return true;
			MessageBoxService.ShowError("Конфигурации различны. Операция запрещена");
			return false;
		}
	}
}