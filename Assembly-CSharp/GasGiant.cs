using System;
using System.Collections.Generic;
using STRINGS;

public class GasGiant : SpaceDestination
{
	public GasGiant(int id, int distance, float startPosition, int thrustCost)
		: base(id, distance, startPosition, thrustCost)
	{
		this.name = UI.SPACEDESINATIONS.GIANTS.GASGIANT.NAME;
		this.typeName = UI.SPACEDESINATIONS.GIANTS.GASGIANT.NAME;
		this.description = UI.SPACEDESINATIONS.GIANTS.GASGIANT.DESCRIPTION;
		this.spriteName = "gasGiant";
		this.elementTable = new Dictionary<SimHashes, Tuple<float, float>>
		{
			{
				SimHashes.Methane,
				new Tuple<float, float>(100f, 200f)
			},
			{
				SimHashes.Hydrogen,
				new Tuple<float, float>(100f, 200f)
			}
		};
		base.GenerateSurfaceElements();
	}
}
