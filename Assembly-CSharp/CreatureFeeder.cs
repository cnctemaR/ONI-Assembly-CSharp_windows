using System;
using Klei.AI;
using UnityEngine;

public class CreatureFeeder : KMonoBehaviour
{
	protected override void OnSpawn()
	{
		Components.CreatureFeeders.Add(this);
		base.Subscribe<CreatureFeeder>(-1452790913, CreatureFeeder.OnAteFromStorageDelegate);
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

	private static readonly EventSystem.IntraObjectHandler<CreatureFeeder> OnAteFromStorageDelegate = new EventSystem.IntraObjectHandler<CreatureFeeder>(delegate(CreatureFeeder component, object data)
	{
		component.OnAteFromStorage(data);
	});
}
