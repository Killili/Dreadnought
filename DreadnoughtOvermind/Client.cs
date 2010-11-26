using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Net.Sockets;
using DreadnoughtOvermind.Common;
using DreadnoughtOvermind.Reports;

namespace DreadnoughtOvermind {
	public class Client : INotifyPropertyChanged {
		private static int IDS = 1;
		public int ID { get; private set; }
		public Client(StateObject so,Reports.Login login) {
			this.networkState = so;
			this.Name = login.Name;
			ID = IDS++;
			Send(new LoginResponse(this));
			addToList();
		}

		#region GUI
		public event PropertyChangedEventHandler PropertyChanged;
		private void addToList() {
			Server.Instance.Dispatcher.BeginInvoke((Action)delegate {
				Server.Instance.Clients.Add(this);
			});
		}
		public string Info { get { return info(); } }
		public string info() {
			return String.Format("{0}({1})", Name, ID);
		}
		#endregion

		#region Network
		public StateObject networkState;
		public void Send(object nw) {
			Server.Instance.Send(this.networkState, nw);
		}
		#endregion

		

		public string Name { get; set; }
	}
}
