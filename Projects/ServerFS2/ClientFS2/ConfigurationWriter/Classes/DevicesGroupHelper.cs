﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FiresecAPI.Models;

namespace ClientFS2.ConfigurationWriter.Classes
{
	public class DevicesGroupHelper
	{
		Device ParentPanel;
		public List<DevicesGroup> DevicesGroups { get; set; }

		public DevicesGroupHelper(Device parentPanel)
		{
			ParentPanel = parentPanel;
			DevicesGroups = new List<DevicesGroup>();

			CreateDevicesGroup("Указатель на таблицу РМ", DriverType.RM_1);
			CreateDevicesGroup("Указатель на таблицу МПТ", DriverType.MPT);
			CreateDevicesGroup("Указатель на таблицу Дымовых", DriverType.SmokeDetector);
			CreateDevicesGroup("Указатель на таблицу Тепловых", DriverType.HeatDetector);
			CreateDevicesGroup("Указатель на таблицу Комбинированных", DriverType.CombinedDetector);
			CreateDevicesGroup("Указатель на таблицу АМ-1", DriverType.AM_1,
				DriverType.ShuzOffButton, DriverType.ShuzOnButton, DriverType.ShuzUnblockButton, DriverType.StartButton, DriverType.StopButton);
			CreateDevicesGroup("Указатель на таблицу ИПР", DriverType.HandDetector);
			CreateDevicesGroup("Указатель на таблицу Охранных извещателей", DriverType.AM1_O);
			var OuterDevices_Group = CreateDevicesGroup("Указатель на таблицу Внешних ИУ", DriverType.Computer);
			OuterDevices_Group.IsRemoteDevicesPointer = true;
			CreateDevicesGroup("Указатель на таблицу МДУ", DriverType.MDU);
			CreateDevicesGroup("Указатель на таблицу БУНС", DriverType.PumpStation);
			CreateDevicesGroup("Указатель на таблицу АМП-4", DriverType.AMP_4);
			CreateDevicesGroup("Указатель на таблицу ОЗ", DriverType.Computer);
			CreateDevicesGroup("Указатель на таблицу Задвижек", DriverType.Valve);
			CreateDevicesGroup("Указатель на таблицу АМ-Т", DriverType.AM1_T);
			CreateDevicesGroup("Указатель на таблицу АМТ-4", DriverType.AMT_4);
			CreateDevicesGroup("Указатель на таблицу ППУ", DriverType.Computer);
			CreateDevicesGroup("Указатель на таблицу АСПТ", DriverType.ASPT);
			CreateDevicesGroup("Указатель на таблицу МУК-1Э", DriverType.Computer);
			CreateDevicesGroup("Указатель на таблицу Выход реле", DriverType.Exit);
			CreateDevicesGroup("Указатель на таблицу радиоканальный ручной", DriverType.RadioHandDetector);
			CreateDevicesGroup("Указатель на таблицу радиоканальный дымовой", DriverType.RadioSmokeDetector);
		}

		DevicesGroup CreateDevicesGroup(string name, params DriverType[] driverTypes)
		{
			var devicesGroup = new DevicesGroup(driverTypes[0], name);
			foreach (var device in ParentPanel.Children)
			{
				if (driverTypes.Contains(device.Driver.DriverType))
				{
					devicesGroup.Devices.Add(device);
				}
			}
			DevicesGroups.Add(devicesGroup);
			return devicesGroup;
		}
	}
}