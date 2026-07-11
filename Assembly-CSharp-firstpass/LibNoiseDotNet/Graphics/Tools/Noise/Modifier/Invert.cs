using System;

namespace LibNoiseDotNet.Graphics.Tools.Noise.Modifier
{
	public class Invert : ModifierModule, IModule3D, IModule
	{
		public Invert()
		{
		}

		public Invert(IModule source)
			: base(source)
		{
		}

		public float GetValue(float x, float y, float z)
		{
			return -((IModule3D)this._sourceModule).GetValue(x, y, z);
		}
	}
}
