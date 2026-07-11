using System;

namespace UnityEngine.Experimental.UIElements.StyleEnums
{
	/// <summary>
	///   <para>This enumeration contains values to control how children are justified during layout.</para>
	/// </summary>
	public enum Justify
	{
		/// <summary>
		///   <para>Items are justified towards the beginning of the main axis.</para>
		/// </summary>
		FlexStart,
		/// <summary>
		///   <para>Items are centered.</para>
		/// </summary>
		Center,
		/// <summary>
		///   <para>Items are justified towards the end of the layout direction.</para>
		/// </summary>
		FlexEnd,
		/// <summary>
		///   <para>Items are evenly distributed in the line; first item is at the beginning of the line, last item is at the end.</para>
		/// </summary>
		SpaceBetween,
		/// <summary>
		///   <para>Items are evenly distributed in the line  with extra space on each end of the line.</para>
		/// </summary>
		SpaceAround
	}
}
