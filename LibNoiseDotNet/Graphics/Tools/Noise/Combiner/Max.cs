using System;

namespace LibNoiseDotNet.Graphics.Tools.Noise.Combiner
{
	public class Max : CombinerModule, IModule3D, IModule
	{
		public Max()
		{
		}

		public Max(IModule left, IModule right)
			: base(left, right)
		{
		}

		public float GetValue(float x, float y, float z)
		{
			return Math.Max(((IModule3D)this._leftModule).GetValue(x, y, z), ((IModule3D)this._rightModule).GetValue(x, y, z));
		}
	}
}
