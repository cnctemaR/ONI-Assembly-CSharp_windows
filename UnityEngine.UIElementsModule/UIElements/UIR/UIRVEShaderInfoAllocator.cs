using System;
using Unity.Collections;

namespace UnityEngine.UIElements.UIR
{
	internal struct UIRVEShaderInfoAllocator
	{
		private static int pageWidth
		{
			get
			{
				return 32;
			}
		}

		private static int pageHeight
		{
			get
			{
				return 8;
			}
		}

		private static Vector2Int AllocToTexelCoord(ref BitmapAllocator32 allocator, BMPAlloc alloc)
		{
			ushort num;
			ushort num2;
			allocator.GetAllocPageAtlasLocation(alloc.page, out num, out num2);
			return new Vector2Int((int)alloc.bitIndex * allocator.entryWidth + (int)num, (int)alloc.pageLine * allocator.entryHeight + (int)num2);
		}

		private static int AllocToConstantBufferIndex(BMPAlloc alloc)
		{
			return (int)alloc.pageLine * UIRVEShaderInfoAllocator.pageWidth + (int)alloc.bitIndex;
		}

		private static bool AtlasRectMatchesPage(ref BitmapAllocator32 allocator, BMPAlloc defAlloc, RectInt atlasRect)
		{
			ushort num;
			ushort num2;
			allocator.GetAllocPageAtlasLocation(defAlloc.page, out num, out num2);
			return (int)num == atlasRect.xMin && (int)num2 == atlasRect.yMin && allocator.entryWidth * UIRVEShaderInfoAllocator.pageWidth == atlasRect.width && allocator.entryHeight * UIRVEShaderInfoAllocator.pageHeight == atlasRect.height;
		}

		public NativeArray<Transform3x4> transformConstants
		{
			get
			{
				return this.m_Transforms;
			}
		}

		public NativeArray<Vector4> clipRectConstants
		{
			get
			{
				return this.m_ClipRects;
			}
		}

		public Texture atlas
		{
			get
			{
				bool atlasReallyCreated = this.m_AtlasReallyCreated;
				Texture texture;
				if (atlasReallyCreated)
				{
					texture = this.m_Atlas.atlas;
				}
				else
				{
					texture = (this.m_VertexTexturingEnabled ? UIRenderDevice.defaultShaderInfoTexFloat : UIRenderDevice.defaultShaderInfoTexARGB8);
				}
				return texture;
			}
		}

		public bool internalAtlasCreated
		{
			get
			{
				return this.m_AtlasReallyCreated;
			}
		}

		public bool isReleased
		{
			get
			{
				bool flag;
				if (this.m_AtlasReallyCreated)
				{
					UIRAtlasManager atlas = this.m_Atlas;
					flag = atlas != null && atlas.IsReleased();
				}
				else
				{
					flag = false;
				}
				return flag;
			}
		}

		public void Construct()
		{
			this.m_OpacityAllocator = (this.m_ClipRectAllocator = (this.m_TransformAllocator = default(BitmapAllocator32)));
			this.m_TransformAllocator.Construct(UIRVEShaderInfoAllocator.pageHeight, 1, 3);
			this.m_TransformAllocator.ForceFirstAlloc((ushort)UIRVEShaderInfoAllocator.identityTransformTexel.x, (ushort)UIRVEShaderInfoAllocator.identityTransformTexel.y);
			this.m_ClipRectAllocator.Construct(UIRVEShaderInfoAllocator.pageHeight, 1, 1);
			this.m_ClipRectAllocator.ForceFirstAlloc((ushort)UIRVEShaderInfoAllocator.infiniteClipRectTexel.x, (ushort)UIRVEShaderInfoAllocator.infiniteClipRectTexel.y);
			this.m_OpacityAllocator.Construct(UIRVEShaderInfoAllocator.pageHeight, 1, 1);
			this.m_OpacityAllocator.ForceFirstAlloc((ushort)UIRVEShaderInfoAllocator.fullOpacityTexel.x, (ushort)UIRVEShaderInfoAllocator.fullOpacityTexel.y);
			this.m_VertexTexturingEnabled = UIRenderDevice.vertexTexturingIsAvailable;
			bool flag = !this.m_VertexTexturingEnabled;
			if (flag)
			{
				int num = 20;
				this.m_Transforms = new NativeArray<Transform3x4>(num, Allocator.Persistent, NativeArrayOptions.UninitializedMemory);
				this.m_ClipRects = new NativeArray<Vector4>(num, Allocator.Persistent, NativeArrayOptions.UninitializedMemory);
				this.m_Transforms[0] = new Transform3x4
				{
					v0 = UIRVEShaderInfoAllocator.identityTransformRow0Value,
					v1 = UIRVEShaderInfoAllocator.identityTransformRow1Value,
					v2 = UIRVEShaderInfoAllocator.identityTransformRow2Value
				};
				this.m_ClipRects[0] = UIRVEShaderInfoAllocator.infiniteClipRectValue;
			}
		}

