using System;
using System.ComponentModel;

namespace System.Xml.Xsl.Runtime
{
	[EditorBrowsable(EditorBrowsableState.Never)]
	public struct Int64Aggregator
	{
		public void Create()
		{
			this.cnt = 0;
		}

		public void Sum(long value)
		{
			if (this.cnt == 0)
			{
				this.result = value;
				this.cnt = 1;
				return;
			}
			this.result += value;
		}

		public void Average(long value)
		{
			if (this.cnt == 0)
			{
				this.result = value;
			}
			else
			{
				this.result += value;
			}
			this.cnt++;
		}

		public void Minimum(long value)
		{
			if (this.cnt == 0 || value < this.result)
			{
				this.result = value;
			}
			this.cnt = 1;
		}

		public void Maximum(long value)
		{
			if (this.cnt == 0 || value > this.result)
			{
				this.result = value;
			}
			this.cnt = 1;
		}

		public long SumResult
		{
			get
			{
				return this.result;
			}
		}

		public long AverageResult
		{
			get
			{
				return this.result / (long)this.cnt;
			}
		}

		public long MinimumResult
		{
			get
			{
				return this.result;
			}
		}

		public long MaximumResult
		{
			get
			{
				return this.result;
			}
		}

		public bool IsEmpty
		{
			get
			{
				return this.cnt == 0;
			}
		}

		private long result;

		private int cnt;
	}
}
