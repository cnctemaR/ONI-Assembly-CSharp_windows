using System;
using System.Runtime.CompilerServices;

namespace System
{
	internal static class AppContextSwitches
	{
		public static bool NoAsyncCurrentCulture
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return AppContextSwitches.GetCachedSwitchValue("Switch.System.Globalization.NoAsyncCurrentCulture", ref AppContextSwitches._noAsyncCurrentCulture);
			}
		}

		public static bool ThrowExceptionIfDisposedCancellationTokenSource
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return AppContextSwitches.GetCachedSwitchValue("Switch.System.Threading.ThrowExceptionIfDisposedCancellationTokenSource", ref AppContextSwitches._throwExceptionIfDisposedCancellationTokenSource);
			}
		}

		public static bool PreserveEventListnerObjectIdentity
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return AppContextSwitches.GetCachedSwitchValue("Switch.System.Diagnostics.EventSource.PreserveEventListnerObjectIdentity", ref AppContextSwitches._preserveEventListnerObjectIdentity);
			}
		}

		public static bool UseLegacyPathHandling
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return AppContextSwitches.GetCachedSwitchValue("Switch.System.IO.UseLegacyPathHandling", ref AppContextSwitches._useLegacyPathHandling);
			}
		}

		public static bool BlockLongPaths
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return AppContextSwitches.GetCachedSwitchValue("Switch.System.IO.BlockLongPaths", ref AppContextSwitches._blockLongPaths);
			}
		}

		public static bool SetActorAsReferenceWhenCopyingClaimsIdentity
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return AppContextSwitches.GetCachedSwitchValue("Switch.System.Security.ClaimsIdentity.SetActorAsReferenceWhenCopyingClaimsIdentity", ref AppContextSwitches._cloneActor);
			}
		}

		public static bool DoNotAddrOfCspParentWindowHandle
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return AppContextSwitches.GetCachedSwitchValue("Switch.System.Security.Cryptography.DoNotAddrOfCspParentWindowHandle", ref AppContextSwitches._doNotAddrOfCspParentWindowHandle);
			}
		}

		private static bool DisableCaching { get; set; }

		static AppContextSwitches()
		{
			bool flag;
			if (AppContext.TryGetSwitch("TestSwitch.LocalAppContext.DisableCaching", out flag))
			{
				AppContextSwitches.DisableCaching = flag;
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static bool GetCachedSwitchValue(string switchName, ref int switchValue)
		{
			return switchValue >= 0 && (switchValue > 0 || AppContextSwitches.GetCachedSwitchValueInternal(switchName, ref switchValue));
		}

		private static bool GetCachedSwitchValueInternal(string switchName, ref int switchValue)
		{
			bool flag;
			AppContext.TryGetSwitch(switchName, out flag);
			if (AppContextSwitches.DisableCaching)
			{
				return flag;
			}
			switchValue = (flag ? 1 : (-1));
			return flag;
		}

		private static int _noAsyncCurrentCulture;

		private static int _throwExceptionIfDisposedCancellationTokenSource;

		private static int _preserveEventListnerObjectIdentity;

		private static int _useLegacyPathHandling;

		private static int _blockLongPaths;

		private static int _cloneActor;

		private static int _doNotAddrOfCspParentWindowHandle;
	}
}
