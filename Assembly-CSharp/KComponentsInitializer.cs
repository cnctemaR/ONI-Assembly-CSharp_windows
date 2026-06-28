using System;

public class KComponentsInitializer : KComponentSpawn
{
	private void Awake()
	{
		KComponentSpawn.instance = this;
		this.comps = new GameComps();
	}
}
