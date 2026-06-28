using System;

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

		private string data;
	}
}
