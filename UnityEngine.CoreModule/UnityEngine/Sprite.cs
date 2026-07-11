using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine
{
	/// <summary>
	///   <para>Represents a Sprite object for use in 2D gameplay.</para>
	/// </summary>
	[NativeHeader("Runtime/2D/Common/ScriptBindings/SpritesMarshalling.h")]
	[NativeHeader("Runtime/2D/Common/SpriteDataAccess.h")]
	[NativeHeader("Runtime/Graphics/SpriteUtility.h")]
	[NativeType("Runtime/Graphics/SpriteFrame.h")]
	public sealed class Sprite : Object
	{
		[RequiredByNativeCode]
		private Sprite()
		{
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern int GetPackingMode();

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern int GetPackingRotation();

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern int GetPacked();

		internal Rect GetTextureRect()
		{
			Rect rect;
			this.GetTextureRect_Injected(out rect);
			return rect;
		}

		internal Vector2 GetTextureRectOffset()
		{
			Vector2 vector;
			this.GetTextureRectOffset_Injected(out vector);
			return vector;
		}

		internal Vector4 GetInnerUVs()
		{
			Vector4 vector;
			this.GetInnerUVs_Injected(out vector);
			return vector;
		}

		internal Vector4 GetOuterUVs()
		{
			Vector4 vector;
			this.GetOuterUVs_Injected(out vector);
			return vector;
		}

		internal Vector4 GetPadding()
		{
			Vector4 vector;
			this.GetPadding_Injected(out vector);
			return vector;
		}

		[FreeFunction("SpritesBindings::CreateSpriteWithoutTextureScripting")]
		internal static Sprite CreateSpriteWithoutTextureScripting(Rect rect, Vector2 pivot, float pixelsToUnits, Texture2D texture)
		{
			return Sprite.CreateSpriteWithoutTextureScripting_Injected(ref rect, ref pivot, pixelsToUnits, texture);
		}

		[FreeFunction("SpritesBindings::CreateSprite")]
		internal static Sprite CreateSprite(Texture2D texture, Rect rect, Vector2 pivot, float pixelsPerUnit, uint extrude, SpriteMeshType meshType, Vector4 border, bool generateFallbackPhysicsShape)
		{
			return Sprite.CreateSprite_Injected(texture, ref rect, ref pivot, pixelsPerUnit, extrude, meshType, ref border, generateFallbackPhysicsShape);
		}

		/// <summary>
		///   <para>Bounds of the Sprite, specified by its center and extents in world space units.</para>
		/// </summary>
		public Bounds bounds
		{
			get
			{
				Bounds bounds;
				this.get_bounds_Injected(out bounds);
				return bounds;
			}
		}

		/// <summary>
		///   <para>Location of the Sprite on the original Texture, specified in pixels.</para>
		/// </summary>
		public Rect rect
		{
			get
			{
				Rect rect;
				this.get_rect_Injected(out rect);
				return rect;
			}
		}

		/// <summary>
		///   <para>Returns the border sizes of the sprite.</para>
		/// </summary>
		public Vector4 border
		{
			get
			{
				Vector4 vector;
				this.get_border_Injected(out vector);
				return vector;
			}
		}

		/// <summary>
		///   <para>Get the reference to the used texture. If packed this will point to the atlas, if not packed will point to the source sprite.</para>
		/// </summary>
		public extern Texture2D texture
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		/// <summary>
		///   <para>The number of pixels in the sprite that correspond to one unit in world space. (Read Only)</para>
		/// </summary>
		public extern float pixelsPerUnit
		{
			[NativeMethod("GetPixelsToUnits")]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		/// <summary>
		///   <para>Returns the texture that contains the alpha channel from the source texture. Unity generates this texture under the hood for sprites that have alpha in the source, and need to be compressed using techniques like ETC1.
		///
		/// Returns NULL if there is no associated alpha texture for the source sprite. This is the case if the sprite has not been setup to use ETC1 compression.</para>
		/// </summary>
		public extern Texture2D associatedAlphaSplitTexture
		{
			[NativeMethod("GetAlphaTexture")]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		/// <summary>
		///   <para>Location of the Sprite's center point in the Rect on the original Texture, specified in pixels.</para>
		/// </summary>
		public Vector2 pivot
		{
			[NativeMethod("GetPivotInPixels")]
			get
			{
				Vector2 vector;
				this.get_pivot_Injected(out vector);
				return vector;
			}
		}

		/// <summary>
		///   <para>Returns true if this Sprite is packed in an atlas.</para>
		/// </summary>
		public bool packed
		{
			get
			{
				return this.GetPacked() == 1;
			}
		}

		/// <summary>
		///   <para>If Sprite is packed (see Sprite.packed), returns its SpritePackingMode.</para>
		/// </summary>
		public SpritePackingMode packingMode
		{
			get
			{
				return (SpritePackingMode)this.GetPackingMode();
			}
		}

		/// <summary>
		///   <para>If Sprite is packed (see Sprite.packed), returns its SpritePackingRotation.</para>
		/// </summary>
		public SpritePackingRotation packingRotation
		{
			get
			{
				return (SpritePackingRotation)this.GetPackingRotation();
			}
		}

		/// <summary>
		///   <para>Get the rectangle this sprite uses on its texture. Raises an exception if this sprite is tightly packed in an atlas.</para>
		/// </summary>
		public Rect textureRect
		{
			get
			{
				Rect rect;
				if (this.packed && this.packingMode != SpritePackingMode.Rectangle)
				{
					rect = Rect.zero;
				}
				else
				{
					rect = this.GetTextureRect();
				}
				return rect;
			}
		}

		/// <summary>
		///   <para>Gets the offset of the rectangle this sprite uses on its texture to the original sprite bounds. If sprite mesh type is FullRect, offset is zero.</para>
		/// </summary>
		public Vector2 textureRectOffset
		{
			get
			{
				Vector2 vector;
				if (this.packed && this.packingMode != SpritePackingMode.Rectangle)
				{
					vector = Vector2.zero;
				}
				else
				{
					vector = this.GetTextureRectOffset();
				}
				return vector;
			}
		}

		/// <summary>
		///   <para>Returns a copy of the array containing sprite mesh vertex positions.</para>
		/// </summary>
		public extern Vector2[] vertices
		{
			[FreeFunction("SpriteAccessLegacy::GetSpriteVertices", HasExplicitThis = true)]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		/// <summary>
		///   <para>Returns a copy of the array containing sprite mesh triangles.</para>
		/// </summary>
		public extern ushort[] triangles
		{
			[FreeFunction("SpriteAccessLegacy::GetSpriteIndices", HasExplicitThis = true)]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		/// <summary>
		///   <para>The base texture coordinates of the sprite mesh.</para>
		/// </summary>
		public extern Vector2[] uv
		{
			[FreeFunction("SpriteAccessLegacy::GetSpriteUVs", HasExplicitThis = true)]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		/// <summary>
		///   <para>The number of physics shapes for the Sprite.</para>
		/// </summary>
		/// <returns>
		///   <para>The number of physics shapes for the Sprite.</para>
		/// </returns>
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern int GetPhysicsShapeCount();

		/// <summary>
		///   <para>The number of points in the selected physics shape for the Sprite.</para>
		/// </summary>
		/// <param name="shapeIdx">The index of the physics shape to retrieve the number of points from.</param>
		/// <returns>
		///   <para>The number of points in the selected physics shape for the Sprite.</para>
		/// </returns>
		public int GetPhysicsShapePointCount(int shapeIdx)
		{
			int physicsShapeCount = this.GetPhysicsShapeCount();
			if (shapeIdx < 0 || shapeIdx >= physicsShapeCount)
			{
				throw new IndexOutOfRangeException(string.Format("Index({0}) is out of bounds(0 - {1})", shapeIdx, physicsShapeCount - 1));
			}
			return this.Internal_GetPhysicsShapePointCount(shapeIdx);
		}

		[NativeMethod("GetPhysicsShapePointCount")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern int Internal_GetPhysicsShapePointCount(int shapeIdx);

		public int GetPhysicsShape(int shapeIdx, List<Vector2> physicsShape)
		{
			int physicsShapeCount = this.GetPhysicsShapeCount();
			if (shapeIdx < 0 || shapeIdx >= physicsShapeCount)
			{
				throw new IndexOutOfRangeException(string.Format("Index({0}) is out of bounds(0 - {1})", shapeIdx, physicsShapeCount - 1));
			}
			Sprite.GetPhysicsShapeImpl(this, shapeIdx, physicsShape);
			return physicsShape.Count;
		}

		[FreeFunction("SpritesBindings::GetPhysicsShape", ThrowsException = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetPhysicsShapeImpl(Sprite sprite, int shapeIdx, List<Vector2> physicsShape);

		public void OverridePhysicsShape(IList<Vector2[]> physicsShapes)
		{
			for (int i = 0; i < physicsShapes.Count; i++)
			{
				Vector2[] array = physicsShapes[i];
				if (array == null)
				{
					throw new ArgumentNullException(string.Format("Physics Shape at {0} is null.", i));
				}
				if (array.Length < 3)
				{
					throw new ArgumentException(string.Format("Physics Shape at {0} has less than 3 vertices ({1}).", i, array.Length));
				}
			}
			Sprite.OverridePhysicsShapeCount(this, physicsShapes.Count);
			for (int j = 0; j < physicsShapes.Count; j++)
			{
				Sprite.OverridePhysicsShape(this, physicsShapes[j], j);
			}
		}

		[FreeFunction("SpritesBindings::OverridePhysicsShapeCount")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void OverridePhysicsShapeCount(Sprite sprite, int physicsShapeCount);

		[FreeFunction("SpritesBindings::OverridePhysicsShape", ThrowsException = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void OverridePhysicsShape(Sprite sprite, Vector2[] physicsShape, int idx);

		/// <summary>
		///   <para>Sets up new Sprite geometry.</para>
		/// </summary>
		/// <param name="vertices">Array of vertex positions in Sprite Rect space.</param>
		/// <param name="triangles">Array of sprite mesh triangle indices.</param>
		[FreeFunction("SpritesBindings::OverrideGeometry", HasExplicitThis = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void OverrideGeometry(Vector2[] vertices, ushort[] triangles);

		internal static Sprite Create(Rect rect, Vector2 pivot, float pixelsToUnits, Texture2D texture)
		{
			return Sprite.CreateSpriteWithoutTextureScripting(rect, pivot, pixelsToUnits, texture);
		}

		internal static Sprite Create(Rect rect, Vector2 pivot, float pixelsToUnits)
		{
			return Sprite.CreateSpriteWithoutTextureScripting(rect, pivot, pixelsToUnits, null);
		}

		/// <summary>
		///   <para>Create a new Sprite object.</para>
		/// </summary>
		/// <param name="texture">Texture from which to obtain the sprite graphic.</param>
		/// <param name="rect">Rectangular section of the texture to use for the sprite.</param>
		/// <param name="pivot">Sprite's pivot point relative to its graphic rectangle.</param>
		/// <param name="pixelsPerUnit">The number of pixels in the sprite that correspond to one unit in world space.</param>
		/// <param name="extrude">Amount by which the sprite mesh should be expanded outwards.</param>
		/// <param name="meshType">Controls the type of mesh generated for the sprite.</param>
		/// <param name="border">The border sizes of the sprite (X=left, Y=bottom, Z=right, W=top).</param>
		/// <param name="generateFallbackPhysicsShape">Generates a default physics shape for the sprite.</param>
		public static Sprite Create(Texture2D texture, Rect rect, Vector2 pivot, float pixelsPerUnit, uint extrude, SpriteMeshType meshType, Vector4 border, bool generateFallbackPhysicsShape)
		{
			Sprite sprite;
			if (texture == null)
			{
				sprite = null;
			}
			else
			{
				if (rect.xMax > (float)texture.width || rect.yMax > (float)texture.height)
				{
					throw new ArgumentException(string.Format("Could not create sprite ({0}, {1}, {2}, {3}) from a {4}x{5} texture.", new object[] { rect.x, rect.y, rect.width, rect.height, texture.width, texture.height }));
				}
				if (pixelsPerUnit <= 0f)
				{
					throw new ArgumentException("pixelsPerUnit must be set to a positive non-zero value.");
				}
				sprite = Sprite.CreateSprite(texture, rect, pivot, pixelsPerUnit, extrude, meshType, border, generateFallbackPhysicsShape);
			}
			return sprite;
		}

		/// <summary>
		///   <para>Create a new Sprite object.</para>
		/// </summary>
		/// <param name="texture">Texture from which to obtain the sprite graphic.</param>
		/// <param name="rect">Rectangular section of the texture to use for the sprite.</param>
		/// <param name="pivot">Sprite's pivot point relative to its graphic rectangle.</param>
		/// <param name="pixelsPerUnit">The number of pixels in the sprite that correspond to one unit in world space.</param>
		/// <param name="extrude">Amount by which the sprite mesh should be expanded outwards.</param>
		/// <param name="meshType">Controls the type of mesh generated for the sprite.</param>
		/// <param name="border">The border sizes of the sprite (X=left, Y=bottom, Z=right, W=top).</param>
		/// <param name="generateFallbackPhysicsShape">Generates a default physics shape for the sprite.</param>
		public static Sprite Create(Texture2D texture, Rect rect, Vector2 pivot, float pixelsPerUnit, uint extrude, SpriteMeshType meshType, Vector4 border)
		{
			return Sprite.Create(texture, rect, pivot, pixelsPerUnit, extrude, meshType, border, false);
		}

		/// <summary>
		///   <para>Create a new Sprite object.</para>
		/// </summary>
		/// <param name="texture">Texture from which to obtain the sprite graphic.</param>
		/// <param name="rect">Rectangular section of the texture to use for the sprite.</param>
		/// <param name="pivot">Sprite's pivot point relative to its graphic rectangle.</param>
		/// <param name="pixelsPerUnit">The number of pixels in the sprite that correspond to one unit in world space.</param>
		/// <param name="extrude">Amount by which the sprite mesh should be expanded outwards.</param>
		/// <param name="meshType">Controls the type of mesh generated for the sprite.</param>
		/// <param name="border">The border sizes of the sprite (X=left, Y=bottom, Z=right, W=top).</param>
		/// <param name="generateFallbackPhysicsShape">Generates a default physics shape for the sprite.</param>
		public static Sprite Create(Texture2D texture, Rect rect, Vector2 pivot, float pixelsPerUnit, uint extrude, SpriteMeshType meshType)
		{
			return Sprite.Create(texture, rect, pivot, pixelsPerUnit, extrude, meshType, Vector4.zero);
		}

		/// <summary>
		///   <para>Create a new Sprite object.</para>
		/// </summary>
		/// <param name="texture">Texture from which to obtain the sprite graphic.</param>
		/// <param name="rect">Rectangular section of the texture to use for the sprite.</param>
		/// <param name="pivot">Sprite's pivot point relative to its graphic rectangle.</param>
		/// <param name="pixelsPerUnit">The number of pixels in the sprite that correspond to one unit in world space.</param>
		/// <param name="extrude">Amount by which the sprite mesh should be expanded outwards.</param>
		/// <param name="meshType">Controls the type of mesh generated for the sprite.</param>
		/// <param name="border">The border sizes of the sprite (X=left, Y=bottom, Z=right, W=top).</param>
		/// <param name="generateFallbackPhysicsShape">Generates a default physics shape for the sprite.</param>
		public static Sprite Create(Texture2D texture, Rect rect, Vector2 pivot, float pixelsPerUnit, uint extrude)
		{
			return Sprite.Create(texture, rect, pivot, pixelsPerUnit, extrude, SpriteMeshType.Tight);
		}

		/// <summary>
		///   <para>Create a new Sprite object.</para>
		/// </summary>
		/// <param name="texture">Texture from which to obtain the sprite graphic.</param>
		/// <param name="rect">Rectangular section of the texture to use for the sprite.</param>
		/// <param name="pivot">Sprite's pivot point relative to its graphic rectangle.</param>
		/// <param name="pixelsPerUnit">The number of pixels in the sprite that correspond to one unit in world space.</param>
		/// <param name="extrude">Amount by which the sprite mesh should be expanded outwards.</param>
		/// <param name="meshType">Controls the type of mesh generated for the sprite.</param>
		/// <param name="border">The border sizes of the sprite (X=left, Y=bottom, Z=right, W=top).</param>
		/// <param name="generateFallbackPhysicsShape">Generates a default physics shape for the sprite.</param>
		public static Sprite Create(Texture2D texture, Rect rect, Vector2 pivot, float pixelsPerUnit)
		{
			return Sprite.Create(texture, rect, pivot, pixelsPerUnit, 0U);
		}

		/// <summary>
		///   <para>Create a new Sprite object.</para>
		/// </summary>
		/// <param name="texture">Texture from which to obtain the sprite graphic.</param>
		/// <param name="rect">Rectangular section of the texture to use for the sprite.</param>
		/// <param name="pivot">Sprite's pivot point relative to its graphic rectangle.</param>
		/// <param name="pixelsPerUnit">The number of pixels in the sprite that correspond to one unit in world space.</param>
		/// <param name="extrude">Amount by which the sprite mesh should be expanded outwards.</param>
		/// <param name="meshType">Controls the type of mesh generated for the sprite.</param>
		/// <param name="border">The border sizes of the sprite (X=left, Y=bottom, Z=right, W=top).</param>
		/// <param name="generateFallbackPhysicsShape">Generates a default physics shape for the sprite.</param>
		public static Sprite Create(Texture2D texture, Rect rect, Vector2 pivot)
		{
			return Sprite.Create(texture, rect, pivot, 100f);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void GetTextureRect_Injected(out Rect ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void GetTextureRectOffset_Injected(out Vector2 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void GetInnerUVs_Injected(out Vector4 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void GetOuterUVs_Injected(out Vector4 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void GetPadding_Injected(out Vector4 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern Sprite CreateSpriteWithoutTextureScripting_Injected(ref Rect rect, ref Vector2 pivot, float pixelsToUnits, Texture2D texture);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern Sprite CreateSprite_Injected(Texture2D texture, ref Rect rect, ref Vector2 pivot, float pixelsPerUnit, uint extrude, SpriteMeshType meshType, ref Vector4 border, bool generateFallbackPhysicsShape);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void get_bounds_Injected(out Bounds ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void get_rect_Injected(out Rect ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void get_border_Injected(out Vector4 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void get_pivot_Injected(out Vector2 ret);
	}
}
