using System;
using System.Collections.Generic;
using STRINGS;

public class VolcanoPlanet : Planet
{
	public VolcanoPlanet(int id, int distance, float startPosition, int thrustCost)
		: base(id, distance, startPosition, thrustCost)
	{
		this.name = UI.SPACEDESINATIONS.PLANETS.VOLCANOPLANET.NAME;
		this.typeName = UI.SPACEDESINATIONS.PLANETS.VOLCANOPLANET.NAME;
		this.description = UI.SPACEDESINATIONS.PLANETS.VOLCANOPLANET.DESCRIPTION;
		this.spriteName = "planet";
		this.elementTable = new Dictionary<SimHashes, Tuple<float, float>>
		{
			{
				SimHashes.Magma,
				new Tuple<float, float>(100f, 200f)
			},
			{
				SimHashes.IgneousRock,
				new Tuple<float, float>(100f, 200f)
			},
			{
				SimHashes.Obsidian,
				new Tuple<float, float>(100f, 200f)
			},
			{
				SimHashes.Katairite,
				new Tuple<float, float>(100f, 200f)
			}
		};
		base.GenerateSurfaceElements();
	}
}
