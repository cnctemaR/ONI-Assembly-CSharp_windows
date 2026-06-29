using System;
using Klei.AI;
using UnityEngine;

public class CreatureFeeder : KMonoBehaviour
{
	protected override void OnSpawn()
	{
		Components.CreatureFeeders.Add(this);
		base.Subscribe(-1452790913, new Action<object>(this.OnAteFromStorage));
	}

	protected override void OnCleanUp()
	{
		Components.CreatureFeeders.Remove(this);
	}

	private void OnAteFromStorage(object data)
	{
		if (string.IsNullOrEmpty(this.effectId))
		{
			return;
		}
		GameObject gameObject = data as GameObject;
		gameObject.GetComponent<Effects>().Add(this.effectId, true);
	}

	public string effectId;
}
