using System;

namespace UnityEngine.Experimental.UIElements.StyleEnums
{
	/// <summary>
	///   <para>This enumeration contains values to specify whether or not an element is visible.</para>
	/// </summary>
	public enum Visibility
	{
		/// <summary>
		///   <para>The element is drawn normally (default).</para>
		/// </summary>
		Visible,
		/// <summary>
		///   <para>The picking and rendering of this element is skipped. It still takes space in the layout.</para>
		/// </summary>
		Hidden
	}
}
