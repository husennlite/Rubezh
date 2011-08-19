﻿using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace DeviceLibrary.Models
{
    [DataContract]
    [Serializable]
    public class Device
    {
        [DataMember]
        [XmlAttribute]
        public string Id { get; set; }

        [DataMember]
        public List<State> States { get; set; }
    }
}