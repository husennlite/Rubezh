﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Shell = FireMonitor;
using Infrastructure;
using Infrastructure.Common;
using FiresecClient;
using FiresecAPI.Models.Layouts;
using System.Windows;
using Infrastructure.Common.Windows;
using FireMonitor.Layout.ViewModels;
using FireMonitor.ViewModels;
using Infrastructure.Common.Windows.ViewModels;

namespace FireMonitor.Layout
{
	internal class Bootstrapper : Shell.Bootstrapper
	{
		private FiresecAPI.Models.Layouts.Layout _layout;

		public Bootstrapper()
		{
			_layout = null;
		}

		protected override bool Run()
		{
			var layouts = FiresecManager.LayoutsConfiguration.Layouts.Where(layout => layout.Users.Contains(FiresecManager.CurrentUser.UID)).ToList();
			if (layouts.Count > 0)
			{
				ServiceFactory.ResourceService.AddResource(new ResourceDescription(typeof(Bootstrapper).Assembly, "DataTemplates/Dictionary.xaml"));
				_layout = layouts.Count == 1 ? layouts[0] : SelectLayout(layouts);
				if (_layout == null)
					return false;
			}
			return base.Run();
		}
		protected override ShellViewModel CreateShell()
		{
			return _layout == null ? base.CreateShell() : new MonitorLayoutShellViewModel(_layout);
		}

		private FiresecAPI.Models.Layouts.Layout SelectLayout(List<FiresecAPI.Models.Layouts.Layout> layouts)
		{
			layouts.Sort((x, y) => string.Compare(x.Caption, y.Caption));
			Application.Current.ShutdownMode = ShutdownMode.OnExplicitShutdown;
			var viewModel = new SelectLayoutViewModel(layouts);
			DialogService.ShowModalWindow(viewModel);
			Application.Current.ShutdownMode = ShutdownMode.OnLastWindowClose;
			return viewModel.SelectedLayout;
		}
	}
}