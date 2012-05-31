﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Infrastructure.Common.Windows.ViewModels;

namespace Infrastructure.Common.Windows.Views
{
	public partial class DialogHeaderView : UserControl
	{
		public DialogHeaderView()
		{
			InitializeComponent();
		}

		private void OnCloseButton(object sender, RoutedEventArgs e)
		{
			(((DialogHeaderViewModel)DataContext).Content).Close();
		}
	}
}