using System;

namespace UnityEngine
{
	/// <summary>
	///   <para>Tiling mode for SpriteRenderer.tileMode.</para>
	/// </summary>
	public enum SpriteTileMode
	{
		/// <summary>
		///   <para>Sprite Renderer tiles the sprite continuously when is set to SpriteRenderer.tileMode.</para>
		/// </summary>
		Continuous,
		/// <summary>
		///   <para>Sprite Renderer tiles the sprite once the Sprite Renderer size is above SpriteRenderer.adaptiveModeThreshold.</para>
		/// </summary>
		Adaptive
	}
}
