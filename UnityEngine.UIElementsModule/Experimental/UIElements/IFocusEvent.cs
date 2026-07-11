using System;

namespace UnityEngine.Experimental.UIElements
{
	public interface IFocusEvent
	{
		/// <summary>
		///   <para>Related target. See implementation for specific meaning.</para>
		/// </summary>
		Focusable relatedTarget { get; }

		/// <summary>
		///   <para>Direction of the focus change.</para>
		/// </summary>
		FocusChangeDirection direction { get; }
	}
}
