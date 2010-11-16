using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace DreadnoughtOvermind.Orders {
	public class Order: INotifyPropertyChanged {
		public Order(string desc) {
			Description = desc;
			State = States.New;
		}
		public enum States { New , Send, Accepted, InProgress, Done }
		

		public string Description { get; private set; }
		
		private States _state;
		public States State {
			get { return _state; }
			set { _state = value; if(PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("State")); }
		}

		#region INotifyPropertyChanged Members

		public event PropertyChangedEventHandler PropertyChanged;
		

		#endregion
	}
}
