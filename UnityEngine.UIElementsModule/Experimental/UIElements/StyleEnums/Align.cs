using System;

namespace UnityEngine.Experimental.UIElements.StyleEnums
{
	/// <summary>
	///   <para>This enumeration contains values to control how an element is aligned in its parent during the layout phase.</para>
	/// </summary>
	public enum Align
	{
		/// <summary>
		///   <para>Default value (currently FlexStart).</para>
		/// </summary>
		Auto,
		/// <summary>
		///   <para>Items are aligned at the beginning on the cross axis.</para>
		/// </summary>
		FlexStart,
		/// <summary>
		///   <para>Items are centered on the cross axis.</para>
		/// </summary>
		Center,
		/// <summary>
		///   <para>Items are aligned at the end on the cross axis.</para>
		/// </summary>
		FlexEnd,
		/// <summary>
		///   <para>Stretches items on the cross axis.</para>
		/// </summary>
		Stretch
	}
}
