using System;

namespace LibNoiseDotNet.Graphics.Tools.Noise.Modifier
{
	public class Exponent : ModifierModule, IModule3D, IModule
	{
		public float ExponentValue
		{
			get
			{
				return this._exponent;
			}
			set
			{
				this._exponent = value;
			}
		}

		public Exponent()
		{
		}

		public Exponent(IModule source)
			: base(source)
		{
		}

		public Exponent(IModule source, float exponent)
			: base(source)
		{
			this._exponent = exponent;
		}

		public float GetValue(float x, float y, float z)
		{
			return (float)Math.Pow((double)Libnoise.FastFloor((((IModule3D)this._sourceModule).GetValue(x, y, z) + 1f) / 2f), (double)this._exponent) * 2f - 1f;
		}

		public const float DEFAULT_EXPONENT = 1f;

		protected float _exponent = 1f;
	}
}
