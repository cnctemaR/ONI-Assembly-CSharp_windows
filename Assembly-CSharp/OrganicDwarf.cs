using System;
using System.Collections.Generic;
using STRINGS;

public class OrganicDwarf : DwarfPlanet
{
	public OrganicDwarf(int id, int distance, float startPosition, int thrustCost)
		: base(id, distance, startPosition, thrustCost)
	{
		this.name = UI.SPACEDESINATIONS.DWARFPLANETS.ORGANICDWARF.NAME;
		this.typeName = UI.SPACEDESINATIONS.DWARFPLANETS.ORGANICDWARF.NAME;
		this.description = UI.SPACEDESINATIONS.DWARFPLANETS.ORGANICDWARF.DESCRIPTION;
		this.spriteName = "organicAsteroid";
		this.elementTable = new Dictionary<SimHashes, Tuple<float, float>>
		{
			{
				SimHashes.SlimeMold,
				new Tuple<float, float>(100f, 200f)
			},
			{
				SimHashes.DirtyWater,
				new Tuple<float, float>(100f, 200f)
			},
			{
				SimHashes.Algae,
				new Tuple<float, float>(100f, 200f)
			},
			{
				SimHashes.CarbonDioxide,
				new Tuple<float, float>(100f, 200f)
			},
			{
				SimHashes.ContaminatedOxygen,
				new Tuple<float, float>(100f, 200f)
			}
		};
		this.recoverableEntities = new Dictionary<string, int>
		{
			{ "Moo", 1 },
			{ "GasGrassSeed", 4 }
		};
		base.GenerateSurfaceElements();
	}
}
