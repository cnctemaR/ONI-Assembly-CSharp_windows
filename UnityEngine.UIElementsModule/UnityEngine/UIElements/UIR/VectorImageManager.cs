using System;
using System.Collections.Generic;
using Unity.Profiling;

namespace UnityEngine.UIElements.UIR
{
	internal class VectorImageManager : IDisposable
	{
		public Texture2D atlas
		{
			get
			{
				GradientSettingsAtlas gradientSettingsAtlas = this.m_GradientSettingsAtlas;
				return (gradientSettingsAtlas != null) ? gradientSettingsAtlas.atlas : null;
			}
		}

		public VectorImageManager(AtlasBase atlas)
		{
			VectorImageManager.instances.Add(this);
			this.m_Atlas = atlas;
			this.m_Registered = new Dictionary<VectorImage, VectorImageRenderInfo>(32);
			this.m_RenderInfoPool = new VectorImageRenderInfoPool();
			this.m_GradientRemapPool = new GradientRemapPool();
			this.m_GradientSettingsAtlas = new GradientSettingsAtlas(4096);
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
					this.m_Registered.Clear();
					this.m_RenderInfoPool.Clear();
					this.m_GradientRemapPool.Clear();
					this.m_GradientSettingsAtlas.Dispose();
					VectorImageManager.instances.Remove(this);
				}
				this.disposed = true;
			}
		}

		public void Reset()
		{
			bool disposed = this.disposed;
			if (disposed)
			{
				DisposeHelper.NotifyDisposedUsed(this);
			}
			else
			{
				this.m_Registered.Clear();
				this.m_RenderInfoPool.Clear();
				this.m_GradientRemapPool.Clear();
				this.m_GradientSettingsAtlas.Reset();
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
				this.m_GradientSettingsAtlas.Commit();
			}
		}

		public GradientRemap AddUser(VectorImage vi, VisualElement context)
		{
			bool disposed = this.disposed;
			GradientRemap gradientRemap;
			if (disposed)
			{
				DisposeHelper.NotifyDisposedUsed(this);
				gradientRemap = null;
			}
			else
			{
				bool flag = vi == null;
				if (flag)
				{
					gradientRemap = null;
				}
				else
				{
					VectorImageRenderInfo vectorImageRenderInfo;
					bool flag2 = this.m_Registered.TryGetValue(vi, out vectorImageRenderInfo);
					if (flag2)
					{
						vectorImageRenderInfo.useCount++;
					}
					else
					{
						vectorImageRenderInfo = this.Register(vi, context);
					}
					gradientRemap = vectorImageRenderInfo.firstGradientRemap;
				}
			}
			return gradientRemap;
		}

		public void RemoveUser(VectorImage vi)
		{
			bool disposed = this.disposed;
			if (disposed)
			{
				DisposeHelper.NotifyDisposedUsed(this);
			}
			else
			{
				bool flag = vi == null;
				if (!flag)
				{
					VectorImageRenderInfo vectorImageRenderInfo;
					bool flag2 = this.m_Registered.TryGetValue(vi, out vectorImageRenderInfo);
					if (flag2)
					{
						vectorImageRenderInfo.useCount--;
						bool flag3 = vectorImageRenderInfo.useCount == 0;
						if (flag3)
						{
							this.Unregister(vi, vectorImageRenderInfo);
						}
					}
				}
			}
		}

		private VectorImageRenderInfo Register(VectorImage vi, VisualElement context)
		{
			VectorImageRenderInfo vectorImageRenderInfo = this.m_RenderInfoPool.Get();
			vectorImageRenderInfo.useCount = 1;
			this.m_Registered[vi] = vectorImageRenderInfo;
			GradientSettings[] settings = vi.settings;
			bool flag = settings != null && settings.Length != 0;
			if (flag)
			{
				int num = vi.settings.Length;
				Alloc alloc = this.m_GradientSettingsAtlas.Add(num);
				bool flag2 = alloc.size > 0U;
				if (flag2)
				{
					TextureId textureId;
					RectInt rectInt;
					bool flag3 = this.m_Atlas.TryGetAtlas(context, vi.atlas, out textureId, out rectInt);
					if (flag3)
					{
						GradientRemap gradientRemap = null;
						for (int i = 0; i < num; i++)
						{
							GradientRemap gradientRemap2 = this.m_GradientRemapPool.Get();
							bool flag4 = i > 0;
							if (flag4)
							{
								gradientRemap.next = gradientRemap2;
							}
							else
							{
								vectorImageRenderInfo.firstGradientRemap = gradientRemap2;
							}
							gradientRemap = gradientRemap2;
							gradientRemap2.origIndex = i;
							gradientRemap2.destIndex = (int)(alloc.start + (uint)i);
							GradientSettings gradientSettings = vi.settings[i];
							RectInt location = gradientSettings.location;
							location.x += rectInt.x;
							location.y += rectInt.y;
							gradientRemap2.location = location;
							gradientRemap2.atlas = textureId;
							gradientRemap2.next = null;
						}
						this.m_GradientSettingsAtlas.Write(alloc, vi.settings, vectorImageRenderInfo.firstGradientRemap);
					}
					else
					{
						GradientRemap gradientRemap3 = null;
						for (int j = 0; j < num; j++)
						{
							GradientRemap gradientRemap4 = this.m_GradientRemapPool.Get();
							bool flag5 = j > 0;
							if (flag5)
							{
								gradientRemap3.next = gradientRemap4;
							}
							else
							{
								vectorImageRenderInfo.firstGradientRemap = gradientRemap4;
							}
							gradientRemap3 = gradientRemap4;
							gradientRemap4.origIndex = j;
							gradientRemap4.destIndex = (int)(alloc.start + (uint)j);
							gradientRemap4.atlas = TextureId.invalid;
							gradientRemap4.next = null;
						}
						this.m_GradientSettingsAtlas.Write(alloc, vi.settings, null);
					}
					vectorImageRenderInfo.gradientSettingsAlloc = alloc;
				}
				else
				{
					bool flag6 = !this.m_LoggedExhaustedSettingsAtlas;
					if (flag6)
					{
						string text = "Exhausted max gradient settings (";
						string text2 = this.m_GradientSettingsAtlas.length.ToString();
						string text3 = ") for atlas: ";
						Texture2D atlas = this.m_GradientSettingsAtlas.atlas;
						Debug.LogError(text + text2 + text3 + ((atlas != null) ? atlas.name : null));
						this.m_LoggedExhaustedSettingsAtlas = true;
					}
				}
			}
			return vectorImageRenderInfo;
		}

		private void Unregister(VectorImage vi, VectorImageRenderInfo renderInfo)
		{
			GradientRemap gradientRemap = renderInfo.firstGradientRemap;
			bool flag = renderInfo.gradientSettingsAlloc.size > 0U;
			if (flag)
			{
				this.m_GradientSettingsAtlas.Remove(renderInfo.gradientSettingsAlloc);
				this.m_Atlas.ReturnAtlas(null, vi.atlas, gradientRemap.atlas);
			}
			while (gradientRemap != null)
			{
				GradientRemap next = gradientRemap.next;
				this.m_GradientRemapPool.Return(gradientRemap);
				gradientRemap = next;
			}
			this.m_Registered.Remove(vi);
			this.m_RenderInfoPool.Return(renderInfo);
		}

		public static List<VectorImageManager> instances = new List<VectorImageManager>(16);

		private static ProfilerMarker s_MarkerRegister = new ProfilerMarker("UIR.VectorImageManager.Register");

		private static ProfilerMarker s_MarkerUnregister = new ProfilerMarker("UIR.VectorImageManager.Unregister");

		private readonly AtlasBase m_Atlas;

		private Dictionary<VectorImage, VectorImageRenderInfo> m_Registered;

		private VectorImageRenderInfoPool m_RenderInfoPool;

		private GradientRemapPool m_GradientRemapPool;

		private GradientSettingsAtlas m_GradientSettingsAtlas;

		private bool m_LoggedExhaustedSettingsAtlas;
	}
}
