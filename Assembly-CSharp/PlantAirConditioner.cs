using System;
using KSerialization;

[SerializationConfig(MemberSerialization.OptIn)]
public class PlantAirConditioner : AirConditioner
{
	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		base.Subscribe(-1396791468, new Action<object>(this.OnFertilized));
		base.Subscribe(-1073674739, new Action<object>(this.OnUnfertilized));
	}

	private void OnFertilized(object data)
	{
		this.operational.SetFlag(this.fertilizedFlag, true);
	}

	private void OnUnfertilized(object data)
	{
		this.operational.SetFlag(this.fertilizedFlag, false);
	}

	private Operational.Flag fertilizedFlag = new Operational.Flag("fertilized", Operational.Flag.Type.Requirement);
}
