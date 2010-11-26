using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using DreadnoughtOvermind.Common;
namespace DreadnoughtOvermind.Reports {
	[Serializable]
	public abstract class Report:NetworkMessage,INotifyPropertyChanged {

		public abstract string Description();

		#region INotifyPropertyChanged Members
		public event PropertyChangedEventHandler PropertyChanged;
		#endregion
	}
}
