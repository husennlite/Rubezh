﻿using System.Collections.Generic;
using System.Threading;
using Common;
using Common.GK;
using Infrastructure;
using Infrastructure.Common.Windows;
using Infrastructure.Events;
using XFiresecAPI;
using System.Windows.Threading;

namespace GKModule
{
	public static class StatesWatcher
	{
		public static void RequestAllStates()
		{
            var thread = new Thread(new ThreadStart(OnRun));
            thread.Start();
			Dispatcher.CurrentDispatcher.ShutdownStarted += (s, e) =>
				{
					if (thread.IsAlive)
						thread.Abort();
				};
		}

        static void OnRun()
        {
            foreach (var gkDatabase in DatabaseManager.GkDatabases)
            {
                if (gkDatabase.RootDevice.GetGKIpAddress() == null)
                    continue;

                foreach (var binaryObject in gkDatabase.BinaryObjects)
                {
                    GetState(binaryObject.BinaryBase, gkDatabase.RootDevice);
                }
            }
			ApplicationService.Invoke(() => { ServiceFactory.Events.GetEvent<GKObjectsStateChangedEvent>().Publish(null); });
        }

		public static void RequestObjectState(XBinaryBase binaryBase)
		{
			GetState(binaryBase, binaryBase.GkDatabaseParent);
		}

		static void GetState(XBinaryBase binaryBase, XDevice gkParent)
		{
			var no = binaryBase.GetDatabaseNo(DatabaseType.Gk);
			var sendResult = SendManager.Send(gkParent, 2, 12, 68, BytesHelper.ShortToBytes(no));
			if (sendResult.HasError)
			{
				ConnectionChanged(false);
				return;
			}
			if (sendResult.Bytes.Count == 68)
			{
				var binaryObjectState = new BinaryObjectState(sendResult.Bytes);
				ApplicationService.Invoke(() => { SetObjectStates(binaryBase, binaryObjectState.States); });
			}
			else
			{
				Logger.Error("StatesWatcher.GetState sendResult.Bytes.Count != 68");
			}
		}

		public static void SetObjectStates(XBinaryBase binaryBase, List<XStateType> states)
		{
			if (binaryBase is XDevice)
			{
				var device = binaryBase as XDevice;
				device.DeviceState.States = states;
			}
			if (binaryBase is XZone)
			{
				var zone = binaryBase as XZone;
				zone.ZoneState.States = states;
			}
			if (binaryBase is XDirection)
			{
				var direction = binaryBase as XDirection;
				direction.DirectionState.States = states;
			}
		}

		static void ConnectionChanged(bool value)
		{
			ApplicationService.Invoke(() => { ServiceFactory.Events.GetEvent<GKConnectionChanged>().Publish(value); });
		}
	}
}