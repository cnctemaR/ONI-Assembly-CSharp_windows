using System;

public class Asteroid : SpaceDestination
{
	public Asteroid(int id, int distance, float startPosition, int thrustCost)
		: base(id, distance, startPosition, thrustCost)
	{
		this.iconSize = 32;
	}
}
