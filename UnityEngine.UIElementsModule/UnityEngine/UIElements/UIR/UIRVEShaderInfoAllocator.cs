using System;

namespace UnityEngine.UIElements.UIR
{
	internal class UIRVEShaderInfoAllocator
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

		private static void AcquireDefaultShaderInfoTexture()
		{
			bool flag = ++UIRVEShaderInfoAllocator.s_DefaultShaderInfoTextureRefCount == 1;
			if (flag)
			{
				UIRVEShaderInfoAllocator.s_DefaultShaderInfoTexture = new Texture2D(64, 64, TextureFormat.RGBAFloat, false);
				UIRVEShaderInfoAllocator.s_DefaultShaderInfoTexture.name = "DefaultShaderInfoTexFloat";
				UIRVEShaderInfoAllocator.s_DefaultShaderInfoTexture.hideFlags = HideFlags.HideAndDontSave;
				UIRVEShaderInfoAllocator.s_DefaultShaderInfoTexture.filterMode = FilterMode.Point;
				UIRVEShaderInfoAllocator.s_DefaultShaderInfoTexture.SetPixel(UIRVEShaderInfoAllocator.identityTransformTexel.x, UIRVEShaderInfoAllocator.identityTransformTexel.y, UIRVEShaderInfoAllocator.identityTransformRow0Value);
				UIRVEShaderInfoAllocator.s_DefaultShaderInfoTexture.SetPixel(UIRVEShaderInfoAllocator.identityTransformTexel.x, UIRVEShaderInfoAllocator.identityTransformTexel.y + 1, UIRVEShaderInfoAllocator.identityTransformRow1Value);
				UIRVEShaderInfoAllocator.s_DefaultShaderInfoTexture.SetPixel(UIRVEShaderInfoAllocator.identityTransformTexel.x, UIRVEShaderInfoAllocator.identityTransformTexel.y + 2, UIRVEShaderInfoAllocator.identityTransformRow2Value);
				UIRVEShaderInfoAllocator.s_DefaultShaderInfoTexture.SetPixel(UIRVEShaderInfoAllocator.infiniteClipRectTexel.x, UIRVEShaderInfoAllocator.infiniteClipRectTexel.y, UIRVEShaderInfoAllocator.infiniteClipRectValue);
				UIRVEShaderInfoAllocator.s_DefaultShaderInfoTexture.SetPixel(UIRVEShaderInfoAllocator.fullOpacityTexel.x, UIRVEShaderInfoAllocator.fullOpacityTexel.y, UIRVEShaderInfoAllocator.fullOpacityValue);
				UIRVEShaderInfoAllocator.s_DefaultShaderInfoTexture.SetPixel(UIRVEShaderInfoAllocator.defaultTextCoreSettingsTexel.x, UIRVEShaderInfoAllocator.defaultTextCoreSettingsTexel.y, Color.white);
				UIRVEShaderInfoAllocator.s_DefaultShaderInfoTexture.SetPixel(UIRVEShaderInfoAllocator.defaultTextCoreSettingsTexel.x, UIRVEShaderInfoAllocator.defaultTextCoreSettingsTexel.y + 1, Color.clear);
				UIRVEShaderInfoAllocator.s_DefaultShaderInfoTexture.SetPixel(UIRVEShaderInfoAllocator.defaultTextCoreSettingsTexel.x, UIRVEShaderInfoAllocator.defaultTextCoreSettingsTexel.y + 2, Color.clear);
				UIRVEShaderInfoAllocator.s_DefaultShaderInfoTexture.SetPixel(UIRVEShaderInfoAllocator.defaultTextCoreSettingsTexel.x, UIRVEShaderInfoAllocator.defaultTextCoreSettingsTexel.y + 3, Color.clear);
				UIRVEShaderInfoAllocator.s_DefaultShaderInfoTexture.Apply(false, true);
			}
		}

		private static void ReleaseDefaultShaderInfoTexture()
		{
			bool flag = --UIRVEShaderInfoAllocator.s_DefaultShaderInfoTextureRefCount == 0;
			if (flag)
			{
				UIRUtility.Destroy(UIRVEShaderInfoAllocator.s_DefaultShaderInfoTexture);
				UIRVEShaderInfoAllocator.s_DefaultShaderInfoTexture = null;
			}
		}

