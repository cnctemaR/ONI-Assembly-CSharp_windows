using System;
using LibNoiseDotNet.Graphics.Tools.Noise.Model;

namespace LibNoiseDotNet.Graphics.Tools.Noise.Builder
{
	public class NoiseMapBuilderSphere : NoiseMapBuilder
	{
		public float EastLonBound
		{
			get
			{
				return this._eastLonBound;
			}
		}

		public float NorthLatBound
		{
			get
			{
				return this._northLatBound;
			}
		}

		public float SouthLatBound
		{
			get
			{
				return this._southLatBound;
			}
		}

		public float WestLonBound
		{
			get
			{
				return this._westLonBound;
			}
		}

		public NoiseMapBuilderSphere()
		{
			this.SetBounds(-90f, 90f, -180f, 180f);
		}

		public void SetBounds(float southLatBound, float northLatBound, float westLonBound, float eastLonBound)
		{
			if (southLatBound >= northLatBound || westLonBound >= eastLonBound)
			{
				throw new ArgumentException("Incoherent bounds : southLatBound >= northLatBound or westLonBound >= eastLonBound");
			}
			this._southLatBound = southLatBound;
			this._northLatBound = northLatBound;
			this._westLonBound = westLonBound;
			this._eastLonBound = eastLonBound;
		}

		public override void Build()
		{
			if (this._southLatBound >= this._northLatBound || this._westLonBound >= this._eastLonBound)
			{
				throw new ArgumentException("Incoherent bounds : southLatBound >= northLatBound or westLonBound >= eastLonBound");
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
			Sphere sphere = new Sphere((IModule3D)this._sourceModule);
			float num = this._eastLonBound - this._westLonBound;
			float num2 = this._northLatBound - this._southLatBound;
			float num3 = num / (float)this._width;
			float num4 = num2 / (float)this._height;
			float num5 = this._westLonBound;
			float num6 = this._southLatBound;
			for (int i = 0; i < this._height; i++)
			{
				num5 = this._westLonBound;
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
						num7 = sphere.GetValue(num6, num5);
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

		private float _eastLonBound;

		private float _northLatBound;

		private float _southLatBound;

		private float _westLonBound;
	}
}
