﻿using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using FiresecAPI.Models;

namespace DeviceLibrary.Models
{
    [DataContract]
    [Serializable]
    public class State
    {
        [DataMember]
        [XmlAttribute]
        public StateType StateType { get; set; }

        [DataMember]
        [XmlAttribute]
        public string Code { get; set; }

        [DataMember]
        public List<Frame> Frames { get; set; }
    }
}