using System;
using System.Collections.Generic;
using STRINGS;

public class Satellite : SpaceDestination
{
	public Satellite(int id, int distance, float startPosition, int thrustCost)
		: base(id, distance, startPosition, thrustCost)
	{
		this.name = UI.SPACEDESINATIONS.DEBRIS.SATELLITE.NAME;
		this.typeName = UI.SPACEDESINATIONS.DEBRIS.SATELLITE.NAME;
		this.description = UI.SPACEDESINATIONS.DEBRIS.SATELLITE.DESCRIPTION;
		this.iconSize = 16;
		this.spriteName = "asteroid";
		this.elementTable = new Dictionary<SimHashes, Tuple<float, float>>
		{
			{
				SimHashes.Steel,
				new Tuple<float, float>(10f, 20f)
			},
			{
				SimHashes.Copper,
				new Tuple<float, float>(10f, 20f)
			},
			{
				SimHashes.Glass,
				new Tuple<float, float>(10f, 20f)
			}
		};
		base.GenerateSurfaceElements();
	}
}
