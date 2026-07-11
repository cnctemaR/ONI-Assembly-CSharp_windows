using System;

namespace System.Net.NetworkInformation
{
	public class NetworkAvailabilityEventArgs : EventArgs
	{
		internal NetworkAvailabilityEventArgs(bool available)
		{
			this.available = available;
		}

		public bool IsAvailable
		{
			get
			{
				return this.available;
			}
		}

		private bool available;
	}
}
