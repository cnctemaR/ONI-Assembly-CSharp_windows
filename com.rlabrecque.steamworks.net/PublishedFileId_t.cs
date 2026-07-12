using System;

namespace Steamworks
{
	[Serializable]
	public struct PublishedFileId_t : IEquatable<PublishedFileId_t>, IComparable<PublishedFileId_t>
	{
		public PublishedFileId_t(ulong value)
		{
			this.m_PublishedFileId = value;
		}

		public override string ToString()
		{
			return this.m_PublishedFileId.ToString();
		}

		public override bool Equals(object other)
		{
			return other is PublishedFileId_t && this == (PublishedFileId_t)other;
		}

		public override int GetHashCode()
		{
			return this.m_PublishedFileId.GetHashCode();
		}

		public static bool operator ==(PublishedFileId_t x, PublishedFileId_t y)
		{
			return x.m_PublishedFileId == y.m_PublishedFileId;
		}

		public static bool operator !=(PublishedFileId_t x, PublishedFileId_t y)
		{
			return !(x == y);
		}

		public static explicit operator PublishedFileId_t(ulong value)
		{
			return new PublishedFileId_t(value);
		}

		public static explicit operator ulong(PublishedFileId_t that)
		{
			return that.m_PublishedFileId;
		}

		public bool Equals(PublishedFileId_t other)
		{
			return this.m_PublishedFileId == other.m_PublishedFileId;
		}

		public int CompareTo(PublishedFileId_t other)
		{
			return this.m_PublishedFileId.CompareTo(other.m_PublishedFileId);
		}

		public static readonly PublishedFileId_t Invalid = new PublishedFileId_t(0UL);

		public ulong m_PublishedFileId;
	}
}
