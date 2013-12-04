﻿using System.Windows;
using Infrastructure.Common.Theme;

namespace GKSDK
{
	public partial class App : Application
	{
		protected override void OnStartup(StartupEventArgs e)
		{
			base.OnStartup(e);
			ThemeHelper.LoadThemeFromRegister();
		}
	}
}