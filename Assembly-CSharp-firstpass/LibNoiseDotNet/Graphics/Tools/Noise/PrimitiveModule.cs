using System;

namespace LibNoiseDotNet.Graphics.Tools.Noise
{
	public abstract class PrimitiveModule : IModule
	{
		public PrimitiveModule()
			: this(0, NoiseQuality.Standard)
		{
		}

		public PrimitiveModule(int seed)
			: this(seed, NoiseQuality.Standard)
		{
		}

		public PrimitiveModule(int seed, NoiseQuality quality)
		{
			this._seed = seed;
			this._quality = quality;
		}

		public virtual int Seed
		{
			get
			{
				return this._seed;
			}
			set
			{
				this._seed = value;
			}
		}

		public virtual NoiseQuality Quality
		{
			get
			{
				return this._quality;
			}
			set
			{
				this._quality = value;
			}
		}

		public const int DEFAULT_SEED = 0;

		public const NoiseQuality DEFAULT_QUALITY = NoiseQuality.Standard;

		protected int _seed;

		protected NoiseQuality _quality = NoiseQuality.Standard;
	}
}
