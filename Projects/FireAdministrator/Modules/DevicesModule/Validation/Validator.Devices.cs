﻿using System;
using System.Collections.Generic;
using System.Linq;
using FiresecAPI;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure.Common.Validation;

namespace DevicesModule.Validation
{
    partial class Validator
    {
        void ValidateDevices()
        {
            _validateDevicesWithSerialNumber = new List<Guid>();

            ValidatePduCount();

            foreach (var device in _firesecConfiguration.DeviceConfiguration.Devices)
            {
                if (device.CanBeNotUsed && device.IsNotUsed)
                    continue;

                ValidateAddressEquality(device);
                ValidateDeviceIndicatorOtherNetwor(device);
                ValidateDeviceOnInvalidChars(device);
                ValidateDeviceMaxDeviceOnLine(device);
                ValidateDeviceOwnerZone(device);
                ValidateDeviceAddressRange(device);
                ValidateDeviceOnEmpty(device);
                ValidateDeviceZoneLogic(device);
                ValidateDeviceSingleInParent(device);
                ValidateDeviceConflictAddressWithMSChannel(device);
                ValidateDeviceDuplicateSerial(device);
                ValidateDeviceSecurity(device);
                ValidateDeviceEvents(device);
                ValidateDeviceLoopLines(device);
                ValidateDeviceMaxExtCount(device);
                ValidateDeviceSecurityPanel(device);
                ValidateDeviceRangeAddress(device);
                ValidateMRK30(device);
                ValidatePumpStation(device);
            }
        }

        void ValidatePduCount()
        {
            int pduCount = 0;

            foreach (var device in _firesecConfiguration.DeviceConfiguration.Devices)
            {
                if (device.Driver.DriverType == DriverType.PDU)
                {
                    ++pduCount;
                }
            }
            if (pduCount > 10)
                _errors.Add(new CommonValidationError("FS", "Устройства", string.Empty, string.Format("Максимальное количество ПДУ - 10, сейчас - {0}", pduCount), ValidationErrorLevel.Warning));
        }

        void ValidateAddressEquality(Device device)
        {
            var addresses = new List<int>();
            foreach (var childDevice in _firesecConfiguration.GetAllChildrenForDevice(device))
            {
                if (childDevice.Driver.HasAddress)
                {
                    if (addresses.Contains(childDevice.IntAddress))
                        _errors.Add(new DeviceValidationError(childDevice, string.Format("Дублируется адрес устройства"), ValidationErrorLevel.CannotWrite));
                    else
                        addresses.Add(childDevice.IntAddress);
                }
            }
        }

        void ValidateDeviceIndicatorOtherNetwor(Device device)
        {
            if (device.Driver.DriverType == DriverType.Indicator)
            {
                if (device.IndicatorLogic.IndicatorLogicType == IndicatorLogicType.Zone)
                    ValidateDeviceIndicatorOtherNetworkZone(device);
                else
                    ValidateDeviceIndicatorOtherNetworkDevice(device);
            }
        }

        void ValidateDeviceIndicatorOtherNetworkDevice(Device device)
        {
            if ((device.Driver.DriverType == DriverType.Indicator) && (device.IndicatorLogic.Device != null))
            {
                if ((device.IndicatorLogic.Device.ParentChannel == null) && (device.IndicatorLogic.Device.ParentChannel.UID != device.ParentChannel.UID))
                    _errors.Add(new DeviceValidationError(device, "Для индикатора указано устройство находящееся в другой сети RS-485", ValidationErrorLevel.CannotWrite));
            }
        }

        void ValidateDeviceIndicatorOtherNetworkZone(Device device)
        {
            if (device.Driver.DriverType == DriverType.Indicator)
            {
                foreach (var zoneUID in device.IndicatorLogic.ZoneUIDs)
                {
                    var zone = _firesecConfiguration.DeviceConfiguration.Zones.FirstOrDefault(x => x.UID == zoneUID);
                    if ((zone.DevicesInZone.Count > 0) && (zone.DevicesInZone.All(x => ((x.ParentChannel != null) && (x.ParentChannel.UID == device.ParentChannel.UID)) == false)))
                        _errors.Add(new DeviceValidationError(device, string.Format("Для индикатора указана зона ({0}) имеющая устройства другой сети RS-485", zone.No), ValidationErrorLevel.CannotWrite));
                }
            }
        }

