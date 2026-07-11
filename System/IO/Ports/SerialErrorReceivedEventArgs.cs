using System;
using Unity;

namespace System.IO.Ports
{
	public class SerialErrorReceivedEventArgs : EventArgs
	{
		internal SerialErrorReceivedEventArgs(SerialError eventType)
		{
			this.eventType = eventType;
		}

		public SerialError EventType
		{
			get
			{
				return this.eventType;
			}
		}

		internal SerialErrorReceivedEventArgs()
		{
			global::Unity.ThrowStub.ThrowNotSupportedException();
		}

		private SerialError eventType;
	}
}
