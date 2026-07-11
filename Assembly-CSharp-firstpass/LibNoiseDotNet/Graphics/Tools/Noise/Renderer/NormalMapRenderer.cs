using System;

namespace LibNoiseDotNet.Graphics.Tools.Noise.Renderer
{
	public class NormalMapRenderer : AbstractImageRenderer
	{
		public NormalMapRenderer()
		{
			this._WrapEnabled = false;
			this._bumpHeight = 1f;
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

		public float BumpHeight
		{
			get
			{
				return this._bumpHeight;
			}
			set
			{
				this._bumpHeight = value;
			}
		}

		public override void Render()
		{
			if (this._noiseMap == null)
			{
				throw new ArgumentException("A noise map must be provided");
			}
			if (this._image == null)
			{
				throw new ArgumentException("An image map must be provided");
			}
			if (this._noiseMap.Width <= 0 || this._noiseMap.Height <= 0)
			{
				throw new ArgumentException("Incoherent noise map size (0,0)");
			}
			int width = this._noiseMap.Width;
			int height = this._noiseMap.Height;
			int num = width - 1;
			int num2 = height - 1;
			int num3 = -num;
			int num4 = -num2;
			this._image.SetSize(width, height);
			for (int i = 0; i < height; i++)
			{
				for (int j = 0; j < width; j++)
				{
					int num5;
					int num6;
					if (this._WrapEnabled)
					{
						if (j == num)
						{
							num5 = num3;
						}
						else
						{
							num5 = 1;
						}
						if (i == num2)
						{
							num6 = num4;
						}
						else
						{
							num6 = 1;
						}
					}
					else
					{
						if (j == num)
						{
							num5 = 0;
						}
						else
						{
							num5 = 1;
						}
						if (i == num2)
						{
							num6 = 0;
						}
						else
						{
							num6 = 1;
						}
					}
					float value = this._noiseMap.GetValue(j, i);
					float value2 = this._noiseMap.GetValue(j + num5, i);
					float value3 = this._noiseMap.GetValue(j, i + num6);
					this._image.SetValue(j, i, this.CalcNormalColor(value, value2, value3, this._bumpHeight));
				}
				if (this._callBack != null)
				{
					this._callBack(i);
				}
			}
		}

		private IColor CalcNormalColor(float nc, float nr, float nu, float bumpHeight)
		{
			nc *= bumpHeight;
			nr *= bumpHeight;
			nu *= bumpHeight;
			float num = nc - nr;
			float num2 = nc - nu;
			float num3 = (float)Math.Sqrt((double)(num2 * num2 + num * num + 1f));
			float num4 = (nc - nr) / num3;
			float num5 = (nc - nu) / num3;
			float num6 = 1f / num3;
			byte b = (byte)(Libnoise.FastFloor((num4 + 1f) * 127.5f) & 255);
			byte b2 = (byte)(Libnoise.FastFloor((num5 + 1f) * 127.5f) & 255);
			byte b3 = (byte)(Libnoise.FastFloor((num6 + 1f) * 127.5f) & 255);
			return new Color(b, b2, b3, byte.MaxValue);
		}

		protected bool _WrapEnabled;

		protected float _bumpHeight;
	}
}
