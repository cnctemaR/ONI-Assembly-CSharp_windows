using System;

public struct LoopingSoundUpdaterComponent
{
	public LoopingSoundUpdaterComponent(LoopingSounds looping_sounds)
	{
		this.loopingSounds = looping_sounds;
	}

	public LoopingSounds loopingSounds;
}
