using System;

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

		private DateTime time;
	}
}
