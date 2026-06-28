using System;

public class WorldGenScreen : KMonoBehaviour
{
	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		WorldGenScreen.Instance = this;
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
	}

	public static WorldGenScreen Instance;
}
