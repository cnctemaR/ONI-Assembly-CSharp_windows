using System;
using System.Collections.Generic;
using KSerialization;
using STRINGS;
using UnityEngine;

public class RailGun : StateMachineComponent<RailGun.StatesInstance>, ISim200ms, ISecondaryInput
{
	public float MaxLaunchMass
	{
		get
		{
			return 200f;
		}
	}

	public float EnergyCost
	{
		get
		{
			return base.smi.EnergyCost();
		}
	}

	public float CurrentEnergy
	{
		get
		{
			return this.hepStorage.Particles;
		}
	}

	public bool AllowLaunchingFromLogic
	{
		get
		{
			return !this.hasLogicWire || (this.hasLogicWire && this.isLogicActive);
		}
	}

	public bool HasLogicWire
	{
		get
		{
			return this.hasLogicWire;
		}
	}

	public bool IsLogicActive
	{
		get
		{
			return this.isLogicActive;
		}
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		this.destinationSelector = base.GetComponent<ClusterDestinationSelector>();
		this.resourceStorage = base.GetComponent<Storage>();
		this.particleStorage = base.GetComponent<HighEnergyParticleStorage>();
		if (RailGun.noSurfaceSightStatusItem == null)
		{
			RailGun.noSurfaceSightStatusItem = new StatusItem("RAILGUN_PATH_NOT_CLEAR", "BUILDING", "status_item_no_sky", StatusItem.IconType.Custom, NotificationType.BadMinor, false, OverlayModes.None.ID, true, 129022, null);
		}
		if (RailGun.noDestinationStatusItem == null)
		{
			RailGun.noDestinationStatusItem = new StatusItem("RAILGUN_NO_DESTINATION", "BUILDING", "status_item_no_sky", StatusItem.IconType.Custom, NotificationType.BadMinor, false, OverlayModes.None.ID, true, 129022, null);
		}
		this.gasInputCell = Grid.OffsetCell(Grid.PosToCell(this), this.gasPortInfo.offset);
		this.gasConsumer = this.CreateConduitConsumer(ConduitType.Gas, this.gasInputCell, out this.gasNetworkItem);
		this.liquidInputCell = Grid.OffsetCell(Grid.PosToCell(this), this.liquidPortInfo.offset);
		this.liquidConsumer = this.CreateConduitConsumer(ConduitType.Liquid, this.liquidInputCell, out this.liquidNetworkItem);
		this.solidInputCell = Grid.OffsetCell(Grid.PosToCell(this), this.solidPortInfo.offset);
		this.solidConsumer = this.CreateSolidConduitConsumer(this.solidInputCell, out this.solidNetworkItem);
		this.CreateMeters();
		base.smi.StartSM();
		if (RailGun.infoStatusItemLogic == null)
		{
			RailGun.infoStatusItemLogic = new StatusItem("LogicOperationalInfo", "BUILDING", "", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID, true, 129022, null);
			RailGun.infoStatusItemLogic.resolveStringCallback = new Func<string, object, string>(RailGun.ResolveInfoStatusItemString);
		}
		this.CheckLogicWireState();
		base.Subscribe<RailGun>(-801688580, RailGun.OnLogicValueChangedDelegate);
	}

	protected override void OnCleanUp()
	{
		Conduit.GetNetworkManager(this.liquidPortInfo.conduitType).RemoveFromNetworks(this.liquidInputCell, this.liquidNetworkItem, true);
		Conduit.GetNetworkManager(this.gasPortInfo.conduitType).RemoveFromNetworks(this.gasInputCell, this.gasNetworkItem, true);
		Game.Instance.solidConduitSystem.RemoveFromNetworks(this.solidInputCell, this.solidConsumer, true);
		base.OnCleanUp();
	}

	private void CreateMeters()
	{
		this.resourceMeter = new MeterController(base.GetComponent<KBatchedAnimController>(), "meter_storage_target", "meter_storage", Meter.Offset.Infront, Grid.SceneLayer.NoLayer, Array.Empty<string>());
		this.particleMeter = new MeterController(base.GetComponent<KBatchedAnimController>(), "meter_orb_target", "meter_orb", Meter.Offset.Infront, Grid.SceneLayer.NoLayer, Array.Empty<string>());
	}

	bool ISecondaryInput.HasSecondaryConduitType(ConduitType type)
	{
		return this.liquidPortInfo.conduitType == type || this.gasPortInfo.conduitType == type || this.solidPortInfo.conduitType == type;
	}

