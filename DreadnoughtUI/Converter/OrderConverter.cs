using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using DreadnoughtOvermind.Orders;
using System.Windows.Media;

namespace DreadnoughtUI.Converter {
	[ValueConversion(typeof(Order.States), typeof(Brush))]
	public class OrderConverter : IValueConverter {
		#region IValueConverter Members

		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			Order.States state = (Order.States)value;
			
			switch(state) {
				case Order.States.Send:
					return new SolidColorBrush(Colors.Gray);
				case Order.States.InProgress:
					return new SolidColorBrush(Colors.Yellow);
				case Order.States.Done:
					return new SolidColorBrush(Colors.Green);
			}

			return new SolidColorBrush(Colors.LightGray);
		}
		
		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			throw new NotImplementedException();
		}

		#endregion
	}
}
