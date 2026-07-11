using System;
using System.Net.Sockets;

namespace System.Net
{
	[Serializable]
	public abstract class EndPoint
	{
		public virtual global::System.Net.Sockets.AddressFamily AddressFamily
		{
			get
			{
				throw EndPoint.NotImplemented();
			}
		}

		public virtual EndPoint Create(SocketAddress address)
		{
			throw EndPoint.NotImplemented();
		}

		public virtual SocketAddress Serialize()
		{
			throw EndPoint.NotImplemented();
		}

		private static Exception NotImplemented()
		{
			return new NotImplementedException();
		}
	}
}
