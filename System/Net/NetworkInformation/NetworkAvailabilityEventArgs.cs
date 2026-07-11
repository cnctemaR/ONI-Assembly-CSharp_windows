using System;
using Unity;

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

		internal NetworkAvailabilityEventArgs()
		{
			global::Unity.ThrowStub.ThrowNotSupportedException();
		}

		private bool isAvailable;
	}
}
