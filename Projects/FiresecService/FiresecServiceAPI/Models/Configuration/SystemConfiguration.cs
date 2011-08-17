﻿using System.Collections.Generic;
using System.Runtime.Serialization;

namespace FiresecAPI.Models
{
    [DataContract]
    public class SystemConfiguration
    {
        public SystemConfiguration()
        {
            Sounds = new List<Sound>();
            JournalFilters = new List<JournalFilter>();
            Instructions = new List<Instruction>();
            //Plans = new List<Plan>();
        }

        [DataMember]
        public List<Sound> Sounds { get; set; }

        [DataMember]
        public List<JournalFilter> JournalFilters { get; set; }

        [DataMember]
        public List<Instruction> Instructions { get; set; }

        //[DataMember]
        //public List<Plan> Plans { get; set; }

    }
}
