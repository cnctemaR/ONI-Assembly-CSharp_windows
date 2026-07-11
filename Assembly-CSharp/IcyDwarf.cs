using System;
using System.Collections.Generic;
using STRINGS;

public class IcyDwarf : DwarfPlanet
{
	public IcyDwarf(int id, int distance, float startPosition, int thrustCost)
		: base(id, distance, startPosition, thrustCost)
	{
		this.name = UI.SPACEDESINATIONS.DWARFPLANETS.ICYDWARF.NAME;
		this.typeName = UI.SPACEDESINATIONS.DWARFPLANETS.ICYDWARF.NAME;
		this.description = UI.SPACEDESINATIONS.DWARFPLANETS.ICYDWARF.DESCRIPTION;
		this.spriteName = "icyMoon";
		this.elementTable = new Dictionary<SimHashes, Tuple<float, float>>
		{
			{
				SimHashes.Ice,
				new Tuple<float, float>(100f, 200f)
			},
			{
				SimHashes.SolidCarbonDioxide,
				new Tuple<float, float>(100f, 200f)
			},
			{
				SimHashes.SolidOxygen,
				new Tuple<float, float>(100f, 200f)
			},
			{
				SimHashes.SolidMethane,
				new Tuple<float, float>(100f, 200f)
			}
		};
		this.recoverableEntities = new Dictionary<string, int>
		{
			{ "ColdBreatherSeed", 3 },
			{ "ColdWheatSeed", 4 }
		};
		base.GenerateSurfaceElements();
	}
}
