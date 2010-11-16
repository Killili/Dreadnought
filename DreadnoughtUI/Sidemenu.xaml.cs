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
using DreadnoughtOvermind.Orders;
using System.Collections.ObjectModel;
namespace DreadnoughtUI {
	/// <summary>
	/// Interaction logic for UserControl1.xaml
	/// </summary>
	public partial class Sidemenu : UserControl {
		
		public Graph Graph;
		public ObservableCollection<Order> Orders = new ObservableCollection<Order>();
		public Sidemenu() {
			InitializeComponent();
			orders.ItemsSource = Orders;
		}

		private void button1_Click(object sender, RoutedEventArgs e) {
			Orders.Add(new Order("TestOrder"));

		}

		private void grid_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e){
			var o = ((FrameworkElement)sender).DataContext as Order;
			o.State = Order.States.Send;
		}

	}
}
