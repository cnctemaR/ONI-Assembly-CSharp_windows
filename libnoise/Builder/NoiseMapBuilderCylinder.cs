using System;
using LibNoiseDotNet.Graphics.Tools.Noise.Model;

namespace LibNoiseDotNet.Graphics.Tools.Noise.Builder
{
	public class NoiseMapBuilderCylinder : NoiseMapBuilder
	{
		public float LowerHeightBound
		{
			get
			{
				return this._lowerHeightBound;
			}
		}

		public float LowerAngleBound
		{
			get
			{
				return this._lowerAngleBound;
			}
		}

		public float UpperAngleBound
		{
			get
			{
				return this._upperAngleBound;
			}
		}

		public float UpperHeightBound
		{
			get
			{
				return this._upperHeightBound;
			}
		}

		public NoiseMapBuilderCylinder()
		{
			this.SetBounds(-180f, 180f, -10f, 10f);
		}

		public void SetBounds(float lowerAngleBound, float upperAngleBound, float lowerHeightBound, float upperHeightBound)
		{
			if (lowerAngleBound >= upperAngleBound || lowerHeightBound >= upperHeightBound)
			{
				throw new ArgumentException("Incoherent bounds : lowerAngleBound >= upperAngleBound or lowerZBound >= upperHeightBound");
			}
			this._lowerAngleBound = lowerAngleBound;
			this._upperAngleBound = upperAngleBound;
			this._lowerHeightBound = lowerHeightBound;
			this._upperHeightBound = upperHeightBound;
		}

		public override void Build()
		{
			if (this._lowerAngleBound >= this._upperAngleBound || this._lowerHeightBound >= this._upperHeightBound)
			{
				throw new ArgumentException("Incoherent bounds : lowerAngleBound >= upperAngleBound or lowerZBound >= upperHeightBound");
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
			Cylinder cylinder = new Cylinder((IModule3D)this._sourceModule);
			float num = this._upperAngleBound - this._lowerAngleBound;
			float num2 = this._upperHeightBound - this._lowerHeightBound;
			float num3 = num / (float)this._width;
			float num4 = num2 / (float)this._height;
			float num5 = this._lowerAngleBound;
			float num6 = this._lowerHeightBound;
			for (int i = 0; i < this._height; i++)
			{
				num5 = this._lowerAngleBound;
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
						num7 = cylinder.GetValue(num5, num6);
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

		private float _lowerAngleBound;

		private float _lowerHeightBound;

		private float _upperAngleBound;

		private float _upperHeightBound;
	}
}
