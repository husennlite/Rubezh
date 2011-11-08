﻿using AlarmModule.ViewModels;
using Infrastructure;
using Infrastructure.Common;
using Microsoft.Practices.Prism.Modularity;

namespace AlarmModule
{
    public class AlarmModule : IModule
    {
        static AlarmWatcher AlarmWatcher;

        public AlarmModule()
        {
            ServiceFactory.Layout.AddAlarmGroups(new AlarmGroupListViewModel());
        }

        public void Initialize()
        {
            RegisterResources();
            CreateViewModels();
        }

        void RegisterResources()
        {
            ServiceFactory.ResourceService.AddResource(new ResourceDescription(GetType().Assembly, "DataTemplates/Dictionary.xaml"));
        }

        static void CreateViewModels()
        {
            AlarmWatcher = new AlarmWatcher();
        }
    }
}