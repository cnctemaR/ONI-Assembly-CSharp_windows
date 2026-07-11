using System;

namespace YamlDotNet.Core
{
	[Serializable]
	public class Version
	{
		public Version(int major, int minor)
		{
			this.Major = major;
			this.Minor = minor;
		}

		public int Major { get; private set; }

		public int Minor { get; private set; }

		public override bool Equals(object obj)
		{
			Version version = obj as Version;
			return version != null && this.Major == version.Major && this.Minor == version.Minor;
		}

		public override int GetHashCode()
		{
			return this.Major.GetHashCode() ^ this.Minor.GetHashCode();
		}
	}
}