		private static Vector2Int AllocToTexelCoord(ref BitmapAllocator32 allocator, BMPAlloc alloc)
		{
			ushort num;
			ushort num2;
			allocator.GetAllocPageAtlasLocation(alloc.page, out num, out num2);
			return new Vector2Int((int)alloc.bitIndex * allocator.entryWidth + (int)num, (int)alloc.pageLine * allocator.entryHeight + (int)num2);
		}

		private static bool AtlasRectMatchesPage(ref BitmapAllocator32 allocator, BMPAlloc defAlloc, RectInt atlasRect)
		{
			ushort num;
			ushort num2;
			allocator.GetAllocPageAtlasLocation(defAlloc.page, out num, out num2);
			return (int)num == atlasRect.xMin && (int)num2 == atlasRect.yMin && allocator.entryWidth * UIRVEShaderInfoAllocator.pageWidth == atlasRect.width && allocator.entryHeight * UIRVEShaderInfoAllocator.pageHeight == atlasRect.height;
		}

		public Texture atlas
		{
			get
			{
				bool storageReallyCreated = this.m_StorageReallyCreated;
				Texture texture;
				if (storageReallyCreated)
				{
					texture = this.m_Storage.texture;
				}
				else
				{
					texture = UIRVEShaderInfoAllocator.s_DefaultShaderInfoTexture;
				}
				return texture;
			}
		}

		public bool internalAtlasCreated
		{
			get
			{
				return this.m_StorageReallyCreated;
			}
		}

		public UIRVEShaderInfoAllocator(ColorSpace colorSpace)
		{
			this.m_ColorSpace = colorSpace;
			this.m_OpacityAllocator = (this.m_ColorAllocator = (this.m_ClipRectAllocator = (this.m_TransformAllocator = (this.m_TextSettingsAllocator = default(BitmapAllocator32)))));
			this.m_TransformAllocator.Construct(UIRVEShaderInfoAllocator.pageHeight, 1, 3);
			this.m_TransformAllocator.ForceFirstAlloc((ushort)UIRVEShaderInfoAllocator.identityTransformTexel.x, (ushort)UIRVEShaderInfoAllocator.identityTransformTexel.y);
			this.m_ClipRectAllocator.Construct(UIRVEShaderInfoAllocator.pageHeight, 1, 1);
			this.m_ClipRectAllocator.ForceFirstAlloc((ushort)UIRVEShaderInfoAllocator.infiniteClipRectTexel.x, (ushort)UIRVEShaderInfoAllocator.infiniteClipRectTexel.y);
			this.m_OpacityAllocator.Construct(UIRVEShaderInfoAllocator.pageHeight, 1, 1);
			this.m_OpacityAllocator.ForceFirstAlloc((ushort)UIRVEShaderInfoAllocator.fullOpacityTexel.x, (ushort)UIRVEShaderInfoAllocator.fullOpacityTexel.y);
			this.m_ColorAllocator.Construct(UIRVEShaderInfoAllocator.pageHeight, 1, 1);
			this.m_ColorAllocator.ForceFirstAlloc((ushort)UIRVEShaderInfoAllocator.clearColorTexel.x, (ushort)UIRVEShaderInfoAllocator.clearColorTexel.y);
			this.m_TextSettingsAllocator.Construct(UIRVEShaderInfoAllocator.pageHeight, 1, 4);
			this.m_TextSettingsAllocator.ForceFirstAlloc((ushort)UIRVEShaderInfoAllocator.defaultTextCoreSettingsTexel.x, (ushort)UIRVEShaderInfoAllocator.defaultTextCoreSettingsTexel.y);
			UIRVEShaderInfoAllocator.AcquireDefaultShaderInfoTexture();
		}

