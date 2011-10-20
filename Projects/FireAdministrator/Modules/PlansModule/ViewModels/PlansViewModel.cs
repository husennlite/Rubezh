﻿using System.Collections.ObjectModel;
using System.Windows.Media;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common;
using PlansModule.Designer;

namespace PlansModule.ViewModels
{
    public class PlansViewModel : RegionViewModel
    {
        public PlansViewModel()
        {
            TestCommand = new RelayCommand(OnTest);
            AddCommand = new RelayCommand(OnAdd);
            AddSubCommand = new RelayCommand(OnAddSub, CanAddEditRemove);
            RemoveCommand = new RelayCommand(OnRemove, CanAddEditRemove);
            EditCommand = new RelayCommand(OnEdit, CanAddEditRemove);

            DesignerCanvas = new DesignerCanvas()
            {
                AllowDrop = true,
                Background = new SolidColorBrush(Colors.DarkGray),
                Width = 100,
                Height = 100
            };

            PlanDesignerViewModel = new PlanDesignerViewModel();
            PlanDesignerViewModel.DesignerCanvas = DesignerCanvas;

            Initialize();
        }

        public void Initialize()
        {
            Plans = new ObservableCollection<PlanViewModel>();
            foreach (var plan in FiresecManager.PlansConfiguration.Plans)
            {
                Plans.Add(new PlanViewModel(plan));
            }

            if (Plans.Count > 0)
                SelectedPlan = Plans[0];
        }

        public ObservableCollection<PlanViewModel> Plans { get; set; }

        PlanViewModel _selectedPlan;
        public PlanViewModel SelectedPlan
        {
            get { return _selectedPlan; }
            set
            {
                _selectedPlan = value;
                OnPropertyChanged("SelectedPlan");

                if (value != null)
                {
                    PlanDesignerViewModel.Save();
                    PlanDesignerViewModel.Initialize(value.Plan);
                }
            }
        }

        public PlanDesignerViewModel PlanDesignerViewModel { get; set; }

        DesignerCanvas _designerCanvas;
        public DesignerCanvas DesignerCanvas
        {
            get { return _designerCanvas; }
            set
            {
                _designerCanvas = value;
                OnPropertyChanged("DesignerCanvas");
            }
        }

        public RelayCommand TestCommand { get; private set; }
        void OnTest()
        {
            var devicesViewModel = new DevicesViewModel();
            devicesViewModel.Initialize();
            ServiceFactory.UserDialogs.ShowWindow(devicesViewModel);
        }

        bool CanAddEditRemove()
        {
            return SelectedPlan != null;
        }

        public RelayCommand AddCommand { get; private set; }
        void OnAdd()
        {
            var planDetailsViewModel = new PlanDetailsViewModel();

            if (ServiceFactory.UserDialogs.ShowModalWindow(planDetailsViewModel))
            {
                Plans.Add(new PlanViewModel(planDetailsViewModel.Plan));
            }
        }

        public RelayCommand AddSubCommand { get; private set; }
        void OnAddSub()
        {
        }

        public RelayCommand RemoveCommand { get; private set; }
        void OnRemove()
        {
            FiresecManager.PlansConfiguration.Plans.Remove(SelectedPlan.Plan);
            Plans.Remove(SelectedPlan);
        }

        public RelayCommand EditCommand { get; private set; }
        void OnEdit()
        {
            var planDetailsViewModel = new PlanDetailsViewModel(SelectedPlan.Plan);
            if (ServiceFactory.UserDialogs.ShowModalWindow(planDetailsViewModel))
            {
                SelectedPlan.Update();
            }
        }

        public override void OnShow()
        {
            ServiceFactory.Layout.ShowMenu(new PlansMenuViewModel(this));
        }

        public override void OnHide()
        {
            ServiceFactory.Layout.ShowMenu(null);
        }
    }
}
