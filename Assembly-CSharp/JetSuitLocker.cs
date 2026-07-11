using System;
using STRINGS;
using UnityEngine;

public class JetSuitLocker : StateMachineComponent<JetSuitLocker.StatesInstance>, ISecondaryInput
{
	public float FuelAvailable
	{
		get
		{
			GameObject fuel = this.GetFuel();
			float num = 0f;
			if (fuel != null)
			{
				num = fuel.GetComponent<PrimaryElement>().Mass / 100f;
				num = Math.Min(num, 1f);
			}
			return num;
		}
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		this.fuel_tag = SimHashes.Petroleum.CreateTag();
		this.fuel_consumer = base.gameObject.AddComponent<ConduitConsumer>();
		this.fuel_consumer.conduitType = this.portInfo.conduitType;
		this.fuel_consumer.consumptionRate = 10f;
		this.fuel_consumer.capacityTag = this.fuel_tag;
		this.fuel_consumer.wrongElementResult = ConduitConsumer.WrongElementResult.Dump;
		this.fuel_consumer.forceAlwaysSatisfied = true;
		this.fuel_consumer.capacityKG = 100f;
		this.fuel_consumer.useSecondaryInput = true;
		RequireInputs requireInputs = base.gameObject.AddComponent<RequireInputs>();
		requireInputs.conduitConsumer = this.fuel_consumer;
		requireInputs.SetRequirements(false, true);
		int num = Grid.PosToCell(base.transform.GetPosition());
		CellOffset rotatedOffset = this.building.GetRotatedOffset(this.portInfo.offset);
		this.secondaryInputCell = Grid.OffsetCell(num, rotatedOffset);
		IUtilityNetworkMgr networkManager = Conduit.GetNetworkManager(this.portInfo.conduitType);
		this.flowNetworkItem = new FlowUtilityNetwork.NetworkItem(this.portInfo.conduitType, Endpoint.Sink, this.secondaryInputCell, base.gameObject);
		networkManager.AddToNetworks(this.secondaryInputCell, this.flowNetworkItem, true);
		this.fuel_meter = new MeterController(base.GetComponent<KBatchedAnimController>(), "meter_target_1", "meter_petrol", Meter.Offset.Infront, Grid.SceneLayer.NoLayer, Vector3.zero, new string[] { "meter_target_1" });
		this.o2_meter = new MeterController(base.GetComponent<KBatchedAnimController>(), "meter_target_2", "meter_oxygen", Meter.Offset.Infront, Grid.SceneLayer.NoLayer, Vector3.zero, new string[] { "meter_target_2" });
		base.smi.StartSM();
	}

	protected override void OnCleanUp()
	{
		IUtilityNetworkMgr networkManager = Conduit.GetNetworkManager(this.portInfo.conduitType);
		networkManager.RemoveFromNetworks(this.secondaryInputCell, this.flowNetworkItem, true);
		base.OnCleanUp();
	}

	private GameObject GetFuel()
	{
		return this.storage.FindFirst(this.fuel_tag);
	}

	public bool IsSuitFullyCharged()
	{
		return this.suit_locker.IsSuitFullyCharged();
	}

	public KPrefabID GetStoredOutfit()
	{
		return this.suit_locker.GetStoredOutfit();
	}

	private void FuelSuit(float dt)
	{
		KPrefabID storedOutfit = this.suit_locker.GetStoredOutfit();
		if (storedOutfit == null)
		{
			return;
		}
		GameObject fuel = this.GetFuel();
		if (fuel == null)
		{
			return;
		}
		PrimaryElement component = fuel.GetComponent<PrimaryElement>();
		if (component == null)
		{
			return;
		}
		JetSuitTank component2 = storedOutfit.GetComponent<JetSuitTank>();
		float num = 375f * dt / 600f;
		num = Mathf.Min(num, 25f - component2.amount);
		num = Mathf.Min(component.Mass, num);
		component.Mass -= num;
		component2.amount += num;
	}

	ConduitType ISecondaryInput.GetSecondaryConduitType()
	{
		return this.portInfo.conduitType;
	}

	CellOffset ISecondaryInput.GetSecondaryConduitOffset()
	{
		return this.portInfo.offset;
	}

	public bool HasFuel()
	{
		GameObject fuel = this.GetFuel();
		return fuel != null && fuel.GetComponent<PrimaryElement>().Mass > 0f;
	}

	private void RefreshMeter()
	{
		this.o2_meter.SetPositionPercent(this.suit_locker.OxygenAvailable);
		this.fuel_meter.SetPositionPercent(this.FuelAvailable);
		this.anim_controller.SetSymbolVisiblity("oxygen_yes_bloom", this.suit_locker.IsOxygenTankFull());
		this.anim_controller.SetSymbolVisiblity("petrol_yes_bloom", this.IsFuelTankFull());
	}

