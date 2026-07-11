using System;

namespace LibNoiseDotNet.Graphics.Tools.Noise
{
	public enum NoisePrimitive : byte
	{
		Constant = 1,
		Spheres,
		Cylinders,
		BevinsValue,
		BevinsGradient,
		ImprovedPerlin = 7,
		SimplexPerlin
	}
}
