using System;

namespace Steamworks
{
	[Serializable]
	public struct HSteamListenSocket : IEquatable<HSteamListenSocket>, IComparable<HSteamListenSocket>
	{
		public HSteamListenSocket(uint value)
		{
			this.m_HSteamListenSocket = value;
		}

		public override string ToString()
		{
			return this.m_HSteamListenSocket.ToString();
		}

		public override bool Equals(object other)
		{
			return other is HSteamListenSocket && this == (HSteamListenSocket)other;
		}

		public override int GetHashCode()
		{
			return this.m_HSteamListenSocket.GetHashCode();
		}

		public static bool operator ==(HSteamListenSocket x, HSteamListenSocket y)
		{
			return x.m_HSteamListenSocket == y.m_HSteamListenSocket;
		}

		public static bool operator !=(HSteamListenSocket x, HSteamListenSocket y)
		{
			return !(x == y);
		}

		public static explicit operator HSteamListenSocket(uint value)
		{
			return new HSteamListenSocket(value);
		}

		public static explicit operator uint(HSteamListenSocket that)
		{
			return that.m_HSteamListenSocket;
		}

		public bool Equals(HSteamListenSocket other)
		{
			return this.m_HSteamListenSocket == other.m_HSteamListenSocket;
		}

		public int CompareTo(HSteamListenSocket other)
		{
			return this.m_HSteamListenSocket.CompareTo(other.m_HSteamListenSocket);
		}

		public static readonly HSteamListenSocket Invalid = new HSteamListenSocket(0U);

		public uint m_HSteamListenSocket;
	}
}
