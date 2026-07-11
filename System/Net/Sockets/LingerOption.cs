using System;

namespace System.Net.Sockets
{
	public class LingerOption
	{
		public LingerOption(bool enable, int seconds)
		{
			this.Enabled = enable;
			this.LingerTime = seconds;
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
				return this.lingerTime;
			}
			set
			{
				this.lingerTime = value;
			}
		}

		private bool enabled;

		private int lingerTime;
	}
}
