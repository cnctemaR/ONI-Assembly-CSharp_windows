using System;
using System.Collections.Generic;
using STRINGS;

public class DustyMoon : DwarfPlanet
{
	public DustyMoon(int id, int distance, float startPosition, int thrustCost)
		: base(id, distance, startPosition, thrustCost)
	{
		this.name = UI.SPACEDESINATIONS.DWARFPLANETS.DUSTYDWARF.NAME;
		this.typeName = UI.SPACEDESINATIONS.DWARFPLANETS.DUSTYDWARF.NAME;
		this.description = UI.SPACEDESINATIONS.DWARFPLANETS.DUSTYDWARF.DESCRIPTION;
		this.spriteName = "asteroid";
		this.elementTable = new Dictionary<SimHashes, Tuple<float, float>>
		{
			{
				SimHashes.Regolith,
				new Tuple<float, float>(100f, 200f)
			},
			{
				SimHashes.MaficRock,
				new Tuple<float, float>(100f, 200f)
			},
			{
				SimHashes.SedimentaryRock,
				new Tuple<float, float>(100f, 200f)
			},
			{
				SimHashes.Radium,
				new Tuple<float, float>(100f, 200f)
			}
		};
		base.GenerateSurfaceElements();
	}
}
