using System;

namespace Harmony
{
	[AttributeUsage(AttributeTargets.Constructor | AttributeTargets.Method)]
	internal class UpgradeToLatestVersion : Attribute
	{
		public UpgradeToLatestVersion(int version)
		{
			this.version = version;
		}

		public int version;
	}
}