		private void ReallyCreateAtlas()
		{
			this.m_Atlas = new UIRAtlasManager(this.m_VertexTexturingEnabled ? RenderTextureFormat.ARGBFloat : RenderTextureFormat.ARGB32, FilterMode.Point, Math.Max(UIRVEShaderInfoAllocator.pageWidth, UIRVEShaderInfoAllocator.pageHeight * 3), 64);
			RectInt rectInt;
			this.m_Atlas.AllocateRect(UIRVEShaderInfoAllocator.pageWidth * this.m_TransformAllocator.entryWidth, UIRVEShaderInfoAllocator.pageHeight * this.m_TransformAllocator.entryHeight, out rectInt);
			RectInt rectInt2;
			this.m_Atlas.AllocateRect(UIRVEShaderInfoAllocator.pageWidth * this.m_ClipRectAllocator.entryWidth, UIRVEShaderInfoAllocator.pageHeight * this.m_ClipRectAllocator.entryHeight, out rectInt2);
			RectInt rectInt3;
			this.m_Atlas.AllocateRect(UIRVEShaderInfoAllocator.pageWidth * this.m_OpacityAllocator.entryWidth, UIRVEShaderInfoAllocator.pageHeight * this.m_OpacityAllocator.entryHeight, out rectInt3);
			bool flag = !UIRVEShaderInfoAllocator.AtlasRectMatchesPage(ref this.m_TransformAllocator, UIRVEShaderInfoAllocator.identityTransform, rectInt);
			if (flag)
			{
				throw new Exception("Atlas identity transform allocation failed unexpectedly");
			}
			bool flag2 = !UIRVEShaderInfoAllocator.AtlasRectMatchesPage(ref this.m_ClipRectAllocator, UIRVEShaderInfoAllocator.infiniteClipRect, rectInt2);
			if (flag2)
			{
				throw new Exception("Atlas infinite clip rect allocation failed unexpectedly");
			}
			bool flag3 = !UIRVEShaderInfoAllocator.AtlasRectMatchesPage(ref this.m_OpacityAllocator, UIRVEShaderInfoAllocator.fullOpacity, rectInt3);
			if (flag3)
			{
				throw new Exception("Atlas full opacity allocation failed unexpectedly");
			}
			Texture2D whiteTexel = UIRenderDevice.whiteTexel;
			bool vertexTexturingEnabled = this.m_VertexTexturingEnabled;
			if (vertexTexturingEnabled)
			{
				Vector2Int vector2Int = UIRVEShaderInfoAllocator.AllocToTexelCoord(ref this.m_TransformAllocator, UIRVEShaderInfoAllocator.identityTransform);
				this.m_Atlas.EnqueueBlit(whiteTexel, vector2Int.x, vector2Int.y, false, UIRVEShaderInfoAllocator.identityTransformRow0Value);
				this.m_Atlas.EnqueueBlit(whiteTexel, vector2Int.x, vector2Int.y + 1, false, UIRVEShaderInfoAllocator.identityTransformRow1Value);
				this.m_Atlas.EnqueueBlit(whiteTexel, vector2Int.x, vector2Int.y + 2, false, UIRVEShaderInfoAllocator.identityTransformRow2Value);
				vector2Int = UIRVEShaderInfoAllocator.AllocToTexelCoord(ref this.m_ClipRectAllocator, UIRVEShaderInfoAllocator.infiniteClipRect);
				this.m_Atlas.EnqueueBlit(whiteTexel, vector2Int.x, vector2Int.y, false, UIRVEShaderInfoAllocator.infiniteClipRectValue);
			}
			Vector2Int vector2Int2 = UIRVEShaderInfoAllocator.AllocToTexelCoord(ref this.m_OpacityAllocator, UIRVEShaderInfoAllocator.fullOpacity);
			this.m_Atlas.EnqueueBlit(whiteTexel, vector2Int2.x, vector2Int2.y, false, UIRVEShaderInfoAllocator.fullOpacityValue);
			this.m_AtlasReallyCreated = true;
		}

