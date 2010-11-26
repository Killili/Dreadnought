using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DreadnoughtOvermind.Common;

namespace DreadnoughtOvermind.Reports {
	[Serializable] 
	public class Chat:Report {
		new public static Action<Object, Chat> OnRecive;
		public override void CallSubscribers(object sender) {
			if(OnRecive != null) OnRecive(sender, this);
		}

		public string Text;
		public string Channel;
		public string Name;
		public Chat(Client client,string channel, string text) {
			if(client != null) {
				Name = client.Name;
			} else {
				Name = "Server";
			}
			Channel = channel;
			Text = text;
		}
		public override string Description() {
			return String.Format("Chat:{0}:{1}:{2}", Name, Channel, Text);
		}


		
	}
}