		private void ReallyCreateStorage()
		{
			this.m_Storage = new ShaderInfoStorageRGBAFloat(64, 4096);
			RectInt rectInt;
			this.m_Storage.AllocateRect(UIRVEShaderInfoAllocator.pageWidth * this.m_TransformAllocator.entryWidth, UIRVEShaderInfoAllocator.pageHeight * this.m_TransformAllocator.entryHeight, out rectInt);
			RectInt rectInt2;
			this.m_Storage.AllocateRect(UIRVEShaderInfoAllocator.pageWidth * this.m_ClipRectAllocator.entryWidth, UIRVEShaderInfoAllocator.pageHeight * this.m_ClipRectAllocator.entryHeight, out rectInt2);
			RectInt rectInt3;
			this.m_Storage.AllocateRect(UIRVEShaderInfoAllocator.pageWidth * this.m_OpacityAllocator.entryWidth, UIRVEShaderInfoAllocator.pageHeight * this.m_OpacityAllocator.entryHeight, out rectInt3);
			RectInt rectInt4;
			this.m_Storage.AllocateRect(UIRVEShaderInfoAllocator.pageWidth * this.m_ColorAllocator.entryWidth, UIRVEShaderInfoAllocator.pageHeight * this.m_ColorAllocator.entryHeight, out rectInt4);
			RectInt rectInt5;
			this.m_Storage.AllocateRect(UIRVEShaderInfoAllocator.pageWidth * this.m_TextSettingsAllocator.entryWidth, UIRVEShaderInfoAllocator.pageHeight * this.m_TextSettingsAllocator.entryHeight, out rectInt5);
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
			bool flag4 = !UIRVEShaderInfoAllocator.AtlasRectMatchesPage(ref this.m_ColorAllocator, UIRVEShaderInfoAllocator.clearColor, rectInt4);
			if (flag4)
			{
				throw new Exception("Atlas clear color allocation failed unexpectedly");
			}
			bool flag5 = !UIRVEShaderInfoAllocator.AtlasRectMatchesPage(ref this.m_TextSettingsAllocator, UIRVEShaderInfoAllocator.defaultTextCoreSettings, rectInt5);
			if (flag5)
			{
				throw new Exception("Atlas text setting allocation failed unexpectedly");
			}
			this.SetTransformValue(UIRVEShaderInfoAllocator.identityTransform, UIRVEShaderInfoAllocator.identityTransformValue);
			this.SetClipRectValue(UIRVEShaderInfoAllocator.infiniteClipRect, UIRVEShaderInfoAllocator.infiniteClipRectValue);
			this.SetOpacityValue(UIRVEShaderInfoAllocator.fullOpacity, UIRVEShaderInfoAllocator.fullOpacityValue.w);
			this.SetColorValue(UIRVEShaderInfoAllocator.clearColor, UIRVEShaderInfoAllocator.clearColorValue);
			this.SetTextCoreSettingValue(UIRVEShaderInfoAllocator.defaultTextCoreSettings, UIRVEShaderInfoAllocator.defaultTextCoreSettingsValue);
			this.m_StorageReallyCreated = true;
		}

		public void Dispose()
		{
			bool flag = this.m_Storage != null;
			if (flag)
			{
				this.m_Storage.Dispose();
			}
			this.m_Storage = null;
			this.m_StorageReallyCreated = false;
			UIRVEShaderInfoAllocator.ReleaseDefaultShaderInfoTexture();
		}

		public void IssuePendingStorageChanges()
		{
			BaseShaderInfoStorage storage = this.m_Storage;
			if (storage != null)
			{
				storage.UpdateTexture();
			}
		}

		public BMPAlloc AllocTransform()
		{
			bool flag = !this.m_StorageReallyCreated;
			if (flag)
			{
				this.ReallyCreateStorage();
			}
			return this.m_TransformAllocator.Allocate(this.m_Storage);
		}

		public BMPAlloc AllocClipRect()
		{
			bool flag = !this.m_StorageReallyCreated;
			if (flag)
			{
				this.ReallyCreateStorage();
			}
			return this.m_ClipRectAllocator.Allocate(this.m_Storage);
		}

		public BMPAlloc AllocOpacity()
		{
			bool flag = !this.m_StorageReallyCreated;
			if (flag)
			{
				this.ReallyCreateStorage();
			}
			return this.m_OpacityAllocator.Allocate(this.m_Storage);
		}

		public BMPAlloc AllocColor()
		{
			bool flag = !this.m_StorageReallyCreated;
			if (flag)
			{
				this.ReallyCreateStorage();
			}
			return this.m_ColorAllocator.Allocate(this.m_Storage);
		}

		public BMPAlloc AllocTextCoreSettings(TextCoreSettings settings)
		{
			bool flag = !this.m_StorageReallyCreated;
			if (flag)
			{
				this.ReallyCreateStorage();
			}
			return this.m_TextSettingsAllocator.Allocate(this.m_Storage);
		}

