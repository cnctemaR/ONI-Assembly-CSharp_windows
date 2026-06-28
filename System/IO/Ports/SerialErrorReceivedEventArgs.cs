using System;

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

		private SerialError eventType;
	}
}
