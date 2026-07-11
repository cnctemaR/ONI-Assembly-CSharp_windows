using System;
using System.Collections.Generic;
using Unity.Profiling;

namespace UnityEngine.UIElements.UIR
{
	internal class TextureBlitter : IDisposable
	{
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
					UIRUtility.Destroy(this.m_BlitMaterial);
					this.m_BlitMaterial = null;
				}
				this.disposed = true;
			}
		}

		static TextureBlitter()
		{
			for (int i = 0; i < 8; i++)
			{
				TextureBlitter.k_TextureIds[i] = Shader.PropertyToID("_MainTex" + i);
			}
		}

		public TextureBlitter(int capacity = 512)
		{
			this.m_PendingBlits = new List<TextureBlitter.BlitInfo>(capacity);
		}

		public void QueueBlit(Texture src, RectInt srcRect, Vector2Int dstPos, bool addBorder, Color tint)
		{
			bool disposed = this.disposed;
			if (disposed)
			{
				DisposeHelper.NotifyDisposedUsed(this);
			}
			else
			{
				this.m_PendingBlits.Add(new TextureBlitter.BlitInfo
				{
					src = src,
					srcRect = srcRect,
					dstPos = dstPos,
					border = (addBorder ? 1 : 0),
					tint = tint
				});
			}
		}

		public void BlitOneNow(RenderTexture dst, Texture src, RectInt srcRect, Vector2Int dstPos, bool addBorder, Color tint)
		{
			bool disposed = this.disposed;
			if (disposed)
			{
				DisposeHelper.NotifyDisposedUsed(this);
			}
			else
			{
				this.m_SingleBlit[0] = new TextureBlitter.BlitInfo
				{
					src = src,
					srcRect = srcRect,
					dstPos = dstPos,
					border = (addBorder ? 1 : 0),
					tint = tint
				};
				this.BeginBlit(dst);
				this.DoBlit(this.m_SingleBlit, 0);
				this.EndBlit();
			}
		}

		public int queueLength
		{
			get
			{
				return this.m_PendingBlits.Count;
			}
		}

		public void Commit(RenderTexture dst)
		{
			bool disposed = this.disposed;
			if (disposed)
			{
				DisposeHelper.NotifyDisposedUsed(this);
			}
			else
			{
				bool flag = this.m_PendingBlits.Count == 0;
				if (!flag)
				{
					this.BeginBlit(dst);
					for (int i = 0; i < this.m_PendingBlits.Count; i += 8)
					{
						this.DoBlit(this.m_PendingBlits, i);
					}
					this.EndBlit();
					this.m_PendingBlits.Clear();
				}
			}
		}

		public void Reset()
		{
			this.m_PendingBlits.Clear();
		}

		private void BeginBlit(RenderTexture dst)
		{
			bool flag = this.m_BlitMaterial == null;
			if (flag)
			{
				Shader shader = Shader.Find("Hidden/Internal-UIRAtlasBlitCopy");
				this.m_BlitMaterial = new Material(shader);
				this.m_BlitMaterial.hideFlags |= HideFlags.DontSaveInEditor;
			}
			this.m_Viewport = Utility.GetActiveViewport();
			this.m_PrevRT = RenderTexture.active;
			GL.LoadPixelMatrix(0f, (float)dst.width, 0f, (float)dst.height);
			Graphics.SetRenderTarget(dst);
		}

		private void DoBlit(IList<TextureBlitter.BlitInfo> blitInfos, int startIndex)
		{
			int num = Mathf.Min(startIndex + 8, blitInfos.Count);
			int i = startIndex;
			int num2 = 0;
			while (i < num)
			{
				Texture src = blitInfos[i].src;
				bool flag = src != null;
				if (flag)
				{
					this.m_BlitMaterial.SetTexture(TextureBlitter.k_TextureIds[num2], src);
				}
				i++;
				num2++;
			}
			this.m_BlitMaterial.SetPass(0);
			GL.Begin(7);
			int j = startIndex;
			int num3 = 0;
			while (j < num)
			{
				TextureBlitter.BlitInfo blitInfo = blitInfos[j];
				float num4 = 1f / (float)blitInfo.src.width;
				float num5 = 1f / (float)blitInfo.src.height;
				float num6 = (float)(blitInfo.dstPos.x - blitInfo.border);
				float num7 = (float)(blitInfo.dstPos.y - blitInfo.border);
				float num8 = (float)(blitInfo.dstPos.x + blitInfo.srcRect.width + blitInfo.border);
				float num9 = (float)(blitInfo.dstPos.y + blitInfo.srcRect.height + blitInfo.border);
				float num10 = (float)(blitInfo.srcRect.x - blitInfo.border) * num4;
				float num11 = (float)(blitInfo.srcRect.y - blitInfo.border) * num5;
				float num12 = (float)(blitInfo.srcRect.xMax + blitInfo.border) * num4;
				float num13 = (float)(blitInfo.srcRect.yMax + blitInfo.border) * num5;
				GL.Color(blitInfo.tint);
				GL.TexCoord3(num10, num11, (float)num3);
				GL.Vertex3(num6, num7, 0f);
				GL.Color(blitInfo.tint);
				GL.TexCoord3(num10, num13, (float)num3);
				GL.Vertex3(num6, num9, 0f);
				GL.Color(blitInfo.tint);
				GL.TexCoord3(num12, num13, (float)num3);
				GL.Vertex3(num8, num9, 0f);
				GL.Color(blitInfo.tint);
				GL.TexCoord3(num12, num11, (float)num3);
				GL.Vertex3(num8, num7, 0f);
				j++;
				num3++;
			}
			GL.End();
		}

		private void EndBlit()
		{
			Graphics.SetRenderTarget(this.m_PrevRT);
			GL.Viewport(new Rect((float)this.m_Viewport.x, (float)this.m_Viewport.y, (float)this.m_Viewport.width, (float)this.m_Viewport.height));
		}

		private const int k_TextureSlotCount = 8;

		private static readonly int[] k_TextureIds = new int[8];

		private static ProfilerMarker s_CommitSampler = new ProfilerMarker("UIR.TextureBlitter.Commit");

		private TextureBlitter.BlitInfo[] m_SingleBlit = new TextureBlitter.BlitInfo[1];

		private Material m_BlitMaterial;

		private RectInt m_Viewport;

		private RenderTexture m_PrevRT;

		private List<TextureBlitter.BlitInfo> m_PendingBlits;

		private struct BlitInfo
		{
			public Texture src;

			public RectInt srcRect;

			public Vector2Int dstPos;

			public int border;

			public Color tint;
		}
	}
}
