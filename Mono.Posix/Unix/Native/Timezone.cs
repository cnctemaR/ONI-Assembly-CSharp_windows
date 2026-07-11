using System;

namespace Mono.Unix.Native
{
	[Map("struct timezone")]
	public struct Timezone : IEquatable<Timezone>
	{
		public override int GetHashCode()
		{
			return this.tz_minuteswest.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			return obj != null && !(obj.GetType() != base.GetType()) && ((Timezone)obj).tz_minuteswest == this.tz_minuteswest;
		}

		public bool Equals(Timezone value)
		{
			return value.tz_minuteswest == this.tz_minuteswest;
		}

		public static bool operator ==(Timezone lhs, Timezone rhs)
		{
			return lhs.Equals(rhs);
		}

		public static bool operator !=(Timezone lhs, Timezone rhs)
		{
			return !lhs.Equals(rhs);
		}

		public int tz_minuteswest;

		private int tz_dsttime;
	}
}
