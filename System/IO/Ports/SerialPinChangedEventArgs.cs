using System;

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

		private SerialPinChange eventType;
	}
}
