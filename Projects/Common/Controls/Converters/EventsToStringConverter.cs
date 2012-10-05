﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Data;
using FiresecAPI;
using FiresecAPI.Models;

namespace Controls.Converters
{
	public class EventsToStringConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			var stateTypes = value as List<StateType>;
			if (stateTypes.IsNotNullOrEmpty())
			{
				var delimString = " или ";
				var result = new StringBuilder();

				foreach (var stateType in stateTypes)
				{
					result.Append(stateType.ToDescription());
					result.Append(delimString);
				}

				return result.ToString().Remove(result.Length - delimString.Length);
			}

			return null;
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}