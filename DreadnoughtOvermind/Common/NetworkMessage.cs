using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DreadnoughtOvermind.Common {
	[Serializable]
	public abstract class NetworkMessage {
		public abstract void CallSubscribers(Object sender);
		public static Action<Object, NetworkMessage> OnRecive;
	}
}
