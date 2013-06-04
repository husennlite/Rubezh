﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Threading;
using Common;
using FiresecAPI;
using FiresecAPI.Models;
using FS2Api;
using ServerFS2.Processor;
using System.Threading.Tasks;
using System.Diagnostics;

namespace ServerFS2.Service
{
	[ServiceBehavior(MaxItemsInObjectGraph = Int32.MaxValue, UseSynchronizationContext = false,
	InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple)]
	public class FS2Contract : IFS2Contract
	{
		bool IsFirstCall = true;
		public static List<CancellationTokenSource> CancellationTokenSources { get; private set; }

		public static void CheckCancellationAndNotify(string message)
		{
			CallbackManager.Add(new FS2Callbac() { FS2ProgressInfo = new FS2ProgressInfo(message) });
			CheckCancellationRequested();
		}

		public static void CheckCancellationRequested()
		{
			if (CancellationTokenSources != null)
			{
				foreach (var cancellationTokenSource in CancellationTokenSources)
				{
					if (cancellationTokenSource.Token.IsCancellationRequested)
					{
						throw new AggregateException("Операция отменена");
					}
					cancellationTokenSource.Token.ThrowIfCancellationRequested();
				}
			}
		}

		public FS2Contract()
		{
			CancellationTokenSources = new List<CancellationTokenSource>();
		}

		#region Main
		public List<FS2Callbac> Poll(Guid clientUID)
		{
			try
			{
				Trace.WriteLine("Poll");
				ClientsManager.Add(clientUID);

				var clientInfo = ClientsManager.ClientInfos.FirstOrDefault(x => x.UID == clientUID);
				if (clientInfo != null)
				{
					var result = CallbackManager.Get(clientInfo);
					if (result.Count == 0)
					{
						if (IsFirstCall)
							clientInfo.PollWaitEvent.WaitOne(TimeSpan.FromSeconds(1));
						else
							clientInfo.PollWaitEvent.WaitOne(TimeSpan.FromMinutes(1));
						result = CallbackManager.Get(clientInfo);
					}
					return result;
				}
				return new List<FS2Callbac>();
			}
			catch (Exception e)
			{
				Logger.Error(e, "FS2Contract.Poll");
				return null;
			}
		}

		public void CancelPoll(Guid clientUID)
		{
			Trace.WriteLine("CancelPoll");
			var clientInfo = ClientsManager.ClientInfos.FirstOrDefault(x => x.UID == clientUID);
			if (clientInfo != null)
			{
				clientInfo.PollWaitEvent.Set();
			}
		}

		public void CancelProgress()
		{
			Trace.WriteLine("CancelProgress");
			try
			{
				foreach (var cancellationTokenSource in CancellationTokenSources)
				{
					cancellationTokenSource.Cancel();
				}
			}
			catch (Exception e)
			{
				Logger.Error(e, "FS2Contract.CancelProgress");
			}
		}
		#endregion

		#region Common
		public OperationResult<DeviceConfiguration> GetDeviceConfiguration()
		{
			return SafeCallWithMonitoringSuspending<DeviceConfiguration>(() =>
			{
				return ConfigurationCash.DeviceConfiguration;
			}, "GetDeviceConfiguration");
		}

		public OperationResult<DriversConfiguration> GetDriversConfiguration()
		{
			return SafeCallWithMonitoringSuspending<DriversConfiguration>(() =>
			{
				return ConfigurationCash.DriversConfiguration;
			}, "GetDriversConfiguration");
		}
		#endregion

		#region Monitor
		public OperationResult<string> GetCoreState()
		{
			throw new NotImplementedException();
		}

		public OperationResult<string> GetCoreDeviceParams()
		{
			throw new NotImplementedException();
		}

		public OperationResult<string> ReadEvents(int fromId, int limit)
		{
			throw new NotImplementedException();
		}

		public void AddToIgnoreList(List<Guid> deviceUIDs)
		{
			throw new NotImplementedException();
		}

		public void RemoveFromIgnoreList(List<Guid> deviceUIDs)
		{
			throw new NotImplementedException();
		}

		public void ResetStates(string states)
		{
			throw new NotImplementedException();
		}

		public void SetZoneGuard(Guid deviceUID, string localZoneNo)
		{
			throw new NotImplementedException();
		}

		public void UnSetZoneGuard(Guid deviceUID, string localZoneNo)
		{
			throw new NotImplementedException();
		}

		public void AddUserMessage(string message)
		{
			throw new NotImplementedException();
		}

		public OperationResult<string> ExecuteRuntimeDeviceMethod(Guid deviceUID, string methodName, string parameters)
		{
			throw new NotImplementedException();
		}

