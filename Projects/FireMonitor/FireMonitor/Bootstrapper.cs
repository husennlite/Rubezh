﻿using System;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using FireMonitor.ViewModels;
using Firesec;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure;
using Infrastructure.Client;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Events;
using Infrastructure.Common.Theme;
using Common.GK;
using Microsoft.Win32;
using Common;
using FiresecAPI;
using System.Threading;
using Infrastructure.Common.BalloonTrayTip;

namespace FireMonitor
{
	public class Bootstrapper : BaseBootstrapper
	{
		public bool IsMulticlient { get; private set; }
		public Bootstrapper(bool isMulticlient)
		{
			IsMulticlient = isMulticlient;
		}

		public void Initialize()
		{
            LoadingErrorManager.Clear();
			AppConfigHelper.InitializeAppSettings();
			VideoService.Initialize(ServiceFactory.AppSettings.LibVlcDllsPath);
			ServiceFactory.Initialize(new LayoutService(), new SecurityService());
			ServiceFactory.ResourceService.AddResource(new ResourceDescription(GetType().Assembly, "DataTemplates/Dictionary.xaml"));
            BalloonHelper.Initialize();

			if (ServiceFactory.LoginService.ExecuteConnect(App.Login, App.Password))
			{
				App.Login = ServiceFactory.LoginService.Login;
				App.Password = ServiceFactory.LoginService.Password;
				try
				{
					CreateModules();

					LoadingService.Show("Чтение конфигурации", 15);
					LoadingService.AddCount(GetModuleCount());

					LoadingService.DoStep("Синхронизация файлов");
					FiresecManager.UpdateFiles();

					BeforeInitialize(true);

                    if (LoadingErrorManager.HasError)
                        MessageBoxService.ShowWarning(LoadingErrorManager.ToString(), "Ошибки при загрузке драйвера FireSec");

					LoadingService.DoStep("Старт полинга сервера");
					FiresecManager.StartPoll(false);
#if RELEASE
                    LoadingService.DoStep("Проверка HASP-ключа");
                    var operationResult = FiresecManager.FiresecDriver.CheckHaspPresence();
                    if (operationResult.HasError)
                        MessageBoxService.ShowWarning("HASP-ключ на сервере не обнаружен. Время работы приложения будет ограничено");
#endif
					LoadingService.DoStep("Проверка прав пользователя");
					if (FiresecManager.CheckPermission(PermissionType.Oper_Login))
					{
						LoadingService.DoStep("Загрузка клиентских настроек");
						ClientSettings.LoadSettings();

						var shell = new MonitorShellViewModel();
						((LayoutService)ServiceFactory.Layout).SetToolbarViewModel((ToolbarViewModel)shell.Toolbar);
						((LayoutService)ServiceFactory.Layout).AddToolbarItem(new SoundViewModel());
						RunShell(shell);
						((LayoutService)ServiceFactory.Layout).AddToolbarItem(new UserViewModel());
						((LayoutService)ServiceFactory.Layout).AddToolbarItem(new AutoActivationViewModel());

						SafeFiresecService.ConfigurationChangedEvent += () => { ApplicationService.Invoke(OnConfigurationChanged); };
					}
					else
					{
						MessageBoxService.Show("Нет прав на работу с программой");
						FiresecManager.Disconnect();
					}
					LoadingService.Close();

					AterInitialize();

					//MutexHelper.KeepAlive();
					ProgressWatcher.Run();
					ServiceFactory.Events.GetEvent<BootstrapperInitializedEvent>().Publish(null);
                    if (Process.GetCurrentProcess().ProcessName != "FireMonitor.vshost")
                    {
                        RegistryKey saveKey = Registry.LocalMachine.CreateSubKey("software\\rubezh\\Firesec-2");
                        saveKey.SetValue("isException", true);
                    }
				}
				catch (Exception e)
				{
					Logger.Error(e, "Bootstrapper.InitializeFs");
					MessageBoxService.ShowException(e);
					if (Application.Current != null)
						Application.Current.Shutdown();
					return;
				}
			}
			else
			{
				if (Application.Current != null)
					Application.Current.Shutdown();
				return;
			}
		}

		bool IsRestarting = false;
		void OnConfigurationChanged()
		{
			try
			{
				if (IsRestarting)
					return;
                FiresecManager.FiresecService.SuspendPoll = true;
				FiresecManager.FSAgent.SuspendPoll = true;
                LoadingErrorManager.Clear();
				IsRestarting = true;
				ProgressWatcher.Close();
				ApplicationService.Restart();

				LoadingService.Show("Перезагрузка конфигурации", 10);
				LoadingService.AddCount(10);

				ApplicationService.CloseAllWindows();
				ServiceFactory.Layout.Close();

				BeforeInitialize(false);
				InitializeModules();
				ServiceFactory.Events.GetEvent<ShowAlarmsEvent>().Publish(null);

				LoadingService.Close();
			}
			finally
			{
				IsRestarting = false;
                FiresecManager.FiresecService.SuspendPoll = false;
				FiresecManager.FSAgent.SuspendPoll = false;
			}
		}

		void CloseOnException(string message)
		{
			MessageBoxService.ShowError(message);
			Application.Current.Shutdown();
		}
	}
}