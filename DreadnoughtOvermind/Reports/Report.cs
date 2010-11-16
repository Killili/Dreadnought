using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
namespace DreadnoughtOvermind.Reports {
	[Serializable]
	public class Report: INotifyPropertyChanged {

		[NonSerialized]
		public static Action<Object,Report> OnRecive;
		public void CallSubscribers(Object sender){
			if(OnRecive != null) OnRecive(sender,this);
		}

		public Report(string desc) {
			Description = desc;
		}
		public string Description { get; set; }

		#region INotifyPropertyChanged Members

		public event PropertyChangedEventHandler PropertyChanged;

		#endregion
	}
}
