using System;
using System.Collections.Generic;
using STRINGS;

public class TerraPlanet : Planet
{
	public TerraPlanet(int id, int distance, float startPosition, int thrustCost)
		: base(id, distance, startPosition, thrustCost)
	{
		this.name = UI.SPACEDESINATIONS.PLANETS.TERRAPLANET.NAME;
		this.typeName = UI.SPACEDESINATIONS.PLANETS.TERRAPLANET.NAME;
		this.description = UI.SPACEDESINATIONS.PLANETS.TERRAPLANET.DESCRIPTION;
		this.spriteName = "terra";
		this.elementTable = new Dictionary<SimHashes, Tuple<float, float>>
		{
			{
				SimHashes.Water,
				new Tuple<float, float>(100f, 200f)
			},
			{
				SimHashes.Algae,
				new Tuple<float, float>(100f, 200f)
			},
			{
				SimHashes.Oxygen,
				new Tuple<float, float>(100f, 200f)
			},
			{
				SimHashes.Dirt,
				new Tuple<float, float>(100f, 200f)
			}
		};
		this.recoverableEntities = new Dictionary<string, int>
		{
			{ "PrickleFlowerSeed", 4 },
			{ "PacuEgg", 4 }
		};
		base.GenerateSurfaceElements();
	}
}
