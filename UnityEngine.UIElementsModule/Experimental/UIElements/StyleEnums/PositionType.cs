using System;

namespace UnityEngine.Experimental.UIElements.StyleEnums
{
	/// <summary>
	///   <para>This enumeration contains values to control how an element is positioned in its parent container.</para>
	/// </summary>
	public enum PositionType
	{
		/// <summary>
		///   <para>The element is positioned in relation to its default box as calculated by layout.</para>
		/// </summary>
		Relative,
		/// <summary>
		///   <para>The element is positioned in relation to its parent box and does not contribute to the layout anymore.</para>
		/// </summary>
		Absolute,
		Manual
	}
}
