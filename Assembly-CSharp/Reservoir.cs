using System;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/Reservoir")]
public class Reservoir : KMonoBehaviour
{
	protected override void OnSpawn()
	{
		base.OnSpawn();
		this.meter = new MeterController(base.GetComponent<KBatchedAnimController>(), "meter_target", "meter", Meter.Offset.Infront, Grid.SceneLayer.NoLayer, new string[] { "meter_fill", "meter_OL" });
		base.Subscribe<Reservoir>(-1697596308, Reservoir.OnStorageChangeDelegate);
		this.OnStorageChange(null);
	}

	private void OnStorageChange(object data)
	{
		this.meter.SetPositionPercent(Mathf.Clamp01(this.storage.MassStored() / this.storage.capacityKg));
	}

	private MeterController meter;

	[MyCmpGet]
	private Storage storage;

	private static readonly EventSystem.IntraObjectHandler<Reservoir> OnStorageChangeDelegate = new EventSystem.IntraObjectHandler<Reservoir>(delegate(Reservoir component, object data)
	{
		component.OnStorageChange(data);
	});
}
