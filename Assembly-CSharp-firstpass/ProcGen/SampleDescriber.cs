using System;
using KSerialization.Converters;

namespace ProcGen
{
	public class SampleDescriber
	{
		public SampleDescriber()
		{
			this.doAvoidPoints = true;
			this.dontRelaxChildren = false;
		}

		public string name { get; set; }

		[StringEnumConverter]
		public SampleDescriber.PointSelectionMethod selectMethod { get; private set; }

		public MinMax density { get; private set; }

		public float avoidRadius { get; private set; }

		[StringEnumConverter]
		public PointGenerator.SampleBehaviour sampleBehaviour { get; private set; }

		public bool doAvoidPoints { get; private set; }

		public bool dontRelaxChildren { get; private set; }

		public MinMax blobSize { get; private set; }

		public enum PointSelectionMethod
		{
			RandomPoints,
			Centroid
		}

		public class Override
		{
			public Override()
			{
			}

			public Override(float? massOverride, float? massMultiplier, float? temperatureOverride, float? temperatureMultiplier, string diseaseOverride, int? diseaseAmountOverride)
			{
				this.massOverride = massOverride;
				this.massMultiplier = massMultiplier;
				this.temperatureOverride = temperatureOverride;
				this.temperatureMultiplier = temperatureMultiplier;
				this.diseaseOverride = diseaseOverride;
				this.diseaseAmountOverride = diseaseAmountOverride;
			}

			public float? massOverride { get; private set; }

			public float? massMultiplier { get; private set; }

			public float? temperatureOverride { get; private set; }

			public float? temperatureMultiplier { get; private set; }

			public string diseaseOverride { get; private set; }

			public int? diseaseAmountOverride { get; private set; }
		}
	}
}
