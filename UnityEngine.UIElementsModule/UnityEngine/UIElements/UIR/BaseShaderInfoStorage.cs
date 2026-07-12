using System;
using Unity.Profiling;

namespace UnityEngine.UIElements.UIR
{
	internal abstract class BaseShaderInfoStorage : IDisposable
	{
		public abstract Texture2D texture { get; }

		public abstract bool AllocateRect(int width, int height, out RectInt uvs);

		public abstract void SetTexel(int x, int y, Color color);

		public abstract void UpdateTexture();

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
				bool flag = !disposing;
				if (flag)
				{
				}
				this.disposed = true;
			}
		}

		protected static int s_TextureCounter;

		internal static ProfilerMarker s_MarkerCopyTexture = new ProfilerMarker("UIR.ShaderInfoStorage.CopyTexture");

		internal static ProfilerMarker s_MarkerGetTextureData = new ProfilerMarker("UIR.ShaderInfoStorage.GetTextureData");

		internal static ProfilerMarker s_MarkerUpdateTexture = new ProfilerMarker("UIR.ShaderInfoStorage.UpdateTexture");
	}
}
