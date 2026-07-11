using System;

namespace LibNoiseDotNet.Graphics.Tools.Noise.Tranformer
{
	public class Turbulence : TransformerModule, IModule3D, IModule
	{
		public IModule SourceModule
		{
			get
			{
				return this._sourceModule;
			}
			set
			{
				this._sourceModule = value;
			}
		}

		public IModule XDistortModule
		{
			get
			{
				return this._xDistortModule;
			}
			set
			{
				this._xDistortModule = value;
			}
		}

		public IModule YDistortModule
		{
			get
			{
				return this._yDistortModule;
			}
			set
			{
				this._yDistortModule = value;
			}
		}

		public IModule ZDistortModule
		{
			get
			{
				return this._zDistortModule;
			}
			set
			{
				this._zDistortModule = value;
			}
		}

		public float Power
		{
			get
			{
				return this._power;
			}
			set
			{
				this._power = value;
			}
		}

		public Turbulence()
		{
			this._power = 1f;
		}

		public Turbulence(IModule source)
			: this()
		{
			this._sourceModule = source;
		}

		public Turbulence(IModule source, IModule xDistortModule, IModule yDistortModule, IModule zDistortModule, float power)
		{
			this._sourceModule = source;
			this._xDistortModule = xDistortModule;
			this._yDistortModule = yDistortModule;
			this._zDistortModule = zDistortModule;
			this._power = power;
		}

		public float GetValue(float x, float y, float z)
		{
			float num = x + 0.1894226f;
			float num2 = y + 0.9937134f;
			float num3 = z + 0.47816467f;
			float num4 = x + 0.40464783f;
			float num5 = y + 0.27661133f;
			float num6 = z + 0.9230499f;
			float num7 = x + 0.821228f;
			float num8 = y + 0.1710968f;
			float num9 = z + 0.6842804f;
			float num10 = x + ((IModule3D)this._xDistortModule).GetValue(num, num2, num3) * this._power;
			float num11 = y + ((IModule3D)this._yDistortModule).GetValue(num4, num5, num6) * this._power;
			float num12 = z + ((IModule3D)this._zDistortModule).GetValue(num7, num8, num9) * this._power;
			return ((IModule3D)this._sourceModule).GetValue(num10, num11, num12);
		}

		public const float DEFAULT_POWER = 1f;

		protected float _power = 1f;

		protected IModule _sourceModule;

		protected IModule _xDistortModule;

		protected IModule _yDistortModule;

		protected IModule _zDistortModule;
	}
}
