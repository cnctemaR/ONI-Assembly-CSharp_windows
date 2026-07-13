using System;
using UnityEngine.UIElements.UIR;

namespace UnityEngine.UIElements
{
	internal class DynamicAtlasPage : IDisposable
	{
		public TextureId textureId { get; private set; }

		public RenderTexture atlas { get; private set; }

		public RenderTextureFormat format { get; }

		public FilterMode filterMode { get; }

		public Vector2Int minSize { get; }

		public Vector2Int maxSize { get; }

		public Vector2Int currentSize
		{
			get
			{
				return this.m_CurrentSize;
			}
		}

		internal Allocator2D allocator
		{
			get
			{
				return this.m_Allocator;
			}
		}

		public DynamicAtlasPage(RenderTextureFormat format, FilterMode filterMode, Vector2Int minSize, Vector2Int maxSize)
		{
			this.textureId = TextureRegistry.instance.AllocAndAcquireDynamic();
			this.format = format;
			this.filterMode = filterMode;
			this.minSize = minSize;
			this.maxSize = maxSize;
			this.m_Allocator = new Allocator2D(minSize, maxSize, this.m_2Padding);
			this.m_Blitter = new TextureBlitter(64);
		}

		private protected bool disposed { protected get; private set; }

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			bool disposed = this.disposed;
			if (!disposed)
			{
				if (disposing)
				{
					bool flag = this.atlas != null;
					if (flag)
					{
						UIRUtility.Destroy(this.atlas);
						this.atlas = null;
					}
					bool flag2 = this.m_Allocator != null;
					if (flag2)
					{
						this.m_Allocator = null;
					}
					bool flag3 = this.m_Blitter != null;
					if (flag3)
					{
						this.m_Blitter.Dispose();
						this.m_Blitter = null;
					}
					bool flag4 = this.textureId != TextureId.invalid;
					if (flag4)
					{
						TextureRegistry.instance.Release(this.textureId);
						this.textureId = TextureId.invalid;
					}
				}
				this.disposed = true;
			}
		}

		public bool TryAdd(Texture2D image, out Allocator2D.Alloc2D alloc, out RectInt rect)
		{
			bool disposed = this.disposed;
			bool flag;
			if (disposed)
			{
				DisposeHelper.NotifyDisposedUsed(this);
				alloc = default(Allocator2D.Alloc2D);
				rect = default(RectInt);
				flag = false;
			}
			else
			{
				bool flag2 = !this.m_Allocator.TryAllocate(image.width + this.m_2Padding, image.height + this.m_2Padding, out alloc);
				if (flag2)
				{
					rect = default(RectInt);
					flag = false;
				}
				else
				{
					this.m_CurrentSize.x = Mathf.Max(this.m_CurrentSize.x, UIRUtility.GetNextPow2(alloc.rect.xMax));
					this.m_CurrentSize.y = Mathf.Max(this.m_CurrentSize.y, UIRUtility.GetNextPow2(alloc.rect.yMax));
					rect = new RectInt(alloc.rect.xMin + this.m_1Padding, alloc.rect.yMin + this.m_1Padding, image.width, image.height);
					this.Update(image, rect);
					flag = true;
				}
			}
			return flag;
		}

		public void Update(Texture2D image, RectInt rect)
		{
			bool disposed = this.disposed;
			if (disposed)
			{
				DisposeHelper.NotifyDisposedUsed(this);
			}
			else
			{
				Debug.Assert(image != null && rect.width > 0 && rect.height > 0);
				this.m_Blitter.QueueBlit(image, new RectInt(0, 0, image.width, image.height), new Vector2Int(rect.x, rect.y), true, Color.white);
			}
		}

		public void Remove(Allocator2D.Alloc2D alloc)
		{
			bool disposed = this.disposed;
			if (disposed)
			{
				DisposeHelper.NotifyDisposedUsed(this);
			}
			else
			{
				Debug.Assert(alloc.rect.width > 0 && alloc.rect.height > 0);
				this.m_Allocator.Free(alloc);
			}
		}

		public void Commit()
		{
			bool disposed = this.disposed;
			if (disposed)
			{
				DisposeHelper.NotifyDisposedUsed(this);
			}
			else
			{
				this.UpdateAtlasTexture();
				this.m_Blitter.Commit(this.atlas);
			}
		}

		private void UpdateAtlasTexture()
		{
			bool flag = this.atlas == null;
			if (flag)
			{
				this.atlas = this.CreateAtlasTexture();
			}
			else
			{
				bool flag2 = this.atlas.width != this.m_CurrentSize.x || this.atlas.height != this.m_CurrentSize.y;
				if (flag2)
				{
					RenderTexture renderTexture = this.CreateAtlasTexture();
					bool flag3 = renderTexture == null;
					if (flag3)
					{
						Debug.LogErrorFormat("Failed to allocate a render texture for the dynamic atlas. Current Size = {0}x{1}. Requested Size = {2}x{3}.", new object[]
						{
							this.atlas.width,
							this.atlas.height,
							this.m_CurrentSize.x,
							this.m_CurrentSize.y
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
			bool flag = this.m_CurrentSize.x == 0 || this.m_CurrentSize.y == 0;
			RenderTexture renderTexture;
			if (flag)
			{
				renderTexture = null;
			}
			else
			{
				renderTexture = new RenderTexture(this.m_CurrentSize.x, this.m_CurrentSize.y, 0, this.format)
				{
					hideFlags = HideFlags.HideAndDontSave,
					name = "UIR Dynamic Atlas Page " + DynamicAtlasPage.s_TextureCounter++.ToString(),
					filterMode = this.filterMode
				};
			}
			return renderTexture;
		}

		private readonly int m_1Padding = 1;

		private readonly int m_2Padding = 2;

		private Allocator2D m_Allocator;

		private TextureBlitter m_Blitter;

		private Vector2Int m_CurrentSize;

		private static int s_TextureCounter;
	}
}
