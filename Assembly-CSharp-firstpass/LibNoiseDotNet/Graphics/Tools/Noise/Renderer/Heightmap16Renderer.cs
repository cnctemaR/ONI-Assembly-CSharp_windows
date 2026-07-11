using System;

namespace LibNoiseDotNet.Graphics.Tools.Noise.Renderer
{
	public class Heightmap16Renderer : AbstractHeightmapRenderer
	{
		public Heightmap16 Heightmap
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
			ushort num;
			if (source <= this._lowerHeightBound)
			{
				num = 0;
			}
			else if (source >= this._upperHeightBound)
			{
				num = ushort.MaxValue;
			}
			else
			{
				num = (ushort)((source - this._lowerHeightBound) / boundDiff * 65535f);
			}
			this._heightmap.SetValue(x, y, num);
		}

		protected Heightmap16 _heightmap;
	}
}
