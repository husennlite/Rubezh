﻿
namespace FiresecClient.Models
{
    public class ZoneState
    {
        public string No { get; private set; }
        public State State { get; set; }

        public ZoneState(string no)
        {
            No = no;
            State = new State(8);
        }
    }
}
