﻿using System.Linq;
using Common;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Events;

namespace JournalModule.ViewModels
{
    public class JournalRecordViewModel : BaseViewModel
    {
        readonly JournalRecord _journalRecord;
        readonly FiresecAPI.Models.Device _device;

        public JournalRecordViewModel(JournalRecord journalRecord)
        {
            ShowPlanCommand = new RelayCommand(OnShowPlan, CanShowPlan);
            ShowTreeCommand = new RelayCommand(OnShowTree, CanShowTree);
            ShowZoneCommand = new RelayCommand(OnShowZone, CanShowZone);

            _journalRecord = journalRecord;
            _device = FiresecManager.DeviceConfiguration.Devices.FirstOrDefault(
                x => x.DatabaseId == journalRecord.DeviceDatabaseId ||
                     x.DatabaseId == _journalRecord.PanelDatabaseId);
        }

        public int Id
        {
            get { return _journalRecord.No; }
        }

        public string DeviceTime
        {
            get { return _journalRecord.DeviceTime.ToString(); }
        }

        public string SystemTime
        {
            get { return _journalRecord.SystemTime.ToString(); }
        }

        public string ZoneName
        {
            get { return _journalRecord.ZoneName; }
        }

        public string Description
        {
            get { return _journalRecord.Description; }
        }

        public string Device
        {
            get { return _journalRecord.DeviceName; }
        }

        public string Panel
        {
            get { return _journalRecord.PanelName; }
        }

        public string User
        {
            get { return _journalRecord.User; }
        }

        public StateType StateType
        {
            get { return _journalRecord.StateType; }
        }

        public RelayCommand ShowPlanCommand { get; private set; }
        void OnShowPlan()
        {
            ServiceFactory.Events.GetEvent<ShowDeviceOnPlanEvent>().Publish(_device.Id);
        }

        bool CanShowPlan()
        {
            if (_device != null)
                return ExistsOnPlan();
            return false;
        }

        bool ExistsOnPlan()
        {
            foreach (var plan in FiresecManager.PlansConfiguration.Plans)
            {
                if (plan != null && plan.ElementDevices.IsNotNullOrEmpty())
                {
                    var elementDevice = plan.ElementDevices.FirstOrDefault(x => x.Id == _device.Id);
                    if (elementDevice != null)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public RelayCommand ShowTreeCommand { get; private set; }
        void OnShowTree()
        {
            ServiceFactory.Events.GetEvent<ShowDeviceEvent>().Publish(_device.Id);
        }

        bool CanShowTree()
        {
            return _device != null;
        }

        public RelayCommand ShowZoneCommand { get; private set; }
        void OnShowZone()
        {
            ServiceFactory.Events.GetEvent<ShowZoneEvent>().Publish(_device.ZoneNo);
        }

        bool CanShowZone()
        {
            if (_device != null)
            {
                return FiresecManager.DeviceConfiguration.Zones.Any(x => x.No == _device.ZoneNo);
            }
            return false;
        }
    }
}