﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;

namespace TestWebServer
{
	[ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, IncludeExceptionDetailInFaults=true)]
	public class TestContract : ITestContract
	{
		public string NewEventsAvailable(string mask)
		{
			MainWindow.AddText("NewEvent mask = " + mask.ToString());
			return "done";
		}
	}
}