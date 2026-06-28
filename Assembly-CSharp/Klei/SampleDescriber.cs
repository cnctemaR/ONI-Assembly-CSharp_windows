using System;
using KSerialization.Converters;

namespace Klei
{
	public class SampleDescriber
	{
		public SampleDescriber()
		{
			this.doAvoidPoints = true;
			this.dontRelaxChildren = false;
		}

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

			public Override(SampleDescriber.Override.Type type, float value = 0f)
			{
				this.type = type;
				this.value = value;
			}

			[StringEnumConverter]
			public SampleDescriber.Override.Type type { get; private set; }

			public float value { get; private set; }

			public enum Type
			{
				MassMultiplier,
				MassOverride,
				TemperatureMultiplier,
				TemperatureOverride
			}
		}
	}
}
