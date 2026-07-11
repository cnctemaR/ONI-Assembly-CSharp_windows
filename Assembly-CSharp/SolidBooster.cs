using System;
using UnityEngine;

public class SolidBooster : RocketEngine
{
	protected override void OnSpawn()
	{
		base.OnSpawn();
		base.gameObject.Subscribe(1366341636, new Action<object>(this.OnReturn));
	}

	[ContextMenu("Fill Tank")]
	public void FillTank()
	{
		Element element = ElementLoader.GetElement(this.fuelTag);
		GameObject gameObject = element.substance.SpawnResource(base.gameObject.transform.GetPosition(), this.fuelStorage.capacityKg / 2f, element.defaultValues.temperature, byte.MaxValue, 0, false, false, false);
		this.fuelStorage.Store(gameObject, false, false, true, false);
		element = ElementLoader.GetElement(GameTags.OxyRock);
		gameObject = element.substance.SpawnResource(base.gameObject.transform.GetPosition(), this.fuelStorage.capacityKg / 2f, element.defaultValues.temperature, byte.MaxValue, 0, false, false, false);
		this.fuelStorage.Store(gameObject, false, false, true, false);
	}

	private void OnReturn(object data)
	{
		if (this.fuelStorage != null && this.fuelStorage.items != null)
		{
			for (int i = this.fuelStorage.items.Count - 1; i >= 0; i--)
			{
				Util.KDestroyGameObject(this.fuelStorage.items[i]);
			}
			this.fuelStorage.items.Clear();
		}
	}

	public Storage fuelStorage;
}
