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
		base.Subscribe(-778359855, new Action<object>(this.PlayDeliveryAnimation));
		base.Subscribe(-887025858, new Action<object>(this.OnRocketLanded));
		RocketModuleCluster component = base.GetComponent<RocketModuleCluster>();
		if (component != null)
		{
			component.CraftInterface.Subscribe(1655598572, new Action<object>(this.OnLaunchConditionChanged));
			return;
		}
		base.Subscribe(705820818, new Action<object>(this.OnRocketLaunched));
	}

	protected override void OnCleanUp()
	{
		base.Unsubscribe(-1697596308, new Action<object>(this.UpdateMeter));
		base.Unsubscribe(-887025858, new Action<object>(this.OnRocketLanded));
		RocketModuleCluster component = base.GetComponent<RocketModuleCluster>();
		if (component != null)
		{
			component.CraftInterface.Unsubscribe(1655598572, new Action<object>(this.OnLaunchConditionChanged));
		}
		else
		{
			base.Unsubscribe(705820818, new Action<object>(this.OnRocketLaunched));
		}
		base.OnCleanUp();
	}

	private void OnLaunchConditionChanged(object data)
	{
		RocketModuleCluster component = base.GetComponent<RocketModuleCluster>();
		if (component != null && component.CraftInterface.IsLaunchRequested())
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
			float num = Math.Min((float)(SpacecraftManager.instance.GetSpacecraftDestination(spacecraftFromLaunchConditionManager.id).OneBasedDistance * this.dataBankConsumption * 2), this.databankStorage.MassStored());
			this.databankStorage.ConsumeIgnoringDisease(DatabankHelper.TAG, num);
		}
	}

	private void OnRocketLaunched(object o)
	{
		KBatchedAnimController component = base.GetComponent<KBatchedAnimController>();
		component.Play("launch_pre", KAnim.PlayMode.Once, 1f, 0f);
		component.Queue("launch", KAnim.PlayMode.Once, 1f, 0f);
		component.Queue("launch_pst", KAnim.PlayMode.Once, 1f, 0f);
	}

	public void ConsumeDataBanksInFlight()
	{
		if (this.databankStorage != null)
		{
			this.databankStorage.ConsumeIgnoringDisease(DatabankHelper.TAG, (float)this.dataBankConsumption);
		}
	}

	private void PlayDeliveryAnimation(object data = null)
	{
		KBatchedAnimController component = base.GetComponent<KBatchedAnimController>();
		HashedString currentAnim = component.currentAnim;
		component.Play("databank_delivery_reaction", KAnim.PlayMode.Once, 1f, 0f);
		component.Queue(currentAnim, KAnim.PlayMode.Once, 1f, 0f);
	}

	private void UpdateMeter(object data = null)
	{
		this.meter.SetPositionPercent(this.databankStorage.MassStored() / this.databankStorage.Capacity());
	}

	public bool HasResourcesToMove(int distance)
	{
		return this.databankStorage.UnitsStored() >= (float)(distance * this.dataBankConsumption);
	}

	public bool IsFull()
	{
		return this.databankStorage.MassStored() >= this.databankStorage.Capacity();
	}

	public float GetDataBanksStored()
	{
		if (!(this.databankStorage != null))
		{
			return 0f;
		}
		return this.databankStorage.UnitsStored();
	}

	public float GetDataBankRange()
	{
		if (this.databankStorage == null)
		{
			return 0f;
		}
		if (this.consumeDataBanksOnLand)
		{
			return this.databankStorage.UnitsStored() / (float)this.dataBankConsumption * RoboPilotCommandModuleConfig.DATABANKRANGE;
		}
		return this.databankStorage.UnitsStored() / (float)this.dataBankConsumption * 600f;
	}

	private MeterController meter;

	private Storage databankStorage;

	public int dataBankConsumption = 2;

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
