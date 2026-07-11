using System;

namespace YamlDotNet.Core.Tokens
{
	[Serializable]
	public class VersionDirective : Token
	{
		public VersionDirective(Version version)
			: this(version, Mark.Empty, Mark.Empty)
		{
		}

		public VersionDirective(Version version, Mark start, Mark end)
			: base(start, end)
		{
			this.version = version;
		}

		public Version Version
		{
			get
			{
				return this.version;
			}
		}

		public override bool Equals(object obj)
		{
			VersionDirective versionDirective = obj as VersionDirective;
			return versionDirective != null && this.version.Equals(versionDirective.version);
		}

		public override int GetHashCode()
		{
			return this.version.GetHashCode();
		}

		private readonly Version version;
	}
}
