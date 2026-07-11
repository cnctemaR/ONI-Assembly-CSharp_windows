using System;
using Unity;

namespace System.Diagnostics
{
	public class DataReceivedEventArgs : EventArgs
	{
		internal DataReceivedEventArgs(string data)
		{
			this.data = data;
		}

		public string Data
		{
			get
			{
				return this.data;
			}
		}

		internal DataReceivedEventArgs()
		{
			global::Unity.ThrowStub.ThrowNotSupportedException();
		}

		private string data;
	}
}
