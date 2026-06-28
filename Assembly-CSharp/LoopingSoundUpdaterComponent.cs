using System;

public struct LoopingSoundUpdaterComponent : IComponent
{
	public LoopingSoundUpdaterComponent(LoopingSounds looping_sounds)
	{
		this.loopingSounds = looping_sounds;
	}

	public void FixedUpdate(float dt)
	{
	}

	public void SimUpdate(float dt)
	{
	}

	public void Update(float dt)
	{
		if (this.loopingSounds != null)
		{
			this.loopingSounds.DoUpdate();
		}
	}

	public LoopingSounds loopingSounds;
}
