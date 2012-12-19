﻿using System;
using System.Linq;
using System.Collections.Generic;
using DevicesModule.Reports;
using DevicesModule.ViewModels;
using FiresecClient;
using Infrastructure;
using Infrastructure.Client;
using Infrastructure.Common;
using Infrastructure.Common.Navigation;
using Infrastructure.Common.Reports;
using Infrastructure.Common.Windows;
using Infrastructure.Events;
using Common;
using DevicesModule.Plans;
using FiresecAPI.Models;
using Infrustructure.Plans.Events;
using FiresecAPI;

namespace DevicesModule
{
	public class DevicesModuleLoader : ModuleBase, IReportProviderModule
	{
		private DevicesViewModel DevicesViewModel;
		private ZonesViewModel ZonesViewModel;
		private NavigationItem _zonesNavigationItem;

		public override void CreateViewModels()
		{
			ServiceFactory.Layout.AddToolbarItem(new ConnectionIndicatorViewModel());
			ServiceFactory.Events.GetEvent<ShowDeviceDetailsEvent>().Subscribe(OnShowDeviceDetails);
			DevicesViewModel = new DevicesViewModel();
			ZonesViewModel = new ZonesViewModel();
		}

		void OnShowDeviceDetails(Guid deviceUID)
		{
			var device = FiresecManager.Devices.FirstOrDefault(x => x.UID == deviceUID);
			if (device == null)
			{
				Logger.Error("DevicesModuleLoader.OnShowDeviceDetails device=null " + deviceUID.ToString());
				return;
			}
			DialogService.ShowWindow(new DeviceDetailsViewModel(device));
		}

		public override void Initialize()
		{
			ServiceFactory.Events.GetEvent<RegisterPlanPresenterEvent<Plan>>().Publish(new PlanPresenter());
			_zonesNavigationItem.IsVisible = FiresecManager.FiresecConfiguration.DeviceConfiguration.Zones.Count > 0;
			DevicesViewModel.Initialize();
			ZonesViewModel.Initialize();
		}
		public override IEnumerable<NavigationItem> CreateNavigation()
		{
			_zonesNavigationItem = new NavigationItem<ShowZoneEvent, Guid>(ZonesViewModel, "Зоны", "/Controls;component/Images/zones.png", null, null, Guid.Empty);
			return new List<NavigationItem>()
			{
				new NavigationItem<ShowDeviceEvent, Guid>(DevicesViewModel, "Устройства", "/Controls;component/Images/tree.png", null, null, Guid.Empty),
				_zonesNavigationItem
			};
		}

		public override string Name
		{
			get { return "Устройства и Зоны"; }
		}

		#region IReportProviderModule Members
		public IEnumerable<IReportProvider> GetReportProviders()
		{
			return new List<IReportProvider>()
			{
				new DriverCounterReport(),
				new DeviceParamsReport(),
				new DeviceListReport(),
				new IndicationBlockReport(),
			};
		}
		#endregion

		public override bool BeforeInitialize(bool firstTime)
		{
			LoadingService.DoStep("Загрузка конфигурации с сервера");
            FiresecManager.GetConfiguration();

			if (firstTime)
			{
				LoadingService.DoStep("Инициализация драйвера устройств");
				var connectionResult = FiresecManager.InitializeFiresecDriver(true);
				if (connectionResult.HasError)
				{
					MessageBoxService.ShowError(connectionResult.Error);
					return false;
				}
			}

			LoadingService.DoStep("Синхронизация конфигурации");
			FiresecManager.FiresecDriver.Synchronyze();
			LoadingService.DoStep("Старт мониторинга");
			FiresecManager.FiresecDriver.StartWatcher(true, true);

			FiresecManager.FSAgent.Start();
			return true;
		}

		public override void AfterInitialize()
		{
			ServiceFactory.SubscribeEvents();
			//ProgressWatcher.Run();
		}
	}
}