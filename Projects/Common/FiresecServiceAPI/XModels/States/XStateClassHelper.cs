﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XFiresecAPI
{
    public static class XStateClassHelper
    {
        public static List<XStateClass> Convert(List<XStateType> stateTypes, bool isConnectionLost)
        {
            var stateClasses = new HashSet<XStateClass>();
            if (isConnectionLost)
            {
                stateClasses.Add(XStateClass.Unknown);
                return stateClasses.ToList();
            }

            foreach (var stateType in stateTypes)
            {
                switch (stateType)
                {
                    case XStateType.Fire2:
                        stateClasses.Add(XStateClass.Fire2);
                        break;
                    case XStateType.Fire1:
                        stateClasses.Add(XStateClass.Fire1);
                        break;
                    case XStateType.Attention:
                        stateClasses.Add(XStateClass.Attention);
                        break;
                    case XStateType.Failure:
                        stateClasses.Add(XStateClass.Failure);
                        break;
                    case XStateType.Ignore:
                        stateClasses.Add(XStateClass.Ignore);
                        break;
                    case XStateType.On:
                        stateClasses.Add(XStateClass.On);
                        break;
                    case XStateType.Test:
                        stateClasses.Add(XStateClass.Info);
                        break;
                }
            }

            if (!stateTypes.Contains(XStateType.Norm))
            {
                stateClasses.Add(XStateClass.AutoOff);
            }
            //stateClasses.Add(XStateClass.Service);

            return stateClasses.ToList();
        }

        public static XStateClass GetMinStateClass(List<XStateClass> stateClasses)
        {
            XStateClass minStateClass = XStateClass.Norm;
            foreach (var stateClass in stateClasses)
            {
                if (stateClass < minStateClass)
                    minStateClass = stateClass;
            }
            return minStateClass;
        }
    }
}