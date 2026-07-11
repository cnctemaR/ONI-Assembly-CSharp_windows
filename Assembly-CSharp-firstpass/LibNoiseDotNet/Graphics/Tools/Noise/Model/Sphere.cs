using System;

namespace LibNoiseDotNet.Graphics.Tools.Noise.Model
{
	public class Sphere : AbstractModel
	{
		public Sphere()
		{
		}

		public Sphere(IModule3D module)
			: base(module)
		{
		}

		public float GetValue(float lat, float lon)
		{
			float num = 0f;
			float num2 = 0f;
			float num3 = 0f;
			Libnoise.LatLonToXYZ(lat, lon, ref num, ref num2, ref num3);
			return ((IModule3D)this._sourceModule).GetValue(num, num2, num3);
		}
	}
}
