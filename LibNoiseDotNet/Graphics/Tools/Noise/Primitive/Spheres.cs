using System;

namespace LibNoiseDotNet.Graphics.Tools.Noise.Primitive
{
	public class Spheres : PrimitiveModule, IModule3D, IModule
	{
		public float Frequency
		{
			get
			{
				return this._frequency;
			}
			set
			{
				this._frequency = value;
			}
		}

		public Spheres()
			: this(1f)
		{
		}

		public Spheres(float frequency)
		{
			this._frequency = frequency;
		}

		public float GetValue(float x, float y, float z)
		{
			x *= this._frequency;
			y *= this._frequency;
			z *= this._frequency;
			float num = (float)Math.Sqrt((double)(x * x + y * y + z * z));
			float num2 = num - (float)Math.Floor((double)num);
			float num3 = 1f - num2;
			float num4 = Math.Min(num2, num3);
			return 1f - num4 * 4f;
		}

		public const float DEFAULT_FREQUENCY = 1f;

		protected float _frequency = 1f;
	}
}
