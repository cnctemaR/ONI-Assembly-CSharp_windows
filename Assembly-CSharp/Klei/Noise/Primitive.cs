using System;
using LibNoiseDotNet.Graphics.Tools.Noise;
using LibNoiseDotNet.Graphics.Tools.Noise.Primitive;

namespace Klei.Noise
{
	public class Primitive : NoiseBase
	{
		public Primitive()
		{
			this.primative = NoisePrimitive.ImprovedPerlin;
			this.quality = NoiseQuality.Best;
			this.seed = 0;
			this.offset = 1f;
		}

		public Primitive(Primitive src)
		{
			this.primative = src.primative;
			this.quality = src.quality;
			this.seed = src.seed;
			this.offset = src.offset;
		}

		public override Type GetObjectType()
		{
			return typeof(Primitive);
		}

		public NoisePrimitive primative { get; set; }

		public NoiseQuality quality { get; set; }

		public int seed { get; set; }

		public float offset { get; set; }

		public IModule3D CreateModule()
		{
			PrimitiveModule primitiveModule = null;
			switch (this.primative)
			{
			case NoisePrimitive.Constant:
				primitiveModule = new Constant(this.offset);
				break;
			case NoisePrimitive.Spheres:
				primitiveModule = new Spheres(this.offset);
				break;
			case NoisePrimitive.Cylinders:
				primitiveModule = new Cylinders(this.offset);
				break;
			case NoisePrimitive.BevinsValue:
				primitiveModule = new BevinsValue();
				break;
			case NoisePrimitive.BevinsGradient:
				primitiveModule = new BevinsGradient();
				break;
			case NoisePrimitive.ImprovedPerlin:
				primitiveModule = new ImprovedPerlin();
				break;
			case NoisePrimitive.SimplexPerlin:
				primitiveModule = new SimplexPerlin();
				break;
			}
			primitiveModule.Quality = this.quality;
			primitiveModule.Seed = WorldGen.GlobalWorldSeed + this.seed;
			return (IModule3D)primitiveModule;
		}
	}
}
