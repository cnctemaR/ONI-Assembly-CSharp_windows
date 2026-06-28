using System;
using System.Runtime.CompilerServices;
using UnityEngine.Internal;
using UnityEngine.Scripting;

namespace UnityEngine
{
	public sealed class Sprite : Object
	{
		public static Sprite Create(Texture2D texture, Rect rect, Vector2 pivot, [DefaultValue("100.0f")] float pixelsPerUnit, [DefaultValue("0")] uint extrude, [DefaultValue("SpriteMeshType.Tight")] SpriteMeshType meshType, [DefaultValue("Vector4.zero")] Vector4 border)
		{
			return Sprite.INTERNAL_CALL_Create(texture, ref rect, ref pivot, pixelsPerUnit, extrude, meshType, ref border);
		}

		[ExcludeFromDocs]
		public static Sprite Create(Texture2D texture, Rect rect, Vector2 pivot, float pixelsPerUnit, uint extrude, SpriteMeshType meshType)
		{
			Vector4 zero = Vector4.zero;
			return Sprite.INTERNAL_CALL_Create(texture, ref rect, ref pivot, pixelsPerUnit, extrude, meshType, ref zero);
		}

		[ExcludeFromDocs]
		public static Sprite Create(Texture2D texture, Rect rect, Vector2 pivot, float pixelsPerUnit, uint extrude)
		{
			Vector4 zero = Vector4.zero;
			SpriteMeshType spriteMeshType = SpriteMeshType.Tight;
			return Sprite.INTERNAL_CALL_Create(texture, ref rect, ref pivot, pixelsPerUnit, extrude, spriteMeshType, ref zero);
		}

		[ExcludeFromDocs]
		public static Sprite Create(Texture2D texture, Rect rect, Vector2 pivot, float pixelsPerUnit)
		{
			Vector4 zero = Vector4.zero;
			SpriteMeshType spriteMeshType = SpriteMeshType.Tight;
			uint num = 0U;
			return Sprite.INTERNAL_CALL_Create(texture, ref rect, ref pivot, pixelsPerUnit, num, spriteMeshType, ref zero);
		}

		[ExcludeFromDocs]
		public static Sprite Create(Texture2D texture, Rect rect, Vector2 pivot)
		{
			Vector4 zero = Vector4.zero;
			SpriteMeshType spriteMeshType = SpriteMeshType.Tight;
			uint num = 0U;
			float num2 = 100f;
			return Sprite.INTERNAL_CALL_Create(texture, ref rect, ref pivot, num2, num, spriteMeshType, ref zero);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern Sprite INTERNAL_CALL_Create(Texture2D texture, ref Rect rect, ref Vector2 pivot, float pixelsPerUnit, uint extrude, SpriteMeshType meshType, ref Vector4 border);

		public Bounds bounds
		{
			get
			{
				Bounds bounds;
				this.INTERNAL_get_bounds(out bounds);
				return bounds;
			}
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_get_bounds(out Bounds value);

		public Rect rect
		{
			get
			{
				Rect rect;
				this.INTERNAL_get_rect(out rect);
				return rect;
			}
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_get_rect(out Rect value);

		public extern float pixelsPerUnit
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern Texture2D texture
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern Texture2D associatedAlphaSplitTexture
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public Rect textureRect
		{
			get
			{
				Rect rect;
				this.INTERNAL_get_textureRect(out rect);
				return rect;
			}
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_get_textureRect(out Rect value);

		public Vector2 textureRectOffset
		{
			get
			{
				Vector2 vector;
				Sprite.Internal_GetTextureRectOffset(this, out vector);
				return vector;
			}
		}

		public extern bool packed
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern SpritePackingMode packingMode
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern SpritePackingRotation packingRotation
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_GetTextureRectOffset(Sprite sprite, out Vector2 output);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_GetPivot(Sprite sprite, out Vector2 output);

		public Vector2 pivot
		{
			get
			{
				Vector2 vector;
				Sprite.Internal_GetPivot(this, out vector);
				return vector;
			}
		}

		public Vector4 border
		{
			get
			{
				Vector4 vector;
				this.INTERNAL_get_border(out vector);
				return vector;
			}
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_get_border(out Vector4 value);

		public extern Vector2[] vertices
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern ushort[] triangles
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern Vector2[] uv
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void OverrideGeometry(Vector2[] vertices, ushort[] triangles);
	}
}
