using System;

namespace Mono.Unix.Native
{
	[Map("struct utimbuf")]
	public struct Utimbuf : IEquatable<Utimbuf>
	{
		public override int GetHashCode()
		{
			return this.actime.GetHashCode() ^ this.modtime.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			if (obj == null || obj.GetType() != base.GetType())
			{
				return false;
			}
			Utimbuf utimbuf = (Utimbuf)obj;
			return utimbuf.actime == this.actime && utimbuf.modtime == this.modtime;
		}

		public bool Equals(Utimbuf value)
		{
			return value.actime == this.actime && value.modtime == this.modtime;
		}

		public static bool operator ==(Utimbuf lhs, Utimbuf rhs)
		{
			return lhs.Equals(rhs);
		}

		public static bool operator !=(Utimbuf lhs, Utimbuf rhs)
		{
			return !lhs.Equals(rhs);
		}

		[time_t]
		public long actime;

		[time_t]
		public long modtime;
	}
}