        void ValidateDeviceMaxDeviceOnLine(Device device)
        {
            if (device.Driver.HasShleif)
            {
                for (int i = 1; i <= device.Driver.ShleifCount; i++)
                {
                    if (device.Children.Count(x => x.IntAddress >> 8 == i) > 255)
                    {
                        _errors.Add(new DeviceValidationError(device, "Число устройств на шлейфе не может превышать 255", ValidationErrorLevel.CannotWrite));
                        return;
                    }
                }
            }
        }

        void ValidateDeviceOnInvalidChars(Device device)
        {
            if (string.IsNullOrWhiteSpace(device.Description) == false)
                if (ValidateString(device.Description) == false)
                    _errors.Add(new DeviceValidationError(device, string.Format("Символы \"{0}\" не допустимы для записи в устройства", InvalidChars(device.Description)), ValidationErrorLevel.CannotWrite));
        }

        void ValidateDeviceOwnerZone(Device device)
        {
            if (device.Driver.IsZoneDevice && device.ZoneUID == Guid.Empty)
                _errors.Add(new DeviceValidationError(device, "Устройство должно содержать хотя бы одну зону", ValidationErrorLevel.CannotWrite));
        }

        void ValidateDeviceAddressRange(Device device)
        {
            if (device.Driver.HasAddress && device.Driver.IsRangeEnabled && (device.IntAddress > device.Driver.MaxAddress || device.IntAddress < device.Driver.MinAddress))
                _errors.Add(new DeviceValidationError(device, string.Format("Устройство должно иметь адрес в диапазоне {0}-{1}", device.Driver.MinAddress, device.Driver.MaxAddress), ValidationErrorLevel.CannotWrite));
        }

        void ValidateDeviceOnEmpty(Device device)
        {
            if (device.Driver.CanWriteDatabase && device.Driver.IsNotValidateZoneAndChildren == false && device.Children.Where(x => x.Driver.IsAutoCreate == false).Count() == 0)
                _errors.Add(new DeviceValidationError(device, "Устройство должно содержать подключенные устройства", ValidationErrorLevel.CannotWrite));
        }

        void ValidateDeviceZoneLogic(Device device)
        {
            if (device.Driver.IsZoneLogicDevice && (device.ZoneLogic == null || device.ZoneLogic.Clauses.Count == 0) &&
                device.Driver.DriverType != DriverType.ASPT && device.Driver.DriverType != DriverType.Exit && device.Driver.DriverType != DriverType.PumpStation)
                _errors.Add(new DeviceValidationError(device, "Отсутствуют настроенные режимы срабатывания", ValidationErrorLevel.CannotWrite));
        }

        void ValidateDeviceSingleInParent(Device device)
        {
            if (device.Driver.IsSingleInParent && device.Parent.Children.Count(x => x.DriverUID == device.DriverUID) > 1)
                _errors.Add(new DeviceValidationError(device, "Устройство должно быть в единственном числе", ValidationErrorLevel.CannotWrite));
        }

        void ValidateDeviceConflictAddressWithMSChannel(Device device)
        {
            var driverAddressProperty = device.Driver.Properties.FirstOrDefault(x => x.Name == "Address");
            if (driverAddressProperty != null)
            {
                var deviceAddressProperty = device.Properties.FirstOrDefault(x => x.Name == driverAddressProperty.Name);
                var address = deviceAddressProperty == null ? driverAddressProperty.Default : deviceAddressProperty.Value;

                var children = device.Children.FirstOrDefault(x => x.AddressFullPath == address);
                if (children != null)
                    _errors.Add(new DeviceValidationError(children, "Конфликт адреса с адресом канала МС", ValidationErrorLevel.CannotWrite));
            }
        }

