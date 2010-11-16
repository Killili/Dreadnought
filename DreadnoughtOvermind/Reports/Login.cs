using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace DreadnoughtOvermind.Reports {
	[Serializable] 
	public class Login : Report {
		
		public Login(string name, string pass)
			: base("Login") {
				this.Name = name;
				this.Password = pass;
		}

		public string Password { get; set; }
		public string Name { get; set; }
	}
	[Serializable] 
	public class LoginResponse : Report {
		
		public int ID { get; private set; }
		public bool Accepted { get; private set; }
		public LoginResponse(Client c): base("Login:Denied") {
			if(c != null) {
				Accepted = true;
				ID = c.ID;
				Description = "Login:Granted(" + ID.ToString() + ")";
			} else {
				Accepted = false;
				ID = 0;
			}
		}
	}
}
