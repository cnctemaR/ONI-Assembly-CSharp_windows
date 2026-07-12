using System;
using System.Runtime.CompilerServices;
using System.Threading;

namespace System
{
	internal class LocalAppContext
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static bool GetCachedSwitchValue(string switchName, ref int switchValue)
		{
			return switchValue >= 0 && (switchValue > 0 || LocalAppContext.GetCachedSwitchValueInternal(switchName, ref switchValue));
		}

		private static bool GetCachedSwitchValueInternal(string switchName, ref int switchValue)
		{
			bool flag;
			AppContext.TryGetSwitch(switchName, out flag);
			if (LocalAppContext.DisableCaching)
			{
				return flag;
			}
			switchValue = (flag ? 1 : (-1));
			return flag;
		}

		private static bool DisableCaching
		{
			get
			{
				return LazyInitializer.EnsureInitialized<bool>(ref LocalAppContext.s_disableCaching, ref LocalAppContext.s_isDisableCachingInitialized, ref LocalAppContext.s_syncObject, delegate
				{
					bool flag;
					AppContext.TryGetSwitch("TestSwitch.LocalAppContext.DisableCaching", out flag);
					return flag;
				});
			}
		}

		private static bool s_isDisableCachingInitialized;

		private static bool s_disableCaching;

		private static object s_syncObject;
	}
}
