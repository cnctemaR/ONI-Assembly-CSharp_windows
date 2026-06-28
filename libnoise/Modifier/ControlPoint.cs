using System;

namespace LibNoiseDotNet.Graphics.Tools.Noise.Modifier
{
	public struct ControlPoint : IEquatable<ControlPoint>
	{
		public ControlPoint(float input, float output)
		{
			this.Input = input;
			this.Output = output;
		}

		public bool Equals(ControlPoint other)
		{
			return this.Input == other.Input;
		}

		public float Input;

		public float Output;
	}
}
