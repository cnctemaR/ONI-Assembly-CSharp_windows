using System;
using System.ComponentModel;

namespace System.Xml.Xsl.Runtime
{
	[EditorBrowsable(EditorBrowsableState.Never)]
	public struct DecimalAggregator
	{
		public void Create()
		{
			this.cnt = 0;
		}

		public void Sum(decimal value)
		{
			if (this.cnt == 0)
			{
				this.result = value;
				this.cnt = 1;
				return;
			}
			this.result += value;
		}

		public void Average(decimal value)
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

		public void Minimum(decimal value)
		{
			if (this.cnt == 0 || value < this.result)
			{
				this.result = value;
			}
			this.cnt = 1;
		}

		public void Maximum(decimal value)
		{
			if (this.cnt == 0 || value > this.result)
			{
				this.result = value;
			}
			this.cnt = 1;
		}

		public decimal SumResult
		{
			get
			{
				return this.result;
			}
		}

		public decimal AverageResult
		{
			get
			{
				return this.result / this.cnt;
			}
		}

		public decimal MinimumResult
		{
			get
			{
				return this.result;
			}
		}

		public decimal MaximumResult
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

		private decimal result;

		private int cnt;
	}
}
