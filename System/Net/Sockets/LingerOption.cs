using System;

namespace System.Net.Sockets
{
	public class LingerOption
	{
		public LingerOption(bool enable, int secs)
		{
			this.enabled = enable;
			this.seconds = secs;
		}

		public bool Enabled
		{
			get
			{
				return this.enabled;
			}
			set
			{
				this.enabled = value;
			}
		}

		public int LingerTime
		{
			get
			{
				return this.seconds;
			}
			set
			{
				this.seconds = value;
			}
		}

		private bool enabled;

		private int seconds;
	}
}
