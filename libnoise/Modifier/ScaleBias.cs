using System;

namespace LibNoiseDotNet.Graphics.Tools.Noise.Modifier
{
	public class ScaleBias : ModifierModule, IModule3D, IModule
	{
		public float Scale
		{
			get
			{
				return this._scale;
			}
			set
			{
				this._scale = value;
			}
		}

		public float Bias
		{
			get
			{
				return this._bias;
			}
			set
			{
				this._bias = value;
			}
		}

		public ScaleBias()
		{
		}

		public ScaleBias(IModule source)
			: base(source)
		{
		}

		public ScaleBias(IModule source, float scale, float bias)
			: base(source)
		{
			this._scale = scale;
			this._bias = bias;
		}

		public float GetValue(float x, float y, float z)
		{
			return ((IModule3D)this._sourceModule).GetValue(x, y, z) * this._scale + this._bias;
		}

		public const float DEFAULT_SCALE = 1f;

		public const float DEFAULT_BIAS = 0f;

		protected float _scale = 1f;

		protected float _bias;
	}
}
