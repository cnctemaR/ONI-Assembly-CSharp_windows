using System;
using KSerialization.Converters;

namespace ProcGen
{
	[Serializable]
	public class SampleDescriber
	{
		public string name { get; set; }

		[StringEnumConverter]
		public SampleDescriber.PointSelectionMethod selectMethod { get; protected set; }

		public MinMax density { get; protected set; }

		public float avoidRadius { get; protected set; }

		[StringEnumConverter]
		public PointGenerator.SampleBehaviour sampleBehaviour { get; protected set; }

		public bool doAvoidPoints { get; protected set; }

		public bool dontRelaxChildren { get; protected set; }

		public MinMax blobSize { get; protected set; }

		public SampleDescriber()
		{
			this.doAvoidPoints = true;
			this.dontRelaxChildren = false;
		}

		public enum PointSelectionMethod
		{
			RandomPoints,
			Centroid
		}

		[Serializable]
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

			public float? massOverride { get; protected set; }

			public float? massMultiplier { get; protected set; }

			public float? temperatureOverride { get; protected set; }

			public float? temperatureMultiplier { get; protected set; }

			public string diseaseOverride { get; protected set; }

			public int? diseaseAmountOverride { get; protected set; }

			public void ModMultiplyMass(float mult)
			{
				if (this.massMultiplier == null)
				{
					this.massMultiplier = new float?(mult);
					return;
				}
				this.massMultiplier *= mult;
			}
		}
	}
}
