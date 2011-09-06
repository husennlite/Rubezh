﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace FiresecAPI.Models
{
    [DataContract]
    public class DeviceState
    {
        public DeviceState()
        {
            States = new List<DeviceDriverState>();
            ParentStates = new List<ParentDeviceState>();
            Parameters = new List<Parameter>();
        }

        public Device Device { get; set; }
        public string PlaceInTree { get; set; }

        [DataMember]
        public Guid UID { get; set; }

        [DataMember]
        public string Id { get; set; }

        [DataMember]
        public List<DeviceDriverState> States { get; set; }

        [DataMember]
        public List<ParentDeviceState> ParentStates { get; set; }

        [DataMember]
        public List<Parameter> Parameters { get; set; }

        public StateType StateType
        {
            get
            {
                var stateTypes = new List<StateType>() { (StateType) 7 };

                stateTypes.AddRange(from DeviceDriverState deviceDriverState in States
                                    where deviceDriverState.IsActive
                                    select deviceDriverState.DriverState.StateType);

                stateTypes.AddRange(from ParentDeviceState parentDeviceState in ParentStates
                                    select parentDeviceState.DriverState.StateType);

                return stateTypes.Min();
            }
        }

        public List<string> ParentStringStates
        {
            get
            {
                var parentStringStates = new List<string>();
                foreach (var parentDeviceState in ParentStates)
                {
                    parentStringStates.Add(parentDeviceState.ParentDevice.Driver.ShortName + " - " + parentDeviceState.DriverState.Name);
                }
                return parentStringStates;
            }
        }

        public bool IsDisabled
        {
            get { return States.Any(x => x.IsActive && x.DriverState.StateType == StateType.Off); }
        }

        public void CopyFrom(DeviceState deviceState)
        {
            Id = deviceState.Id;
            States = deviceState.States;
            ParentStates = deviceState.ParentStates;
            Parameters = deviceState.Parameters;
        }

        #region Alarming

        bool _isAutomaticOff = false;
        public bool IsAutomaticOff
        {
            get { return _isAutomaticOff; }
            set
            {
                if (_isAutomaticOff != value)
                {
                    _isAutomaticOff = value;
                    if (value)
                        AlarmAdded(AlarmType.Auto, UID);
                    else
                        AlarmRemoved(AlarmType.Auto, UID);
                }
            }
        }

        bool _isOff = false;
        public bool IsOff
        {
            get { return _isOff; }
            set
            {
                if (_isOff != value)
                {
                    _isOff = value;
                    if (value)
                        AlarmAdded(AlarmType.Off, UID);
                    else
                        AlarmRemoved(AlarmType.Off, UID);
                }
            }
        }

        bool _isFailure = false;
        public bool IsFailure
        {
            get { return _isFailure; }
            set
            {
                if (_isFailure != value)
                {
                    _isFailure = value;
                    if (value)
                        AlarmAdded(AlarmType.Failure, UID);
                    else
                        AlarmRemoved(AlarmType.Failure, UID);
                }
            }
        }

        bool _isFire = false;
        public bool IsFire
        {
            get { return _isFire; }
            set
            {
                if (_isFire != value)
                {
                    _isFire = value;
                    if (value)
                        AlarmAdded(AlarmType.Fire, UID);
                    else
                        AlarmRemoved(AlarmType.Fire, UID);
                }
            }
        }

        bool _isAttention = false;
        public bool IsAttention
        {
            get { return _isAttention; }
            set
            {
                if (_isAttention != value)
                {
                    _isAttention = value;
                    if (value)
                        AlarmAdded(AlarmType.Attention, UID);
                    else
                        AlarmRemoved(AlarmType.Attention, UID);
                }
            }
        }

        bool _isInfo = false;
        public bool IsInfo
        {
            get { return _isInfo; }
            set
            {
                if (_isInfo != value)
                {
                    _isInfo = value;
                    if (value)
                        AlarmAdded(AlarmType.Info, UID);
                    else
                        AlarmRemoved(AlarmType.Info, UID);
                }
            }
        }

        bool _isService = false;
        public bool IsService
        {
            get { return _isService; }
            set
            {
                if (_isService != value)
                {
                    _isService = value;
                    if (value)
                        AlarmAdded(AlarmType.Service, UID);
                    else
                        AlarmRemoved(AlarmType.Service, UID);
                }
            }
        }

        public static event Action<AlarmType, Guid> AlarmAdded;
        static void OnAlarmAdded(AlarmType alarmType, Guid deviceUID)
        {
            if (AlarmAdded != null)
                AlarmAdded(alarmType, deviceUID);
        }

        public static event Action<AlarmType, Guid> AlarmRemoved;
        static void OnAlarmRemoved(AlarmType alarmType, Guid deviceUID)
        {
            if (AlarmRemoved != null)
                AlarmRemoved(alarmType, deviceUID);
        }

        #endregion Automatic

        public event Action StateChanged;
        public void OnStateChanged()
        {
            if (StateChanged != null)
                StateChanged();
        }
    }
}