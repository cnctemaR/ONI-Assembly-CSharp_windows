using System;
using Unity;

namespace System.IO.Ports
{
	public class SerialDataReceivedEventArgs : EventArgs
	{
		internal SerialDataReceivedEventArgs(SerialData eventType)
		{
			this.eventType = eventType;
		}

		public SerialData EventType
		{
			get
			{
				return this.eventType;
			}
		}

		internal SerialDataReceivedEventArgs()
		{
			global::Unity.ThrowStub.ThrowNotSupportedException();
		}

		private SerialData eventType;
	}
}
