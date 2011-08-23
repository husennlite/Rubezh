﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using Common;
using FiresecAPI.Models;
using Infrastructure.Common;

namespace FiltersModule.ViewModels
{
    public class FilterDetailsViewModel : DialogContent, IDataErrorInfo
    {
        public static readonly int DefaultDaysCount = 10;

        public FilterDetailsViewModel()
        {
            Title = "Добавить фильтр";

            JournalFilter = new JournalFilter();

            Initialize();
        }

        public FilterDetailsViewModel(JournalFilter journalFilter)
        {
            Title = "Редактировать фильтр";

            JournalFilter = new JournalFilter()
            {
                Name = journalFilter.Name,
                LastRecordsCount = journalFilter.LastRecordsCount,
                LastDaysCount = journalFilter.LastDaysCount,
                IsLastDaysCountActive = journalFilter.IsLastDaysCountActive
            };

            Initialize();

            EventViewModels.Where(
                eventViewModel => journalFilter.Events.Any(
                    x => x == eventViewModel.Id)).All(x => x.IsChecked = true);

            CategoryViewModels.Where(
                categoryViewModel => journalFilter.Categories.Any(
                    x => x == categoryViewModel.Id)).All(x => x.IsChecked = true);
        }

        void Initialize()
        {
            _existingNames = FiresecClient.FiresecManager.SystemConfiguration.JournalFilters.
                Where(journalFilter => journalFilter.Name != JournalFilter.Name).Select(journalFilter => journalFilter.Name).ToList();

            if (_existingNames == null)
                _existingNames = new List<string>();

            EventViewModels = new ObservableCollection<EventViewModel>();
            foreach (StateType stateType in Enum.GetValues(typeof(StateType)))
            {
                if (string.IsNullOrEmpty(EnumsConverter.StateTypeToEventName(stateType)) == false)
                    EventViewModels.Add(new EventViewModel(stateType));
            }

            CategoryViewModels = new ObservableCollection<CategoryViewModel>();
            foreach (DeviceCategoryType deviceCategoryType in Enum.GetValues(typeof(DeviceCategoryType)))
            {
                CategoryViewModels.Add(new CategoryViewModel(deviceCategoryType));
            }

            OkCommand = new RelayCommand(OnOk, CanOk);
            CancelCommand = new RelayCommand(OnCancel);
        }

        public JournalFilter JournalFilter { get; private set; }
        List<string> _existingNames;

        public string FilterName
        {
            get { return JournalFilter.Name; }
            set
            {
                JournalFilter.Name = value;
                OnPropertyChanged("FilterName");
            }
        }

        public int RecordsMaxCount
        {
            get { return new JournalFilter().LastRecordsCount; }
        }

        public ObservableCollection<EventViewModel> EventViewModels { get; private set; }
        public ObservableCollection<CategoryViewModel> CategoryViewModels { get; private set; }

        public JournalFilter GetModel()
        {
            JournalFilter.Events =
                EventViewModels.Where(x => x.IsChecked).Select(x => x.Id).Cast<StateType>().ToList();

            JournalFilter.Categories =
                CategoryViewModels.Where(x => x.IsChecked).Select(x => x.Id).Cast<DeviceCategoryType>().ToList();

            return JournalFilter;
        }

        public RelayCommand OkCommand { get; private set; }
        void OnOk()
        {
            JournalFilter.Name = JournalFilter.Name.Trim();
            Close(true);
        }

        bool CanOk(object obj)
        {
            return this["FilterName"] == null;
        }

        public RelayCommand CancelCommand { get; private set; }
        void OnCancel()
        {
            Close(false);
        }

        public string Error { get { return null; } }

        public string this[string propertyName]
        {
            get
            {
                if (propertyName != "FilterName")
                    throw new ArgumentException();

                if (string.IsNullOrWhiteSpace(FilterName))
                {
                    return "Нужно задать имя";
                }

                var name = FilterName.Trim();
                if (_existingNames.IsNotNullOrEmpty() &&
                    _existingNames.Any(x => x == name))
                {
                    return "Фильтр с таким именем уже существует";
                }

                return null;
            }
        }
    }
}