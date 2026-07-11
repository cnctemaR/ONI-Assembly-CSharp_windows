using System;
using Unity;

namespace System.Timers
{
	public class ElapsedEventArgs : EventArgs
	{
		internal ElapsedEventArgs(DateTime time)
		{
			this.time = time;
		}

		public DateTime SignalTime
		{
			get
			{
				return this.time;
			}
		}

		internal ElapsedEventArgs()
		{
			global::Unity.ThrowStub.ThrowNotSupportedException();
		}

		private DateTime time;
	}
}
