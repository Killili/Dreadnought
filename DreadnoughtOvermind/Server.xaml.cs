using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;
using System.Collections.ObjectModel;
using DreadnoughtOvermind.Orders;
using DreadnoughtOvermind.Reports;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

namespace DreadnoughtOvermind {
	public partial class Server : Application {

		private ObservableCollection<Client> clients = new ObservableCollection<Client>();
		public ObservableCollection<Client> Clients {
			get { return clients; }
		}
		
		public ObservableCollection<Order> orders = new ObservableCollection<Order>();
		public ObservableCollection<Report> reports = new ObservableCollection<Report>();
		public Server()
			: base() {
		}


		private Socket server;
		
		void startServer(object sender, EventArgs e) {
			server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
			server.Bind(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 33445));
			server.Listen(10);
			server.BeginAccept(new AsyncCallback(connectRequest), server);
		}

		void connectRequest(IAsyncResult res) {
			Socket server = res.AsyncState as Socket;
			Socket socket = server.EndAccept(res);
			Console.WriteLine("Connect from {0}",socket.RemoteEndPoint);
			StateObject so = new StateObject();
			so.Socket = socket;
			socket.BeginReceive(so.Chunk, 0 , StateObject.ChunkSize , SocketFlags.None, new AsyncCallback(readCallback), so);
			server.BeginAccept(new AsyncCallback(connectRequest), server);
		}

		void readCallback(IAsyncResult res) {
			StateObject so = (StateObject)res.AsyncState;
			
			int bytesRead = so.Socket.EndReceive(res);

			if(bytesRead > 0) {
				so.Buffer.AddRange(so.Chunk.Take(bytesRead));
				if(so.Buffer.Count >= 4 && so.CurrentMessageLength == 0) {
					so.CurrentMessageLength = BitConverter.ToInt32(so.Buffer.Take(4).ToArray(),0);
				} else if(so.Buffer.Count >= so.CurrentMessageLength) {
					BinaryFormatter bf = new BinaryFormatter();
					lock(so) {
						object data = bf.Deserialize(new MemoryStream(so.Buffer.ToArray(), 4, so.CurrentMessageLength-4));
						if(data.GetType() == typeof(Login)) {
							so.Client = new Client(so);
							Send(so,new LoginResponse(so.Client));
						}
						so.Buffer.RemoveRange(0, so.CurrentMessageLength);
						so.CurrentMessageLength = 0;
					}
				}
				so.Socket.BeginReceive(so.Chunk, 0, StateObject.ChunkSize, SocketFlags.None, new AsyncCallback(readCallback), so);
			}
		}

		
		public void Send(StateObject so , object obj) {
			MemoryStream memStream = new MemoryStream(5);
			BinaryFormatter serializer = new BinaryFormatter();
			memStream.Position = 4;
			serializer.Serialize(memStream, obj);
			memStream.Position = 0;
			memStream.Write( BitConverter.GetBytes((int)memStream.Length) , 0,4);
			so.Socket.BeginSend(memStream.GetBuffer(), 0, (int)memStream.Length, 0,
					new AsyncCallback(sendCallback), so);
		}

		private void sendCallback(IAsyncResult res) {
			try {
				StateObject so = (StateObject)res.AsyncState;

				int bytesSent = so.Socket.EndSend(res);

			} catch(Exception e) {
				Console.WriteLine(e.ToString());
			}
		}


	}
	public class StateObject {
		// Client  socket.
		public Socket Socket = null;
		public Client Client = null;
		// Size of receive buffer.
		public const int ChunkSize = 1024;
		// Receive buffer.
		public byte[] Chunk = new byte[ChunkSize];
		// Received data string.
		public List<byte> Buffer = new List<byte>();
		public int CurrentMessageLength = 0;
	}

}
