using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine.Analytics
{
	[RequiredByNativeCode]
	[NativeHeader("UnityAnalyticsScriptingClasses.h")]
	[NativeHeader("Modules/UnityAnalytics/Public/UnityAnalytics.h")]
	public static class AnalyticsSessionInfo
	{
		[field: DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public static event AnalyticsSessionInfo.SessionStateChanged sessionStateChanged;

		[RequiredByNativeCode]
		internal static void CallSessionStateChanged(AnalyticsSessionState sessionState, long sessionId, long sessionElapsedTime, bool sessionChanged)
		{
			AnalyticsSessionInfo.SessionStateChanged sessionStateChanged = AnalyticsSessionInfo.sessionStateChanged;
			bool flag = sessionStateChanged != null;
			if (flag)
			{
				sessionStateChanged(sessionState, sessionId, sessionElapsedTime, sessionChanged);
			}
		}

		public static extern AnalyticsSessionState sessionState
		{
			[NativeMethod("GetPlayerSessionState")]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static extern long sessionId
		{
			[NativeMethod("GetPlayerSessionId")]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static extern long sessionCount
		{
			[NativeMethod("GetPlayerSessionCount")]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static extern long sessionElapsedTime
		{
			[NativeMethod("GetPlayerSessionElapsedTime")]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static extern bool sessionFirstRun
		{
			[NativeMethod("GetPlayerSessionFirstRun", false, true)]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static extern string userId
		{
			[NativeMethod("GetUserId")]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static string customUserId
		{
			get
			{
				bool flag = !Analytics.IsInitialized();
				string text;
				if (flag)
				{
					text = null;
				}
				else
				{
					text = AnalyticsSessionInfo.customUserIdInternal;
				}
				return text;
			}
			set
			{
				bool flag = Analytics.IsInitialized();
				if (flag)
				{
					AnalyticsSessionInfo.customUserIdInternal = value;
				}
			}
		}

		public static string customDeviceId
		{
			get
			{
				bool flag = !Analytics.IsInitialized();
				string text;
				if (flag)
				{
					text = null;
				}
				else
				{
					text = AnalyticsSessionInfo.customDeviceIdInternal;
				}
				return text;
			}
			set
			{
				bool flag = Analytics.IsInitialized();
				if (flag)
				{
					AnalyticsSessionInfo.customDeviceIdInternal = value;
				}
			}
		}

		[field: DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public static event AnalyticsSessionInfo.IdentityTokenChanged identityTokenChanged;

		[RequiredByNativeCode]
		internal static void CallIdentityTokenChanged(string token)
		{
			AnalyticsSessionInfo.IdentityTokenChanged identityTokenChanged = AnalyticsSessionInfo.identityTokenChanged;
			bool flag = identityTokenChanged != null;
			if (flag)
			{
				identityTokenChanged(token);
			}
		}

		public static string identityToken
		{
			get
			{
				bool flag = !Analytics.IsInitialized();
				string text;
				if (flag)
				{
					text = null;
				}
				else
				{
					text = AnalyticsSessionInfo.identityTokenInternal;
				}
				return text;
			}
		}

		[StaticAccessor("GetUnityAnalytics()", StaticAccessorType.Dot)]
		private static extern string identityTokenInternal
		{
			[NativeMethod("GetIdentityToken")]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		[StaticAccessor("GetUnityAnalytics()", StaticAccessorType.Dot)]
		private static extern string customUserIdInternal
		{
			[NativeMethod("GetCustomUserId")]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[NativeMethod("SetCustomUserId")]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		[StaticAccessor("GetUnityAnalytics()", StaticAccessorType.Dot)]
		private static extern string customDeviceIdInternal
		{
			[NativeMethod("GetCustomDeviceId")]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[NativeMethod("SetCustomDeviceId")]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public delegate void SessionStateChanged(AnalyticsSessionState sessionState, long sessionId, long sessionElapsedTime, bool sessionChanged);

		public delegate void IdentityTokenChanged(string token);
	}
}
