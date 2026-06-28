using System;

public class AmbientSoundManager : KMonoBehaviour
{
	public static AmbientSoundManager Instance { get; private set; }

	protected override void OnPrefabInit()
	{
		AmbientSoundManager.Instance = this;
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
	}

	protected override void OnCleanUp()
	{
		base.OnCleanUp();
		AmbientSoundManager.Instance = null;
	}

	[MyCmpAdd]
	private LoopingSounds loopingSounds;
}
