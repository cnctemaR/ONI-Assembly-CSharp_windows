using System;

namespace LibNoiseDotNet.Graphics.Tools.Noise
{
	public enum NoiseFilter : byte
	{
		Pipe,
		SumFractal,
		SinFractal,
		Billow = 19,
		MultiFractal,
		HeterogeneousMultiFractal,
		HybridMultiFractal,
		RidgedMultiFractal,
		Voronoi = 30
	}
}
