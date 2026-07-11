using System;

namespace Steamworks
{
	[Serializable]
	public struct SiteId_t : IEquatable<SiteId_t>, IComparable<SiteId_t>
	{
		public SiteId_t(ulong value)
		{
			this.m_SiteId = value;
		}

		public override string ToString()
		{
			return this.m_SiteId.ToString();
		}

		public override bool Equals(object other)
		{
			return other is SiteId_t && this == (SiteId_t)other;
		}

		public override int GetHashCode()
		{
			return this.m_SiteId.GetHashCode();
		}

		public static bool operator ==(SiteId_t x, SiteId_t y)
		{
			return x.m_SiteId == y.m_SiteId;
		}

		public static bool operator !=(SiteId_t x, SiteId_t y)
		{
			return !(x == y);
		}

		public static explicit operator SiteId_t(ulong value)
		{
			return new SiteId_t(value);
		}

		public static explicit operator ulong(SiteId_t that)
		{
			return that.m_SiteId;
		}

		public bool Equals(SiteId_t other)
		{
			return this.m_SiteId == other.m_SiteId;
		}

		public int CompareTo(SiteId_t other)
		{
			return this.m_SiteId.CompareTo(other.m_SiteId);
		}

		public static readonly SiteId_t Invalid = new SiteId_t(0UL);

		public ulong m_SiteId;
	}
}
