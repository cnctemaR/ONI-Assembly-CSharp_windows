using System;
using UnityEngine;

public class SolidBooster : RocketEngine
{
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

	public Storage fuelStorage;
}
