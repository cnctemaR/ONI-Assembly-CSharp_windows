using System;

namespace ProcGen.Noise
{
	public class SampleSettings : NoiseBase
	{
		public override Type GetObjectType()
		{
			return typeof(SampleSettings);
		}

		public float zoom { get; set; }

		public bool normalise { get; set; }

		public bool seamless { get; set; }

		public Vector2f lowerBound { get; set; }

		public Vector2f upperBound { get; set; }

		public SampleSettings()
		{
			this.zoom = 0.1f;
			this.lowerBound = new Vector2f(2, 2);
			this.upperBound = new Vector2f(4, 4);
			this.seamless = false;
			this.normalise = false;
		}
	}
}
