using System;
using System.Runtime.CompilerServices;

namespace System.Diagnostics
{
	public class Stopwatch
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern long GetTimestamp();

		public static Stopwatch StartNew()
		{
			Stopwatch stopwatch = new Stopwatch();
			stopwatch.Start();
			return stopwatch;
		}

		public TimeSpan Elapsed
		{
			get
			{
				if (Stopwatch.IsHighResolution)
				{
					return TimeSpan.FromTicks(this.ElapsedTicks / (Stopwatch.Frequency / 10000000L));
				}
				return TimeSpan.FromTicks(this.ElapsedTicks);
			}
		}

		public long ElapsedMilliseconds
		{
			get
			{
				if (Stopwatch.IsHighResolution)
				{
					return this.ElapsedTicks / (Stopwatch.Frequency / 1000L);
				}
				return checked((long)this.Elapsed.TotalMilliseconds);
			}
		}

		public long ElapsedTicks
		{
			get
			{
				if (!this.is_running)
				{
					return this.elapsed;
				}
				return Stopwatch.GetTimestamp() - this.started + this.elapsed;
			}
		}

		public bool IsRunning
		{
			get
			{
				return this.is_running;
			}
		}

		public void Reset()
		{
			this.elapsed = 0L;
			this.is_running = false;
		}

		public void Start()
		{
			if (this.is_running)
			{
				return;
			}
			this.started = Stopwatch.GetTimestamp();
			this.is_running = true;
		}

		public void Stop()
		{
			if (!this.is_running)
			{
				return;
			}
			this.elapsed += Stopwatch.GetTimestamp() - this.started;
			if (this.elapsed < 0L)
			{
				this.elapsed = 0L;
			}
			this.is_running = false;
		}

		public void Restart()
		{
			this.started = Stopwatch.GetTimestamp();
			this.elapsed = 0L;
			this.is_running = true;
		}

		public static readonly long Frequency = 10000000L;

		public static readonly bool IsHighResolution = true;

		private long elapsed;

		private long started;

		private bool is_running;
	}
}
