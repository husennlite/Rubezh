﻿using System;
using System.Linq;
using System.Windows.Media;
using FiresecAPI.Models;
using FiresecClient;
using Infrustructure.Plans.Elements;

namespace DevicesModule.Plans.Designer
{
	public static class Helper
	{
		public static Zone GetZone(IElementZone element)
		{
			return element.ZoneUID != Guid.Empty ? FiresecManager.Zones.FirstOrDefault(x => x.UID == element.ZoneUID) : null;
		}
		public static Plan GetPlan(ElementSubPlan element)
		{
			return FiresecManager.PlansConfiguration.AllPlans.FirstOrDefault(x => x.UID == element.PlanUID);
		}
		public static void SetZone(IElementZone element)
		{
			Zone zone = GetZone(element);
			element.BackgroundColor = GetZoneColor(zone);
		}
		public static void SetZone(IElementZone element, Zone zone)
		{
			element.ZoneUID = zone == null ? Guid.Empty : zone.UID;
			element.BackgroundColor = GetZoneColor(zone);
		}
		public static Color GetZoneColor(Zone zone)
		{
			Color color = Colors.Gray;
			if (zone != null)
			{
				if (zone.ZoneType == ZoneType.Fire)
					color = Colors.Green;

				if (zone.ZoneType == ZoneType.Guard)
					color = Colors.Brown;
			}
			return color;
		}

		public static Device GetDevice(ElementDevice element)
		{
			return element.DeviceUID == null ? null : FiresecManager.Devices.FirstOrDefault(x => x.UID == element.DeviceUID);
		}
		public static void SetDevice(ElementDevice element, Device device)
		{
			ResetDevice(element);
			element.DeviceUID = device == null ? Guid.Empty : device.UID;
			if (device != null)
				device.PlanElementUIDs.Add(element.UID);
		}
		public static Device SetDevice(ElementDevice element)
		{
			Device device = GetDevice(element);
			if (device != null)
				device.PlanElementUIDs.Add(element.UID);
			return device;
		}
		public static void ResetDevice(ElementDevice element)
		{
			Device device = GetDevice(element);
			if (device != null)
				device.PlanElementUIDs.Remove(element.UID);
		}

		public static string GetZoneTitle(IElementZone element)
		{
			Zone zone = GetZone(element);
			return GetZoneTitle(zone);
		}
		public static string GetZoneTitle(Zone zone)
		{
			return zone == null ? "Несвязанная зона" : zone.PresentationName;
		}
		public static string GetSubPlanTitle(ElementSubPlan element)
		{
			Plan plan = GetPlan(element);
			return plan == null ? "Несвязанный подплан" : plan.Caption;
		}
		public static string GetDeviceTitle(ElementDevice element)
		{
			var device = GetDevice(element);
			return device == null ? "Неизвестное устройство" : device.DottedPresentationAddressAndName;
		}
	}
}