        void ValidateDeviceDuplicateSerial(Device device)
        {
            var driverSerialNumberProperty = device.Driver.Properties.FirstOrDefault(x => x.Name == "SerialNo");
            if (driverSerialNumberProperty == null || _validateDevicesWithSerialNumber.Contains(device.DriverUID))
                return;

            var similarDevices = device.Parent.Children.Where(x => x.DriverUID == device.DriverUID).ToList();
            if (similarDevices.Count > 1)
            {
                _validateDevicesWithSerialNumber.Add(device.DriverUID);
                var serialNumbers = similarDevices.Select(x => GetSerialNumber(x)).ToList();
                for (int i = 0; i < serialNumbers.Count; i++)
                {
                    if (string.IsNullOrWhiteSpace(serialNumbers[i]) || serialNumbers.Count(x => x == serialNumbers[i]) > 1)
                        _errors.Add(new DeviceValidationError(similarDevices[i], "При наличии в конфигурации одинаковых USB устройств, их серийные номера должны быть указаны и отличны", ValidationErrorLevel.CannotWrite));
                }
            }
        }

        string GetSerialNumber(Device device)
        {
            var deviceSerialNumberProperty = device.Properties.FirstOrDefault(x => x.Name == "SerialNo");
            return deviceSerialNumberProperty == null ? device.Driver.Properties.First(x => x.Name == "SerialNo").Default : deviceSerialNumberProperty.Value;
        }

        void ValidateDeviceSecurity(Device device)
        {
            if (device.Driver.DeviceType == DeviceType.Sequrity)
            {
                if ((device.IntAddress & 0xff) > 250)
                    _errors.Add(new DeviceValidationError(device, "Не рекомендуется использовать адрес охранного устройства больше 250", ValidationErrorLevel.CannotWrite));
                if (device.Parent.Driver.IsChildAddressReservedRange)
                    return;
                if (device.Parent.Driver.Properties.Any(x => x.Name == "DeviceCountSecDev") == false)
                    _errors.Add(new DeviceValidationError(device, "Устройство подключено к недопустимому устройству", ValidationErrorLevel.CannotWrite));
            }
        }

        void ValidateDeviceEvents(Device device)
        {
            var eventProperty = device.Properties.FirstOrDefault(x => x.Name == "Event1");
            if (eventProperty != null && eventProperty.Value.Length > 20)
            {
                _errors.Add(new DeviceValidationError(device, "Длинное описание события - в прибор будет записано первые 20 символов", ValidationErrorLevel.Warning));
            }
            else
            {
                eventProperty = device.Properties.FirstOrDefault(x => x.Name == "Event2");
                if (eventProperty != null && eventProperty.Value.Length > 20)
                    _errors.Add(new DeviceValidationError(device, "Длинное описание события - в прибор будет записано первые 20 символов", ValidationErrorLevel.Warning));
            }
        }

        void ValidateDeviceLoopLines(Device device)
        {
            var loopLineProperty = device.Properties.FirstOrDefault(x => x.Name == "LoopLine1");
            if (loopLineProperty != null)
            {
                var badChildren = device.Children.Where(x => x.IntAddress >> 8 == 2).ToList();
                badChildren.ForEach(x => _errors.Add(new DeviceValidationError(x, "Данное устройство находится на четном номере АЛС, что недопустимо для кольцевых АЛС", ValidationErrorLevel.CannotWrite)));
            }

            loopLineProperty = device.Properties.FirstOrDefault(x => x.Name == "LoopLine2");
            if (loopLineProperty != null)
            {
                var badChildren = device.Children.Where(x => x.IntAddress >> 8 == 4).ToList();
                badChildren.ForEach(x => _errors.Add(new DeviceValidationError(x, "Данное устройство находится на четном номере АЛС, что недопустимо для кольцевых АЛС", ValidationErrorLevel.CannotWrite)));
            }
        }

