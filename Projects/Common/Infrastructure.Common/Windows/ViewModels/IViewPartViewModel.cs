﻿using System.ComponentModel;
using System.Windows.Input;
using System.Collections.Generic;
using System;

namespace Infrastructure.Common.Windows.ViewModels
{
	public interface IViewPartViewModel : INotifyPropertyChanged
	{
		BaseViewModel Menu { get; }
		string Key { get; }
		void OnShow();
		void OnHide();

		void RegisterShortcut(KeyGesture keyGesture, Action command);
		void RegisterShortcut(KeyGesture keyGesture, RelayCommand command);
		void RegisterShortcut<T>(KeyGesture keyGesture, RelayCommand<T> command, Func<T> getArg);
	}
}