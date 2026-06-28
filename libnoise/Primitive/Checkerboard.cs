using System;

namespace LibNoiseDotNet.Graphics.Tools.Noise.Primitive
{
	public class Checkerboard : PrimitiveModule, IModule3D, IModule
	{
		public float GetValue(float x, float y, float z)
		{
			int num = (((double)x > 0.0) ? ((int)x) : ((int)x - 1));
			int num2 = (((double)y > 0.0) ? ((int)y) : ((int)y - 1));
			int num3 = (((double)z > 0.0) ? ((int)z) : ((int)z - 1));
			if (((num & 1) ^ (num2 & 1) ^ (num3 & 1)) == 0)
			{
				return 1f;
			}
			return -1f;
		}
	}
}
