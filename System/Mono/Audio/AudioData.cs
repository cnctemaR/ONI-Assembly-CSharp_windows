using System;

namespace Mono.Audio
{
	internal abstract class AudioData
	{
		public abstract int Channels { get; }

		public abstract int Rate { get; }

		public abstract AudioFormat Format { get; }

		public virtual void Setup(AudioDevice dev)
		{
			dev.SetFormat(this.Format, this.Channels, this.Rate);
		}

		public abstract void Play(AudioDevice dev);

		public virtual bool IsStopped
		{
			get
			{
				return this.stopped;
			}
			set
			{
				this.stopped = value;
			}
		}

		protected const int buffer_size = 4096;

		private bool stopped;
	}
}
