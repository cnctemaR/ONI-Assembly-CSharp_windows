using System;

namespace UnityEngine.Experimental.UIElements
{
	/// <summary>
	///   <para>Controls how many items can be selected at once.</para>
	/// </summary>
	public enum SelectionType
	{
		/// <summary>
		///   <para>Selections are disabled.</para>
		/// </summary>
		None,
		/// <summary>
		///   <para>Only one item is selectable.</para>
		/// </summary>
		Single,
		/// <summary>
		///   <para>Multiple items are selectable at once.</para>
		/// </summary>
		Multiple
	}
}
