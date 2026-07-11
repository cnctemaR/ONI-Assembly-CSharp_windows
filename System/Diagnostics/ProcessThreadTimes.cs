using System;

namespace System.Diagnostics
{
	internal class ProcessThreadTimes
	{
		public DateTime StartTime
		{
			get
			{
				return DateTime.FromFileTime(this.create);
			}
		}

		public DateTime ExitTime
		{
			get
			{
				return DateTime.FromFileTime(this.exit);
			}
		}

		public TimeSpan PrivilegedProcessorTime
		{
			get
			{
				return new TimeSpan(this.kernel);
			}
		}

		public TimeSpan UserProcessorTime
		{
			get
			{
				return new TimeSpan(this.user);
			}
		}

		public TimeSpan TotalProcessorTime
		{
			get
			{
				return new TimeSpan(this.user + this.kernel);
			}
		}

		internal long create;

		internal long exit;

		internal long kernel;

		internal long user;
	}
}
