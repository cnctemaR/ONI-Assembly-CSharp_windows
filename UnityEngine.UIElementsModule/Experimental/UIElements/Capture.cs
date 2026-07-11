using System;

namespace UnityEngine.Experimental.UIElements
{
	/// <summary>
	///   <para>Used to specify the phases where an event handler should be executed.</para>
	/// </summary>
	public enum Capture
	{
		/// <summary>
		///   <para>The event handler should be executed during the target and bubble up phases.</para>
		/// </summary>
		NoCapture,
		/// <summary>
		///   <para>The event handler should be executed during the capture and the target phases.</para>
		/// </summary>
		Capture
	}
}
