using System;

public class Planet : SpaceDestination
{
	public Planet(int id, int distance, float startPosition, int thrustCost)
		: base(id, distance, startPosition, thrustCost)
	{
		this.iconSize = 96;
		this.spriteName = "planet";
	}
}