        void ValidateDeviceMaxExtCount(Device device)
        {
            if (device.Driver.HasShleif && device.Children.IsNotNullOrEmpty())
            {
                var childZones = new List<Zone>();
                foreach (var childDevice in device.Children)
                {
                    if (childDevice.Driver.IsZoneDevice && childDevice.ZoneUID != Guid.Empty)
                    {
                        var zone = _firesecConfiguration.DeviceConfiguration.Zones.FirstOrDefault(x => x.UID == childDevice.ZoneUID);
                        if (zone != null)
                            childZones.Add(zone);
                    }
                }

                var childZonesDevices = new List<Device>();
                childZones.ForEach(x => childZonesDevices.AddRange(x.DevicesInZoneLogic));

                int extendedDevicesCount = childZonesDevices.Where(x => x.Driver.IsZoneLogicDevice && x.Parent != device).Distinct().Count();
                if (extendedDevicesCount > 250)
                    _errors.Add(new DeviceValidationError(device, string.Format("В приборе не может быть более 250 внешних устройств. Сейчас : {0}", extendedDevicesCount), ValidationErrorLevel.CannotWrite));
            }
        }

        void ValidateDeviceSecurityPanel(Device device)
        {
            var driverSecurityDeviceCountProperty = device.Driver.Properties.FirstOrDefault(x => x.Name == "DeviceCountSecDev");
            if (driverSecurityDeviceCountProperty != null)
            {
                var securityDeviceCountPropertyValue = driverSecurityDeviceCountProperty.Parameters.First(x => x.Value == driverSecurityDeviceCountProperty.Default).Name;
                var deviceSecurityDeviceCountProperty = device.Properties.FirstOrDefault(x => x.Name == "DeviceCountSecDev");
                if (deviceSecurityDeviceCountProperty != null)
                    securityDeviceCountPropertyValue = deviceSecurityDeviceCountProperty.Value;

                if (securityDeviceCountPropertyValue == driverSecurityDeviceCountProperty.Parameters[0].Name)
                    ValidateDeviceCountAndOrderOnShlief(device, 64, 0);
                else if (securityDeviceCountPropertyValue == driverSecurityDeviceCountProperty.Parameters[1].Name)
                    ValidateDeviceCountAndOrderOnShlief(device, 48, 16);
                else if (securityDeviceCountPropertyValue == driverSecurityDeviceCountProperty.Parameters[2].Name)
                    ValidateDeviceCountAndOrderOnShlief(device, 32, 32);
                else if (securityDeviceCountPropertyValue == driverSecurityDeviceCountProperty.Parameters[3].Name)
                    ValidateDeviceCountAndOrderOnShlief(device, 16, 48);
                else if (securityDeviceCountPropertyValue == driverSecurityDeviceCountProperty.Parameters[4].Name)
                    ValidateDeviceCountAndOrderOnShlief(device, 0, 64);
            }
        }

        void ValidateDeviceCountAndOrderOnShlief(Device device, int firstShliefMaxCount, int secondShliefMaxCount)
        {
            int deviceOnFirstShliefCount = 0;
            int deviceOnSecondShliefCount = 0;
            int shliefNumber = 0;
            int firstShliefDeviceNumber = 0;
            int firstShliefDevicePrevNumber = 0;
            int secondShliefDeviceNumber = 0;
            int secondShliefDevicePrevNumber = 0;
            bool isFirstShliefOrederCorrupt = false;
            bool isSecondShliefOrederCorrupt = false;

            foreach (var intAddress in device.Children.Where(x => x.Driver.DeviceType == DeviceType.Sequrity).Select(x => x.IntAddress))
            {
                shliefNumber = intAddress >> 8;
                if (shliefNumber == 1)
                {
                    ++deviceOnFirstShliefCount;
                    firstShliefDevicePrevNumber = firstShliefDeviceNumber;
                    firstShliefDeviceNumber = intAddress & 0xff;
                    if (isFirstShliefOrederCorrupt == false)
                    {
                        if (firstShliefDeviceNumber < 176 || (firstShliefDevicePrevNumber > 0 && (firstShliefDeviceNumber - firstShliefDevicePrevNumber) > 1))
                            isFirstShliefOrederCorrupt = true;
                    }
                }
                else if (shliefNumber == 2)
                {
                    ++deviceOnSecondShliefCount;
                    secondShliefDevicePrevNumber = secondShliefDeviceNumber;
                    secondShliefDeviceNumber = intAddress & 0xff;
                    if (isSecondShliefOrederCorrupt == false)
                    {
                        if (secondShliefDeviceNumber < 176 || (secondShliefDevicePrevNumber > 0 && (secondShliefDeviceNumber - secondShliefDevicePrevNumber) > 1))
                            isSecondShliefOrederCorrupt = true;
                    }
                }
            }
            if (deviceOnFirstShliefCount > firstShliefMaxCount)
                _errors.Add(new DeviceValidationError(device, "Превышено максимальное количество подключаемых охранных устройств на 1-ом шлейфе", ValidationErrorLevel.CannotWrite));
            if (deviceOnSecondShliefCount > secondShliefMaxCount)
                _errors.Add(new DeviceValidationError(device, "Превышено максимальное количество подключаемых охранных устройств на 2-ом шлейфе", ValidationErrorLevel.CannotWrite));
            if (isFirstShliefOrederCorrupt)
                _errors.Add(new DeviceValidationError(device, "Рекомендуется неразрывная последовательность адресов охранных устройств на 1-ом шлейфе начиная  с 176 адреса", ValidationErrorLevel.Warning));
            if (isSecondShliefOrederCorrupt)
                _errors.Add(new DeviceValidationError(device, "Рекомендуется неразрывная последовательность адресов охранных устройств на 2-ом шлейфе начиная  с 176 адреса", ValidationErrorLevel.Warning));
        }

