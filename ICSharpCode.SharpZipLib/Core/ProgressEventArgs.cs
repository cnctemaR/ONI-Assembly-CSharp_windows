using System;

namespace ICSharpCode.SharpZipLib.Core
{
	public class ProgressEventArgs : EventArgs
	{
		public ProgressEventArgs(string name, long processed, long target)
		{
			this.name_ = name;
			this.processed_ = processed;
			this.target_ = target;
		}

		public string Name
		{
			get
			{
				return this.name_;
			}
		}

		public bool ContinueRunning
		{
			get
			{
				return this.continueRunning_;
			}
			set
			{
				this.continueRunning_ = value;
			}
		}

		public float PercentComplete
		{
			get
			{
				float num;
				if (this.target_ <= 0L)
				{
					num = 0f;
				}
				else
				{
					num = (float)this.processed_ / (float)this.target_ * 100f;
				}
				return num;
			}
		}

		public long Processed
		{
			get
			{
				return this.processed_;
			}
		}

		public long Target
		{
			get
			{
				return this.target_;
			}
		}

		private string name_;

		private long processed_;

		private long target_;

		private bool continueRunning_ = true;
	}
}
