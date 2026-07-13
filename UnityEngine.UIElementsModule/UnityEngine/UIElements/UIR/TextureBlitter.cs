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
				TextureBlitter.k_TextureIds[i] = Shader.PropertyToID("_MainTex" + i.ToString());
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
				Shader shader = Shader.Find(Shaders.k_AtlasBlit);
				this.m_BlitMaterial = new Material(shader);
				this.m_BlitMaterial.hideFlags |= HideFlags.DontSaveInEditor;
			}
			bool flag2 = this.m_Properties == null;
			if (flag2)
			{
				this.m_Properties = new MaterialPropertyBlock();
			}
			this.m_Viewport = Utility.GetActiveViewport();
			this.m_PrevRT = RenderTexture.active;
			GL.LoadPixelMatrix(0f, (float)dst.width, 0f, (float)dst.height);
			Graphics.SetRenderTarget(dst);
			this.m_BlitMaterial.SetPass(0);
		}

		private void DoBlit(IList<TextureBlitter.BlitInfo> blitInfos, int startIndex)
		{
			int num = Mathf.Min(blitInfos.Count - startIndex, 8);
			int num2 = startIndex + num;
			int i = startIndex;
			int num3 = 0;
			while (i < num2)
			{
				Texture src = blitInfos[i].src;
				bool flag = src != null;
				if (flag)
				{
					this.m_Properties.SetTexture(TextureBlitter.k_TextureIds[num3], src);
				}
				i++;
				num3++;
			}
			Utility.SetPropertyBlock(this.m_Properties);
			GL.Begin(7);
			int j = startIndex;
			int num4 = 0;
			while (j < num2)
			{
				TextureBlitter.BlitInfo blitInfo = blitInfos[j];
				float num5 = 1f / (float)blitInfo.src.width;
				float num6 = 1f / (float)blitInfo.src.height;
				float num7 = (float)(blitInfo.dstPos.x - blitInfo.border);
				float num8 = (float)(blitInfo.dstPos.y - blitInfo.border);
				float num9 = (float)(blitInfo.dstPos.x + blitInfo.srcRect.width + blitInfo.border);
				float num10 = (float)(blitInfo.dstPos.y + blitInfo.srcRect.height + blitInfo.border);
				float num11 = (float)(blitInfo.srcRect.x - blitInfo.border) * num5;
				float num12 = (float)(blitInfo.srcRect.y - blitInfo.border) * num6;
				float num13 = (float)(blitInfo.srcRect.xMax + blitInfo.border) * num5;
				float num14 = (float)(blitInfo.srcRect.yMax + blitInfo.border) * num6;
				GL.Color(blitInfo.tint);
				GL.TexCoord3(num11, num12, (float)num4);
				GL.Vertex3(num7, num8, 0f);
				GL.Color(blitInfo.tint);
				GL.TexCoord3(num11, num14, (float)num4);
				GL.Vertex3(num7, num10, 0f);
				GL.Color(blitInfo.tint);
				GL.TexCoord3(num13, num14, (float)num4);
				GL.Vertex3(num9, num10, 0f);
				GL.Color(blitInfo.tint);
				GL.TexCoord3(num13, num12, (float)num4);
				GL.Vertex3(num9, num8, 0f);
				j++;
				num4++;
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

		private MaterialPropertyBlock m_Properties;

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
