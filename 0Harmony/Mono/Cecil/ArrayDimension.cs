using System;

namespace Mono.Cecil
{
	internal struct ArrayDimension
	{
		public int? LowerBound
		{
			get
			{
				return this.lower_bound;
			}
			set
			{
				this.lower_bound = value;
			}
		}

		public int? UpperBound
		{
			get
			{
				return this.upper_bound;
			}
			set
			{
				this.upper_bound = value;
			}
		}

		public bool IsSized
		{
			get
			{
				return this.lower_bound != null || this.upper_bound != null;
			}
		}

		public ArrayDimension(int? lowerBound, int? upperBound)
		{
			this.lower_bound = lowerBound;
			this.upper_bound = upperBound;
		}

		public override string ToString()
		{
			if (this.IsSized)
			{
				int? num = this.lower_bound;
				string text = num.ToString();
				string text2 = "...";
				num = this.upper_bound;
				return text + text2 + num.ToString();
			}
			return string.Empty;
		}

		private int? lower_bound;

		private int? upper_bound;
	}
}
