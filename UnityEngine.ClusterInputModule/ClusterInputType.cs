using System;

namespace UnityEngine
{
	/// <summary>
	///   <para>Values to determine the type of input value to be expect from one entry of ClusterInput.</para>
	/// </summary>
	public enum ClusterInputType
	{
		/// <summary>
		///   <para>Device that return a binary result of pressed or not pressed.</para>
		/// </summary>
		Button,
		/// <summary>
		///   <para>Device is an analog axis that provides continuous value represented by a float.</para>
		/// </summary>
		Axis,
		/// <summary>
		///   <para>Device that provide position and orientation values.</para>
		/// </summary>
		Tracker,
		/// <summary>
		///   <para>A user customized input.</para>
		/// </summary>
		CustomProvidedInput
	}
}
