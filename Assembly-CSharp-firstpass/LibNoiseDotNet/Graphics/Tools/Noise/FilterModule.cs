using System;

namespace LibNoiseDotNet.Graphics.Tools.Noise
{
	public abstract class FilterModule : IModule
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

		public float Lacunarity
		{
			get
			{
				return this._lacunarity;
			}
			set
			{
				this._lacunarity = value;
				this.ComputeSpectralWeights();
			}
		}

		public float OctaveCount
		{
			get
			{
				return this._octaveCount;
			}
			set
			{
				this._octaveCount = Libnoise.Clamp(value, 1f, 30f);
			}
		}

		public float Offset
		{
			get
			{
				return this._offset;
			}
			set
			{
				this._offset = value;
			}
		}

		public float Gain
		{
			get
			{
				return this._gain;
			}
			set
			{
				this._gain = value;
			}
		}

		public float SpectralExponent
		{
			get
			{
				return this._spectralExponent;
			}
			set
			{
				this._spectralExponent = value;
				this.ComputeSpectralWeights();
			}
		}

		public IModule4D Primitive4D
		{
			get
			{
				return this._source4D;
			}
			set
			{
				this._source4D = value;
			}
		}

		public IModule3D Primitive3D
		{
			get
			{
				return this._source3D;
			}
			set
			{
				this._source3D = value;
			}
		}

		public IModule2D Primitive2D
		{
			get
			{
				return this._source2D;
			}
			set
			{
				this._source2D = value;
			}
		}

		public IModule1D Primitive1D
		{
			get
			{
				return this._source1D;
			}
			set
			{
				this._source1D = value;
			}
		}

		protected FilterModule()
			: this(1f, 2f, 0.9f, 6f)
		{
		}

		protected FilterModule(float frequency, float lacunarity, float exponent, float octaveCount)
		{
			this._frequency = frequency;
			this._lacunarity = lacunarity;
			this._spectralExponent = exponent;
			this._octaveCount = octaveCount;
			this.ComputeSpectralWeights();
		}

		protected void ComputeSpectralWeights()
		{
			for (int i = 0; i < 30; i++)
			{
				this._spectralWeights[i] = (float)Math.Pow((double)this._lacunarity, (double)((float)(-(float)i) * this._spectralExponent));
			}
		}

		public const float DEFAULT_FREQUENCY = 1f;

		public const float DEFAULT_LACUNARITY = 2f;

		public const float DEFAULT_OCTAVE_COUNT = 6f;

		public const int MAX_OCTAVE = 30;

		public const float DEFAULT_OFFSET = 1f;

		public const float DEFAULT_GAIN = 2f;

		public const float DEFAULT_SPECTRAL_EXPONENT = 0.9f;

		protected float _frequency = 1f;

		protected float _lacunarity = 2f;

		protected float _octaveCount = 6f;

		protected float[] _spectralWeights = new float[30];

		protected float _offset = 1f;

		protected float _gain = 2f;

		protected float _spectralExponent = 0.9f;

		protected IModule4D _source4D;

		protected IModule3D _source3D;

		protected IModule2D _source2D;

		protected IModule1D _source1D;
	}
}