		public OperationResult<bool> ExecuteCommand(Guid deviceUID, string methodName)
		{
			throw new NotImplementedException();
		}

		public OperationResult<bool> CheckHaspPresence()
		{
			throw new NotImplementedException();
		}
		#endregion

		#region Administrator
		public OperationResult<bool> SetNewConfig(DeviceConfiguration deviceConfiguration)
		{
			return SafeCall<bool>(() =>
			{
				return MainManager.SetNewConfig(deviceConfiguration);
			}, "SetNewConfig");
		}

		public OperationResult<bool> DeviceWriteConfig(Guid deviceUID, bool isUSB)
		{
			return SafeCallWithMonitoringSuspending<bool>(() =>
			{
				return MainManager.DeviceWriteConfig(FindDevice(deviceUID), isUSB);
			}, "DeviceWriteConfig");
		}

		public OperationResult<bool> DeviceSetPassword(Guid deviceUID, bool isUSB, DevicePasswordType devicePasswordType, string password)
		{
			return SafeCallWithMonitoringSuspending<bool>(() =>
			{
				return MainManager.DeviceSetPassword(FindDevice(deviceUID), isUSB, devicePasswordType, password);
			}, "DeviceSetPassword");
		}

		public OperationResult<bool> DeviceDatetimeSync(Guid deviceUID, bool isUSB)
		{
			Trace.WriteLine("DeviceDatetimeSync");
			//for (int i = 0; i < 10; i++)
			//{
			//    FS2Contract.CheckCancellationRequested();
			//    var fs2ProgressInfo = new FS2ProgressInfo()
			//    {
			//        Comment = "Test Callbac " + i.ToString()
			//    };
			//    CallbackManager.Add(new FS2Callbac() { FS2ProgressInfo = fs2ProgressInfo });
			//    Thread.Sleep(1000);
			//}
			//return new OperationResult<bool>();

			return TaskSafeCallWithMonitoringSuspending<bool>(() =>
			{
				MainManager.DeviceDatetimeSync(FindDevice(deviceUID), isUSB);
				return true;
			}, "DeviceDatetimeSync");
		}

		public OperationResult<string> DeviceGetInformation(Guid deviceUID, bool isUSB)
		{
			return SafeCallWithMonitoringSuspending<string>(() =>
			{
				return MainManager.DeviceGetInformation(FindDevice(deviceUID), isUSB);
			}, "DeviceGetInformation");
		}

		public OperationResult<List<string>> DeviceGetSerialList(Guid deviceUID)
		{
			return SafeCallWithMonitoringSuspending<List<string>>(() =>
			{
				return MainManager.DeviceGetSerialList(FindDevice(deviceUID));
			}, "DeviceGetSerialList");
		}

		public OperationResult<string> DeviceUpdateFirmware(Guid deviceUID, bool isUSB, string fileName)
		{
			return SafeCallWithMonitoringSuspending<string>(() =>
			{
				return MainManager.DeviceUpdateFirmware(FindDevice(deviceUID), isUSB, fileName);
			}, "DeviceUpdateFirmware");
		}

		public OperationResult<string> DeviceVerifyFirmwareVersion(Guid deviceUID, bool isUSB, string fileName)
		{
			return SafeCallWithMonitoringSuspending<string>(() =>
			{
				return MainManager.DeviceVerifyFirmwareVersion(FindDevice(deviceUID), isUSB, fileName);
			}, "DeviceVerifyFirmwareVersion");
		}

		public OperationResult<DeviceConfiguration> DeviceReadConfig(Guid deviceUID, bool isUSB)
		{
			return SafeCallWithMonitoringSuspending<DeviceConfiguration>(() =>
			{
				return MainManager.DeviceReadConfig(FindDevice(deviceUID), isUSB);
			}, "DeviceReadConfig");
		}

		public OperationResult<List<FS2JournalItem>> DeviceReadEventLog(Guid deviceUID, bool isUSB)
		{
			return SafeCallWithMonitoringSuspending<List<FS2JournalItem>>(() =>
			{
				return MainManager.DeviceReadEventLog(FindDevice(deviceUID), isUSB);
			}, "DeviceReadEventLog");
		}

		public OperationResult<DeviceConfiguration> DeviceAutoDetectChildren(Guid deviceUID, bool fastSearch)
		{
			return SafeCallWithMonitoringSuspending<DeviceConfiguration>(() =>
			{
				return MainManager.DeviceAutoDetectChildren(FindDevice(deviceUID), fastSearch);
			}, "DeviceAutoDetectChildren");
		}

