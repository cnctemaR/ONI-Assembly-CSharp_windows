using System;

namespace Mono.Unix.Native
{
	[Map("struct timespec")]
	public struct Timespec : IEquatable<Timespec>
	{
		public override int GetHashCode()
		{
			return this.tv_sec.GetHashCode() ^ this.tv_nsec.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			if (obj == null || obj.GetType() != base.GetType())
			{
				return false;
			}
			Timespec timespec = (Timespec)obj;
			return timespec.tv_sec == this.tv_sec && timespec.tv_nsec == this.tv_nsec;
		}

		public bool Equals(Timespec value)
		{
			return value.tv_sec == this.tv_sec && value.tv_nsec == this.tv_nsec;
		}

		public static bool operator ==(Timespec lhs, Timespec rhs)
		{
			return lhs.Equals(rhs);
		}

		public static bool operator !=(Timespec lhs, Timespec rhs)
		{
			return !lhs.Equals(rhs);
		}

		[time_t]
		public long tv_sec;

		public long tv_nsec;
	}
}
