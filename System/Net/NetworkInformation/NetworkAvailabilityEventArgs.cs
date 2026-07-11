using System;

namespace System.Net.NetworkInformation
{
	public class NetworkAvailabilityEventArgs : EventArgs
	{
		internal NetworkAvailabilityEventArgs(bool isAvailable)
		{
			this.isAvailable = isAvailable;
		}

		public bool IsAvailable
		{
			get
			{
				return this.isAvailable;
			}
		}

		private bool isAvailable;
	}
}
