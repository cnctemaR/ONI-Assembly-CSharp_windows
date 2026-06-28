using System;
using LibNoiseDotNet.Graphics.Tools.Noise;
using LibNoiseDotNet.Graphics.Tools.Noise.Filter;

namespace Klei.Noise
{
	public class Filter : NoiseBase
	{
		public Filter()
		{
			this.filter = NoiseFilter.RidgedMultiFractal;
			this.frequency = 10f;
			this.lacunarity = 3f;
			this.octaves = 10;
			this.offset = 1f;
			this.gain = 0f;
			this.exponent = 0f;
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

		public override Type GetObjectType()
		{
			return typeof(Filter);
		}

		public NoiseFilter filter { get; set; }

		public float frequency { get; set; }

		public float lacunarity { get; set; }

		public int octaves { get; set; }

		public float offset { get; set; }

		public float gain { get; set; }

		public float exponent { get; set; }

		public IModule3D CreateModule()
		{
			FilterModule filterModule = null;
			NoiseFilter filter = this.filter;
			switch (filter)
			{
			case NoiseFilter.Billow:
				filterModule = new Billow();
				break;
			case NoiseFilter.MultiFractal:
				filterModule = new MultiFractal();
				break;
			case NoiseFilter.HeterogeneousMultiFractal:
				filterModule = new HeterogeneousMultiFractal();
				break;
			case NoiseFilter.HybridMultiFractal:
				filterModule = new HybridMultiFractal();
				break;
			case NoiseFilter.RidgedMultiFractal:
				filterModule = new RidgedMultiFractal();
				break;
			default:
				switch (filter)
				{
				case NoiseFilter.Pipe:
					filterModule = new Pipe();
					break;
				case NoiseFilter.SumFractal:
					filterModule = new SumFractal();
					break;
				case NoiseFilter.SinFractal:
					filterModule = new SinFractal();
					break;
				}
				break;
			case NoiseFilter.Voronoi:
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
			}
			return (IModule3D)filterModule;
		}

		public void SetSouces(IModule3D target, IModule3D sourceModule)
		{
			(target as FilterModule).Primitive3D = sourceModule;
		}
	}
}
