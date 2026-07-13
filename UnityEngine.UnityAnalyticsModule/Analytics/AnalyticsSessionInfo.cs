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

		public static string userId
		{
			[NativeMethod("GetUserId")]
			get
			{
				string stringAndDispose;
				try
				{
					ManagedSpanWrapper managedSpanWrapper;
					AnalyticsSessionInfo.get_userId_Injected(out managedSpanWrapper);
				}
				finally
				{
					ManagedSpanWrapper managedSpanWrapper;
					stringAndDispose = OutStringMarshaller.GetStringAndDispose(managedSpanWrapper);
				}
				return stringAndDispose;
			}
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
		private static string identityTokenInternal
		{
			[NativeMethod("GetIdentityToken")]
			get
			{
				string stringAndDispose;
				try
				{
					ManagedSpanWrapper managedSpanWrapper;
					AnalyticsSessionInfo.get_identityTokenInternal_Injected(out managedSpanWrapper);
				}
				finally
				{
					ManagedSpanWrapper managedSpanWrapper;
					stringAndDispose = OutStringMarshaller.GetStringAndDispose(managedSpanWrapper);
				}
				return stringAndDispose;
			}
		}

		[StaticAccessor("GetUnityAnalytics()", StaticAccessorType.Dot)]
		private unsafe static string customUserIdInternal
		{
			[NativeMethod("GetCustomUserId")]
			get
			{
				string stringAndDispose;
				try
				{
					ManagedSpanWrapper managedSpanWrapper;
					AnalyticsSessionInfo.get_customUserIdInternal_Injected(out managedSpanWrapper);
				}
				finally
				{
					ManagedSpanWrapper managedSpanWrapper;
					stringAndDispose = OutStringMarshaller.GetStringAndDispose(managedSpanWrapper);
				}
				return stringAndDispose;
			}
			[NativeMethod("SetCustomUserId")]
			set
			{
				try
				{
					ManagedSpanWrapper managedSpanWrapper;
					if (!StringMarshaller.TryMarshalEmptyOrNullString(value, ref managedSpanWrapper))
					{
						ReadOnlySpan<char> readOnlySpan = value.AsSpan();
						fixed (char* ptr = readOnlySpan.GetPinnableReference())
						{
							managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
						}
					}
					AnalyticsSessionInfo.set_customUserIdInternal_Injected(ref managedSpanWrapper);
				}
				finally
				{
					char* ptr = null;
				}
			}
		}

		[StaticAccessor("GetUnityAnalytics()", StaticAccessorType.Dot)]
		private unsafe static string customDeviceIdInternal
		{
			[NativeMethod("GetCustomDeviceId")]
			get
			{
				string stringAndDispose;
				try
				{
					ManagedSpanWrapper managedSpanWrapper;
					AnalyticsSessionInfo.get_customDeviceIdInternal_Injected(out managedSpanWrapper);
				}
				finally
				{
					ManagedSpanWrapper managedSpanWrapper;
					stringAndDispose = OutStringMarshaller.GetStringAndDispose(managedSpanWrapper);
				}
				return stringAndDispose;
			}
			[NativeMethod("SetCustomDeviceId")]
			set
			{
				try
				{
					ManagedSpanWrapper managedSpanWrapper;
					if (!StringMarshaller.TryMarshalEmptyOrNullString(value, ref managedSpanWrapper))
					{
						ReadOnlySpan<char> readOnlySpan = value.AsSpan();
						fixed (char* ptr = readOnlySpan.GetPinnableReference())
						{
							managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
						}
					}
					AnalyticsSessionInfo.set_customDeviceIdInternal_Injected(ref managedSpanWrapper);
				}
				finally
				{
					char* ptr = null;
				}
			}
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_userId_Injected(out ManagedSpanWrapper ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_identityTokenInternal_Injected(out ManagedSpanWrapper ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_customUserIdInternal_Injected(out ManagedSpanWrapper ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_customUserIdInternal_Injected(ref ManagedSpanWrapper value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_customDeviceIdInternal_Injected(out ManagedSpanWrapper ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_customDeviceIdInternal_Injected(ref ManagedSpanWrapper value);

		public delegate void SessionStateChanged(AnalyticsSessionState sessionState, long sessionId, long sessionElapsedTime, bool sessionChanged);

		public delegate void IdentityTokenChanged(string token);
	}
}
