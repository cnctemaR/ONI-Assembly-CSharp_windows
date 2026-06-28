using System;

namespace LibNoiseDotNet.Graphics.Tools.Noise.Renderer
{
	public class Heightmap8Renderer : AbstractHeightmapRenderer
	{
		public Heightmap8 Heightmap
		{
			get
			{
				return this._heightmap;
			}
			set
			{
				this._heightmap = value;
			}
		}

		protected override void SetHeightmapSize(int width, int height)
		{
			this._heightmap.SetSize(width, height);
		}

		protected override bool CheckHeightmap()
		{
			return this._heightmap != null;
		}

		protected override void RenderHeight(int x, int y, float source, float boundDiff)
		{
			byte b;
			if (source <= this._lowerHeightBound)
			{
				b = 0;
			}
			else if (source >= this._upperHeightBound)
			{
				b = byte.MaxValue;
			}
			else
			{
				b = (byte)((source - this._lowerHeightBound) / boundDiff * 255f);
			}
			this._heightmap.SetValue(x, y, b);
		}

		protected Heightmap8 _heightmap;
	}
}