	public bool IsFuelTankFull()
	{
		KPrefabID storedOutfit = this.GetStoredOutfit();
		if (storedOutfit != null)
		{
			JetSuitTank component = storedOutfit.GetComponent<JetSuitTank>();
			return component == null || component.PercentFull() >= 1f;
		}
		return false;
	}

	[MyCmpReq]
	private Building building;

	[MyCmpReq]
	private Storage storage;

	[MyCmpReq]
	private SuitLocker suit_locker;

	[MyCmpReq]
	private KBatchedAnimController anim_controller;

	public const float FUEL_CAPACITY = 100f;

	[SerializeField]
	public ConduitPortInfo portInfo;

	private int secondaryInputCell = -1;

	private FlowUtilityNetwork.NetworkItem flowNetworkItem;

	private ConduitConsumer fuel_consumer;

	private Tag fuel_tag;

	private MeterController o2_meter;

	private MeterController fuel_meter;

	public class States : GameStateMachine<JetSuitLocker.States, JetSuitLocker.StatesInstance, JetSuitLocker>
	{
		public override void InitializeStates(out StateMachine.BaseState default_state)
		{
			default_state = this.empty;
			base.serializable = true;
			this.root.Update("RefreshMeter", delegate(JetSuitLocker.StatesInstance smi, float dt)
			{
				smi.master.RefreshMeter();
			}, UpdateRate.RENDER_200ms, false);
			this.empty.EventTransition(GameHashes.OnStorageChange, this.charging, (JetSuitLocker.StatesInstance smi) => smi.master.GetStoredOutfit() != null);
			this.charging.DefaultState(this.charging.notoperational).EventTransition(GameHashes.OnStorageChange, this.empty, (JetSuitLocker.StatesInstance smi) => smi.master.GetStoredOutfit() == null).Transition(this.charged, (JetSuitLocker.StatesInstance smi) => smi.master.IsSuitFullyCharged(), UpdateRate.SIM_200ms);
			this.charging.notoperational.TagTransition(GameTags.Operational, this.charging.operational, false);
			this.charging.operational.TagTransition(GameTags.Operational, this.charging.notoperational, true).Transition(this.charging.nofuel, (JetSuitLocker.StatesInstance smi) => !smi.master.HasFuel(), UpdateRate.SIM_200ms).Update("FuelSuit", delegate(JetSuitLocker.StatesInstance smi, float dt)
			{
				smi.master.FuelSuit(dt);
			}, UpdateRate.SIM_1000ms, false);
			this.charging.nofuel.TagTransition(GameTags.Operational, this.charging.notoperational, true).Transition(this.charging.operational, (JetSuitLocker.StatesInstance smi) => smi.master.HasFuel(), UpdateRate.SIM_200ms).ToggleStatusItem(BUILDING.STATUSITEMS.SUIT_LOCKER.NO_FUEL.NAME, BUILDING.STATUSITEMS.SUIT_LOCKER.NO_FUEL.TOOLTIP, "status_item_no_liquid_to_pump", StatusItem.IconType.Custom, NotificationType.BadMinor, false, default(HashedString), 0, null, null, null);
			this.charged.EventTransition(GameHashes.OnStorageChange, this.empty, (JetSuitLocker.StatesInstance smi) => smi.master.GetStoredOutfit() == null);
		}

		public GameStateMachine<JetSuitLocker.States, JetSuitLocker.StatesInstance, JetSuitLocker, object>.State empty;

		public JetSuitLocker.States.ChargingStates charging;

		public GameStateMachine<JetSuitLocker.States, JetSuitLocker.StatesInstance, JetSuitLocker, object>.State charged;

		public class ChargingStates : GameStateMachine<JetSuitLocker.States, JetSuitLocker.StatesInstance, JetSuitLocker, object>.State
		{
			public GameStateMachine<JetSuitLocker.States, JetSuitLocker.StatesInstance, JetSuitLocker, object>.State notoperational;

			public GameStateMachine<JetSuitLocker.States, JetSuitLocker.StatesInstance, JetSuitLocker, object>.State operational;

			public GameStateMachine<JetSuitLocker.States, JetSuitLocker.StatesInstance, JetSuitLocker, object>.State nofuel;
		}
	}

	public class StatesInstance : GameStateMachine<JetSuitLocker.States, JetSuitLocker.StatesInstance, JetSuitLocker, object>.GameInstance
	{
		public StatesInstance(JetSuitLocker jet_suit_locker)
			: base(jet_suit_locker)
		{
		}
	}
}
