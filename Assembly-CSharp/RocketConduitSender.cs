using System;
using UnityEngine;

public class RocketConduitSender : StateMachineComponent<RocketConduitSender.StatesInstance>, ISecondaryInput
{
	public void AddConduitPortToNetwork()
	{
		if (this.conduitPort == null)
		{
			return;
		}
		int num = Grid.OffsetCell(Grid.PosToCell(base.gameObject), this.conduitPortInfo.offset);
		IUtilityNetworkMgr networkManager = Conduit.GetNetworkManager(this.conduitPortInfo.conduitType);
		this.conduitPort.inputCell = num;
		this.conduitPort.networkItem = new FlowUtilityNetwork.NetworkItem(this.conduitPortInfo.conduitType, Endpoint.Sink, num, base.gameObject);
		networkManager.AddToNetworks(num, this.conduitPort.networkItem, true);
	}

	public void RemoveConduitPortFromNetwork()
	{
		if (this.conduitPort == null)
		{
			return;
		}
		Conduit.GetNetworkManager(this.conduitPortInfo.conduitType).RemoveFromNetworks(this.conduitPort.inputCell, this.conduitPort.networkItem, true);
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		this.FindPartner();
		base.Subscribe<RocketConduitSender>(-1118736034, RocketConduitSender.TryFindPartnerDelegate);
		base.Subscribe<RocketConduitSender>(546421097, RocketConduitSender.OnLaunchedDelegate);
		base.Subscribe<RocketConduitSender>(-735346771, RocketConduitSender.OnLandedDelegate);
		base.smi.StartSM();
		Components.RocketConduitSenders.Add(this);
	}

	protected override void OnCleanUp()
	{
		this.RemoveConduitPortFromNetwork();
		base.OnCleanUp();
		Components.RocketConduitSenders.Remove(this);
	}

	private void FindPartner()
	{
		WorldContainer world = ClusterManager.Instance.GetWorld(base.gameObject.GetMyWorldId());
		if (world != null && world.IsModuleInterior)
		{
			foreach (RocketConduitReceiver rocketConduitReceiver in world.GetComponent<Clustercraft>().ModuleInterface.GetPassengerModule().GetComponents<RocketConduitReceiver>())
			{
				if (rocketConduitReceiver.conduitPortInfo.conduitType == this.conduitPortInfo.conduitType)
				{
					this.partnerReceiver = rocketConduitReceiver;
					break;
				}
			}
		}
		else
		{
			ClustercraftExteriorDoor component = base.gameObject.GetComponent<ClustercraftExteriorDoor>();
			if (component.HasTargetWorld())
			{
				WorldContainer targetWorld = component.GetTargetWorld();
				foreach (RocketConduitReceiver rocketConduitReceiver2 in Components.RocketConduitReceivers.GetWorldItems(targetWorld.id, false))
				{
					if (rocketConduitReceiver2.conduitPortInfo.conduitType == this.conduitPortInfo.conduitType)
					{
						this.partnerReceiver = rocketConduitReceiver2;
						break;
					}
				}
			}
		}
		if (this.partnerReceiver == null)
		{
			global::Debug.LogWarning("No rocket conduit receiver found?");
			return;
		}
		this.conduitPort = new RocketConduitSender.ConduitPort(base.gameObject, this.conduitPortInfo, this.conduitStorage);
		if (world != null)
		{
			this.AddConduitPortToNetwork();
		}
		this.partnerReceiver.SetStorage(this.conduitStorage);
	}

	bool ISecondaryInput.HasSecondaryConduitType(ConduitType type)
	{
		return this.conduitPortInfo.conduitType == type;
	}

	CellOffset ISecondaryInput.GetSecondaryConduitOffset(ConduitType type)
	{
		if (this.conduitPortInfo.conduitType == type)
		{
			return this.conduitPortInfo.offset;
		}
		return CellOffset.none;
	}

	public Storage conduitStorage;

	[SerializeField]
	public ConduitPortInfo conduitPortInfo;

	private RocketConduitSender.ConduitPort conduitPort;

	private RocketConduitReceiver partnerReceiver;

	private static readonly EventSystem.IntraObjectHandler<RocketConduitSender> TryFindPartnerDelegate = new EventSystem.IntraObjectHandler<RocketConduitSender>(delegate(RocketConduitSender component, object data)
	{
		component.FindPartner();
	});

	private static readonly EventSystem.IntraObjectHandler<RocketConduitSender> OnLandedDelegate = new EventSystem.IntraObjectHandler<RocketConduitSender>(delegate(RocketConduitSender component, object data)
	{
		component.AddConduitPortToNetwork();
	});