	public CellOffset GetSecondaryConduitOffset(ConduitType type)
	{
		if (this.liquidPortInfo.conduitType == type)
		{
			return this.liquidPortInfo.offset;
		}
		if (this.gasPortInfo.conduitType == type)
		{
			return this.gasPortInfo.offset;
		}
		if (this.solidPortInfo.conduitType == type)
		{
			return this.solidPortInfo.offset;
		}
		return CellOffset.none;
	}

	private LogicCircuitNetwork GetNetwork()
	{
		int portCell = base.GetComponent<LogicPorts>().GetPortCell(RailGun.PORT_ID);
		return Game.Instance.logicCircuitManager.GetNetworkForCell(portCell);
	}

	private void CheckLogicWireState()
	{
		LogicCircuitNetwork network = this.GetNetwork();
		this.hasLogicWire = network != null;
		int num = ((network != null) ? network.OutputValue : 1);
		bool flag = LogicCircuitNetwork.IsBitActive(0, num);
		this.isLogicActive = flag;
		base.smi.sm.allowedFromLogic.Set(this.AllowLaunchingFromLogic, base.smi, false);
		base.GetComponent<KSelectable>().ToggleStatusItem(RailGun.infoStatusItemLogic, network != null, this);
	}

	private void OnLogicValueChanged(object data)
	{
		if (((LogicValueChanged)data).portID == RailGun.PORT_ID)
		{
			this.CheckLogicWireState();
		}
	}

	private static string ResolveInfoStatusItemString(string format_str, object data)
	{
		RailGun railGun = (RailGun)data;
		Operational operational = railGun.operational;
		return railGun.AllowLaunchingFromLogic ? BUILDING.STATUSITEMS.LOGIC.LOGIC_CONTROLLED_ENABLED : BUILDING.STATUSITEMS.LOGIC.LOGIC_CONTROLLED_DISABLED;
	}

	public void Sim200ms(float dt)
	{
		WorldContainer myWorld = this.GetMyWorld();
		Extents extents = base.GetComponent<Building>().GetExtents();
		int x = extents.x;
		int num = extents.x + extents.width - 2;
		int num2 = extents.y + extents.height;
		int num3 = Grid.XYToCell(x, num2);
		int num4 = Grid.XYToCell(num, num2);
		bool flag = true;
		int num5 = (int)myWorld.maximumBounds.y;
		for (int i = num3; i <= num4; i++)
		{
			int num6 = i;
			while (Grid.CellRow(num6) <= num5)
			{
				if (!Grid.IsValidCell(num6) || Grid.Solid[num6])
				{
					flag = false;
				}
				num6 = Grid.CellAbove(num6);
			}
		}
		this.operational.SetFlag(RailGun.noSurfaceSight, flag);
		this.operational.SetFlag(RailGun.noDestination, this.destinationSelector.GetDestinationWorld() >= 0);
		KSelectable component = base.GetComponent<KSelectable>();
		component.ToggleStatusItem(RailGun.noSurfaceSightStatusItem, !flag, null);
		component.ToggleStatusItem(RailGun.noDestinationStatusItem, this.destinationSelector.GetDestinationWorld() < 0, null);
		this.UpdateMeters();
	}

	private void UpdateMeters()
	{
		this.resourceMeter.SetPositionPercent(Mathf.Clamp01(this.resourceStorage.MassStored() / this.resourceStorage.capacityKg));
		this.particleMeter.SetPositionPercent(Mathf.Clamp01(this.particleStorage.Particles / this.particleStorage.capacity));
	}

	private void LaunchProjectile()
	{
		Extents extents = base.GetComponent<Building>().GetExtents();
		Vector2I vector2I = Grid.PosToXY(base.transform.position);
		vector2I.y += extents.height + 1;
		int num = Grid.XYToCell(vector2I.x, vector2I.y);
		GameObject gameObject = Util.KInstantiate(Assets.GetPrefab("RailGunPayload"), Grid.CellToPosCBC(num, Grid.SceneLayer.Front));
		float num2 = 0f;
		while (num2 < this.launchMass && this.resourceStorage.MassStored() > 0f)
		{
			num2 += this.resourceStorage.Transfer(gameObject.GetComponent<Storage>(), GameTags.Stored, this.launchMass - num2, false, true);
		}
		this.particleStorage.ConsumeAndGet(base.smi.EnergyCost());
		gameObject.SetActive(true);
		if (this.destinationSelector.GetDestinationWorld() >= 0)
		{
			RailGunPayload.StatesInstance smi = gameObject.GetSMI<RailGunPayload.StatesInstance>();
			smi.takeoffVelocity = 35f;
			smi.StartSM();
			smi.Launch(base.gameObject.GetMyWorldLocation(), this.destinationSelector.GetDestination());
		}
	}

