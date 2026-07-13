using System;

namespace LibTessDotNet
{
	internal struct ContourVertex
	{
		public override string ToString()
		{
			return string.Format("{0}, {1}", this.Position, this.Data);
		}

		public Vec3 Position;

		public object Data;
	}
}
