using System;
using System.Collections.Generic;
using STRINGS;

public class IceGiant : SpaceDestination
{
	public IceGiant(int id, int distance, float startPosition, int thrustCost)
		: base(id, distance, startPosition, thrustCost)
	{
		this.name = UI.SPACEDESINATIONS.GIANTS.ICEGIANT.NAME;
		this.typeName = UI.SPACEDESINATIONS.GIANTS.ICEGIANT.NAME;
		this.description = UI.SPACEDESINATIONS.GIANTS.ICEGIANT.DESCRIPTION;
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
		base.GenerateSurfaceElements();
	}
}
