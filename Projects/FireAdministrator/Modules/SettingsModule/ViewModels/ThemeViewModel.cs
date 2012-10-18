﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Theme;
using Infrastructure.Common.Windows.ViewModels;

namespace SettingsModule.ViewModels
{
    public class ThemeViewModel : BaseViewModel
    {
        public static List<Theme> ThemeInitialize()
        {
            var themes = Enum.GetValues(typeof(Theme)).Cast<Theme>().ToList();
            return themes;
        }
        public ThemeViewModel()
        {
            Themes = ThemeInitialize();
            if (ThemeHelper.CurrentTheme != null)
                SelectedTheme = (Theme)Enum.Parse(typeof(Theme), ThemeHelper.CurrentTheme);
        }

        private Theme _selectedTheme;
        public Theme SelectedTheme
        {
            get { return _selectedTheme; }
            set
            {
                _selectedTheme = value;
                ThemeHelper.SetThemeIntoRegister(_selectedTheme);
                ThemeHelper.LoadThemeFromRegister();
                OnPropertyChanged("SelectedTheme");
            }
        }
        public List<Theme> Themes { get; private set; }
    }
}