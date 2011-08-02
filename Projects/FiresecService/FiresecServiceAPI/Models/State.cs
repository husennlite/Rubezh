﻿using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace FiresecAPI.Models
{
    public enum StateType
    {
        Fire = 0,
        Attention = 1,
        Failure = 2,
        Service = 3,
        Off = 4,
        Unknown = 5,
        Info = 6,
        Norm = 7,
        No = 8
    }

    [Serializable]
    [DataContract]
    public class State
    {
        [XmlAttribute]
        [DataMember]
        public int Id { get; set; }

        public StateType StateType
        {
            get { return (StateType) Id; }
        }

        public string EventName
        {
            get
            {
                switch (Id)
                {
                    case 0:
                        return "Тревога";

                    case 1:
                        return "Внимание";

                    case 2:
                        return "Неисправность";

                    case 3:
                        return "Требуется обслуживание";

                    case 4:
                        return "Тревоги отключены";

                    case 6:
                        return "Информация";

                    case 7:
                        return "Прочие";

                    default:
                        return "";
                }
            }
        }

        public override string ToString()
        {
            switch (Id)
            {
                case 0:
                    return "Тревога";

                case 1:
                    return "Внимание (предтревожное)";

                case 2:
                    return "Неисправность";

                case 3:
                    return "Требуется обслуживание";

                case 4:
                    return "Обход устройств";

                case 5:
                    return "Неопределено";

                case 6:
                    return "Норма(*)";

                case 7:
                    return "Норма";

                case 8:
                    return "Нет состояния";

                default:
                    return "";
            }
        }

        public static bool operator ==(State value1, State value2)
        {
            if (value1 == null && value2 == null)
            {
                return true;
            }

            if (value1 != null && value2 != null)
            {
                return value1.Id == value2.Id;
            }

            return false;
        }

        public static bool operator !=(State value1, State value2)
        {
            if (value1 == null && value2 == null)
            {
                return false;
            }

            if (value1 != null && value2 != null)
            {
                return value1.Id == value2.Id;
            }

            return true;
        }
    }
}