        void ValidateDeviceRangeAddress(Device device)
        {
            if (device.Driver.IsChildAddressReservedRange && device.Driver.ChildAddressReserveRangeCount > 0)
            {
                if (device.Children.Any(x => x.IntAddress < device.IntAddress || (x.IntAddress - device.IntAddress) > device.Driver.ChildAddressReserveRangeCount))
                    _errors.Add(new DeviceValidationError(device, string.Format("Для всех подключенных устройтв необходимо выбрать адрес из диапазона: {0}", device.PresentationAddress), ValidationErrorLevel.Warning));
            }
        }

        void ValidateMRK30(Device device)
        {
            if (device.Driver.DriverType == DriverType.MRK_30)
            {
                var reservedCount = device.GetReservedCount();
                if (reservedCount < 1 || reservedCount > 30)
                    _errors.Add(new DeviceValidationError(device, string.Format("Количество подключаемых устройств должно быть в диапазоне 1 - 30: {0}", device.PresentationAddress), ValidationErrorLevel.CannotWrite));

                int minChildAddress = device.IntAddress + 1;
                int maxChildAddress = device.IntAddress + reservedCount;
                if (maxChildAddress / 256 != minChildAddress / 256)
                    maxChildAddress = maxChildAddress - maxChildAddress % 256;

                foreach (var childDevice in device.Parent.Children)
                {
                    if ((childDevice.IntAddress >= minChildAddress) && (childDevice.IntAddress <= maxChildAddress))
                    {
                        if (childDevice.Parent.UID != device.UID)
                            _errors.Add(new DeviceValidationError(device, string.Format("Устройство находится в зарезервированном диапазоне адресов МРК-30: {0}", childDevice.PresentationAddress), ValidationErrorLevel.CannotWrite));
                    }
                }

                foreach (var childDevice in device.Children)
                {
                    if ((childDevice.IntAddress < minChildAddress) && (childDevice.IntAddress > maxChildAddress))
                    {
                        _errors.Add(new DeviceValidationError(device, string.Format("Устройство находится за пределами диапазона адресов МРК-30: {0}", childDevice.PresentationAddress), ValidationErrorLevel.CannotWrite));
                    }
                }
            }
        }

        void ValidatePumpStation(Device device)
        {
            if (device.Driver.DriverType == DriverType.PumpStation)
            {
                if (device.Children.Count > 0 && device.ZoneLogic.Clauses.Count == 0)
                    _errors.Add(new DeviceValidationError(device, "Отсутствуют настроенные режимы срабатывания", ValidationErrorLevel.CannotWrite));
            }
        }

        bool ValidateString(string str)
        {
            return str.All(x => ValidChars.Contains(x));
        }

        string InvalidChars(string str)
        {
            return new string(str.Where(x => ValidChars.Contains(x) == false).ToArray());
        }
    }
}