		public OperationResult<List<DeviceCustomFunction>> DeviceCustomFunctionList(DriverType driverType)
		{
			return SafeCallWithMonitoringSuspending<List<DeviceCustomFunction>>(() =>
			{
				return MainManager.DeviceCustomFunctionList(driverType);
			}, "DeviceCustomFunctionList");
		}

		public OperationResult<string> DeviceCustomFunctionExecute(Guid deviceUID, bool isUSB, string functionName)
		{
			return SafeCallWithMonitoringSuspending<string>(() =>
			{
				return MainManager.DeviceCustomFunctionExecute(FindDevice(deviceUID), isUSB, functionName);
			}, "DeviceCustomFunctionExecute");
		}

		public OperationResult<string> DeviceGetGuardUsersList(Guid deviceUID)
		{
			return SafeCallWithMonitoringSuspending<string>(() =>
			{
				return MainManager.DeviceGetGuardUsersList(FindDevice(deviceUID));
			}, "DeviceGetGuardUsersList");
		}

		public OperationResult<bool> DeviceSetGuardUsersList(Guid deviceUID, string users)
		{
			return SafeCallWithMonitoringSuspending<bool>(() =>
			{
				return MainManager.DeviceSetGuardUsersList(FindDevice(deviceUID), users);
			}, "DeviceSetGuardUsersList");
		}

		public OperationResult<string> DeviceGetMDS5Data(Guid deviceUID)
		{
			return SafeCallWithMonitoringSuspending<string>(() =>
			{
				return MainManager.DeviceGetMDS5Data(FindDevice(deviceUID));
			}, "DeviceGetMDS5Data");
		}

		public OperationResult<bool> SetConfigurationParameters(Guid deviceUID, List<Property> properties)
		{
			return SafeCall<bool>(() =>
			{
				return MainManager.SetConfigurationParameters(FindDevice(deviceUID), properties);
			}, "SetConfigurationParameters");
		}

		public OperationResult<List<Property>> GetConfigurationParameters(Guid deviceUID)
		{
			return SafeCall<List<Property>>(() =>
			{
				return MainManager.GetConfigurationParameters(FindDevice(deviceUID));
			}, "GetConfigurationParameters");
		}
		#endregion

		#region Helpers
		Device FindDevice(Guid deviceUID)
		{
			var device = ConfigurationManager.DeviceConfiguration.Devices.FirstOrDefault(x => x.UID == deviceUID);
			if (device == null)
				throw new FS2Exception("Невозможно выполнить операцию с устройством, отсутствующим в конфигурации");
			return device;
		}

		OperationResult<T> TaskSafeCallWithMonitoringSuspending<T>(Func<T> func, string methodName)
		{
			try
			{
				Trace.WriteLine("TaskSafeCallWithMonitoringSuspending");
				var cancellationTokenSource = new CancellationTokenSource();
				CancellationTokenSources.Add(cancellationTokenSource);
				var cancellationToken = cancellationTokenSource.Token;
				var task = Task.Factory.StartNew(
					() =>
						{
						return SafeCallWithMonitoringSuspending<T>(func, methodName);
						},
					cancellationToken);
				try
				{
					task.Wait();
				}
				catch (AggregateException ae)
				{
					return new OperationResult<T>("Операция отменена");
				}
				catch (Exception ex)
				{
					return new OperationResult<T>(ex.Message);
				}
				var result = task.Result;
				CancellationTokenSources.Remove(cancellationTokenSource);
				task.Dispose();
				cancellationTokenSource.Dispose();
				return result;
			}
			catch (Exception e)
			{
				return new OperationResult<T>(e.Message);
			}
		}

		OperationResult<T> SafeCallWithMonitoringSuspending<T>(Func<T> func, string methodName)
		{
			try
			{
				MainManager.SuspendMonitoring();
				return SafeCall<T>(func, methodName);
			}
			catch(Exception e)
			{
				return new OperationResult<T>(e.Message);
			}
			finally
			{
				MainManager.ResumeMonitoring();
			}
		}

		OperationResult<T> SafeCall<T>(Func<T> func, string methodName)
		{
			IsFirstCall = false;
			var resultData = new OperationResult<T>();
			try
			{
				var result = func();
				resultData.Result = result;
				resultData.HasError = false;
				resultData.Error = null;
				return resultData;
			}
			catch (Exception e)
			{
				string exceptionText = "Exception " + e.Message + " при вызове " + methodName;
				Logger.Error(e, exceptionText);
				resultData.Result = default(T);
				resultData.HasError = true;
				resultData.Error = e.Message;
			}
			return resultData;
		}
		#endregion
	}
}