﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DeviveModelManager
{
    public class MonitorHelper
    {
        public static TreeItem CreateMonitor()
        {
            TreeItem monitor = new TreeItem();
            monitor.ModelInfo = new Assad.modelInfoType();
            monitor.ModelInfo.name = "Монитор." + ViewModel.StaticVersion;
            monitor.ModelInfo.type1 = "rubezh." + ViewModel.StaticVersion + "." + "monitor";
            monitor.ModelInfo.model = "1.0";

            List<Assad.modelInfoTypeState> states = new List<Assad.modelInfoTypeState>();
            states.Add(CreateState("Тревога"));
            states.Add(CreateState("Внимание (предтревожное)"));
            states.Add(CreateState("Неисправность"));
            states.Add(CreateState("Требуется обслуживание"));
            states.Add(CreateState("Обход устройств"));
            states.Add(CreateState("Неопределено"));
            states.Add(CreateState("Норма(*)"));
            states.Add(CreateState("Норма"));

            monitor.ModelInfo.state = states.ToArray();

            return monitor;
        }

        static Assad.modelInfoTypeState CreateState(string name)
        {
            Assad.modelInfoTypeState state = new Assad.modelInfoTypeState();
            state.state = name;
            List<Assad.modelInfoTypeStateValue> stateValues = new List<Assad.modelInfoTypeStateValue>();
            stateValues.Add(new Assad.modelInfoTypeStateValue() { value = "Есть" });
            stateValues.Add(new Assad.modelInfoTypeStateValue() { value = "Нет" });
            state.value = stateValues.ToArray();
            return state;
        }
    }
}