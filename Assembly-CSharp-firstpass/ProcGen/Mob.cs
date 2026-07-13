using System;
using KSerialization.Converters;

namespace ProcGen
{
	[Serializable]
	public class Mob : SampleDescriber
	{
		public Mob()
		{
		}

		public Mob(Mob.Location location)
		{
			this.location = location;
		}

		public string prefabName { get; private set; }

		public int width { get; private set; }

		public int height { get; private set; }

		public int paddingX { get; private set; }

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
			ShallowLiquid,
			Surface,
			LiquidFloor,
			AnyFloor,
			LiquidCeiling,
			Liquid,
			EntombedFloorPeek
		}
	}
}
