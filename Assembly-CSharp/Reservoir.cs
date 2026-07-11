using System;
using UnityEngine;

public class Reservoir : KMonoBehaviour
{
	protected override void OnSpawn()
	{
		base.OnSpawn();
		this.meter = new MeterController(base.GetComponent<KBatchedAnimController>(), "meter_target", "meter", Meter.Offset.Infront, Grid.SceneLayer.NoLayer, new string[] { "meter_fill", "meter_OL" });
		base.Subscribe(-1697596308, new Action<object>(this.OnStorageChange));
		this.OnStorageChange(null);
	}

	private void OnStorageChange(object data)
	{
		this.meter.SetPositionPercent(Mathf.Clamp01(this.storage.MassStored() / this.storage.capacityKg));
	}

	private MeterController meter;

	[MyCmpGet]
	private Storage storage;
}
