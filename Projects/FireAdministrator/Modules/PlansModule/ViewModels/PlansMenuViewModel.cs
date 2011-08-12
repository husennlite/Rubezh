﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common;

namespace InstructionsModule.ViewModels
{
    public class InstructionsMenuViewModel
    {
        public InstructionsMenuViewModel(RelayCommand addCommand, RelayCommand editCommand, RelayCommand removeCommand)
        {
            AddCommand = addCommand;
            RemoveCommand = removeCommand;
            EditCommand = editCommand;
        }

        public RelayCommand AddCommand { get; private set; }
        public RelayCommand EditCommand { get; private set; }
        public RelayCommand RemoveCommand { get; private set; }
    }
}
