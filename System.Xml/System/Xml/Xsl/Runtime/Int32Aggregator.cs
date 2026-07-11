using System;
using System.ComponentModel;

namespace System.Xml.Xsl.Runtime
{
	[EditorBrowsable(EditorBrowsableState.Never)]
	public struct Int32Aggregator
	{
		public void Create()
		{
			this.cnt = 0;
		}

		public void Sum(int value)
		{
			if (this.cnt == 0)
			{
				this.result = value;
				this.cnt = 1;
				return;
			}
			this.result += value;
		}

		public void Average(int value)
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

		public void Minimum(int value)
		{
			if (this.cnt == 0 || value < this.result)
			{
				this.result = value;
			}
			this.cnt = 1;
		}

		public void Maximum(int value)
		{
			if (this.cnt == 0 || value > this.result)
			{
				this.result = value;
			}
			this.cnt = 1;
		}

		public int SumResult
		{
			get
			{
				return this.result;
			}
		}

		public int AverageResult
		{
			get
			{
				return this.result / this.cnt;
			}
		}

		public int MinimumResult
		{
			get
			{
				return this.result;
			}
		}

		public int MaximumResult
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

		private int result;

		private int cnt;
	}
}
