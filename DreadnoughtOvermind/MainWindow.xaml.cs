﻿using System;
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

namespace DreadnoughtOvermind {
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	
	public partial class MainWindow : Window {
		public String popo = "ARSCH";
		public MainWindow() {
			InitializeComponent();
		}

		private void listBox1_SelectionChanged(object sender, SelectionChangedEventArgs e) {
			var o = ((ListBox)sender).SelectedItem as Client;
			//o.Send("Test");
		}
	}
}
