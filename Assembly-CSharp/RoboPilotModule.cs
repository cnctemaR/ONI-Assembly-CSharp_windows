using System;

public class RoboPilotModule : KMonoBehaviour
{
	protected override void OnSpawn()
	{
		this.databankStorage = base.GetComponent<Storage>();
		this.meter = new MeterController(base.GetComponent<KBatchedAnimController>(), "meter_target", "meter", Meter.Offset.Infront, Grid.SceneLayer.NoLayer, new string[] { "meter_target", "meter_fill", "meter_frame" });
		this.meter.gameObject.GetComponent<KBatchedAnimTracker>().matchParentOffset = true;
		this.UpdateMeter(null);
		this.databankStorage.SetOffsets(RoboPilotModule.dataDeliveryOffsets);
		base.Subscribe(-1697596308, new Action<object>(this.UpdateMeter));
		base.Subscribe(-887025858, new Action<object>(this.OnRocketLanded));
		base.GetComponent<RocketModuleCluster>().CraftInterface.Subscribe(1655598572, new Action<object>(this.OnLaunchConditionChanged));
	}

	private void OnLaunchConditionChanged(object data)
	{
		RocketModuleCluster component = base.GetComponent<RocketModuleCluster>();
		if (component.CraftInterface.IsLaunchRequested())
		{
			component.CraftInterface.GetComponent<Clustercraft>().Launch(false);
		}
	}

	private void OnRocketLanded(object o)
	{
		if (this.consumeDataBanksOnLand)
		{
			LaunchConditionManager launchConditionManager = base.GetComponent<RocketModule>().FindLaunchConditionManager();
			Spacecraft spacecraftFromLaunchConditionManager = SpacecraftManager.instance.GetSpacecraftFromLaunchConditionManager(launchConditionManager);
			float num = Math.Min((float)(SpacecraftManager.instance.GetSpacecraftDestination(spacecraftFromLaunchConditionManager.id).OneBasedDistance * this.dataBankConsumption), this.databankStorage.MassStored());
			this.databankStorage.ConsumeIgnoringDisease(this.dataBankType, num);
		}
	}

	public void ConsumeDataBanksInFlight()
	{
		this.databankStorage.ConsumeIgnoringDisease(this.dataBankType, (float)this.dataBankConsumption);
	}

	private void UpdateMeter(object data = null)
	{
		this.meter.SetPositionPercent(this.databankStorage.MassStored() / this.databankStorage.Capacity());
	}

	public bool HasResourcesToMove(int distance)
	{
		return this.databankStorage.MassStored() > (float)(distance * this.dataBankConsumption);
	}

	public float GetDataBanksStored()
	{
		if (!(this.databankStorage != null))
		{
			return 0f;
		}
		return this.databankStorage.MassStored();
	}

	public float FlightEfficiencyModifier()
	{
		if (this.GetDataBanksStored() > 0f)
		{
			return this.flightEfficiencyModifier;
		}
		return 0f;
	}

	private MeterController meter;

	private Storage databankStorage;

	public int dataBankConsumption = 2;

	public Tag dataBankType;

	private float flightEfficiencyModifier = 0.1f;

	public bool consumeDataBanksOnLand;

	private static CellOffset[] dataDeliveryOffsets = new CellOffset[]
	{
		new CellOffset(0, 0),
		new CellOffset(1, 0),
		new CellOffset(2, 0),
		new CellOffset(3, 0),
		new CellOffset(-1, 0),
		new CellOffset(-2, 0),
		new CellOffset(-3, 0)
	};
}
