using System;
using Klei;
using STRINGS;
using TUNING;
using UnityEngine;

public class JetSuitLocker : StateMachineComponent<JetSuitLocker.StatesInstance>, ISecondaryInput
{
	public float FuelAvailable
	{
		get
		{
			return Math.Min((0f + this.storage.GetMassAvailable(this.fuel_tag)) / 100f, 1f);
		}
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		this.fuel_tag = GameTags.CombustibleLiquid;
		this.fuel_consumer = base.gameObject.AddComponent<ConduitConsumer>();
		this.fuel_consumer.conduitType = this.portInfo.conduitType;
		this.fuel_consumer.consumptionRate = 10f;
		this.fuel_consumer.capacityTag = GameTags.CombustibleLiquid;
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
		Conduit.GetNetworkManager(this.portInfo.conduitType).RemoveFromNetworks(this.secondaryInputCell, this.flowNetworkItem, true);
		base.OnCleanUp();
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
		if (!this.HasFuel())
		{
			return;
		}
		JetSuitTank component = storedOutfit.GetComponent<JetSuitTank>();
		float num = dt * 10f;
		num = Mathf.Min(num, 100f - component.amount);
		while (num > 0f && this.HasFuel())
		{
			float num2 = this.storage.GetMassAvailable(this.fuel_tag);
			num2 = Mathf.Min(num2, num);
			component.amount += num2;
			num -= num2;
			SimHashes simHashes = SimHashes.Petroleum;
			float num3;
			SimUtil.DiseaseInfo diseaseInfo;
			float num4;
			this.storage.ConsumeAndGetDisease(this.fuel_tag, num2, out num3, out diseaseInfo, out num4, out simHashes);
			component.lastFuelUsed = simHashes;
		}
	}

	bool ISecondaryInput.HasSecondaryConduitType(ConduitType type)
	{
		return this.portInfo.conduitType == type;
	}

	public CellOffset GetSecondaryConduitOffset(ConduitType type)
	{
		if (this.portInfo.conduitType == type)
		{
			return this.portInfo.offset;
		}
		return CellOffset.none;
	}

	public bool HasFuel()
	{
		return this.storage.Has(this.fuel_tag) && this.storage.GetMassAvailable(this.fuel_tag) > 0f;
	}

	private void RefreshMeter()
	{
		this.o2_meter.SetPositionPercent(this.suit_locker.OxygenAvailable);
		this.fuel_meter.SetPositionPercent(this.FuelAvailable);
		this.anim_controller.SetSymbolVisiblity("oxygen_yes_bloom", this.IsOxygenTankAboveMinimumLevel());
		this.anim_controller.SetSymbolVisiblity("petrol_yes_bloom", this.IsFuelTankAboveMinimumLevel());
	}

	public bool IsOxygenTankAboveMinimumLevel()
	{
		KPrefabID storedOutfit = this.GetStoredOutfit();
		if (storedOutfit != null)
		{
			SuitTank component = storedOutfit.GetComponent<SuitTank>();
			return component == null || component.PercentFull() >= global::TUNING.EQUIPMENT.SUITS.MINIMUM_USABLE_SUIT_CHARGE;
		}
		return false;
	}

	public bool IsFuelTankAboveMinimumLevel()
	{
		KPrefabID storedOutfit = this.GetStoredOutfit();
		if (storedOutfit != null)
		{
			JetSuitTank component = storedOutfit.GetComponent<JetSuitTank>();
			return component == null || component.PercentFull() >= global::TUNING.EQUIPMENT.SUITS.MINIMUM_USABLE_SUIT_CHARGE;
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
			base.serializable = StateMachine.SerializeType.Both_DEPRECATED;
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
			this.charging.nofuel.TagTransition(GameTags.Operational, this.charging.notoperational, true).Transition(this.charging.operational, (JetSuitLocker.StatesInstance smi) => smi.master.HasFuel(), UpdateRate.SIM_200ms).ToggleStatusItem(BUILDING.STATUSITEMS.SUIT_LOCKER.NO_FUEL.NAME, BUILDING.STATUSITEMS.SUIT_LOCKER.NO_FUEL.TOOLTIP, "status_item_no_liquid_to_pump", StatusItem.IconType.Custom, NotificationType.BadMinor, false, default(HashedString), 129022, null, null, null);
			this.charged.Transition(this.charging, (JetSuitLocker.StatesInstance smi) => !smi.master.IsSuitFullyCharged(), UpdateRate.SIM_200ms).EventTransition(GameHashes.OnStorageChange, this.empty, (JetSuitLocker.StatesInstance smi) => smi.master.GetStoredOutfit() == null);
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
