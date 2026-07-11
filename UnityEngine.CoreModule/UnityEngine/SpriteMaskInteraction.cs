using System;

namespace UnityEngine
{
	/// <summary>
	///   <para>This enum controls the mode under which the sprite will interact with the masking system.</para>
	/// </summary>
	public enum SpriteMaskInteraction
	{
		/// <summary>
		///   <para>The sprite will not interact with the masking system.</para>
		/// </summary>
		None,
		/// <summary>
		///   <para>The sprite will be visible only in areas where a mask is present.</para>
		/// </summary>
		VisibleInsideMask,
		/// <summary>
		///   <para>The sprite will be visible only in areas where no mask is present.</para>
		/// </summary>
		VisibleOutsideMask
	}
}
