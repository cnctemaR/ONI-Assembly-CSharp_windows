using System;

namespace Steamworks
{
	[Serializable]
	public struct HSteamNetPollGroup : IEquatable<HSteamNetPollGroup>, IComparable<HSteamNetPollGroup>
	{
		public HSteamNetPollGroup(uint value)
		{
			this.m_HSteamNetPollGroup = value;
		}

		public override string ToString()
		{
			return this.m_HSteamNetPollGroup.ToString();
		}

		public override bool Equals(object other)
		{
			return other is HSteamNetPollGroup && this == (HSteamNetPollGroup)other;
		}

		public override int GetHashCode()
		{
			return this.m_HSteamNetPollGroup.GetHashCode();
		}

		public static bool operator ==(HSteamNetPollGroup x, HSteamNetPollGroup y)
		{
			return x.m_HSteamNetPollGroup == y.m_HSteamNetPollGroup;
		}

		public static bool operator !=(HSteamNetPollGroup x, HSteamNetPollGroup y)
		{
			return !(x == y);
		}

		public static explicit operator HSteamNetPollGroup(uint value)
		{
			return new HSteamNetPollGroup(value);
		}

		public static explicit operator uint(HSteamNetPollGroup that)
		{
			return that.m_HSteamNetPollGroup;
		}

		public bool Equals(HSteamNetPollGroup other)
		{
			return this.m_HSteamNetPollGroup == other.m_HSteamNetPollGroup;
		}

		public int CompareTo(HSteamNetPollGroup other)
		{
			return this.m_HSteamNetPollGroup.CompareTo(other.m_HSteamNetPollGroup);
		}

		public static readonly HSteamNetPollGroup Invalid = new HSteamNetPollGroup(0U);

		public uint m_HSteamNetPollGroup;
	}
}
