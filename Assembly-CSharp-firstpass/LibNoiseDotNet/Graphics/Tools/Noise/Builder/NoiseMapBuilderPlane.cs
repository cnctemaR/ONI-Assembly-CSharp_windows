using System;
using LibNoiseDotNet.Graphics.Tools.Noise.Model;

namespace LibNoiseDotNet.Graphics.Tools.Noise.Builder
{
	public class NoiseMapBuilderPlane : NoiseMapBuilder
	{
		public NoiseMapBuilderPlane()
		{
			this._seamless = false;
			this._lowerXBound = (this._lowerZBound = (this._upperXBound = (this._upperZBound = 0f)));
		}

		public NoiseMapBuilderPlane(float lowerXBound, float upperXBound, float lowerZBound, float upperZBound, bool seamless)
		{
			this._seamless = seamless;
			this.SetBounds(lowerXBound, upperXBound, lowerZBound, upperZBound);
		}

		public bool Seamless
		{
			get
			{
				return this._seamless;
			}
			set
			{
				this._seamless = value;
			}
		}

		public float LowerXBound
		{
			get
			{
				return this._lowerXBound;
			}
		}

		public float LowerZBound
		{
			get
			{
				return this._lowerZBound;
			}
		}

		public float UpperXBound
		{
			get
			{
				return this._upperXBound;
			}
		}

		public float UpperZBound
		{
			get
			{
				return this._upperZBound;
			}
		}

		public void SetBounds(float lowerXBound, float upperXBound, float lowerZBound, float upperZBound)
		{
			if (lowerXBound >= upperXBound || lowerZBound >= upperZBound)
			{
				throw new ArgumentException("Incoherent bounds : lowerXBound >= upperXBound or lowerZBound >= upperZBound");
			}
			this._lowerXBound = lowerXBound;
			this._upperXBound = upperXBound;
			this._lowerZBound = lowerZBound;
			this._upperZBound = upperZBound;
		}

		public override void Build()
		{
			if (this._lowerXBound >= this._upperXBound || this._lowerZBound >= this._upperZBound)
			{
				throw new ArgumentException("Incoherent bounds : lowerXBound >= upperXBound or lowerZBound >= upperZBound");
			}
			if (this._width < 0 || this._height < 0)
			{
				throw new ArgumentException("Dimension must be greater or equal 0");
			}
			if (this._sourceModule == null)
			{
				throw new ArgumentException("A source module must be provided");
			}
			if (this._noiseMap == null)
			{
				throw new ArgumentException("A noise map must be provided");
			}
			this._noiseMap.SetSize(this._width, this._height);
			Plane plane = new Plane(this._sourceModule);
			float num = this._upperXBound - this._lowerXBound;
			float num2 = this._upperZBound - this._lowerZBound;
			float num3 = num / (float)this._width;
			float num4 = num2 / (float)this._height;
			float num5 = this._lowerXBound;
			float num6 = this._lowerZBound;
			for (int i = 0; i < this._height; i++)
			{
				num5 = this._lowerXBound;
				for (int j = 0; j < this._width; j++)
				{
					FilterLevel filterLevel = FilterLevel.Source;
					if (this._filter != null)
					{
						filterLevel = this._filter.IsFiltered(j, i);
					}
					float num7;
					if (filterLevel == FilterLevel.Constant)
					{
						num7 = this._filter.ConstantValue;
					}
					else
					{
						if (this._seamless)
						{
							float value = plane.GetValue(num5, num6);
							float value2 = plane.GetValue(num5 + num, num6);
							float value3 = plane.GetValue(num5, num6 + num2);
							float value4 = plane.GetValue(num5 + num, num6 + num2);
							float num8 = 1f - (num5 - this._lowerXBound) / num;
							float num9 = 1f - (num6 - this._lowerZBound) / num2;
							float num10 = Libnoise.Lerp(value, value2, num8);
							float num11 = Libnoise.Lerp(value3, value4, num8);
							num7 = Libnoise.Lerp(num10, num11, num9);
						}
						else
						{
							num7 = plane.GetValue(num5, num6);
						}
						if (filterLevel == FilterLevel.Filter)
						{
							num7 = this._filter.FilterValue(j, i, num7);
						}
					}
					this._noiseMap.SetValue(j, i, num7);
					num5 += num3;
				}
				num6 += num4;
				if (this._callBack != null)
				{
					this._callBack(i);
				}
			}
		}

		private bool _seamless;

		private float _lowerXBound;

		private float _lowerZBound;

		private float _upperXBound;

		private float _upperZBound;
	}
}