	private ConduitConsumer CreateConduitConsumer(ConduitType inputType, int inputCell, out FlowUtilityNetwork.NetworkItem flowNetworkItem)
	{
		ConduitConsumer conduitConsumer = base.gameObject.AddComponent<ConduitConsumer>();
		conduitConsumer.conduitType = inputType;
		conduitConsumer.useSecondaryInput = true;
		IUtilityNetworkMgr networkManager = Conduit.GetNetworkManager(inputType);
		flowNetworkItem = new FlowUtilityNetwork.NetworkItem(inputType, Endpoint.Sink, inputCell, base.gameObject);
		networkManager.AddToNetworks(inputCell, flowNetworkItem, true);
		return conduitConsumer;
	}

	private SolidConduitConsumer CreateSolidConduitConsumer(int inputCell, out FlowUtilityNetwork.NetworkItem flowNetworkItem)
	{
		SolidConduitConsumer solidConduitConsumer = base.gameObject.AddComponent<SolidConduitConsumer>();
		solidConduitConsumer.useSecondaryInput = true;
		flowNetworkItem = new FlowUtilityNetwork.NetworkItem(ConduitType.Solid, Endpoint.Sink, inputCell, base.gameObject);
		Game.Instance.solidConduitSystem.AddToNetworks(inputCell, flowNetworkItem, true);
		return solidConduitConsumer;
	}

	[Serialize]
	public float launchMass = 200f;

	public float MinLaunchMass = 2f;

	[MyCmpGet]
	private Operational operational;

	[MyCmpGet]
	private KAnimControllerBase kac;

	[MyCmpGet]
	public HighEnergyParticleStorage hepStorage;

	public Storage resourceStorage;

	private MeterController resourceMeter;

	private HighEnergyParticleStorage particleStorage;

	private MeterController particleMeter;

	private ClusterDestinationSelector destinationSelector;

	public static readonly Operational.Flag noSurfaceSight = new Operational.Flag("noSurfaceSight", Operational.Flag.Type.Requirement);

	private static StatusItem noSurfaceSightStatusItem;

	public static readonly Operational.Flag noDestination = new Operational.Flag("noDestination", Operational.Flag.Type.Requirement);

	private static StatusItem noDestinationStatusItem;

	[SerializeField]
	public ConduitPortInfo liquidPortInfo;

	private int liquidInputCell = -1;

	private FlowUtilityNetwork.NetworkItem liquidNetworkItem;

	private ConduitConsumer liquidConsumer;

	[SerializeField]
	public ConduitPortInfo gasPortInfo;

	private int gasInputCell = -1;

	private FlowUtilityNetwork.NetworkItem gasNetworkItem;

	private ConduitConsumer gasConsumer;

	[SerializeField]
	public ConduitPortInfo solidPortInfo;

	private int solidInputCell = -1;

	private FlowUtilityNetwork.NetworkItem solidNetworkItem;

	private SolidConduitConsumer solidConsumer;

	public static readonly HashedString PORT_ID = "LogicLaunching";

	private bool hasLogicWire;

	private bool isLogicActive;

	private static StatusItem infoStatusItemLogic;

	public bool FreeStartHex;

	public bool FreeDestinationHex;

	private static readonly EventSystem.IntraObjectHandler<RailGun> OnLogicValueChangedDelegate = new EventSystem.IntraObjectHandler<RailGun>(delegate(RailGun component, object data)
	{
		component.OnLogicValueChanged(data);
	});

	public class StatesInstance : GameStateMachine<RailGun.States, RailGun.StatesInstance, RailGun, object>.GameInstance
	{
		public StatesInstance(RailGun smi)
			: base(smi)
		{
		}

		public bool HasResources()
		{
			return base.smi.master.resourceStorage.MassStored() >= base.smi.master.launchMass;
		}

		public bool HasEnergy()
		{
			return base.smi.master.particleStorage.Particles > this.EnergyCost();
		}

		public bool HasDestination()
		{
			return base.smi.master.destinationSelector.GetDestinationWorld() != base.smi.master.GetMyWorldId();
		}

		public bool IsDestinationReachable(bool forceRefresh = false)
		{
			if (forceRefresh)
			{
				this.UpdatePath();
			}
			return base.smi.master.destinationSelector.GetDestinationWorld() != base.smi.master.GetMyWorldId() && this.PathLength() != -1;
		}

