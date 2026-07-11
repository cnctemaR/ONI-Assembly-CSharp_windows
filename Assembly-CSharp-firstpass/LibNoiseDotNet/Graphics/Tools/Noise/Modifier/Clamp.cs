using System;

namespace LibNoiseDotNet.Graphics.Tools.Noise.Modifier
{
	public class Clamp : ModifierModule, IModule3D, IModule
	{
		public float LowerBound
		{
			get
			{
				return this._lowerBound;
			}
			set
			{
				this._lowerBound = value;
			}
		}

		public float UpperBound
		{
			get
			{
				return this._upperBound;
			}
			set
			{
				this._upperBound = value;
			}
		}

		public Clamp()
		{
		}

		public Clamp(IModule source)
			: base(source)
		{
		}

		public Clamp(IModule source, float lower, float upper)
			: base(source)
		{
			this._lowerBound = lower;
			this._upperBound = upper;
		}

		public float GetValue(float x, float y, float z)
		{
			float value = ((IModule3D)this._sourceModule).GetValue(x, y, z);
			if (value < this._lowerBound)
			{
				return this._lowerBound;
			}
			if (value > this._upperBound)
			{
				return this._upperBound;
			}
			return value;
		}

		public const float DEFAULT_LOWER_BOUND = -1f;

		public const float DEFAULT_UPPER_BOUND = 1f;

		protected float _lowerBound = -1f;

		protected float _upperBound = 1f;
	}
}