		public void SetTransformValue(BMPAlloc alloc, Matrix4x4 xform)
		{
			Debug.Assert(alloc.IsValid());
			Vector2Int vector2Int = UIRVEShaderInfoAllocator.AllocToTexelCoord(ref this.m_TransformAllocator, alloc);
			this.m_Storage.SetTexel(vector2Int.x, vector2Int.y, xform.GetRow(0));
			this.m_Storage.SetTexel(vector2Int.x, vector2Int.y + 1, xform.GetRow(1));
			this.m_Storage.SetTexel(vector2Int.x, vector2Int.y + 2, xform.GetRow(2));
		}

		public void SetClipRectValue(BMPAlloc alloc, Vector4 clipRect)
		{
			Debug.Assert(alloc.IsValid());
			Vector2Int vector2Int = UIRVEShaderInfoAllocator.AllocToTexelCoord(ref this.m_ClipRectAllocator, alloc);
			this.m_Storage.SetTexel(vector2Int.x, vector2Int.y, clipRect);
		}

		public void SetOpacityValue(BMPAlloc alloc, float opacity)
		{
			Debug.Assert(alloc.IsValid());
			Vector2Int vector2Int = UIRVEShaderInfoAllocator.AllocToTexelCoord(ref this.m_OpacityAllocator, alloc);
			this.m_Storage.SetTexel(vector2Int.x, vector2Int.y, new Color(1f, 1f, 1f, opacity));
		}

		public void SetColorValue(BMPAlloc alloc, Color color)
		{
			Debug.Assert(alloc.IsValid());
			Vector2Int vector2Int = UIRVEShaderInfoAllocator.AllocToTexelCoord(ref this.m_ColorAllocator, alloc);
			bool flag = this.m_ColorSpace == ColorSpace.Linear;
			if (flag)
			{
				this.m_Storage.SetTexel(vector2Int.x, vector2Int.y, color.linear);
			}
			else
			{
				this.m_Storage.SetTexel(vector2Int.x, vector2Int.y, color);
			}
		}

