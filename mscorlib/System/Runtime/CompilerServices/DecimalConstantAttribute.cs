using System;
using System.Runtime.InteropServices;

namespace System.Runtime.CompilerServices
{
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Parameter, Inherited = false)]
	[ComVisible(true)]
	[Serializable]
	public sealed class DecimalConstantAttribute : Attribute
	{
		[CLSCompliant(false)]
		public DecimalConstantAttribute(byte scale, byte sign, uint hi, uint mid, uint low)
		{
			this.scale = scale;
			this.sign = Convert.ToBoolean(sign);
			this.hi = (int)hi;
			this.mid = (int)mid;
			this.low = (int)low;
		}

		public DecimalConstantAttribute(byte scale, byte sign, int hi, int mid, int low)
		{
			this.scale = scale;
			this.sign = Convert.ToBoolean(sign);
			this.hi = hi;
			this.mid = mid;
			this.low = low;
		}

		public decimal Value
		{
			get
			{
				return new decimal(this.low, this.mid, this.hi, this.sign, this.scale);
			}
		}

		private byte scale;

		private bool sign;

		private int hi;

		private int mid;

		private int low;
	}
}