	private static readonly EventSystem.IntraObjectHandler<RocketConduitSender> OnLaunchedDelegate = new EventSystem.IntraObjectHandler<RocketConduitSender>(delegate(RocketConduitSender component, object data)
	{
		component.RemoveConduitPortFromNetwork();
	});

	private class ConduitPort
	{
		public ConduitPort(GameObject parent, ConduitPortInfo info, Storage targetStorage)
		{
			this.conduitPortInfo = info;
			ConduitConsumer conduitConsumer = parent.AddComponent<ConduitConsumer>();
			conduitConsumer.conduitType = this.conduitPortInfo.conduitType;
			conduitConsumer.useSecondaryInput = true;
			conduitConsumer.storage = targetStorage;
			conduitConsumer.capacityKG = targetStorage.capacityKg;
			conduitConsumer.alwaysConsume = true;
			conduitConsumer.forceAlwaysSatisfied = true;
			this.conduitConsumer = conduitConsumer;
			this.conduitConsumer.keepZeroMassObject = false;
		}

		public ConduitPortInfo conduitPortInfo;

		public int inputCell;

		public FlowUtilityNetwork.NetworkItem networkItem;

		private ConduitConsumer conduitConsumer;
	}

	public class StatesInstance : GameStateMachine<RocketConduitSender.States, RocketConduitSender.StatesInstance, RocketConduitSender, object>.GameInstance
	{
		public StatesInstance(RocketConduitSender smi)
			: base(smi)
		{
		}
	}

	public class States : GameStateMachine<RocketConduitSender.States, RocketConduitSender.StatesInstance, RocketConduitSender>
	{
		public override void InitializeStates(out StateMachine.BaseState default_state)
		{
			default_state = this.on;
			base.serializable = StateMachine.SerializeType.Both_DEPRECATED;
			this.on.DefaultState(this.on.waiting);
			this.on.waiting.ToggleMainStatusItem(Db.Get().BuildingStatusItems.Normal, null).EventTransition(GameHashes.OnStorageChange, this.on.working, (RocketConduitSender.StatesInstance smi) => smi.GetComponent<Storage>().MassStored() > 0f);
			this.on.working.ToggleMainStatusItem(Db.Get().BuildingStatusItems.Working, null).DefaultState(this.on.working.ground);
			this.on.working.notOnGround.Enter(delegate(RocketConduitSender.StatesInstance smi)
			{
				smi.gameObject.AddOrGetDef<AutoStorageDropper.Def>().invertElementFilter = true;
			}).UpdateTransition(this.on.working.ground, delegate(RocketConduitSender.StatesInstance smi, float f)
			{
				WorldContainer myWorld = smi.master.GetMyWorld();
				return myWorld && myWorld.IsModuleInterior && !myWorld.GetComponent<Clustercraft>().ModuleInterface.GetPassengerModule().HasTag(GameTags.RocketNotOnGround);
			}, UpdateRate.SIM_200ms, false).Exit(delegate(RocketConduitSender.StatesInstance smi)
			{
				smi.gameObject.AddOrGetDef<AutoStorageDropper.Def>().invertElementFilter = false;
			});
			this.on.working.ground.Enter(delegate(RocketConduitSender.StatesInstance smi)
			{
				smi.master.partnerReceiver.conduitPort.conduitDispenser.alwaysDispense = true;
			}).UpdateTransition(this.on.working.notOnGround, delegate(RocketConduitSender.StatesInstance smi, float f)
			{
				WorldContainer myWorld2 = smi.master.GetMyWorld();
				return myWorld2 && myWorld2.IsModuleInterior && myWorld2.GetComponent<Clustercraft>().ModuleInterface.GetPassengerModule().HasTag(GameTags.RocketNotOnGround);
			}, UpdateRate.SIM_200ms, false).Exit(delegate(RocketConduitSender.StatesInstance smi)
			{
				smi.master.partnerReceiver.conduitPort.conduitDispenser.alwaysDispense = false;
			});
		}

		public RocketConduitSender.States.onStates on;

		public class onStates : GameStateMachine<RocketConduitSender.States, RocketConduitSender.StatesInstance, RocketConduitSender, object>.State
		{
			public RocketConduitSender.States.workingStates working;

			public GameStateMachine<RocketConduitSender.States, RocketConduitSender.StatesInstance, RocketConduitSender, object>.State waiting;
		}

		public class workingStates : GameStateMachine<RocketConduitSender.States, RocketConduitSender.StatesInstance, RocketConduitSender, object>.State
		{
			public GameStateMachine<RocketConduitSender.States, RocketConduitSender.StatesInstance, RocketConduitSender, object>.State notOnGround;

			public GameStateMachine<RocketConduitSender.States, RocketConduitSender.StatesInstance, RocketConduitSender, object>.State ground;
		}
	}
}
