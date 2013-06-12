﻿using FiresecAPI;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common.Windows;

namespace DevicesModule.ViewModels
{
    public static class FS2DeviceCustomFunctionExecuteHelper
    {
        static Device Device;
		static bool IsUsb;
        static string FunctionCode;
        static OperationResult<string> OperationResult;

		public static void Run(Device device, bool isUsb, string functionCode)
        {
            Device = device;
			IsUsb = isUsb;
            FunctionCode = functionCode;

			ServiceFactory.FS2ProgressService.Run(OnPropgress, OnlCompleted, device.PresentationAddressAndName + ". Выполнение функции");
        }

        static void OnPropgress()
        {
			OperationResult = FiresecManager.FS2ClientContract.DeviceCustomFunctionExecute(Device.UID, IsUsb, FunctionCode);
        }

        static void OnlCompleted()
        {
            if (OperationResult.HasError)
            {
				MessageBoxService.ShowError(OperationResult.Error, "Ошибка при выполнении операции");
                return;
            }
            var result = OperationResult.Result;
            result = result.Replace("[OK]", "");
            MessageBoxService.Show(result);
        }
    }
}