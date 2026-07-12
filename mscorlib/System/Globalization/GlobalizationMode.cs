using System;

namespace System.Globalization
{
	internal static class GlobalizationMode
	{
		internal static bool Invariant { get; } = GlobalizationMode.GetGlobalizationInvariantMode();

		private static bool GetGlobalizationInvariantMode()
		{
			return false;
		}

		private const string c_InvariantModeConfigSwitch = "System.Globalization.Invariant";
	}
}
