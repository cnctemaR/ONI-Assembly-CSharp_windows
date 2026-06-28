using System;

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

		private SerialData eventType;
	}
}
