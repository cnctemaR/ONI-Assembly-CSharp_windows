using System;
using System.Collections.Generic;
using STRINGS;

public class CarbonaceousAsteroid : Asteroid
{
	public CarbonaceousAsteroid(int id, int distance, float startPosition, int thrustCost)
		: base(id, distance, startPosition, thrustCost)
	{
		this.name = UI.SPACEDESINATIONS.ASTEROIDS.CARBONACEOUSASTEROID.NAME;
		this.typeName = UI.SPACEDESINATIONS.ASTEROIDS.CARBONACEOUSASTEROID.NAME;
		this.description = UI.SPACEDESINATIONS.ASTEROIDS.CARBONACEOUSASTEROID.DESCRIPTION;
		this.spriteName = "asteroid";
		this.elementTable = new Dictionary<SimHashes, Tuple<float, float>>
		{
			{
				SimHashes.RefinedCarbon,
				new Tuple<float, float>(100f, 200f)
			},
			{
				SimHashes.Carbon,
				new Tuple<float, float>(100f, 200f)
			},
			{
				SimHashes.Diamond,
				new Tuple<float, float>(100f, 200f)
			}
		};
		base.GenerateSurfaceElements();
	}
}
