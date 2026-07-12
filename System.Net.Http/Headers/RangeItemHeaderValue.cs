using System;

namespace System.Net.Http.Headers
{
	public class RangeItemHeaderValue : ICloneable
	{
		public RangeItemHeaderValue(long? from, long? to)
		{
			if (from == null && to == null)
			{
				throw new ArgumentException();
			}
			long? num2;
			if (from != null && to != null)
			{
				long? num = from;
				num2 = to;
				if ((num.GetValueOrDefault() > num2.GetValueOrDefault()) & ((num != null) & (num2 != null)))
				{
					throw new ArgumentOutOfRangeException("from");
				}
			}
			num2 = from;
			long num3 = 0L;
			if ((num2.GetValueOrDefault() < num3) & (num2 != null))
			{
				throw new ArgumentOutOfRangeException("from");
			}
			num2 = to;
			num3 = 0L;
			if ((num2.GetValueOrDefault() < num3) & (num2 != null))
			{
				throw new ArgumentOutOfRangeException("to");
			}
			this.From = from;
			this.To = to;
		}

		public long? From { get; private set; }

		public long? To { get; private set; }

		object ICloneable.Clone()
		{
			return base.MemberwiseClone();
		}

		public override bool Equals(object obj)
		{
			RangeItemHeaderValue rangeItemHeaderValue = obj as RangeItemHeaderValue;
			if (rangeItemHeaderValue != null)
			{
				long? num = rangeItemHeaderValue.From;
				long? num2 = this.From;
				if ((num.GetValueOrDefault() == num2.GetValueOrDefault()) & (num != null == (num2 != null)))
				{
					num2 = rangeItemHeaderValue.To;
					num = this.To;
					return (num2.GetValueOrDefault() == num.GetValueOrDefault()) & (num2 != null == (num != null));
				}
			}
			return false;
		}

		public override int GetHashCode()
		{
			return this.From.GetHashCode() ^ this.To.GetHashCode();
		}

		public override string ToString()
		{
			if (this.From == null)
			{
				return "-" + this.To.Value.ToString();
			}
			if (this.To == null)
			{
				return this.From.Value.ToString() + "-";
			}
			return this.From.Value.ToString() + "-" + this.To.Value.ToString();
		}
	}
}
