using System;

public class DwarfPlanet : SpaceDestination
{
	public DwarfPlanet(int id, int distance, float startPosition, int thrustCost)
		: base(id, distance, startPosition, thrustCost)
	{
		this.iconSize = 64;
		this.spriteName = "planet";
	}
}
