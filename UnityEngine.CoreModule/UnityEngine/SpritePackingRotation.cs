using System;

namespace UnityEngine
{
	/// <summary>
	///   <para>Sprite rotation modes for the Sprite Packer.</para>
	/// </summary>
	public enum SpritePackingRotation
	{
		/// <summary>
		///   <para>No rotation.</para>
		/// </summary>
		None,
		/// <summary>
		///   <para>Sprite is flipped horizontally when packed.</para>
		/// </summary>
		FlipHorizontal,
		/// <summary>
		///   <para>Sprite is flipped vertically when packed.</para>
		/// </summary>
		FlipVertical,
		/// <summary>
		///   <para>Sprite is rotated 180 degree when packed.</para>
		/// </summary>
		Rotate180,
		/// <summary>
		///   <para>Any rotation.</para>
		/// </summary>
		Any = 15
	}
}
