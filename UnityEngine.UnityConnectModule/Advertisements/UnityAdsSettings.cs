using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;

namespace UnityEngine.Advertisements
{
	[NativeHeader("Modules/UnityConnect/UnityAds/UnityAdsSettings.h")]
	internal static class UnityAdsSettings
	{
		[StaticAccessor("GetUnityAdsSettings()", StaticAccessorType.Dot)]
		public static extern bool enabled
		{
			[ThreadSafe]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[ThreadSafe]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		[Obsolete("warning No longer supported and will always return true")]
		public static bool IsPlatformEnabled(RuntimePlatform platform)
		{
			return true;
		}

		[Obsolete("warning No longer supported and will do nothing")]
		public static void SetPlatformEnabled(RuntimePlatform platform, bool value)
		{
		}

		[StaticAccessor("GetUnityAdsSettings()", StaticAccessorType.Dot)]
		public static extern bool initializeOnStartup
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		[StaticAccessor("GetUnityAdsSettings()", StaticAccessorType.Dot)]
		public static extern bool testMode
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		[StaticAccessor("GetUnityAdsSettings()", StaticAccessorType.Dot)]
		public static string GetGameId(RuntimePlatform platform)
		{
			string stringAndDispose;
			try
			{
				ManagedSpanWrapper managedSpanWrapper;
				UnityAdsSettings.GetGameId_Injected(platform, out managedSpanWrapper);
			}
			finally
			{
				ManagedSpanWrapper managedSpanWrapper;
				stringAndDispose = OutStringMarshaller.GetStringAndDispose(managedSpanWrapper);
			}
			return stringAndDispose;
		}

		[StaticAccessor("GetUnityAdsSettings()", StaticAccessorType.Dot)]
		public unsafe static void SetGameId(RuntimePlatform platform, string gameId)
		{
			try
			{
				ManagedSpanWrapper managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(gameId, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = gameId.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				UnityAdsSettings.SetGameId_Injected(platform, ref managedSpanWrapper);
			}
			finally
			{
				char* ptr = null;
			}
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetGameId_Injected(RuntimePlatform platform, out ManagedSpanWrapper ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetGameId_Injected(RuntimePlatform platform, ref ManagedSpanWrapper gameId);
	}
}
