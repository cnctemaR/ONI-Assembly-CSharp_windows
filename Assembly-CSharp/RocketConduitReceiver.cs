using System;
using UnityEngine;

public class RocketConduitReceiver : StateMachineComponent<RocketConduitReceiver.StatesInstance>, ISecondaryOutput
{
	public void AddConduitPortToNetwork()
	{
		if (this.conduitPort.conduitDispenser == null)
		{
			return;
		}
		int num = Grid.OffsetCell(Grid.PosToCell(base.gameObject), this.conduitPortInfo.offset);
		IUtilityNetworkMgr networkManager = Conduit.GetNetworkManager(this.conduitPortInfo.conduitType);
		this.conduitPort.outputCell = num;
		this.conduitPort.networkItem = new FlowUtilityNetwork.NetworkItem(this.conduitPortInfo.conduitType, Endpoint.Source, num, base.gameObject);
		networkManager.AddToNetworks(num, this.conduitPort.networkItem, true);
	}

	public void RemoveConduitPortFromNetwork()
	{
		if (this.conduitPort.conduitDispenser == null)
		{
			return;
		}
		Conduit.GetNetworkManager(this.conduitPortInfo.conduitType).RemoveFromNetworks(this.conduitPort.outputCell, this.conduitPort.networkItem, true);
	}

	private bool CanTransferFromSender()
	{
		bool flag = false;
		if ((base.smi.master.senderConduitStorage.MassStored() > 0f || base.smi.master.senderConduitStorage.items.Count > 0) && base.smi.master.conduitPort.conduitDispenser.GetConduitManager().GetPermittedFlow(base.smi.master.conduitPort.outputCell) != ConduitFlow.FlowDirections.None)
		{
			flag = true;
		}
		return flag;
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		this.FindPartner();
		base.Subscribe<RocketConduitReceiver>(-1118736034, RocketConduitReceiver.TryFindPartner);
		base.Subscribe<RocketConduitReceiver>(546421097, RocketConduitReceiver.OnLaunchedDelegate);
		base.Subscribe<RocketConduitReceiver>(-735346771, RocketConduitReceiver.OnLandedDelegate);
		base.smi.StartSM();
		Components.RocketConduitReceivers.Add(this);
	}

	protected override void OnCleanUp()
	{
		this.RemoveConduitPortFromNetwork();
		base.OnCleanUp();
		Components.RocketConduitReceivers.Remove(this);
	}

