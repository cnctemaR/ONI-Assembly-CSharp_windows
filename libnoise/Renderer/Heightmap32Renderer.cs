using System;

namespace LibNoiseDotNet.Graphics.Tools.Noise.Renderer
{
	public class Heightmap32Renderer : AbstractHeightmapRenderer
	{
		public Heightmap32 Heightmap
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
			float num;
			if (source <= this._lowerHeightBound)
			{
				num = this._lowerHeightBound;
			}
			else if (source >= this._upperHeightBound)
			{
				num = this._upperHeightBound;
			}
			else
			{
				num = source;
			}
			this._heightmap.SetValue(x, y, num);
		}

		protected Heightmap32 _heightmap;
	}
}
