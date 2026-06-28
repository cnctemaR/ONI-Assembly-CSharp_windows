using System;

public class GameComps : Comps
{
	public GameComps()
	{
		GameComps.Gravities = base.Add<GravityComponents>(new GravityComponents());
		GameComps.Fallers = base.Add<FallerComponents>(new FallerComponents());
	}

	public override void Shutdown()
	{
		base.Shutdown();
		GameComps.Gravities = null;
		GameComps.Fallers = null;
	}

	public static GravityComponents Gravities;

	public static FallerComponents Fallers;
}
