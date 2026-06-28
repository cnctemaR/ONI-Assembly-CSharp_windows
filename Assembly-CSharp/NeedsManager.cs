using System;

public class NeedsManager : KMonoBehaviour
{
	protected override void OnSpawn()
	{
		GameClock.Instance.Subscribe(631075836, new Action<object>(this.OnNewDay));
	}

	private void OnNewDay(object data)
	{
		foreach (MinionIdentity minionIdentity in Components.LiveMinionIdentities)
		{
			MinionModifiers component = minionIdentity.GetComponent<MinionModifiers>();
			component.OnNewDay();
		}
	}
}
