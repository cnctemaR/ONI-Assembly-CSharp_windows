using System;

namespace LibNoiseDotNet.Graphics.Tools.Noise.Renderer
{
	public struct GradientPoint : IEquatable<GradientPoint>
	{
		public GradientPoint(float position, IColor color)
		{
			this.Color = color;
			this.Position = position;
			this._hashcode = (int)this.Position ^ this.Color.GetHashCode();
		}

		public bool Equals(GradientPoint other)
		{
			return this.Position == other.Position;
		}

		public override int GetHashCode()
		{
			return this._hashcode;
		}

		public IColor Color;

		public float Position;

		private int _hashcode;
	}
}
