using System;

namespace VYaml.Parser
{
	internal struct VersionDirective : ITokenContent
	{
		public VersionDirective(int major, int minor)
		{
			this.Major = major;
			this.Minor = minor;
		}

		public readonly int Major;

		public readonly int Minor;
	}
}
