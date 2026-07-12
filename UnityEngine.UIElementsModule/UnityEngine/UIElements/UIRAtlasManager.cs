using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Unity.Profiling;
using UnityEngine.UIElements.UIR;

namespace UnityEngine.UIElements
{
	internal class UIRAtlasManager : IDisposable
	{
		[field: DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public static event Action<UIRAtlasManager> atlasManagerCreated;

		[field: DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public static event Action<UIRAtlasManager> atlasManagerDisposed;

		public static UIRAtlasManager.ReadOnlyList<UIRAtlasManager> Instances()
		{
			return UIRAtlasManager.s_InstancesreadOnly;
		}

		public int maxImageSize { get; }

		public RenderTextureFormat format { get; }

		public RenderTexture atlas { get; private set; }

		public UIRAtlasManager(RenderTextureFormat format = RenderTextureFormat.ARGB32, FilterMode filterMode = FilterMode.Bilinear, int maxImageSize = 64, int initialSize = 64)
		{
			bool flag = filterMode != FilterMode.Bilinear && filterMode > FilterMode.Point;
			if (flag)
			{
				throw new NotSupportedException("The only supported atlas filter modes are point or bilinear");
			}
			this.format = format;
			this.maxImageSize = maxImageSize;
			this.m_FloatFormat = format == RenderTextureFormat.ARGBFloat;
			this.m_FilterMode = filterMode;
			this.m_UVs = new Dictionary<Texture2D, RectInt>(64);
			this.m_Blitter = new TextureBlitter(64);
			this.m_InitialSize = initialSize;
			this.m_2SidePadding = ((filterMode == FilterMode.Point) ? 0 : 2);
			this.m_1SidePadding = ((filterMode == FilterMode.Point) ? 0 : 1);
			this.Reset();
			UIRAtlasManager.s_Instances.Add(this);
			bool flag2 = UIRAtlasManager.atlasManagerCreated != null;
			if (flag2)
			{
				UIRAtlasManager.atlasManagerCreated(this);
			}
		}

		private protected bool disposed { protected get; private set; }

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			UIRAtlasManager.s_Instances.Remove(this);
			bool disposed = this.disposed;
			if (!disposed)
			{
				if (disposing)
				{
					UIRUtility.Destroy(this.atlas);
					this.atlas = null;
					bool flag = this.m_Allocator != null;
					if (flag)
					{
						this.m_Allocator.Dispose();
						this.m_Allocator = null;
					}
					bool flag2 = this.m_Blitter != null;
					if (flag2)
					{
						this.m_Blitter.Dispose();
						this.m_Blitter = null;
					}
					bool flag3 = UIRAtlasManager.atlasManagerDisposed != null;
					if (flag3)
					{
						UIRAtlasManager.atlasManagerDisposed(this);
					}
				}
				this.disposed = true;
			}
		}

		private static void LogDisposeError()
		{
			Debug.LogError("An attempt to use a disposed atlas manager has been detected.");
		}

		public static void MarkAllForReset()
		{
			UIRAtlasManager.s_GlobalResetVersion++;
		}

		public void MarkForReset()
		{
			this.m_ResetVersion = UIRAtlasManager.s_GlobalResetVersion - 1;
		}

		public bool RequiresReset()
		{
			return this.m_ResetVersion != UIRAtlasManager.s_GlobalResetVersion;
		}

		public bool IsReleased()
		{
			return this.atlas != null && !this.atlas.IsCreated();
		}

		public void Reset()
		{
			bool disposed = this.disposed;
			if (disposed)
			{
				UIRAtlasManager.LogDisposeError();
			}
			else
			{
				this.m_Blitter.Reset();
				this.m_UVs.Clear();
				this.m_Allocator = new UIRAtlasAllocator(this.m_InitialSize, 4096, this.m_1SidePadding);
				this.m_ForceReblitAll = false;
				this.m_ColorSpace = QualitySettings.activeColorSpace;
				UIRUtility.Destroy(this.atlas);
				this.atlas = null;
				this.m_ResetVersion = UIRAtlasManager.s_GlobalResetVersion;
			}
		}

		public bool TryGetLocation(Texture2D image, out RectInt uvs)
		{
			uvs = default(RectInt);
			bool disposed = this.disposed;
			bool flag;
			if (disposed)
			{
				UIRAtlasManager.LogDisposeError();
				flag = false;
			}
			else
			{
				bool flag2 = image == null;
				if (flag2)
				{
					flag = false;
				}
				else
				{
					bool flag3 = this.m_UVs.TryGetValue(image, out uvs);
					if (flag3)
					{
						flag = true;
					}
					else
					{
						bool flag4 = !this.IsTextureValid(image);
						if (flag4)
						{
							flag = false;
						}
						else
						{
							bool flag5 = !this.AllocateRect(image.width, image.height, out uvs);
							if (flag5)
							{
								flag = false;
							}
							else
							{
								this.m_UVs[image] = uvs;
								this.m_Blitter.QueueBlit(image, new RectInt(0, 0, image.width, image.height), new Vector2Int(uvs.x, uvs.y), true, Color.white);
								flag = true;
							}
						}
					}
				}
			}
			return flag;
		}

		public bool AllocateRect(int width, int height, out RectInt uvs)
		{
			bool flag = !this.m_Allocator.TryAllocate(width + this.m_2SidePadding, height + this.m_2SidePadding, out uvs);
			bool flag2;
			if (flag)
			{
				flag2 = false;
			}
			else
			{
				uvs = new RectInt(uvs.x + this.m_1SidePadding, uvs.y + this.m_1SidePadding, width, height);
				flag2 = true;
			}
			return flag2;
		}

		public void EnqueueBlit(Texture image, RectInt srcRect, int x, int y, bool addBorder, Color tint)
		{
			this.m_Blitter.QueueBlit(image, srcRect, new Vector2Int(x, y), addBorder, tint);
		}

		public static bool IsTextureFormatSupported(TextureFormat format)
		{
			switch (format)
			{
			case TextureFormat.Alpha8:
			case TextureFormat.ARGB4444:
			case TextureFormat.RGB24:
			case TextureFormat.RGBA32:
			case TextureFormat.ARGB32:
			case TextureFormat.RGB565:
			case TextureFormat.R16:
			case TextureFormat.DXT1:
			case TextureFormat.DXT5:
			case TextureFormat.RGBA4444:
			case TextureFormat.BGRA32:
			case TextureFormat.BC7:
			case TextureFormat.BC4:
			case TextureFormat.BC5:
			case TextureFormat.DXT1Crunched:
			case TextureFormat.DXT5Crunched:
			case TextureFormat.PVRTC_RGB2:
			case TextureFormat.PVRTC_RGBA2:
			case TextureFormat.PVRTC_RGB4:
			case TextureFormat.PVRTC_RGBA4:
			case TextureFormat.ETC_RGB4:
			case TextureFormat.EAC_R:
			case TextureFormat.EAC_R_SIGNED:
			case TextureFormat.EAC_RG:
			case TextureFormat.EAC_RG_SIGNED:
			case TextureFormat.ETC2_RGB:
			case TextureFormat.ETC2_RGBA1:
			case TextureFormat.ETC2_RGBA8:
			case TextureFormat.ASTC_4x4:
			case TextureFormat.ASTC_5x5:
			case TextureFormat.ASTC_6x6:
			case TextureFormat.ASTC_8x8:
			case TextureFormat.ASTC_10x10:
			case TextureFormat.ASTC_12x12:
			case TextureFormat.ASTC_RGBA_4x4:
			case TextureFormat.ASTC_RGBA_5x5:
			case TextureFormat.ASTC_RGBA_6x6:
			case TextureFormat.ASTC_RGBA_8x8:
			case TextureFormat.ASTC_RGBA_10x10:
			case TextureFormat.ASTC_RGBA_12x12:
			case TextureFormat.ETC_RGB4_3DS:
			case TextureFormat.ETC_RGBA8_3DS:
			case TextureFormat.RG16:
			case TextureFormat.R8:
			case TextureFormat.ETC_RGB4Crunched:
			case TextureFormat.ETC2_RGBA8Crunched:
				return true;
			case TextureFormat.RHalf:
			case TextureFormat.RGHalf:
			case TextureFormat.RGBAHalf:
			case TextureFormat.RFloat:
			case TextureFormat.RGFloat:
			case TextureFormat.RGBAFloat:
			case TextureFormat.YUY2:
			case TextureFormat.RGB9e5Float:
			case TextureFormat.BC6H:
			case TextureFormat.ASTC_HDR_4x4:
			case TextureFormat.ASTC_HDR_5x5:
			case TextureFormat.ASTC_HDR_6x6:
			case TextureFormat.ASTC_HDR_8x8:
			case TextureFormat.ASTC_HDR_10x10:
			case TextureFormat.ASTC_HDR_12x12:
			case TextureFormat.RG32:
			case TextureFormat.RGB48:
			case TextureFormat.RGBA64:
				return false;
			}
			return false;
		}

		private bool IsTextureValid(Texture2D image)
		{
			bool isReadable = image.isReadable;
			bool flag;
			if (isReadable)
			{
				flag = false;
			}
			else
			{
				bool flag2 = image.width > this.maxImageSize || image.height > this.maxImageSize;
				if (flag2)
				{
					flag = false;
				}
				else
				{
					bool flag3 = !UIRAtlasManager.IsTextureFormatSupported(image.format);
					if (flag3)
					{
						flag = false;
					}
					else
					{
						bool flag4 = !this.m_FloatFormat && this.m_ColorSpace == ColorSpace.Linear && image.activeTextureColorSpace > ColorSpace.Gamma;
						if (flag4)
						{
							flag = false;
						}
						else
						{
							bool flag5 = SystemInfo.graphicsShaderLevel >= 35;
							if (flag5)
							{
								bool flag6 = image.filterMode != FilterMode.Bilinear && image.filterMode > FilterMode.Point;
								if (flag6)
								{
									return false;
								}
							}
							else
							{
								bool flag7 = this.m_FilterMode != image.filterMode;
								if (flag7)
								{
									return false;
								}
							}
							bool flag8 = image.wrapMode != TextureWrapMode.Clamp;
							flag = !flag8;
						}
					}
				}
			}
			return flag;
		}

		public void Commit()
		{
			bool disposed = this.disposed;
			if (disposed)
			{
				UIRAtlasManager.LogDisposeError();
			}
			else
			{
				this.UpdateAtlasTexture();
				bool forceReblitAll = this.m_ForceReblitAll;
				if (forceReblitAll)
				{
					this.m_ForceReblitAll = false;
					this.m_Blitter.Reset();
					foreach (KeyValuePair<Texture2D, RectInt> keyValuePair in this.m_UVs)
					{
						this.m_Blitter.QueueBlit(keyValuePair.Key, new RectInt(0, 0, keyValuePair.Key.width, keyValuePair.Key.height), new Vector2Int(keyValuePair.Value.x, keyValuePair.Value.y), true, Color.white);
					}
				}
				this.m_Blitter.Commit(this.atlas);
			}
		}

		private void UpdateAtlasTexture()
		{
			bool flag = this.atlas == null;
			if (flag)
			{
				bool flag2 = this.m_UVs.Count > this.m_Blitter.queueLength;
				if (flag2)
				{
					this.m_ForceReblitAll = true;
				}
				this.atlas = this.CreateAtlasTexture();
			}
			else
			{
				bool flag3 = this.atlas.width != this.m_Allocator.physicalWidth || this.atlas.height != this.m_Allocator.physicalHeight;
				if (flag3)
				{
					RenderTexture renderTexture = this.CreateAtlasTexture();
					bool flag4 = renderTexture == null;
					if (flag4)
					{
						Debug.LogErrorFormat("Failed to allocate a render texture for the dynamic atlas. Current Size = {0}x{1}. Requested Size = {2}x{3}.", new object[]
						{
							this.atlas.width,
							this.atlas.height,
							this.m_Allocator.physicalWidth,
							this.m_Allocator.physicalHeight
						});
					}
					else
					{
						this.m_Blitter.BlitOneNow(renderTexture, this.atlas, new RectInt(0, 0, this.atlas.width, this.atlas.height), new Vector2Int(0, 0), false, Color.white);
					}
					UIRUtility.Destroy(this.atlas);
					this.atlas = renderTexture;
				}
			}
		}

		private RenderTexture CreateAtlasTexture()
		{
			bool flag = this.m_Allocator.physicalWidth == 0 || this.m_Allocator.physicalHeight == 0;
			RenderTexture renderTexture;
			if (flag)
			{
				renderTexture = null;
			}
			else
			{
				renderTexture = new RenderTexture(this.m_Allocator.physicalWidth, this.m_Allocator.physicalHeight, 0, this.format)
				{
					hideFlags = HideFlags.HideAndDontSave,
					name = "UIR Atlas " + UIRAtlasManager.s_TextureCounter++.ToString(),
					filterMode = this.m_FilterMode
				};
			}
			return renderTexture;
		}

		private static List<UIRAtlasManager> s_Instances = new List<UIRAtlasManager>();

		private static UIRAtlasManager.ReadOnlyList<UIRAtlasManager> s_InstancesreadOnly = new UIRAtlasManager.ReadOnlyList<UIRAtlasManager>(UIRAtlasManager.s_Instances);

		private int m_InitialSize;

		private UIRAtlasAllocator m_Allocator;

		private Dictionary<Texture2D, RectInt> m_UVs;

		private bool m_ForceReblitAll;

		private bool m_FloatFormat;

		private FilterMode m_FilterMode;

		private ColorSpace m_ColorSpace;

		private TextureBlitter m_Blitter;

		private int m_2SidePadding;

		private int m_1SidePadding;

		private static ProfilerMarker s_MarkerReset = new ProfilerMarker("UIR.AtlasManager.Reset");

		private static int s_TextureCounter;

		private static int s_GlobalResetVersion;

		private int m_ResetVersion = UIRAtlasManager.s_GlobalResetVersion;

		public struct ReadOnlyList<T> : IEnumerable<T>, IEnumerable
		{
			public ReadOnlyList(List<T> list)
			{
				this.m_List = list;
			}

			public IEnumerator<T> GetEnumerator()
			{
				return this.m_List.GetEnumerator();
			}

			IEnumerator IEnumerable.GetEnumerator()
			{
				return this.m_List.GetEnumerator();
			}

			public int Count
			{
				get
				{
					return this.m_List.Count;
				}
			}

			public T this[int i]
			{
				get
				{
					return this.m_List[i];
				}
			}

			private List<T> m_List;
		}
	}
}
