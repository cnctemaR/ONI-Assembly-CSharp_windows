using System;

namespace LibNoiseDotNet.Graphics.Tools.Noise.Renderer
{
	public abstract class AbstractHeightmapRenderer : AbstractRenderer
	{
		public AbstractHeightmapRenderer()
		{
			this._WrapEnabled = false;
		}

		public float LowerHeightBound
		{
			get
			{
				return this._lowerHeightBound;
			}
		}

		public float UpperHeightBound
		{
			get
			{
				return this._upperHeightBound;
			}
		}

		public bool WrapEnabled
		{
			get
			{
				return this._WrapEnabled;
			}
			set
			{
				this._WrapEnabled = value;
			}
		}

		public void SetBounds(float lowerBound, float upperBound)
		{
			if (lowerBound == upperBound || lowerBound > upperBound)
			{
				throw new ArgumentException("Incoherent bounds : lowerBound == upperBound or lowerBound > upperBound");
			}
			this._lowerHeightBound = lowerBound;
			this._upperHeightBound = upperBound;
		}

		public void ExactFit()
		{
			this._noiseMap.MinMax(out this._lowerHeightBound, out this._upperHeightBound);
		}

		public override void Render()
		{
			if (this._noiseMap == null)
			{
				throw new ArgumentException("A noise map must be provided");
			}
			if (!this.CheckHeightmap())
			{
				throw new ArgumentException("An heightmap must be provided");
			}
			if (this._noiseMap.Width <= 0 || this._noiseMap.Height <= 0)
			{
				throw new ArgumentException("Incoherent noise map size (0,0)");
			}
			if (this._lowerHeightBound == this._upperHeightBound || this._lowerHeightBound > this._upperHeightBound)
			{
				throw new ArgumentException("Incoherent bounds : lowerBound == upperBound or lowerBound > upperBound");
			}
			int width = this._noiseMap.Width;
			int height = this._noiseMap.Height;
			int num = width - 1;
			int num2 = height - 1;
			int num3 = 0;
			int num4 = 0;
			this.SetHeightmapSize(width, height);
			float num5 = this._upperHeightBound - this._lowerHeightBound;
			for (int i = 0; i < height; i++)
			{
				for (int j = 0; j < width; j++)
				{
					float num6 = this._noiseMap.GetValue(j, i);
					if (this._WrapEnabled)
					{
						int num7;
						if (j == num)
						{
							num7 = num3;
						}
						else if (j == num3)
						{
							num7 = num;
						}
						else
						{
							num7 = j;
						}
						int num8;
						if (i == num2)
						{
							num8 = num4;
						}
						else if (i == num4)
						{
							num8 = num2;
						}
						else
						{
							num8 = i;
						}
						if (num7 != j || num8 != i)
						{
							float value = this._noiseMap.GetValue(num7, num8);
							num6 = Libnoise.Lerp(num6, value, 0.5f);
						}
					}
					this.RenderHeight(j, i, num6, num5);
				}
				if (this._callBack != null)
				{
					this._callBack(i);
				}
			}
		}

		protected abstract bool CheckHeightmap();

		protected abstract void SetHeightmapSize(int width, int height);

		protected abstract void RenderHeight(int x, int y, float source, float boundDiff);

		protected float _lowerHeightBound;

		protected float _upperHeightBound;

		protected bool _WrapEnabled;
	}
}
