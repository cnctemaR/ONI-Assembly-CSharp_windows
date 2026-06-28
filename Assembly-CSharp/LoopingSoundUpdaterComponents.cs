using System;

public class LoopingSoundUpdaterComponents : KGameObjectComponentManager2<LoopingSoundUpdaterComponent>
{
	public LoopingSoundUpdaterComponents()
		: base(true, false, false)
	{
	}

	public HandleVector<int>.Handle Add(LoopingSounds looping_sounds)
	{
		return base.Add(looping_sounds.gameObject, new LoopingSoundUpdaterComponent(looping_sounds));
	}
}