		public void Dispose()
		{
			bool flag = this.m_Atlas != null;
			if (flag)
			{
				this.m_Atlas.Dispose();
			}
			this.m_Atlas = null;
			bool isCreated = this.m_ClipRects.IsCreated;
			if (isCreated)
			{
				this.m_ClipRects.Dispose();
			}
			bool isCreated2 = this.m_Transforms.IsCreated;
			if (isCreated2)
			{
				this.m_Transforms.Dispose();
			}
			this.m_AtlasReallyCreated = false;
		}

		public void IssuePendingAtlasBlits()
		{
			UIRAtlasManager atlas = this.m_Atlas;
			if (atlas != null)
			{
				atlas.Commit();
			}
		}

		public BMPAlloc AllocTransform()
		{
			bool flag = !this.m_AtlasReallyCreated;
			if (flag)
			{
				this.ReallyCreateAtlas();
			}
			bool vertexTexturingEnabled = this.m_VertexTexturingEnabled;
			BMPAlloc bmpalloc;
			if (vertexTexturingEnabled)
			{
				bmpalloc = this.m_TransformAllocator.Allocate(this.m_Atlas);
			}
			else
			{
				BMPAlloc bmpalloc2 = this.m_TransformAllocator.Allocate(null);
				bool flag2 = UIRVEShaderInfoAllocator.AllocToConstantBufferIndex(bmpalloc2) < this.m_Transforms.Length;
				if (flag2)
				{
					bmpalloc = bmpalloc2;
				}
				else
				{
					this.m_TransformAllocator.Free(bmpalloc2);
					bmpalloc = BMPAlloc.Invalid;
				}
			}
			return bmpalloc;
		}

		public BMPAlloc AllocClipRect()
		{
			bool flag = !this.m_AtlasReallyCreated;
			if (flag)
			{
				this.ReallyCreateAtlas();
			}
			bool vertexTexturingEnabled = this.m_VertexTexturingEnabled;
			BMPAlloc bmpalloc;
			if (vertexTexturingEnabled)
			{
				bmpalloc = this.m_ClipRectAllocator.Allocate(this.m_Atlas);
			}
			else
			{
				BMPAlloc bmpalloc2 = this.m_ClipRectAllocator.Allocate(null);
				bool flag2 = UIRVEShaderInfoAllocator.AllocToConstantBufferIndex(bmpalloc2) < this.m_ClipRects.Length;
				if (flag2)
				{
					bmpalloc = bmpalloc2;
				}
				else
				{
					this.m_ClipRectAllocator.Free(bmpalloc2);
					bmpalloc = BMPAlloc.Invalid;
				}
			}
			return bmpalloc;
		}

		public BMPAlloc AllocOpacity()
		{
			bool flag = !this.m_AtlasReallyCreated;
			if (flag)
			{
				this.ReallyCreateAtlas();
			}
			return this.m_OpacityAllocator.Allocate(this.m_Atlas);
		}

		public void SetTransformValue(BMPAlloc alloc, Matrix4x4 xform)
		{
			Debug.Assert(alloc.IsValid());
			bool vertexTexturingEnabled = this.m_VertexTexturingEnabled;
			if (vertexTexturingEnabled)
			{
				Vector2Int vector2Int = UIRVEShaderInfoAllocator.AllocToTexelCoord(ref this.m_TransformAllocator, alloc);
				this.m_Atlas.EnqueueBlit(UIRenderDevice.whiteTexel, vector2Int.x, vector2Int.y, false, xform.GetRow(0));
				this.m_Atlas.EnqueueBlit(UIRenderDevice.whiteTexel, vector2Int.x, vector2Int.y + 1, false, xform.GetRow(1));
				this.m_Atlas.EnqueueBlit(UIRenderDevice.whiteTexel, vector2Int.x, vector2Int.y + 2, false, xform.GetRow(2));
			}
			else
			{
				this.m_Transforms[UIRVEShaderInfoAllocator.AllocToConstantBufferIndex(alloc)] = new Transform3x4
				{
					v0 = xform.GetRow(0),
					v1 = xform.GetRow(1),
					v2 = xform.GetRow(2)
				};
			}
		}

		public void SetClipRectValue(BMPAlloc alloc, Vector4 clipRect)
		{
			Debug.Assert(alloc.IsValid());
			bool vertexTexturingEnabled = this.m_VertexTexturingEnabled;
			if (vertexTexturingEnabled)
			{
				Vector2Int vector2Int = UIRVEShaderInfoAllocator.AllocToTexelCoord(ref this.m_ClipRectAllocator, alloc);
				this.m_Atlas.EnqueueBlit(UIRenderDevice.whiteTexel, vector2Int.x, vector2Int.y, false, clipRect);
			}
			else
			{
				this.m_ClipRects[UIRVEShaderInfoAllocator.AllocToConstantBufferIndex(alloc)] = clipRect;
			}
		}

