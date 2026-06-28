using System;
using KSerialization.Converters;

namespace ProcGen
{
	public class Mob : SampleDescriber
	{
		public Mob()
		{
		}

		public Mob(Mob.Location location)
		{
			this.location = location;
		}

		public MinMax units { get; private set; }

		public string prefabName { get; private set; }

		[StringEnumConverter]
		public Mob.Location location { get; private set; }

		public enum Location
		{
			Floor,
			Ceiling,
			Air,
			BackWall,
			NearWater,
			NearLiquid,
			Solid,
			Water,
			ShallowLiquid
		}
	}
}
