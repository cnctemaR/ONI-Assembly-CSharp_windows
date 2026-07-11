using System;

namespace System
{
	internal static class AppContextDefaultValues
	{
		public static void PopulateDefaultValues()
		{
		}

		public static bool TryGetSwitchOverride(string switchName, out bool overrideValue)
		{
			overrideValue = false;
			return false;
		}

		internal const string SwitchNoAsyncCurrentCulture = "Switch.System.Globalization.NoAsyncCurrentCulture";

		internal const string SwitchThrowExceptionIfDisposedCancellationTokenSource = "Switch.System.Threading.ThrowExceptionIfDisposedCancellationTokenSource";

		internal const string SwitchPreserveEventListnerObjectIdentity = "Switch.System.Diagnostics.EventSource.PreserveEventListnerObjectIdentity";

		internal const string SwitchUseLegacyPathHandling = "Switch.System.IO.UseLegacyPathHandling";

		internal const string SwitchBlockLongPaths = "Switch.System.IO.BlockLongPaths";

		internal const string SwitchDoNotAddrOfCspParentWindowHandle = "Switch.System.Security.Cryptography.DoNotAddrOfCspParentWindowHandle";

		internal const string SwitchSetActorAsReferenceWhenCopyingClaimsIdentity = "Switch.System.Security.ClaimsIdentity.SetActorAsReferenceWhenCopyingClaimsIdentity";
	}
}
