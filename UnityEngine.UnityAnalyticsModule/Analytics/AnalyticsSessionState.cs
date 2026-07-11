using System;
using UnityEngine.Scripting;

namespace UnityEngine.Analytics
{
	/// <summary>
	///   <para>Session tracking states.</para>
	/// </summary>
	[RequiredByNativeCode]
	public enum AnalyticsSessionState
	{
		/// <summary>
		///   <para>Session tracking has stopped.</para>
		/// </summary>
		kSessionStopped,
		/// <summary>
		///   <para>Session tracking has started.</para>
		/// </summary>
		kSessionStarted,
		/// <summary>
		///   <para>Session tracking has paused.</para>
		/// </summary>
		kSessionPaused,
		/// <summary>
		///   <para>Session tracking has resumed.</para>
		/// </summary>
		kSessionResumed
	}
}
