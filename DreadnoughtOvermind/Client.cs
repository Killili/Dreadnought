using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Net.Sockets;

namespace DreadnoughtOvermind {
	public class Client : INotifyPropertyChanged {
		private static int IDS = 1;
		public int ID { get; private set; }
		public Client(StateObject so) {
			this.networkState = so;
			ID = IDS++;
		}

		#region INotifyPropertyChanged Members
		public event PropertyChangedEventHandler PropertyChanged;
		#endregion
		public StateObject networkState;
		public override string ToString() {
			return "Client:" + networkState.Socket.RemoteEndPoint.ToString();
		}
	}
}
