using System;

namespace LibNoiseDotNet.Graphics.Tools.Noise.Primitive
{
	public class Constant : PrimitiveModule, IModule4D, IModule3D, IModule2D, IModule1D, IModule
	{
		public Constant()
			: this(0.5f)
		{
		}

		public Constant(float value)
		{
			this._constant = value;
		}

		public float ConstantValue
		{
			get
			{
				return this._constant;
			}
			set
			{
				this._constant = value;
			}
		}

		public float GetValue(float x, float y, float z, float t)
		{
			return this._constant;
		}

		public float GetValue(float x, float y, float z)
		{
			return this._constant;
		}

		public float GetValue(float x, float y)
		{
			return this._constant;
		}

		public float GetValue(float x)
		{
			return this._constant;
		}

		public const float DEFAULT_VALUE = 0.5f;

		protected float _constant = 0.5f;
	}
}
