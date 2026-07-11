using System;
using Unity;

namespace System.IO.Ports
{
	public class SerialPinChangedEventArgs : EventArgs
	{
		internal SerialPinChangedEventArgs(SerialPinChange eventType)
		{
			this.eventType = eventType;
		}

		public SerialPinChange EventType
		{
			get
			{
				return this.eventType;
			}
		}

		internal SerialPinChangedEventArgs()
		{
			global::Unity.ThrowStub.ThrowNotSupportedException();
		}

		private SerialPinChange eventType;
	}
}