		public void SetTextCoreSettingValue(BMPAlloc alloc, TextCoreSettings settings)
		{
			Debug.Assert(alloc.IsValid());
			Vector2Int vector2Int = UIRVEShaderInfoAllocator.AllocToTexelCoord(ref this.m_TextSettingsAllocator, alloc);
			Color color = new Color(-settings.underlayOffset.x, settings.underlayOffset.y, settings.underlaySoftness, settings.outlineWidth);
			bool flag = this.m_ColorSpace == ColorSpace.Linear;
			if (flag)
			{
				this.m_Storage.SetTexel(vector2Int.x, vector2Int.y, settings.faceColor.linear);
				this.m_Storage.SetTexel(vector2Int.x, vector2Int.y + 1, settings.outlineColor.linear);
				this.m_Storage.SetTexel(vector2Int.x, vector2Int.y + 2, settings.underlayColor.linear);
			}
			else
			{
				this.m_Storage.SetTexel(vector2Int.x, vector2Int.y, settings.faceColor);
				this.m_Storage.SetTexel(vector2Int.x, vector2Int.y + 1, settings.outlineColor);
				this.m_Storage.SetTexel(vector2Int.x, vector2Int.y + 2, settings.underlayColor);
			}
			this.m_Storage.SetTexel(vector2Int.x, vector2Int.y + 3, color);
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

		public void FreeColor(BMPAlloc alloc)
		{
			Debug.Assert(alloc.IsValid());
			this.m_ColorAllocator.Free(alloc);
		}

		public void FreeTextCoreSettings(BMPAlloc alloc)
		{
			Debug.Assert(alloc.IsValid());
			this.m_TextSettingsAllocator.Free(alloc);
		}

		public Color32 TransformAllocToVertexData(BMPAlloc alloc)
		{
			Debug.Assert(UIRVEShaderInfoAllocator.pageWidth == 32 && UIRVEShaderInfoAllocator.pageHeight == 8);
			ushort num = 0;
			ushort num2 = 0;
			this.m_TransformAllocator.GetAllocPageAtlasLocation(alloc.page, out num, out num2);
			return new Color32((byte)(num >> 5), (byte)(num2 >> 3), (byte)((int)alloc.pageLine * UIRVEShaderInfoAllocator.pageWidth + (int)alloc.bitIndex), 0);
		}

		public Color32 ClipRectAllocToVertexData(BMPAlloc alloc)
		{
			Debug.Assert(UIRVEShaderInfoAllocator.pageWidth == 32 && UIRVEShaderInfoAllocator.pageHeight == 8);
			ushort num = 0;
			ushort num2 = 0;
			this.m_ClipRectAllocator.GetAllocPageAtlasLocation(alloc.page, out num, out num2);
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

		public Color32 ColorAllocToVertexData(BMPAlloc alloc)
		{
			Debug.Assert(UIRVEShaderInfoAllocator.pageWidth == 32 && UIRVEShaderInfoAllocator.pageHeight == 8);
			ushort num;
			ushort num2;
			this.m_ColorAllocator.GetAllocPageAtlasLocation(alloc.page, out num, out num2);
			return new Color32((byte)(num >> 5), (byte)(num2 >> 3), (byte)((int)alloc.pageLine * UIRVEShaderInfoAllocator.pageWidth + (int)alloc.bitIndex), 0);
		}

		public Color32 TextCoreSettingsToVertexData(BMPAlloc alloc)
		{
			Debug.Assert(UIRVEShaderInfoAllocator.pageWidth == 32 && UIRVEShaderInfoAllocator.pageHeight == 8);
			ushort num;
			ushort num2;
			this.m_TextSettingsAllocator.GetAllocPageAtlasLocation(alloc.page, out num, out num2);
			return new Color32((byte)(num >> 5), (byte)(num2 >> 3), (byte)((int)alloc.pageLine * UIRVEShaderInfoAllocator.pageWidth + (int)alloc.bitIndex), 0);
		}

		private BaseShaderInfoStorage m_Storage;

		private BitmapAllocator32 m_TransformAllocator;

		private BitmapAllocator32 m_ClipRectAllocator;

		private BitmapAllocator32 m_OpacityAllocator;

		private BitmapAllocator32 m_ColorAllocator;

		private BitmapAllocator32 m_TextSettingsAllocator;

		private bool m_StorageReallyCreated;

		private ColorSpace m_ColorSpace;

		private static readonly Vector2Int identityTransformTexel = new Vector2Int(0, 0);

		private static readonly Vector2Int infiniteClipRectTexel = new Vector2Int(0, 32);

		private static readonly Vector2Int fullOpacityTexel = new Vector2Int(32, 32);

		private static readonly Vector2Int clearColorTexel = new Vector2Int(0, 40);

		private static readonly Vector2Int defaultTextCoreSettingsTexel = new Vector2Int(32, 0);

		private static readonly Matrix4x4 identityTransformValue = Matrix4x4.identity;

		private static readonly Vector4 identityTransformRow0Value = UIRVEShaderInfoAllocator.identityTransformValue.GetRow(0);

		private static readonly Vector4 identityTransformRow1Value = UIRVEShaderInfoAllocator.identityTransformValue.GetRow(1);

		private static readonly Vector4 identityTransformRow2Value = UIRVEShaderInfoAllocator.identityTransformValue.GetRow(2);

		private static readonly Vector4 infiniteClipRectValue = new Vector4(0f, 0f, 0f, 0f);

		private static readonly Vector4 fullOpacityValue = new Vector4(1f, 1f, 1f, 1f);

		private static readonly Vector4 clearColorValue = new Vector4(0f, 0f, 0f, 0f);

		private static readonly TextCoreSettings defaultTextCoreSettingsValue = new TextCoreSettings
		{
			faceColor = Color.white,
			outlineColor = Color.clear,
			outlineWidth = 0f,
			underlayColor = Color.clear,
			underlayOffset = Vector2.zero,
			underlaySoftness = 0f
		};

		public static readonly BMPAlloc identityTransform;

		public static readonly BMPAlloc infiniteClipRect;

		public static readonly BMPAlloc fullOpacity;

		public static readonly BMPAlloc clearColor;

		public static readonly BMPAlloc defaultTextCoreSettings;

		private static int s_DefaultShaderInfoTextureRefCount;

		private static Texture2D s_DefaultShaderInfoTexture;
	}
}
