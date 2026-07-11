using System;

namespace UnityEngine
{
	/// <summary>
	///   <para>SpriteRenderer draw mode.</para>
	/// </summary>
	public enum SpriteDrawMode
	{
		/// <summary>
		///   <para>Displays the full sprite.</para>
		/// </summary>
		Simple,
		/// <summary>
		///   <para>The SpriteRenderer will render the sprite as a 9-slice image where the corners will remain constant and the other sections will scale.</para>
		/// </summary>
		Sliced,
		/// <summary>
		///   <para>The SpriteRenderer will render the sprite as a 9-slice image where the corners will remain constant and the other sections will tile.</para>
		/// </summary>
		Tiled
	}
}
