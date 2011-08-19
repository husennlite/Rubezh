﻿using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace DeviceLibrary.Models
{
    [DataContract]
    [Serializable]
    public class Frame
    {
        [DataMember]
        [XmlAttribute]
        public int Id { get; set; }

        [DataMember]
        [XmlAttribute]
        public int Duration { get; set; }

        [DataMember]
        [XmlAttribute]
        public int Layer { get; set; }

        [DataMember]
        public string Image { get; set; }
    }
}