		public int PathLength()
		{
			if (base.smi.m_cachedPath == null)
			{
				this.UpdatePath();
			}
			if (base.smi.m_cachedPath == null)
			{
				return -1;
			}
			int num = base.smi.m_cachedPath.Count;
			if (base.master.FreeStartHex)
			{
				num--;
			}
			if (base.master.FreeDestinationHex)
			{
				num--;
			}
			return num;
		}

		public void UpdatePath()
		{
			this.m_cachedPath = ClusterGrid.Instance.GetPath(base.gameObject.GetMyWorldLocation(), base.smi.master.destinationSelector.GetDestination(), base.smi.master.destinationSelector);
		}

		public float EnergyCost()
		{
			return Mathf.Max(0f, 0f + (float)this.PathLength() * 10f);
		}

		public bool MayTurnOn()
		{
			return this.HasEnergy() && this.IsDestinationReachable(false) && base.master.operational.IsOperational && base.sm.allowedFromLogic.Get(this);
		}

		public const int INVALID_PATH_LENGTH = -1;

		private List<AxialI> m_cachedPath;
	}

	public class States : GameStateMachine<RailGun.States, RailGun.StatesInstance, RailGun>
	{
		public override void InitializeStates(out StateMachine.BaseState default_state)
		{
			default_state = this.off;
			base.serializable = StateMachine.SerializeType.ParamsOnly;
			this.root.EventHandler(GameHashes.ClusterDestinationChanged, delegate(RailGun.StatesInstance smi)
			{
				smi.UpdatePath();
			});
			this.off.PlayAnim("off").EventTransition(GameHashes.OnParticleStorageChanged, this.on, (RailGun.StatesInstance smi) => smi.MayTurnOn()).EventTransition(GameHashes.ClusterDestinationChanged, this.on, (RailGun.StatesInstance smi) => smi.MayTurnOn())
				.EventTransition(GameHashes.OperationalChanged, this.on, (RailGun.StatesInstance smi) => smi.MayTurnOn())
				.ParamTransition<bool>(this.allowedFromLogic, this.on, (RailGun.StatesInstance smi, bool p) => smi.MayTurnOn());
			this.on.DefaultState(this.on.power_on).EventTransition(GameHashes.OperationalChanged, this.on.power_off, (RailGun.StatesInstance smi) => !smi.master.operational.IsOperational).EventTransition(GameHashes.ClusterDestinationChanged, this.on.power_off, (RailGun.StatesInstance smi) => !smi.IsDestinationReachable(false))
				.EventTransition(GameHashes.ClusterFogOfWarRevealed, (RailGun.StatesInstance smi) => Game.Instance, this.on.power_off, (RailGun.StatesInstance smi) => !smi.IsDestinationReachable(true))
				.EventTransition(GameHashes.OnParticleStorageChanged, this.on.power_off, (RailGun.StatesInstance smi) => !smi.MayTurnOn())
				.ParamTransition<bool>(this.allowedFromLogic, this.on.power_off, (RailGun.StatesInstance smi, bool p) => !p)
				.ToggleMainStatusItem(Db.Get().BuildingStatusItems.Normal, null);
			this.on.power_on.PlayAnim("power_on").OnAnimQueueComplete(this.on.wait_for_storage);
			this.on.power_off.PlayAnim("power_off").OnAnimQueueComplete(this.off);
			this.on.wait_for_storage.PlayAnim("on", KAnim.PlayMode.Loop).EventTransition(GameHashes.ClusterDestinationChanged, this.on.power_off, (RailGun.StatesInstance smi) => !smi.HasEnergy()).EventTransition(GameHashes.OnStorageChange, this.on.working, (RailGun.StatesInstance smi) => smi.HasResources() && smi.sm.cooldownTimer.Get(smi) <= 0f)
				.EventTransition(GameHashes.OperationalChanged, this.on.working, (RailGun.StatesInstance smi) => smi.HasResources() && smi.sm.cooldownTimer.Get(smi) <= 0f)
				.ParamTransition<float>(this.cooldownTimer, this.on.cooldown, (RailGun.StatesInstance smi, float p) => p > 0f);
			this.on.working.DefaultState(this.on.working.pre).Enter(delegate(RailGun.StatesInstance smi)
			{
				smi.master.operational.SetActive(true, false);
			}).Exit(delegate(RailGun.StatesInstance smi)
			{
				smi.master.operational.SetActive(false, false);
			});
			this.on.working.pre.PlayAnim("working_pre").OnAnimQueueComplete(this.on.working.loop);
			this.on.working.loop.PlayAnim("working_loop").OnAnimQueueComplete(this.on.working.fire);
			this.on.working.fire.Enter(delegate(RailGun.StatesInstance smi)
			{
				if (smi.IsDestinationReachable(false))
				{
					smi.master.LaunchProjectile();
					smi.sm.payloadsFiredSinceCooldown.Delta(1, smi);
					if (smi.sm.payloadsFiredSinceCooldown.Get(smi) >= 6)
					{
						smi.sm.cooldownTimer.Set(30f, smi, false);
					}
				}
			}).GoTo(this.on.working.bounce);
			this.on.working.bounce.ParamTransition<float>(this.cooldownTimer, this.on.working.pst, (RailGun.StatesInstance smi, float p) => p > 0f || !smi.HasResources()).ParamTransition<int>(this.payloadsFiredSinceCooldown, this.on.working.loop, (RailGun.StatesInstance smi, int p) => p < 6 && smi.HasResources());
			this.on.working.pst.PlayAnim("working_pst").OnAnimQueueComplete(this.on.wait_for_storage);
			this.on.cooldown.DefaultState(this.on.cooldown.pre).ToggleMainStatusItem(Db.Get().BuildingStatusItems.RailGunCooldown, null);
			this.on.cooldown.pre.PlayAnim("cooldown_pre").OnAnimQueueComplete(this.on.cooldown.loop);
			this.on.cooldown.loop.PlayAnim("cooldown_loop", KAnim.PlayMode.Loop).ParamTransition<float>(this.cooldownTimer, this.on.cooldown.pst, (RailGun.StatesInstance smi, float p) => p <= 0f).Update(delegate(RailGun.StatesInstance smi, float dt)
			{
				this.cooldownTimer.Delta(-dt, smi);
			}, UpdateRate.SIM_1000ms, false);
			this.on.cooldown.pst.PlayAnim("cooldown_pst").OnAnimQueueComplete(this.on.wait_for_storage).Exit(delegate(RailGun.StatesInstance smi)
			{
				smi.sm.payloadsFiredSinceCooldown.Set(0, smi, false);
			});
		}

