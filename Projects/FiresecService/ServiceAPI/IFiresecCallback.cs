﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;

namespace ServiceAPI
{
    public interface IFiresecCallback
    {
        [OperationContract(IsOneWay = true)]
        void StateChanged(string deviceId);
    }
}
