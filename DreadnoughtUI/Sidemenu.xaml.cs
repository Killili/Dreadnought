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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DreadnoughtUI {
	/// <summary>
	/// Interaction logic for UserControl1.xaml
	/// </summary>
	public partial class Sidemenu : UserControl {
		public string Output { get; set; }

		public Graph Graph;

		public Sidemenu() {
			InitializeComponent();
			Output = "";
		}

		private void button1_Click(object sender, RoutedEventArgs e) {
			if(Graph == null) {
				Graph = new Graph();
				//Graph.Chart.DataContext = new KeyValuePair<int, int>[] { new KeyValuePair<int, int>(0, 0) };
			}
			Graph.Show();
		}
	}
}
