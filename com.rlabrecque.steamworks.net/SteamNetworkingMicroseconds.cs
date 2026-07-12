using System;

namespace Steamworks
{
	[Serializable]
	public struct SteamNetworkingMicroseconds : IEquatable<SteamNetworkingMicroseconds>, IComparable<SteamNetworkingMicroseconds>
	{
		public SteamNetworkingMicroseconds(long value)
		{
			this.m_SteamNetworkingMicroseconds = value;
		}

		public override string ToString()
		{
			return this.m_SteamNetworkingMicroseconds.ToString();
		}

		public override bool Equals(object other)
		{
			return other is SteamNetworkingMicroseconds && this == (SteamNetworkingMicroseconds)other;
		}

		public override int GetHashCode()
		{
			return this.m_SteamNetworkingMicroseconds.GetHashCode();
		}

		public static bool operator ==(SteamNetworkingMicroseconds x, SteamNetworkingMicroseconds y)
		{
			return x.m_SteamNetworkingMicroseconds == y.m_SteamNetworkingMicroseconds;
		}

		public static bool operator !=(SteamNetworkingMicroseconds x, SteamNetworkingMicroseconds y)
		{
			return !(x == y);
		}

		public static explicit operator SteamNetworkingMicroseconds(long value)
		{
			return new SteamNetworkingMicroseconds(value);
		}

		public static explicit operator long(SteamNetworkingMicroseconds that)
		{
			return that.m_SteamNetworkingMicroseconds;
		}

		public bool Equals(SteamNetworkingMicroseconds other)
		{
			return this.m_SteamNetworkingMicroseconds == other.m_SteamNetworkingMicroseconds;
		}

		public int CompareTo(SteamNetworkingMicroseconds other)
		{
			return this.m_SteamNetworkingMicroseconds.CompareTo(other.m_SteamNetworkingMicroseconds);
		}

		public long m_SteamNetworkingMicroseconds;
	}
}
