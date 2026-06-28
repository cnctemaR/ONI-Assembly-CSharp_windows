using System;

namespace LibNoiseDotNet.Graphics.Tools.Noise.Model
{
	public class Cylinder : AbstractModel
	{
		public Cylinder()
		{
		}

		public Cylinder(IModule3D module)
			: base(module)
		{
		}

		public float GetValue(float angle, float height)
		{
			float num = (float)Math.Cos((double)(angle * 0.017453292f));
			float num2 = (float)Math.Sin((double)(angle * 0.017453292f));
			return ((IModule3D)this._sourceModule).GetValue(num, height, num2);
		}
	}
}