		public void SetOpacityValue(BMPAlloc alloc, float opacity)
		{
			Debug.Assert(alloc.IsValid());
			Vector2Int vector2Int = UIRVEShaderInfoAllocator.AllocToTexelCoord(ref this.m_OpacityAllocator, alloc);
			this.m_Atlas.EnqueueBlit(UIRenderDevice.whiteTexel, vector2Int.x, vector2Int.y, false, new Color(1f, 1f, 1f, opacity));
		}

		public void FreeTransform(BMPAlloc alloc)
		{
			Debug.Assert(alloc.IsValid());
			this.m_TransformAllocator.Free(alloc);
		}

		public void FreeClipRect(BMPAlloc alloc)
		{
			Debug.Assert(alloc.IsValid());
			this.m_ClipRectAllocator.Free(alloc);
		}

		public void FreeOpacity(BMPAlloc alloc)
		{
			Debug.Assert(alloc.IsValid());
			this.m_OpacityAllocator.Free(alloc);
		}

		public Color32 TransformAllocToVertexData(BMPAlloc alloc)
		{
			Debug.Assert(UIRVEShaderInfoAllocator.pageWidth == 32 && UIRVEShaderInfoAllocator.pageHeight == 8);
			ushort num = 0;
			ushort num2 = 0;
			bool vertexTexturingEnabled = this.m_VertexTexturingEnabled;
			if (vertexTexturingEnabled)
			{
				this.m_TransformAllocator.GetAllocPageAtlasLocation(alloc.page, out num, out num2);
			}
			return new Color32((byte)(num >> 5), (byte)(num2 >> 3), (byte)((int)alloc.pageLine * UIRVEShaderInfoAllocator.pageWidth + (int)alloc.bitIndex), 0);
		}

		public Color32 ClipRectAllocToVertexData(BMPAlloc alloc)
		{
			Debug.Assert(UIRVEShaderInfoAllocator.pageWidth == 32 && UIRVEShaderInfoAllocator.pageHeight == 8);
			ushort num = 0;
			ushort num2 = 0;
			bool vertexTexturingEnabled = this.m_VertexTexturingEnabled;
			if (vertexTexturingEnabled)
			{
				this.m_ClipRectAllocator.GetAllocPageAtlasLocation(alloc.page, out num, out num2);
			}
			return new Color32((byte)(num >> 5), (byte)(num2 >> 3), (byte)((int)alloc.pageLine * UIRVEShaderInfoAllocator.pageWidth + (int)alloc.bitIndex), 0);
		}

		public Color32 OpacityAllocToVertexData(BMPAlloc alloc)
		{
			Debug.Assert(UIRVEShaderInfoAllocator.pageWidth == 32 && UIRVEShaderInfoAllocator.pageHeight == 8);
			ushort num;
			ushort num2;
			this.m_OpacityAllocator.GetAllocPageAtlasLocation(alloc.page, out num, out num2);
			return new Color32((byte)(num >> 5), (byte)(num2 >> 3), (byte)((int)alloc.pageLine * UIRVEShaderInfoAllocator.pageWidth + (int)alloc.bitIndex), 0);
		}

		private UIRAtlasManager m_Atlas;

		private BitmapAllocator32 m_TransformAllocator;

		private BitmapAllocator32 m_ClipRectAllocator;

		private BitmapAllocator32 m_OpacityAllocator;

		private bool m_AtlasReallyCreated;

		private bool m_VertexTexturingEnabled;

		private NativeArray<Transform3x4> m_Transforms;

		private NativeArray<Vector4> m_ClipRects;

		internal static readonly Vector2Int identityTransformTexel = new Vector2Int(0, 0);

		internal static readonly Vector2Int infiniteClipRectTexel = new Vector2Int(0, 32);

		internal static readonly Vector2Int fullOpacityTexel = new Vector2Int(32, 32);

		internal static readonly Vector4 identityTransformRow0Value = new Vector4(1f, 0f, 0f, 0f);

		internal static readonly Vector4 identityTransformRow1Value = new Vector4(0f, 1f, 0f, 0f);

		internal static readonly Vector4 identityTransformRow2Value = new Vector4(0f, 0f, 1f, 0f);

		internal static readonly Vector4 infiniteClipRectValue = new Vector4(float.MinValue, float.MinValue, float.MaxValue, float.MaxValue);

		internal static readonly Vector4 fullOpacityValue = new Vector4(1f, 1f, 1f, 1f);

		public static readonly BMPAlloc identityTransform;

		public static readonly BMPAlloc infiniteClipRect;

		public static readonly BMPAlloc fullOpacity;
	}
}
