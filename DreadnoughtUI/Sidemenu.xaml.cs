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
	public class OrderEventArgs : EventArgs {
		public double Speed;
		public OrderEventArgs(double speed) {
			this.Speed = speed;
		}
	}
	/// <summary>
	/// Interaction logic for UserControl1.xaml
	/// </summary>
	public partial class Sidemenu : UserControl {
		
		public Graph Graph;

		public event EventHandler<OrderEventArgs> Orders;

		public Sidemenu() {
			InitializeComponent();
		}

		private void button1_Click(object sender, RoutedEventArgs e) {
			Graph = new Graph();
			Graph.Show();
		}

		private void slider1_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e) {
			Orders(this, new OrderEventArgs(slider1.Value));
		}
	}
}
