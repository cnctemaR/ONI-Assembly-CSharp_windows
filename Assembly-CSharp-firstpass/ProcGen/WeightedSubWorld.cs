using System;

namespace ProcGen
{
	public class WeightedSubWorld : IWeighted
	{
		public WeightedSubWorld(float weight, SubWorld subWorld)
		{
			this.weight = weight;
			this.subWorld = subWorld;
		}

		public SubWorld subWorld { get; set; }

		public float weight { get; set; }

		public override int GetHashCode()
		{
			return this.subWorld.GetHashCode();
		}
	}
}
