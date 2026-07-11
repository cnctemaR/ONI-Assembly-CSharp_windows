using System;
using System.ComponentModel;
using System.Threading;

namespace System.Timers
{
	[global::System.ComponentModel.DefaultProperty("Interval")]
	[global::System.ComponentModel.DefaultEvent("Elapsed")]
	public class Timer : global::System.ComponentModel.Component, global::System.ComponentModel.ISupportInitialize
	{
		public Timer()
			: this(100.0)
		{
		}

		public Timer(double interval)
		{
			if (interval > 2147483647.0)
			{
				throw new ArgumentException("Invalid value: " + interval, "interval");
			}
			this.autoReset = true;
			this.Interval = interval;
		}

		[global::System.ComponentModel.Category("Behavior")]
		[TimersDescription("Occurs when the Interval has elapsed.")]
		public event ElapsedEventHandler Elapsed;

		[global::System.ComponentModel.Category("Behavior")]
		[global::System.ComponentModel.DefaultValue(true)]
		[TimersDescription("Indicates whether the timer will be restarted when it is enabled.")]
		public bool AutoReset
		{
			get
			{
				return this.autoReset;
			}
			set
			{
				this.autoReset = value;
			}
		}

		[TimersDescription("Indicates whether the timer is enabled to fire events at a defined interval.")]
		[global::System.ComponentModel.Category("Behavior")]
		[global::System.ComponentModel.DefaultValue(false)]
		public bool Enabled
		{
			get
			{
				object @lock = this._lock;
				bool flag;
				lock (@lock)
				{
					flag = this.timer != null;
				}
				return flag;
			}
			set
			{
				object @lock = this._lock;
				lock (@lock)
				{
					bool flag = this.timer != null;
					if (flag != value)
					{
						if (value)
						{
							this.timer = new Timer(new TimerCallback(Timer.Callback), this, (int)this.interval, (!this.autoReset) ? 0 : ((int)this.interval));
						}
						else
						{
							this.timer.Dispose();
							this.timer = null;
						}
					}
				}
			}
		}

		[global::System.ComponentModel.DefaultValue(100)]
		[TimersDescription("The number of milliseconds between timer events.")]
		[global::System.ComponentModel.RecommendedAsConfigurable(true)]
		[global::System.ComponentModel.Category("Behavior")]
		public double Interval
		{
			get
			{
				return this.interval;
			}
			set
			{
				if (value <= 0.0)
				{
					throw new ArgumentException("Invalid value: " + value);
				}
				object @lock = this._lock;
				lock (@lock)
				{
					this.interval = value;
					if (this.timer != null)
					{
						this.timer.Change((int)this.interval, (!this.autoReset) ? 0 : ((int)this.interval));
					}
				}
			}
		}

		public override global::System.ComponentModel.ISite Site
		{
			get
			{
				return base.Site;
			}
			set
			{
				base.Site = value;
			}
		}

		[global::System.ComponentModel.Browsable(false)]
		[global::System.ComponentModel.DefaultValue(null)]
		[TimersDescription("The object used to marshal the event handler calls issued when an interval has elapsed.")]
		public global::System.ComponentModel.ISynchronizeInvoke SynchronizingObject
		{
			get
			{
				return this.so;
			}
			set
			{
				this.so = value;
			}
		}

		public void BeginInit()
		{
		}

		public void Close()
		{
			this.Enabled = false;
		}

		public void EndInit()
		{
		}

		public void Start()
		{
			this.Enabled = true;
		}

		public void Stop()
		{
			this.Enabled = false;
		}

		protected override void Dispose(bool disposing)
		{
			this.Close();
			base.Dispose(disposing);
		}

		private static void Callback(object state)
		{
			Timer timer = (Timer)state;
			if (!timer.Enabled)
			{
				return;
			}
			ElapsedEventHandler elapsed = timer.Elapsed;
			if (!timer.autoReset)
			{
				timer.Enabled = false;
			}
			if (elapsed == null)
			{
				return;
			}
			ElapsedEventArgs e = new ElapsedEventArgs(DateTime.Now);
			if (timer.so != null && timer.so.InvokeRequired)
			{
				timer.so.BeginInvoke(elapsed, new object[] { timer, e });
			}
			else
			{
				try
				{
					elapsed(timer, e);
				}
				catch
				{
				}
			}
		}

		private double interval;

		private bool autoReset;

		private Timer timer;

		private object _lock = new object();

		private global::System.ComponentModel.ISynchronizeInvoke so;
	}
}
