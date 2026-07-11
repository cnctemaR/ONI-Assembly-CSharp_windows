using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine.Analytics
{
	/// <summary>
	///   <para>Accesses for Analytics session information (common for all game instances).</para>
	/// </summary>
	[NativeHeader("Modules/UnityAnalytics/CoreStats/UnityConnectClient.h")]
	[RequiredByNativeCode]
	[NativeHeader("UnityAnalyticsScriptingClasses.h")]
	public static class AnalyticsSessionInfo
	{
		[field: DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public static event AnalyticsSessionInfo.SessionStateChanged sessionStateChanged;

		[RequiredByNativeCode]
		internal static void CallSessionStateChanged(AnalyticsSessionState sessionState, long sessionId, long sessionElapsedTime, bool sessionChanged)
		{
			AnalyticsSessionInfo.SessionStateChanged sessionStateChanged = AnalyticsSessionInfo.sessionStateChanged;
			if (sessionStateChanged != null)
			{
				sessionStateChanged(sessionState, sessionId, sessionElapsedTime, sessionChanged);
			}
		}

		/// <summary>
		///   <para>Session state.</para>
		/// </summary>
		public static extern AnalyticsSessionState sessionState
		{
			[NativeMethod("GetPlayerSessionState")]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		/// <summary>
		///   <para>Session id is used for tracking player game session.</para>
		/// </summary>
		public static extern long sessionId
		{
			[NativeMethod("GetPlayerSessionId")]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		/// <summary>
		///   <para>Session time since the begining of player game session.</para>
		/// </summary>
		public static extern long sessionElapsedTime
		{
			[NativeMethod("GetPlayerSessionElapsedTime")]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		/// <summary>
		///   <para>UserId is random GUID to track a player and is persisted across game session.</para>
		/// </summary>
		public static extern string userId
		{
			[NativeMethod("GetUserId")]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		/// <summary>
		///   <para>This event occurs when a Analytics session state changes.</para>
		/// </summary>
		/// <param name="sessionState">Current session state.</param>
		/// <param name="sessionId">Current session id.</param>
		/// <param name="sessionElapsedTime">Game player current session time.</param>
		/// <param name="sessionChanged">Set to true when sessionId has changed.</param>
		public delegate void SessionStateChanged(AnalyticsSessionState sessionState, long sessionId, long sessionElapsedTime, bool sessionChanged);
	}
}
