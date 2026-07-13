using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine
{
	[NativeHeader("Runtime/2D/Common/SpriteDataAccess.h")]
	[NativeHeader("Runtime/2D/Common/ScriptBindings/SpritesMarshalling.h")]
	[ExcludeFromPreset]
	[NativeType("Runtime/Graphics/SpriteFrame.h")]
	[NativeHeader("Runtime/Graphics/SpriteUtility.h")]
	public sealed class Sprite : Object
	{
		[RequiredByNativeCode]
		private Sprite()
		{
		}

		internal int GetPackingMode()
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Sprite>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return Sprite.GetPackingMode_Injected(intPtr);
		}

		internal int GetPackingRotation()
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Sprite>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return Sprite.GetPackingRotation_Injected(intPtr);
		}

		internal int GetPacked()
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Sprite>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return Sprite.GetPacked_Injected(intPtr);
		}

		internal Rect GetTextureRect()
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Sprite>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Rect rect;
			Sprite.GetTextureRect_Injected(intPtr, out rect);
			return rect;
		}

		internal Vector2 GetTextureRectOffset()
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Sprite>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Vector2 vector;
			Sprite.GetTextureRectOffset_Injected(intPtr, out vector);
			return vector;
		}

		internal Vector4 GetInnerUVs()
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Sprite>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Vector4 vector;
			Sprite.GetInnerUVs_Injected(intPtr, out vector);
			return vector;
		}

		internal Vector4 GetOuterUVs()
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Sprite>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Vector4 vector;
			Sprite.GetOuterUVs_Injected(intPtr, out vector);
			return vector;
		}

		internal Vector4 GetPadding()
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Sprite>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Vector4 vector;
			Sprite.GetPadding_Injected(intPtr, out vector);
			return vector;
		}

		[FreeFunction("SpritesBindings::CreateSpriteWithoutTextureScripting")]
		internal static Sprite CreateSpriteWithoutTextureScripting(Rect rect, Vector2 pivot, float pixelsToUnits, Texture2D texture)
		{
			return Unmarshal.UnmarshalUnityObject<Sprite>(Sprite.CreateSpriteWithoutTextureScripting_Injected(ref rect, ref pivot, pixelsToUnits, Object.MarshalledUnityObject.Marshal<Texture2D>(texture)));
		}

		[FreeFunction("SpritesBindings::CreateSprite", ThrowsException = true)]
		internal static Sprite CreateSprite(Texture2D texture, Rect rect, Vector2 pivot, float pixelsPerUnit, uint extrude, SpriteMeshType meshType, Vector4 border, bool generateFallbackPhysicsShape, [UnityMarshalAs(NativeType.ScriptingObjectPtr)] SecondarySpriteTexture[] secondaryTexture)
		{
			return Unmarshal.UnmarshalUnityObject<Sprite>(Sprite.CreateSprite_Injected(Object.MarshalledUnityObject.Marshal<Texture2D>(texture), ref rect, ref pivot, pixelsPerUnit, extrude, meshType, ref border, generateFallbackPhysicsShape, secondaryTexture));
		}

		public Bounds bounds
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Sprite>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Bounds bounds;
				Sprite.get_bounds_Injected(intPtr, out bounds);
				return bounds;
			}
		}

		public Rect rect
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Sprite>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Rect rect;
				Sprite.get_rect_Injected(intPtr, out rect);
				return rect;
			}
		}

		public Vector4 border
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Sprite>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Vector4 vector;
				Sprite.get_border_Injected(intPtr, out vector);
				return vector;
			}
		}

		public Texture2D texture
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Sprite>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Unmarshal.UnmarshalUnityObject<Texture2D>(Sprite.get_texture_Injected(intPtr));
			}
		}

		internal uint extrude
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Sprite>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Sprite.get_extrude_Injected(intPtr);
			}
		}

		internal Texture2D GetSecondaryTexture(int index)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Sprite>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return Unmarshal.UnmarshalUnityObject<Texture2D>(Sprite.GetSecondaryTexture_Injected(intPtr, index));
		}

		public int GetSecondaryTextureCount()
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Sprite>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return Sprite.GetSecondaryTextureCount_Injected(intPtr);
		}

		[FreeFunction("SpritesBindings::GetSecondaryTextures", ThrowsException = true, HasExplicitThis = true)]
		public int GetSecondaryTextures([UnityMarshalAs(NativeType.ScriptingObjectPtr)] [NotNull] SecondarySpriteTexture[] secondaryTexture)
		{
			if (secondaryTexture == null)
			{
				ThrowHelper.ThrowArgumentNullException(secondaryTexture, "secondaryTexture");
			}
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Sprite>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return Sprite.GetSecondaryTextures_Injected(intPtr, secondaryTexture);
		}

		public float pixelsPerUnit
		{
			[NativeMethod("GetPixelsToUnits")]
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Sprite>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Sprite.get_pixelsPerUnit_Injected(intPtr);
			}
		}

		public float spriteAtlasTextureScale
		{
			[NativeMethod("GetSpriteAtlasTextureScale")]
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Sprite>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Sprite.get_spriteAtlasTextureScale_Injected(intPtr);
			}
		}

		public Texture2D associatedAlphaSplitTexture
		{
			[NativeMethod("GetAlphaTexture")]
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Sprite>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Unmarshal.UnmarshalUnityObject<Texture2D>(Sprite.get_associatedAlphaSplitTexture_Injected(intPtr));
			}
		}

		public Vector2 pivot
		{
			[NativeMethod("GetPivotInPixels")]
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Sprite>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Vector2 vector;
				Sprite.get_pivot_Injected(intPtr, out vector);
				return vector;
			}
		}

		public bool packed
		{
			get
			{
				return this.GetPacked() == 1;
			}
		}

		public SpritePackingMode packingMode
		{
			get
			{
				return (SpritePackingMode)this.GetPackingMode();
			}
		}

		public SpritePackingRotation packingRotation
		{
			get
			{
				return (SpritePackingRotation)this.GetPackingRotation();
			}
		}

		public Rect textureRect
		{
			get
			{
				return this.GetTextureRect();
			}
		}

		public Vector2 textureRectOffset
		{
			get
			{
				return this.GetTextureRectOffset();
			}
		}

		public Vector2[] vertices
		{
			[FreeFunction("SpriteAccessLegacy::GetSpriteVertices", HasExplicitThis = true)]
			[return: UnityMarshalAs(NativeType.ScriptingObjectPtr)]
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Sprite>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Sprite.get_vertices_Injected(intPtr);
			}
		}

		public ushort[] triangles
		{
			[FreeFunction("SpriteAccessLegacy::GetSpriteIndices", HasExplicitThis = true)]
			[return: UnityMarshalAs(NativeType.ScriptingObjectPtr)]
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Sprite>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Sprite.get_triangles_Injected(intPtr);
			}
		}

		public Vector2[] uv
		{
			[FreeFunction("SpriteAccessLegacy::GetSpriteUVs", HasExplicitThis = true)]
			[return: UnityMarshalAs(NativeType.ScriptingObjectPtr)]
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Sprite>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Sprite.get_uv_Injected(intPtr);
			}
		}

		public int GetPhysicsShapeCount()
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Sprite>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return Sprite.GetPhysicsShapeCount_Injected(intPtr);
		}

		public uint GetScriptableObjectsCount()
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Sprite>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return Sprite.GetScriptableObjectsCount_Injected(intPtr);
		}

		[FreeFunction("SpritesBindings::GetScriptableObjects", ThrowsException = true, HasExplicitThis = true)]
		public uint GetScriptableObjects([NotNull] [UnityMarshalAs(NativeType.ScriptingObjectPtr)] ScriptableObject[] scriptableObjects)
		{
			if (scriptableObjects == null)
			{
				ThrowHelper.ThrowArgumentNullException(scriptableObjects, "scriptableObjects");
			}
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Sprite>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return Sprite.GetScriptableObjects_Injected(intPtr, scriptableObjects);
		}

		public bool AddScriptableObject([NotNull] ScriptableObject obj)
		{
			if (obj == null)
			{
				ThrowHelper.ThrowArgumentNullException(obj, "obj");
			}
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Sprite>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			IntPtr intPtr2 = Object.MarshalledUnityObject.MarshalNotNull<ScriptableObject>(obj);
			if (intPtr2 == 0)
			{
				ThrowHelper.ThrowArgumentNullException(obj, "obj");
			}
			return Sprite.AddScriptableObject_Injected(intPtr, intPtr2);
		}

		public bool RemoveScriptableObjectAt(uint i)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Sprite>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return Sprite.RemoveScriptableObjectAt_Injected(intPtr, i);
		}

		public bool SetScriptableObjectAt([NotNull] ScriptableObject obj, uint i)
		{
			if (obj == null)
			{
				ThrowHelper.ThrowArgumentNullException(obj, "obj");
			}
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Sprite>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			IntPtr intPtr2 = Object.MarshalledUnityObject.MarshalNotNull<ScriptableObject>(obj);
			if (intPtr2 == 0)
			{
				ThrowHelper.ThrowArgumentNullException(obj, "obj");
			}
			return Sprite.SetScriptableObjectAt_Injected(intPtr, intPtr2, i);
		}

		public int GetPhysicsShapePointCount(int shapeIdx)
		{
			int physicsShapeCount = this.GetPhysicsShapeCount();
			bool flag = shapeIdx < 0 || shapeIdx >= physicsShapeCount;
			if (flag)
			{
				throw new IndexOutOfRangeException(string.Format("Index({0}) is out of bounds(0 - {1})", shapeIdx, physicsShapeCount - 1));
			}
			return this.Internal_GetPhysicsShapePointCount(shapeIdx);
		}

		[NativeMethod("GetPhysicsShapePointCount")]
		private int Internal_GetPhysicsShapePointCount(int shapeIdx)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Sprite>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return Sprite.Internal_GetPhysicsShapePointCount_Injected(intPtr, shapeIdx);
		}

		public int GetPhysicsShape(int shapeIdx, List<Vector2> physicsShape)
		{
			int physicsShapeCount = this.GetPhysicsShapeCount();
			bool flag = shapeIdx < 0 || shapeIdx >= physicsShapeCount;
			if (flag)
			{
				throw new IndexOutOfRangeException(string.Format("Index({0}) is out of bounds(0 - {1})", shapeIdx, physicsShapeCount - 1));
			}
			Sprite.GetPhysicsShapeImpl(this, shapeIdx, physicsShape);
			return physicsShape.Count;
		}

		public ReadOnlySpan<Vector2> GetPhysicsShape(int shapeIdx)
		{
			int physicsShapeCount = this.GetPhysicsShapeCount();
			bool flag = shapeIdx < 0 || shapeIdx >= physicsShapeCount;
			if (flag)
			{
				throw new IndexOutOfRangeException(string.Format("Index({0}) is out of bounds(0 - {1})", shapeIdx, physicsShapeCount - 1));
			}
			return Sprite.GetPhysicsShapeSpanImpl(this, shapeIdx);
		}

		[FreeFunction("SpritesBindings::GetPhysicsShape", ThrowsException = true)]
		private unsafe static void GetPhysicsShapeImpl(Sprite sprite, int shapeIdx, [NotNull] List<Vector2> physicsShape)
		{
			if (physicsShape == null)
			{
				ThrowHelper.ThrowArgumentNullException(physicsShape, "physicsShape");
			}
			try
			{
				IntPtr intPtr = Object.MarshalledUnityObject.Marshal<Sprite>(sprite);
				fixed (Vector2[] array = NoAllocHelpers.ExtractArrayFromList<Vector2>(physicsShape))
				{
					BlittableArrayWrapper blittableArrayWrapper;
					if (array.Length != 0)
					{
						blittableArrayWrapper = new BlittableArrayWrapper((void*)(&array[0]), array.Length);
					}
					BlittableListWrapper blittableListWrapper = new BlittableListWrapper(blittableArrayWrapper, physicsShape.Count);
					Sprite.GetPhysicsShapeImpl_Injected(intPtr, shapeIdx, ref blittableListWrapper);
				}
			}
			finally
			{
				BlittableListWrapper blittableListWrapper;
				blittableListWrapper.Unmarshal<Vector2>(physicsShape);
			}
		}

		[FreeFunction("SpritesBindings::GetPhysicsShape", ThrowsException = true)]
		private static ReadOnlySpan<Vector2> GetPhysicsShapeSpanImpl(Sprite sprite, int shapeIdx)
		{
			ManagedSpanWrapper managedSpanWrapper;
			Sprite.GetPhysicsShapeSpanImpl_Injected(Object.MarshalledUnityObject.Marshal<Sprite>(sprite), shapeIdx, out managedSpanWrapper);
			return ManagedSpanWrapper.ToReadOnlySpan<Vector2>(managedSpanWrapper);
		}

		public void OverridePhysicsShape(IList<Vector2[]> physicsShapes)
		{
			bool flag = physicsShapes == null;
			if (flag)
			{
				throw new ArgumentNullException("physicsShapes");
			}
			for (int i = 0; i < physicsShapes.Count; i++)
			{
				Vector2[] array = physicsShapes[i];
				bool flag2 = array == null;
				if (flag2)
				{
					throw new ArgumentNullException("physicsShape", string.Format("Physics Shape at {0} is null.", i));
				}
				bool flag3 = array.Length < 3;
				if (flag3)
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
		private static void OverridePhysicsShapeCount(Sprite sprite, int physicsShapeCount)
		{
			Sprite.OverridePhysicsShapeCount_Injected(Object.MarshalledUnityObject.Marshal<Sprite>(sprite), physicsShapeCount);
		}

		[FreeFunction("SpritesBindings::OverridePhysicsShape", ThrowsException = true)]
		private unsafe static void OverridePhysicsShape(Sprite sprite, [NotNull] Vector2[] physicsShape, int idx)
		{
			if (physicsShape == null)
			{
				ThrowHelper.ThrowArgumentNullException(physicsShape, "physicsShape");
			}
			IntPtr intPtr = Object.MarshalledUnityObject.Marshal<Sprite>(sprite);
			Span<Vector2> span = new Span<Vector2>(physicsShape);
			fixed (Vector2* pinnableReference = span.GetPinnableReference())
			{
				ManagedSpanWrapper managedSpanWrapper = new ManagedSpanWrapper((void*)pinnableReference, span.Length);
				Sprite.OverridePhysicsShape_Injected(intPtr, ref managedSpanWrapper, idx);
			}
		}

		[FreeFunction("SpritesBindings::OverrideGeometry", HasExplicitThis = true)]
		public unsafe void OverrideGeometry([NotNull] Vector2[] vertices, [NotNull] ushort[] triangles)
		{
			if (vertices == null)
			{
				ThrowHelper.ThrowArgumentNullException(vertices, "vertices");
			}
			if (triangles == null)
			{
				ThrowHelper.ThrowArgumentNullException(triangles, "triangles");
			}
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Sprite>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Span<Vector2> span = new Span<Vector2>(vertices);
			fixed (Vector2* ptr = span.GetPinnableReference())
			{
				ManagedSpanWrapper managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, span.Length);
				Span<ushort> span2 = new Span<ushort>(triangles);
				fixed (ushort* pinnableReference = span2.GetPinnableReference())
				{
					ManagedSpanWrapper managedSpanWrapper2 = new ManagedSpanWrapper((void*)pinnableReference, span2.Length);
					Sprite.OverrideGeometry_Injected(intPtr, ref managedSpanWrapper, ref managedSpanWrapper2);
					ptr = null;
				}
			}
		}

		[VisibleToOtherModules]
		internal static Sprite Create(Rect rect, Vector2 pivot, float pixelsToUnits, Texture2D texture)
		{
			return Sprite.CreateSpriteWithoutTextureScripting(rect, pivot, pixelsToUnits, texture);
		}

		internal static Sprite Create(Rect rect, Vector2 pivot, float pixelsToUnits)
		{
			return Sprite.CreateSpriteWithoutTextureScripting(rect, pivot, pixelsToUnits, null);
		}

		public static Sprite Create(Texture2D texture, Rect rect, Vector2 pivot, float pixelsPerUnit, uint extrude, SpriteMeshType meshType, Vector4 border, bool generateFallbackPhysicsShape)
		{
			return Sprite.Create(texture, rect, pivot, pixelsPerUnit, extrude, meshType, border, generateFallbackPhysicsShape, null);
		}

		public static Sprite Create(Texture2D texture, Rect rect, Vector2 pivot, float pixelsPerUnit, uint extrude, SpriteMeshType meshType, Vector4 border, bool generateFallbackPhysicsShape, SecondarySpriteTexture[] secondaryTextures)
		{
			bool flag = texture == null;
			Sprite sprite;
			if (flag)
			{
				sprite = null;
			}
			else
			{
				bool flag2 = rect.xMax > (float)texture.width || rect.yMax > (float)texture.height;
				if (flag2)
				{
					throw new ArgumentException(string.Format("Could not create sprite ({0}, {1}, {2}, {3}) from a {4}x{5} texture.", new object[] { rect.x, rect.y, rect.width, rect.height, texture.width, texture.height }));
				}
				bool flag3 = pixelsPerUnit <= 0f;
				if (flag3)
				{
					throw new ArgumentException("pixelsPerUnit must be set to a positive non-zero value.");
				}
				bool flag4 = secondaryTextures != null;
				if (flag4)
				{
					foreach (SecondarySpriteTexture secondarySpriteTexture in secondaryTextures)
					{
						bool flag5 = secondarySpriteTexture.texture == texture;
						if (flag5)
						{
							throw new ArgumentException(string.Format("{0} is using source Texture as Secondary Texture.", secondarySpriteTexture.name));
						}
					}
				}
				sprite = Sprite.CreateSprite(texture, rect, pivot, pixelsPerUnit, extrude, meshType, border, generateFallbackPhysicsShape, secondaryTextures);
			}
			return sprite;
		}

		public static Sprite Create(Texture2D texture, Rect rect, Vector2 pivot, float pixelsPerUnit, uint extrude, SpriteMeshType meshType, Vector4 border)
		{
			return Sprite.Create(texture, rect, pivot, pixelsPerUnit, extrude, meshType, border, false);
		}

		public static Sprite Create(Texture2D texture, Rect rect, Vector2 pivot, float pixelsPerUnit, uint extrude, SpriteMeshType meshType)
		{
			return Sprite.Create(texture, rect, pivot, pixelsPerUnit, extrude, meshType, Vector4.zero);
		}

		public static Sprite Create(Texture2D texture, Rect rect, Vector2 pivot, float pixelsPerUnit, uint extrude)
		{
			return Sprite.Create(texture, rect, pivot, pixelsPerUnit, extrude, SpriteMeshType.Tight);
		}

		public static Sprite Create(Texture2D texture, Rect rect, Vector2 pivot, float pixelsPerUnit)
		{
			return Sprite.Create(texture, rect, pivot, pixelsPerUnit, 0U);
		}

		public static Sprite Create(Texture2D texture, Rect rect, Vector2 pivot)
		{
			return Sprite.Create(texture, rect, pivot, 100f);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int GetPackingMode_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int GetPackingRotation_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int GetPacked_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetTextureRect_Injected(IntPtr _unity_self, out Rect ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetTextureRectOffset_Injected(IntPtr _unity_self, out Vector2 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetInnerUVs_Injected(IntPtr _unity_self, out Vector4 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetOuterUVs_Injected(IntPtr _unity_self, out Vector4 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetPadding_Injected(IntPtr _unity_self, out Vector4 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr CreateSpriteWithoutTextureScripting_Injected([In] ref Rect rect, [In] ref Vector2 pivot, float pixelsToUnits, IntPtr texture);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr CreateSprite_Injected(IntPtr texture, [In] ref Rect rect, [In] ref Vector2 pivot, float pixelsPerUnit, uint extrude, SpriteMeshType meshType, [In] ref Vector4 border, bool generateFallbackPhysicsShape, SecondarySpriteTexture[] secondaryTexture);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_bounds_Injected(IntPtr _unity_self, out Bounds ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_rect_Injected(IntPtr _unity_self, out Rect ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_border_Injected(IntPtr _unity_self, out Vector4 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr get_texture_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern uint get_extrude_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr GetSecondaryTexture_Injected(IntPtr _unity_self, int index);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int GetSecondaryTextureCount_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int GetSecondaryTextures_Injected(IntPtr _unity_self, SecondarySpriteTexture[] secondaryTexture);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float get_pixelsPerUnit_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float get_spriteAtlasTextureScale_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr get_associatedAlphaSplitTexture_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_pivot_Injected(IntPtr _unity_self, out Vector2 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern Vector2[] get_vertices_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern ushort[] get_triangles_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern Vector2[] get_uv_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int GetPhysicsShapeCount_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern uint GetScriptableObjectsCount_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern uint GetScriptableObjects_Injected(IntPtr _unity_self, ScriptableObject[] scriptableObjects);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool AddScriptableObject_Injected(IntPtr _unity_self, IntPtr obj);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool RemoveScriptableObjectAt_Injected(IntPtr _unity_self, uint i);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool SetScriptableObjectAt_Injected(IntPtr _unity_self, IntPtr obj, uint i);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int Internal_GetPhysicsShapePointCount_Injected(IntPtr _unity_self, int shapeIdx);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetPhysicsShapeImpl_Injected(IntPtr sprite, int shapeIdx, ref BlittableListWrapper physicsShape);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetPhysicsShapeSpanImpl_Injected(IntPtr sprite, int shapeIdx, out ManagedSpanWrapper ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void OverridePhysicsShapeCount_Injected(IntPtr sprite, int physicsShapeCount);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void OverridePhysicsShape_Injected(IntPtr sprite, ref ManagedSpanWrapper physicsShape, int idx);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void OverrideGeometry_Injected(IntPtr _unity_self, ref ManagedSpanWrapper vertices, ref ManagedSpanWrapper triangles);
	}
}
