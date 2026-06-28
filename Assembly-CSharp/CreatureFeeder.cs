using System;

public class CreatureFeeder : KMonoBehaviour
{
	protected override void OnSpawn()
	{
		Components.CreatureFeeders.Add(this);
	}

	protected override void OnCleanUp()
	{
		Components.CreatureFeeders.Remove(this);
	}
}
