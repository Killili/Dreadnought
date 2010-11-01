using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Controls.DataVisualization.Charting;
using System.Reflection;

namespace DreadnoughtUI {
	/// <summary>
	/// Interaction logic for Graph.xaml
	/// </summary>
	public partial class Graph : Window {








		public SolidColorBrush MyColor {
			get { return Brushes.Green; }
		}


		public System.Windows.Controls.DataVisualization.Charting.Chart Chart { get { return this.chart; } }
		public Graph() {
			InitializeComponent();
			this.DataContext = this;
		}

		private void button1_Click(object sender, RoutedEventArgs e) {
			AddLine("Test", new KeyValuePair<int, float>[] { new KeyValuePair<int, float>(1, 1), new KeyValuePair<int, float>(2, 2) });
		}

		private int colorPos = 10;
		private Color getNextColor(){
			colorPos += 1;
			int pos = colorPos;
			foreach(PropertyInfo p in typeof(Colors).GetProperties()) {
				if(p.PropertyType == typeof(Color)) {
					pos--;
					if(pos <= 0)
						return (Color)p.GetGetMethod().Invoke(typeof(Colors), null);
				}	
			}
			colorPos = 10;
			return Colors.Green;
		}

		public void AddLine(string name, KeyValuePair<int, float>[] data) {
			LineSeries ls = new LineSeries();
			ls.DependentValueBinding = new Binding("Value");
			ls.IndependentValueBinding = new Binding("Key");
			ls.ItemsSource = data;
			ls.Title = name;
			Style originalStyle = Resources["DataPointStyle"] as Style;
			var dpStyle = new Style() { BasedOn = originalStyle };
			dpStyle.TargetType = typeof(LineDataPoint);
			dpStyle.Setters.Add(new Setter(BackgroundProperty, new SolidColorBrush(getNextColor())));
			ls.DataPointStyle = dpStyle;
			chart.Series.Add(ls);
			((LegendItem)ls.LegendItems[0]).MouseUp += delegate { chart.Series.Remove(ls); };
		}
	}
}
