using System;
using System.Runtime.CompilerServices;

namespace System
{
	internal static class LocalAppContextSwitches
	{
		public static bool AllowArbitraryTypeInstantiation
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return LocalAppContext.GetCachedSwitchValue("Switch.System.Data.AllowArbitraryDataSetTypeInstantiation", ref global::System.LocalAppContextSwitches.s_allowArbitraryTypeInstantiation);
			}
		}

		private static int s_allowArbitraryTypeInstantiation;
	}
}
