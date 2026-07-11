using System;

namespace LibNoiseDotNet.Graphics.Tools.Noise.Renderer
{
	public class ImageRenderer : AbstractImageRenderer
	{
		public ImageRenderer()
		{
			this._lightEnabled = false;
			this._WrapEnabled = false;
			this._lightAzimuth = 45f;
			this._lightBrightness = 1f;
			this._lightContrast = 1f;
			this._lightElevation = 45f;
			this._lightIntensity = 1f;
			this._lightColor = Color.WHITE;
			this._recalcLightValues = true;
		}

		public bool LightEnabled
		{
			get
			{
				return this._lightEnabled;
			}
			set
			{
				this._lightEnabled = value;
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

		public Image BackgroundImage
		{
			get
			{
				return this._backgroundImage;
			}
			set
			{
				this._backgroundImage = value;
			}
		}

		public GradientColor Gradient
		{
			get
			{
				return this._gradient;
			}
			set
			{
				this._gradient = value;
			}
		}

		public float LightAzimuth
		{
			get
			{
				return this._lightAzimuth;
			}
			set
			{
				this._lightAzimuth = value;
				this._recalcLightValues = true;
			}
		}

		public float LightBrightness
		{
			get
			{
				return this._lightBrightness;
			}
			set
			{
				this._lightBrightness = value;
				this._recalcLightValues = true;
			}
		}

		public float LightContrast
		{
			get
			{
				return this._lightContrast;
			}
			set
			{
				if (value <= 0f)
				{
					throw new ArgumentException("Contrast must be greater than 0");
				}
				this._lightContrast = value;
				this._recalcLightValues = true;
			}
		}

		public float LightElevation
		{
			get
			{
				return this._lightElevation;
			}
			set
			{
				this._lightElevation = value;
				this._recalcLightValues = true;
			}
		}

		public float LightIntensity
		{
			get
			{
				return this._lightIntensity;
			}
			set
			{
				if (value < 0f)
				{
					throw new ArgumentException("Intensity must be greater or equals to 0");
				}
				this._lightIntensity = value;
				this._recalcLightValues = true;
			}
		}

		public Color LightColor
		{
			get
			{
				return this._lightColor;
			}
			set
			{
				this._lightColor = value;
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
			if (this._gradient.CountGradientPoints() < 2)
			{
				throw new ArgumentException("Not enought points in the gradient");
			}
			int width = this._noiseMap.Width;
			int height = this._noiseMap.Height;
			int num = width - 1;
			int num2 = height - 1;
			int num3 = -num;
			int num4 = -num2;
			if (this._backgroundImage != null && (this._backgroundImage.Width != width || this._backgroundImage.Height != height))
			{
				throw new ArgumentException("Incoherent background image size");
			}
			if (!this._image.Equals(this._backgroundImage))
			{
				this._image.SetSize(width, height);
			}
			IColor color = Color.WHITE;
			for (int i = 0; i < height; i++)
			{
				for (int j = 0; j < width; j++)
				{
					float value = this._noiseMap.GetValue(j, i);
					IColor color2 = this._gradient.GetColor(value);
					float num9;
					if (this._lightEnabled)
					{
						int num5;
						int num6;
						int num7;
						int num8;
						if (this._WrapEnabled)
						{
							if (j == 0)
							{
								num5 = num;
								num6 = 1;
							}
							else if (j == num)
							{
								num5 = -1;
								num6 = num3;
							}
							else
							{
								num5 = -1;
								num6 = 1;
							}
							if (i == 0)
							{
								num7 = num2;
								num8 = 1;
							}
							else if (i == num2)
							{
								num7 = -1;
								num8 = num4;
							}
							else
							{
								num7 = -1;
								num8 = 1;
							}
						}
						else
						{
							if (j == 0)
							{
								num5 = 0;
								num6 = 1;
							}
							else if (j == num)
							{
								num5 = -1;
								num6 = 0;
							}
							else
							{
								num5 = -1;
								num6 = 1;
							}
							if (i == 0)
							{
								num7 = 0;
								num8 = 1;
							}
							else if (i == num2)
							{
								num7 = -1;
								num8 = 0;
							}
							else
							{
								num7 = -1;
								num8 = 1;
							}
						}
						float value2 = this._noiseMap.GetValue(j, i);
						float value3 = this._noiseMap.GetValue(j + num5, i);
						float value4 = this._noiseMap.GetValue(j + num6, i);
						float value5 = this._noiseMap.GetValue(j, i + num7);
						float value6 = this._noiseMap.GetValue(j, i + num8);
						num9 = this.CalcLightIntensity(value2, value3, value4, value5, value6);
						num9 *= this._lightBrightness;
					}
					else
					{
						num9 = 1f;
					}
					if (this._backgroundImage != null)
					{
						color = this._backgroundImage.GetValue(j, i);
					}
					this._image.SetValue(j, i, this.CalcDestColor(color2, color, num9));
				}
				if (this._callBack != null)
				{
					this._callBack(i);
				}
			}
		}

		private IColor CalcDestColor(IColor sourceColor, IColor backgroundColor, float lightValue)
		{
			float num = (float)sourceColor.Red / 255f;
			float num2 = (float)sourceColor.Green / 255f;
			float num3 = (float)sourceColor.Blue / 255f;
			float num4 = (float)sourceColor.Alpha / 255f;
			float num5 = (float)backgroundColor.Red / 255f;
			float num6 = (float)backgroundColor.Green / 255f;
			float num7 = (float)backgroundColor.Blue / 255f;
			float num8 = Libnoise.Lerp(num5, num, num4);
			float num9 = Libnoise.Lerp(num6, num2, num4);
			float num10 = Libnoise.Lerp(num7, num3, num4);
			if (this._lightEnabled)
			{
				float num11 = lightValue * (float)this._lightColor.Red / 255f;
				float num12 = lightValue * (float)this._lightColor.Green / 255f;
				float num13 = lightValue * (float)this._lightColor.Blue / 255f;
				num8 *= num11;
				num9 *= num12;
				num10 *= num13;
			}
			num8 = Libnoise.Clamp01(num8);
			num9 = Libnoise.Clamp01(num9);
			num10 = Libnoise.Clamp01(num10);
			return new Color((byte)((uint)(num8 * 255f) & 255U), (byte)((uint)(num9 * 255f) & 255U), (byte)((uint)(num10 * 255f) & 255U), Math.Max(sourceColor.Alpha, backgroundColor.Alpha));
		}

		private float CalcLightIntensity(float center, float left, float right, float down, float up)
		{
			if (this._recalcLightValues)
			{
				this._cosAzimuth = (float)Math.Cos((double)(this._lightAzimuth * 0.017453292f));
				this._sinAzimuth = (float)Math.Sin((double)(this._lightAzimuth * 0.017453292f));
				this._cosElevation = (float)Math.Cos((double)(this._lightElevation * 0.017453292f));
				this._sinElevation = (float)Math.Sin((double)(this._lightElevation * 0.017453292f));
				this._recalcLightValues = false;
			}
			float num = 1.4142135f * this._sinElevation / 2f;
			float num2 = (1f - num) * this._lightContrast * 1.4142135f * this._cosElevation * this._cosAzimuth;
			float num3 = (1f - num) * this._lightContrast * 1.4142135f * this._cosElevation * this._sinAzimuth;
			float num4 = num2 * (left - right) + num3 * (down - up) + num;
			if ((double)num4 < 0.0)
			{
				num4 = 0f;
			}
			return num4;
		}

		protected Image _backgroundImage;

		protected GradientColor _gradient;

		private bool _lightEnabled;

		private bool _WrapEnabled;

		private float _lightAzimuth;

		private float _lightBrightness;

		private float _lightContrast;

		private float _lightElevation;

		private float _lightIntensity;

		private Color _lightColor;

		private bool _recalcLightValues;

		private float _cosAzimuth;

		private float _cosElevation;

		private float _sinAzimuth;

		private float _sinElevation;
	}
}