		public GameStateMachine<RailGun.States, RailGun.StatesInstance, RailGun, object>.State off;

		public RailGun.States.OnStates on;

		public StateMachine<RailGun.States, RailGun.StatesInstance, RailGun, object>.FloatParameter cooldownTimer;

		public StateMachine<RailGun.States, RailGun.StatesInstance, RailGun, object>.IntParameter payloadsFiredSinceCooldown;

		public StateMachine<RailGun.States, RailGun.StatesInstance, RailGun, object>.BoolParameter allowedFromLogic;

		public StateMachine<RailGun.States, RailGun.StatesInstance, RailGun, object>.BoolParameter updatePath;

		public class WorkingStates : GameStateMachine<RailGun.States, RailGun.StatesInstance, RailGun, object>.State
		{
			public GameStateMachine<RailGun.States, RailGun.StatesInstance, RailGun, object>.State pre;

			public GameStateMachine<RailGun.States, RailGun.StatesInstance, RailGun, object>.State loop;

			public GameStateMachine<RailGun.States, RailGun.StatesInstance, RailGun, object>.State fire;

			public GameStateMachine<RailGun.States, RailGun.StatesInstance, RailGun, object>.State bounce;

			public GameStateMachine<RailGun.States, RailGun.StatesInstance, RailGun, object>.State pst;
		}

		public class CooldownStates : GameStateMachine<RailGun.States, RailGun.StatesInstance, RailGun, object>.State
		{
			public GameStateMachine<RailGun.States, RailGun.StatesInstance, RailGun, object>.State pre;

			public GameStateMachine<RailGun.States, RailGun.StatesInstance, RailGun, object>.State loop;

			public GameStateMachine<RailGun.States, RailGun.StatesInstance, RailGun, object>.State pst;
		}

		public class OnStates : GameStateMachine<RailGun.States, RailGun.StatesInstance, RailGun, object>.State
		{
			public GameStateMachine<RailGun.States, RailGun.StatesInstance, RailGun, object>.State power_on;

			public GameStateMachine<RailGun.States, RailGun.StatesInstance, RailGun, object>.State wait_for_storage;

			public GameStateMachine<RailGun.States, RailGun.StatesInstance, RailGun, object>.State power_off;

			public RailGun.States.WorkingStates working;

			public RailGun.States.CooldownStates cooldown;
		}
	}
}
