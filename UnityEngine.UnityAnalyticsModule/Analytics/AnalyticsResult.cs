using System;

namespace UnityEngine.Analytics
{
	/// <summary>
	///   <para>Analytics API result.</para>
	/// </summary>
	public enum AnalyticsResult
	{
		/// <summary>
		///   <para>Analytics API result: Success.</para>
		/// </summary>
		Ok,
		/// <summary>
		///   <para>Analytics API result: Analytics not initialized.</para>
		/// </summary>
		NotInitialized,
		/// <summary>
		///   <para>Analytics API result: Analytics is disabled.</para>
		/// </summary>
		AnalyticsDisabled,
		/// <summary>
		///   <para>Analytics API result: Too many parameters.</para>
		/// </summary>
		TooManyItems,
		/// <summary>
		///   <para>Analytics API result: Argument size limit.</para>
		/// </summary>
		SizeLimitReached,
		/// <summary>
		///   <para>Analytics API result: Too many requests.</para>
		/// </summary>
		TooManyRequests,
		/// <summary>
		///   <para>Analytics API result: Invalid argument value.</para>
		/// </summary>
		InvalidData,
		/// <summary>
		///   <para>Analytics API result: This platform doesn't support Analytics.</para>
		/// </summary>
		UnsupportedPlatform
	}
}
