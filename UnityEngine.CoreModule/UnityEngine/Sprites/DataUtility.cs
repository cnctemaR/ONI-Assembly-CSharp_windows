using System;

namespace UnityEngine.Sprites
{
	/// <summary>
	///   <para>Helper utilities for accessing Sprite data.</para>
	/// </summary>
	public sealed class DataUtility
	{
		/// <summary>
		///   <para>Inner UV's of the Sprite.</para>
		/// </summary>
		/// <param name="sprite"></param>
		public static Vector4 GetInnerUV(Sprite sprite)
		{
			return sprite.GetInnerUVs();
		}

		/// <summary>
		///   <para>Outer UV's of the Sprite.</para>
		/// </summary>
		/// <param name="sprite"></param>
		public static Vector4 GetOuterUV(Sprite sprite)
		{
			return sprite.GetOuterUVs();
		}

		/// <summary>
		///   <para>Return the padding on the sprite.</para>
		/// </summary>
		/// <param name="sprite"></param>
		public static Vector4 GetPadding(Sprite sprite)
		{
			return sprite.GetPadding();
		}

		/// <summary>
		///   <para>Minimum width and height of the Sprite.</para>
		/// </summary>
		/// <param name="sprite"></param>
		public static Vector2 GetMinSize(Sprite sprite)
		{
			Vector2 vector;
			vector.x = sprite.border.x + sprite.border.z;
			vector.y = sprite.border.y + sprite.border.w;
			return vector;
		}
	}
}
