using System;
using System.Collections.Generic;
using System.Windows.Data;

namespace Zander.Modules.ServerBrowser.Converters {
	public class CollectionToCommaList : IValueConverter {
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			var convertedValue = (IEnumerable<string>)value;

			return string.Join(", ", convertedValue);
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			throw new NotImplementedException();
		}
	}
}
