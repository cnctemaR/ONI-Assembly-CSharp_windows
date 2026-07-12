using System;
using KSerialization;

[SerializationConfig(MemberSerialization.OptIn)]
public class PlantAirConditioner : AirConditioner
{
	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		base.Subscribe<PlantAirConditioner>(-1396791468, PlantAirConditioner.OnFertilizedDelegate);
		base.Subscribe<PlantAirConditioner>(-1073674739, PlantAirConditioner.OnUnfertilizedDelegate);
	}

	private void OnFertilized(object data)
	{
		this.operational.SetFlag(PlantAirConditioner.fertilizedFlag, true);
	}

	private void OnUnfertilized(object data)
	{
		this.operational.SetFlag(PlantAirConditioner.fertilizedFlag, false);
	}

	private static readonly Operational.Flag fertilizedFlag = new Operational.Flag("fertilized", Operational.Flag.Type.Requirement);

	private static readonly EventSystem.IntraObjectHandler<PlantAirConditioner> OnFertilizedDelegate = new EventSystem.IntraObjectHandler<PlantAirConditioner>(delegate(PlantAirConditioner component, object data)
	{
		component.OnFertilized(data);
	});

	private static readonly EventSystem.IntraObjectHandler<PlantAirConditioner> OnUnfertilizedDelegate = new EventSystem.IntraObjectHandler<PlantAirConditioner>(delegate(PlantAirConditioner component, object data)
	{
		component.OnUnfertilized(data);
	});
}
