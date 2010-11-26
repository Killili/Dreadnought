using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using DreadnoughtOvermind.Common;

namespace DreadnoughtOvermind.Reports {
	[Serializable] 
	public class Login : Report {
		new public static Action<Object, Login> OnRecive;
		public override void CallSubscribers(object sender) {
			if(OnRecive != null) OnRecive(sender, this);
		}
		
		public Login(string name, string pass){
				this.Name = name;
				this.Password = pass;
		}

		public string Password { get; set; }
		public string Name { get; set; }

		public override string Description() {
			return "Login";
		}
	}
	[Serializable]
	public class LoginResponse : Report {
		new public static Action<Object, LoginResponse> OnRecive;
		public override void CallSubscribers(object sender) {
			if(OnRecive != null) OnRecive(sender, this);
		}
		public int ID { get; private set; }
		public bool Accepted { get; private set; }
		public LoginResponse(Client c){
			if(c != null) {
				Accepted = true;
				ID = c.ID;
			} else {
				Accepted = false;
				ID = 0;
			}
		}

		public override string Description() {
			if(Accepted) {
				return "Login:Granted";
			} else {
				return "Login:Denied";
			}
		}

	}
}
