using System;
using System.Collections.Generic;
using STRINGS;

public class MetallicAsteroid : Asteroid
{
	public MetallicAsteroid(int id, int distance, float startPosition, int thrustCost)
		: base(id, distance, startPosition, thrustCost)
	{
		this.name = UI.SPACEDESINATIONS.ASTEROIDS.METALLICASTEROID.NAME;
		this.typeName = UI.SPACEDESINATIONS.ASTEROIDS.METALLICASTEROID.NAME;
		this.description = UI.SPACEDESINATIONS.ASTEROIDS.METALLICASTEROID.DESCRIPTION;
		this.spriteName = "nebula";
		this.elementTable = new Dictionary<SimHashes, Tuple<float, float>>
		{
			{
				SimHashes.Iron,
				new Tuple<float, float>(100f, 200f)
			},
			{
				SimHashes.Copper,
				new Tuple<float, float>(100f, 200f)
			},
			{
				SimHashes.Obsidian,
				new Tuple<float, float>(100f, 200f)
			}
		};
		this.recoverableEntities = new Dictionary<string, int> { { "HatchMetal", 3 } };
		base.GenerateSurfaceElements();
	}
}
