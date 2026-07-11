using System;
using System.ComponentModel;

namespace System.Xml.Xsl.Runtime
{
	[EditorBrowsable(EditorBrowsableState.Never)]
	public struct DoubleAggregator
	{
		public void Create()
		{
			this.cnt = 0;
		}

		public void Sum(double value)
		{
			if (this.cnt == 0)
			{
				this.result = value;
				this.cnt = 1;
				return;
			}
			this.result += value;
		}

		public void Average(double value)
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

		public void Minimum(double value)
		{
			if (this.cnt == 0 || value < this.result || double.IsNaN(value))
			{
				this.result = value;
			}
			this.cnt = 1;
		}

		public void Maximum(double value)
		{
			if (this.cnt == 0 || value > this.result || double.IsNaN(value))
			{
				this.result = value;
			}
			this.cnt = 1;
		}

		public double SumResult
		{
			get
			{
				return this.result;
			}
		}

		public double AverageResult
		{
			get
			{
				return this.result / (double)this.cnt;
			}
		}

		public double MinimumResult
		{
			get
			{
				return this.result;
			}
		}

		public double MaximumResult
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

		private double result;

		private int cnt;
	}
}
