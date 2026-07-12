using System;

namespace Steamworks
{
	[Serializable]
	public struct SteamInventoryUpdateHandle_t : IEquatable<SteamInventoryUpdateHandle_t>, IComparable<SteamInventoryUpdateHandle_t>
	{
		public SteamInventoryUpdateHandle_t(ulong value)
		{
			this.m_SteamInventoryUpdateHandle = value;
		}

		public override string ToString()
		{
			return this.m_SteamInventoryUpdateHandle.ToString();
		}

		public override bool Equals(object other)
		{
			return other is SteamInventoryUpdateHandle_t && this == (SteamInventoryUpdateHandle_t)other;
		}

		public override int GetHashCode()
		{
			return this.m_SteamInventoryUpdateHandle.GetHashCode();
		}

		public static bool operator ==(SteamInventoryUpdateHandle_t x, SteamInventoryUpdateHandle_t y)
		{
			return x.m_SteamInventoryUpdateHandle == y.m_SteamInventoryUpdateHandle;
		}

		public static bool operator !=(SteamInventoryUpdateHandle_t x, SteamInventoryUpdateHandle_t y)
		{
			return !(x == y);
		}

		public static explicit operator SteamInventoryUpdateHandle_t(ulong value)
		{
			return new SteamInventoryUpdateHandle_t(value);
		}

		public static explicit operator ulong(SteamInventoryUpdateHandle_t that)
		{
			return that.m_SteamInventoryUpdateHandle;
		}

		public bool Equals(SteamInventoryUpdateHandle_t other)
		{
			return this.m_SteamInventoryUpdateHandle == other.m_SteamInventoryUpdateHandle;
		}

		public int CompareTo(SteamInventoryUpdateHandle_t other)
		{
			return this.m_SteamInventoryUpdateHandle.CompareTo(other.m_SteamInventoryUpdateHandle);
		}

		public static readonly SteamInventoryUpdateHandle_t Invalid = new SteamInventoryUpdateHandle_t(ulong.MaxValue);

		public ulong m_SteamInventoryUpdateHandle;
	}
}