	private void FindPartner()
	{
		if (this.senderConduitStorage != null)
		{
			return;
		}
		RocketConduitSender rocketConduitSender = null;
		WorldContainer world = ClusterManager.Instance.GetWorld(base.gameObject.GetMyWorldId());
		if (world != null && world.IsModuleInterior)
		{
			foreach (RocketConduitSender rocketConduitSender2 in world.GetComponent<Clustercraft>().ModuleInterface.GetPassengerModule().GetComponents<RocketConduitSender>())
			{
				if (rocketConduitSender2.conduitPortInfo.conduitType == this.conduitPortInfo.conduitType)
				{
					rocketConduitSender = rocketConduitSender2;
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
				foreach (RocketConduitSender rocketConduitSender3 in Components.RocketConduitSenders.GetWorldItems(targetWorld.id, false))
				{
					if (rocketConduitSender3.conduitPortInfo.conduitType == this.conduitPortInfo.conduitType)
					{
						rocketConduitSender = rocketConduitSender3;
						break;
					}
				}
			}
		}
		if (rocketConduitSender == null)
		{
			global::Debug.LogWarning("No warp conduit sender found?");
			return;
		}
		this.SetStorage(rocketConduitSender.conduitStorage);
	}

	public void SetStorage(Storage conduitStorage)
	{
		this.senderConduitStorage = conduitStorage;
		this.conduitPort.SetPortInfo(base.gameObject, this.conduitPortInfo, conduitStorage);
		if (base.gameObject.GetMyWorld() != null)
		{
			this.AddConduitPortToNetwork();
		}
	}

	bool ISecondaryOutput.HasSecondaryConduitType(ConduitType type)
	{
		return type == this.conduitPortInfo.conduitType;
	}

	CellOffset ISecondaryOutput.GetSecondaryConduitOffset(ConduitType type)
	{
		if (type == this.conduitPortInfo.conduitType)
		{
			return this.conduitPortInfo.offset;
		}
		return CellOffset.none;
	}

	[SerializeField]
	public ConduitPortInfo conduitPortInfo;

	public RocketConduitReceiver.ConduitPort conduitPort;

	public Storage senderConduitStorage;

	private static readonly EventSystem.IntraObjectHandler<RocketConduitReceiver> TryFindPartner = new EventSystem.IntraObjectHandler<RocketConduitReceiver>(delegate(RocketConduitReceiver component, object data)
	{
		component.FindPartner();
	});

	private static readonly EventSystem.IntraObjectHandler<RocketConduitReceiver> OnLandedDelegate = new EventSystem.IntraObjectHandler<RocketConduitReceiver>(delegate(RocketConduitReceiver component, object data)
	{
		component.AddConduitPortToNetwork();
	});

	private static readonly EventSystem.IntraObjectHandler<RocketConduitReceiver> OnLaunchedDelegate = new EventSystem.IntraObjectHandler<RocketConduitReceiver>(delegate(RocketConduitReceiver component, object data)
	{
		component.RemoveConduitPortFromNetwork();
	});

	public struct ConduitPort
	{
		public void SetPortInfo(GameObject parent, ConduitPortInfo info, Storage senderStorage)
		{
			this.portInfo = info;
			ConduitDispenser conduitDispenser = parent.AddComponent<ConduitDispenser>();
			conduitDispenser.conduitType = this.portInfo.conduitType;
			conduitDispenser.useSecondaryOutput = true;
			conduitDispenser.alwaysDispense = true;
			conduitDispenser.storage = senderStorage;
			this.conduitDispenser = conduitDispenser;
		}

		public ConduitPortInfo portInfo;

		public int outputCell;

		public FlowUtilityNetwork.NetworkItem networkItem;

		public ConduitDispenser conduitDispenser;
	}

	public class StatesInstance : GameStateMachine<RocketConduitReceiver.States, RocketConduitReceiver.StatesInstance, RocketConduitReceiver, object>.GameInstance
	{
		public StatesInstance(RocketConduitReceiver master)
			: base(master)
		{
		}
	}

	public class States : GameStateMachine<RocketConduitReceiver.States, RocketConduitReceiver.StatesInstance, RocketConduitReceiver>
	{
		public override void InitializeStates(out StateMachine.BaseState default_state)
		{
			default_state = this.off;
			base.serializable = StateMachine.SerializeType.Both_DEPRECATED;
			this.off.EventTransition(GameHashes.OperationalFlagChanged, this.on, (RocketConduitReceiver.StatesInstance smi) => smi.GetComponent<Operational>().GetFlag(WarpConduitStatus.warpConnectedFlag));
			this.on.DefaultState(this.on.empty);
			this.on.empty.ToggleMainStatusItem(Db.Get().BuildingStatusItems.Normal, null).Update(delegate(RocketConduitReceiver.StatesInstance smi, float dt)
			{
				if (smi.master.CanTransferFromSender())
				{
					smi.GoTo(this.on.hasResources);
				}
			}, UpdateRate.SIM_200ms, false);
			this.on.hasResources.ToggleMainStatusItem(Db.Get().BuildingStatusItems.Working, null).Update(delegate(RocketConduitReceiver.StatesInstance smi, float dt)
			{
				if (!smi.master.CanTransferFromSender())
				{
					smi.GoTo(this.on.empty);
				}
			}, UpdateRate.SIM_200ms, false);
		}

		public GameStateMachine<RocketConduitReceiver.States, RocketConduitReceiver.StatesInstance, RocketConduitReceiver, object>.State off;

		public RocketConduitReceiver.States.onStates on;

		public class onStates : GameStateMachine<RocketConduitReceiver.States, RocketConduitReceiver.StatesInstance, RocketConduitReceiver, object>.State
		{
			public GameStateMachine<RocketConduitReceiver.States, RocketConduitReceiver.StatesInstance, RocketConduitReceiver, object>.State hasResources;

			public GameStateMachine<RocketConduitReceiver.States, RocketConduitReceiver.StatesInstance, RocketConduitReceiver, object>.State empty;
		}
	}
}
