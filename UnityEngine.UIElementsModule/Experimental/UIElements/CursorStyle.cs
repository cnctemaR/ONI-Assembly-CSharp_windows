using System;

namespace UnityEngine.Experimental.UIElements
{
	/// <summary>
	///   <para>Script interface for VisualElement cursor style property IStyle.cursor.</para>
	/// </summary>
	public struct CursorStyle : IEquatable<CursorStyle>
	{
		/// <summary>
		///   <para>The texture to use for the cursor style. To use a texture as a cursor, import the texture with "Read/Write enabled" in the texture importer (or using the "Cursor" defaults).</para>
		/// </summary>
		public Texture2D texture { get; set; }

		/// <summary>
		///   <para>The offset from the top left of the texture to use as the target point (must be within the bounds of the cursor).</para>
		/// </summary>
		public Vector2 hotspot { get; set; }

		internal int defaultCursorId { get; set; }

		public override int GetHashCode()
		{
			return this.texture.GetHashCode() ^ this.hotspot.GetHashCode() ^ this.defaultCursorId.GetHashCode();
		}

		public override bool Equals(object other)
		{
			return other is CursorStyle && this.Equals((CursorStyle)other);
		}

		public bool Equals(CursorStyle other)
		{
			return this.texture.Equals(other.texture) && this.hotspot.Equals(other.hotspot) && this.defaultCursorId == other.defaultCursorId;
		}

		public static bool operator ==(CursorStyle lhs, CursorStyle rhs)
		{
			return lhs.texture == rhs.texture && lhs.hotspot == rhs.hotspot;
		}

		public static bool operator !=(CursorStyle lhs, CursorStyle rhs)
		{
			return !(lhs == rhs);
		}
	}
}
