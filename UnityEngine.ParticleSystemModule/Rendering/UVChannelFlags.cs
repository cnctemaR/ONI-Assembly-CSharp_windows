using System;

namespace UnityEngine.Rendering
{
	/// <summary>
	///   <para>A flag representing each UV channel.</para>
	/// </summary>
	[Flags]
	public enum UVChannelFlags
	{
		/// <summary>
		///   <para>First UV channel.</para>
		/// </summary>
		UV0 = 1,
		/// <summary>
		///   <para>Second UV channel.</para>
		/// </summary>
		UV1 = 2,
		/// <summary>
		///   <para>Third UV channel.</para>
		/// </summary>
		UV2 = 4,
		/// <summary>
		///   <para>Fourth UV channel.</para>
		/// </summary>
		UV3 = 8
	}
}
