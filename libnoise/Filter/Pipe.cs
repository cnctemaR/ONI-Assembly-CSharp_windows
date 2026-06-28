using System;

namespace LibNoiseDotNet.Graphics.Tools.Noise.Filter
{
	public class Pipe : FilterModule, IModule4D, IModule3D, IModule2D, IModule1D, IModule
	{
		public float GetValue(float x, float y, float z, float t)
		{
			x *= this._frequency;
			y *= this._frequency;
			z *= this._frequency;
			t *= this._frequency;
			return this._source4D.GetValue(x, y, z, t);
		}

		public float GetValue(float x, float y, float z)
		{
			x *= this._frequency;
			y *= this._frequency;
			z *= this._frequency;
			return this._source3D.GetValue(x, y, z);
		}

		public float GetValue(float x, float y)
		{
			x *= this._frequency;
			y *= this._frequency;
			return this._source2D.GetValue(x, y);
		}

		public float GetValue(float x)
		{
			x *= this._frequency;
			return this._source1D.GetValue(x);
		}
	}
}
