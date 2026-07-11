using System;
using System.Collections.Generic;
using STRINGS;

public class RockyAsteroid : Asteroid
{
	public RockyAsteroid(int id, int distance, float startPosition, int thrustCost)
		: base(id, distance, startPosition, thrustCost)
	{
		this.name = UI.SPACEDESINATIONS.ASTEROIDS.ROCKYASTEROID.NAME;
		this.typeName = UI.SPACEDESINATIONS.ASTEROIDS.ROCKYASTEROID.NAME;
		this.description = UI.SPACEDESINATIONS.ASTEROIDS.ROCKYASTEROID.DESCRIPTION;
		this.spriteName = "asteroid";
		this.elementTable = new Dictionary<SimHashes, Tuple<float, float>>
		{
			{
				SimHashes.IronOre,
				new Tuple<float, float>(100f, 200f)
			},
			{
				SimHashes.Cuprite,
				new Tuple<float, float>(100f, 200f)
			},
			{
				SimHashes.SedimentaryRock,
				new Tuple<float, float>(100f, 200f)
			},
			{
				SimHashes.IgneousRock,
				new Tuple<float, float>(100f, 200f)
			}
		};
		this.recoverableEntities = new Dictionary<string, int> { { "HatchHard", 3 } };
		base.GenerateSurfaceElements();
	}
}
