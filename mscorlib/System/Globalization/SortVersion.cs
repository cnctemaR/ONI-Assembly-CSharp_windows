using System;

namespace System.Globalization
{
	[Serializable]
	public sealed class SortVersion : IEquatable<SortVersion>
	{
		public int FullVersion
		{
			get
			{
				return this.m_NlsVersion;
			}
		}

		public Guid SortId
		{
			get
			{
				return this.m_SortId;
			}
		}

		public SortVersion(int fullVersion, Guid sortId)
		{
			this.m_SortId = sortId;
			this.m_NlsVersion = fullVersion;
		}

		internal SortVersion(int nlsVersion, int effectiveId, Guid customVersion)
		{
			this.m_NlsVersion = nlsVersion;
			if (customVersion == Guid.Empty)
			{
				byte b = (byte)((uint)effectiveId >> 24);
				byte b2 = (byte)((effectiveId & 16711680) >> 16);
				byte b3 = (byte)((effectiveId & 65280) >> 8);
				byte b4 = (byte)(effectiveId & 255);
				customVersion = new Guid(0, 0, 0, 0, 0, 0, 0, b, b2, b3, b4);
			}
			this.m_SortId = customVersion;
		}

		public override bool Equals(object obj)
		{
			SortVersion sortVersion = obj as SortVersion;
			return sortVersion != null && this.Equals(sortVersion);
		}

		public bool Equals(SortVersion other)
		{
			return !(other == null) && this.m_NlsVersion == other.m_NlsVersion && this.m_SortId == other.m_SortId;
		}

		public override int GetHashCode()
		{
			return (this.m_NlsVersion * 7) | this.m_SortId.GetHashCode();
		}

		public static bool operator ==(SortVersion left, SortVersion right)
		{
			if (left != null)
			{
				return left.Equals(right);
			}
			return right == null || right.Equals(left);
		}

		public static bool operator !=(SortVersion left, SortVersion right)
		{
			return !(left == right);
		}

		private int m_NlsVersion;

		private Guid m_SortId;
	}
}
