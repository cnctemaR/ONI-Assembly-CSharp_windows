using System;

namespace rail
{
	public class RailComparableID : IEquatable<RailComparableID>, IComparable<RailComparableID>
	{
		public RailComparableID()
		{
		}

		public RailComparableID(ulong id)
		{
			this.id_ = id;
		}

		public bool IsValid()
		{
			return this.id_ > 0UL;
		}

		public override bool Equals(object other)
		{
			return other is RailComparableID && this == (RailComparableID)other;
		}

		public override int GetHashCode()
		{
			return this.id_.GetHashCode();
		}

		public static bool operator ==(RailComparableID x, RailComparableID y)
		{
			if (x == null)
			{
				return y == null;
			}
			return x.Equals(y);
		}

		public static bool operator !=(RailComparableID x, RailComparableID y)
		{
			return !(x == y);
		}

		public static explicit operator RailComparableID(ulong value)
		{
			return new RailComparableID(value);
		}

		public static explicit operator ulong(RailComparableID that)
		{
			return that.id_;
		}

		public bool Equals(RailComparableID other)
		{
			return other != null && (this == other || (!(base.GetType() != other.GetType()) && other.id_ == this.id_));
		}

		public int CompareTo(RailComparableID other)
		{
			return this.id_.CompareTo(other.id_);
		}

		public override string ToString()
		{
			return this.id_.ToString();
		}

		public static readonly RailComparableID Nil = new RailComparableID(0UL);

		public ulong id_;
	}
}
