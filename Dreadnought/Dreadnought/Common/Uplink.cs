using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using DreadnoughtOvermind;
using DreadnoughtOvermind.Reports;


namespace Dreadnought.Common {
	class Uplink {

		public Uplink(string name,string pass) {
			TcpClient client = new TcpClient();
			IPEndPoint serverEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 33445);
			client.Connect(serverEndPoint);
			netStream = client.GetStream();
			Thread rt = new Thread(new ParameterizedThreadStart(reciveThread));
			rt.Start(netStream);
			LoginResponse.OnRecive += handleLogin;
			Send(new DreadnoughtOvermind.Reports.Login("Test", "Blub"));
		}

		private void handleLogin(Object sender , Report res ) {
			var rep = res as LoginResponse;
			if(rep.Accepted) {
				this.ID = rep.ID;
			} else {
				throw new Exception("Login Denied");
			}
		}

		private NetworkStream netStream;
		private BinaryFormatter serializer = new BinaryFormatter();
		public void Send(object obj) {
			MemoryStream memStream = new MemoryStream();
			serializer.Serialize(memStream, obj);
			byte[] len = BitConverter.GetBytes((int)memStream.Length+4);
			netStream.Write(len, 0, len.Length);
			memStream.Position = 0;
			memStream.CopyTo(netStream);
		}

		private volatile bool shutdown = false;
		void reciveThread(object stream) {
			NetworkStream ns = stream as NetworkStream;
			byte[] chunk = new byte[1024];
			int currentMessageLength = 0;
			List<byte> buffer = new List<byte>();
			int len = 0;
			while(!shutdown) {
				len = ns.Read(chunk, 0, chunk.Length);
				buffer.AddRange(chunk.Take(len));
				if(buffer.Count >= 4 && currentMessageLength == 0) {
					currentMessageLength = BitConverter.ToInt32(buffer.Take(4).ToArray(), 0);
				} 
				if(buffer.Count >= currentMessageLength) {
					Object res = serializer.Deserialize(new MemoryStream(buffer.ToArray(), 4, currentMessageLength-4));
					if(res.GetType().IsSubclassOf(typeof(Report))) {
						((Report)res).CallSubscribers(this);
					}
					buffer.RemoveRange(0, currentMessageLength);
					currentMessageLength = 0;
				}
			}
		}

		public int ID { get; set; }
	}
}
