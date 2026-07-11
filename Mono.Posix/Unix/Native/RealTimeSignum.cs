using System;

namespace Mono.Unix.Native
{
	public struct RealTimeSignum : IEquatable<RealTimeSignum>
	{
		public RealTimeSignum(int offset)
		{
			if (offset < 0)
			{
				throw new ArgumentOutOfRangeException("Offset cannot be negative");
			}
			if (offset > RealTimeSignum.MaxOffset)
			{
				throw new ArgumentOutOfRangeException("Offset greater than maximum supported SIGRT");
			}
			this.rt_offset = offset;
		}

		public int Offset
		{
			get
			{
				return this.rt_offset;
			}
		}

		public override int GetHashCode()
		{
			return this.rt_offset.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			return obj != null && !(obj.GetType() != base.GetType()) && this.Equals((RealTimeSignum)obj);
		}

		public bool Equals(RealTimeSignum value)
		{
			return this.Offset == value.Offset;
		}

		public static bool operator ==(RealTimeSignum lhs, RealTimeSignum rhs)
		{
			return lhs.Equals(rhs);
		}

		public static bool operator !=(RealTimeSignum lhs, RealTimeSignum rhs)
		{
			return !lhs.Equals(rhs);
		}

		private int rt_offset;

		private static readonly int MaxOffset = UnixSignal.GetSIGRTMAX() - UnixSignal.GetSIGRTMIN() - 1;

		public static readonly RealTimeSignum MinValue = new RealTimeSignum(0);

		public static readonly RealTimeSignum MaxValue = new RealTimeSignum(RealTimeSignum.MaxOffset);
	}
}
