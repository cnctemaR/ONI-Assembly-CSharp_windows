using System;

public class LoopingSoundUpdaterComponents : KGameObjectComponentManager<LoopingSoundUpdaterComponent>
{
	public HandleVector<int>.Handle Add(LoopingSounds looping_sounds)
	{
		return base.Add(looping_sounds.gameObject, new LoopingSoundUpdaterComponent(looping_sounds));
	}

	public override void Update(float dt)
	{
		for (int i = 0; i < this.data.Count; i++)
		{
			LoopingSoundUpdaterComponent loopingSoundUpdaterComponent = this.data[i];
			if (loopingSoundUpdaterComponent.loopingSounds != null)
			{
				loopingSoundUpdaterComponent.loopingSounds.DoUpdate();
			}
		}
	}
}
