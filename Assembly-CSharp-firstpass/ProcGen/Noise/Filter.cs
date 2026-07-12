using System;
using LibNoiseDotNet.Graphics.Tools.Noise;
using LibNoiseDotNet.Graphics.Tools.Noise.Filter;

namespace ProcGen.Noise
{
	public class Filter : NoiseBase
	{
		public override Type GetObjectType()
		{
			return typeof(Filter);
		}

		public Filter.NoiseFilter filter { get; set; }

		public float frequency { get; set; }

		public float lacunarity { get; set; }

		public int octaves { get; set; }

		public float offset { get; set; }

		public float gain { get; set; }

		public float exponent { get; set; }

		public float scale { get; set; }

		public float bias { get; set; }

		public Filter()
		{
			this.filter = Filter.NoiseFilter.RidgedMultiFractal;
			this.frequency = 0.1f;
			this.lacunarity = 3f;
			this.octaves = 0;
			this.offset = 1f;
			this.gain = 1f;
			this.exponent = 0.9f;
			this.scale = 1f;
			this.bias = 0f;
		}

		public Filter(Filter src)
		{
			this.filter = src.filter;
			this.frequency = src.frequency;
			this.lacunarity = src.lacunarity;
			this.octaves = src.octaves;
			this.offset = src.offset;
			this.gain = src.gain;
			this.exponent = src.exponent;
		}

		public IModule3D CreateModule()
		{
			FilterModule filterModule = null;
			switch (this.filter)
			{
			case Filter.NoiseFilter.Pipe:
				filterModule = new Pipe();
				break;
			case Filter.NoiseFilter.SumFractal:
				filterModule = new SumFractal();
				break;
			case Filter.NoiseFilter.SinFractal:
				filterModule = new SinFractal();
				break;
			case Filter.NoiseFilter.Billow:
				filterModule = new Billow();
				break;
			case Filter.NoiseFilter.MultiFractal:
				filterModule = new MultiFractal();
				break;
			case Filter.NoiseFilter.HeterogeneousMultiFractal:
				filterModule = new HeterogeneousMultiFractal();
				break;
			case Filter.NoiseFilter.HybridMultiFractal:
				filterModule = new HybridMultiFractal();
				break;
			case Filter.NoiseFilter.RidgedMultiFractal:
				filterModule = new RidgedMultiFractal();
				break;
			case Filter.NoiseFilter.Voronoi:
				filterModule = new Voronoi();
				break;
			}
			if (filterModule != null)
			{
				filterModule.Frequency = this.frequency;
				filterModule.Lacunarity = this.lacunarity;
				filterModule.OctaveCount = (float)this.octaves;
				filterModule.Offset = this.offset;
				filterModule.Gain = this.gain;
				filterModule.SpectralExponent = this.exponent;
				if (this.filter == Filter.NoiseFilter.Billow)
				{
					Billow billow = (Billow)filterModule;
					billow.Scale = this.scale;
					billow.Bias = this.bias;
				}
			}
			return (IModule3D)filterModule;
		}

		public void SetSouces(IModule3D target, IModule3D sourceModule)
		{
			(target as FilterModule).Primitive3D = sourceModule;
		}

		public enum NoiseFilter
		{
			_UNSET_,
			Pipe,
			SumFractal,
			SinFractal,
			Billow,
			MultiFractal,
			HeterogeneousMultiFractal,
			HybridMultiFractal,
			RidgedMultiFractal,
			Voronoi
		}
	}
}
