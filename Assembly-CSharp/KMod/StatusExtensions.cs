using System;

namespace KMod
{
	internal static class StatusExtensions
	{
		public static bool ShouldTryInstall(this Mod.Status status)
		{
			return status == Mod.Status.NotInstalled || status == Mod.Status.CannotInstall;
		}
	}
}
