using System;
using UnityEngine;

public class WarpConduitReceiver : StateMachineComponent<WarpConduitReceiver.StatesInstance>, ISecondaryOutput
{
	private bool IsReceiving()
	{
		return base.smi.master.gasPort.IsOn() || base.smi.master.liquidPort.IsOn() || base.smi.master.solidPort.IsOn();
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		this.FindPartner();
		if (this.solidPort.solidDispenser != null)
		{
			this.solidPort.solidDispenser.solidOnly = true;
		}
		base.smi.StartSM();
	}

	private void FindPartner()
	{
		if (this.senderGasStorage != null)
		{
			return;
		}
		WarpConduitSender warpConduitSender = null;
		SaveGame.Instance.GetComponent<WorldGenSpawner>().SpawnTag("WarpConduitSender");
		foreach (WarpConduitSender warpConduitSender2 in global::UnityEngine.Object.FindObjectsOfType<WarpConduitSender>())
		{
			if (warpConduitSender2.GetMyWorldId() != this.GetMyWorldId())
			{
				warpConduitSender = warpConduitSender2;
				break;
			}
		}
		if (warpConduitSender == null)
		{
			global::Debug.LogWarning("No warp conduit sender found - maybe POI stomping or failure to spawn?");
			return;
		}
		this.SetStorage(warpConduitSender.gasStorage, warpConduitSender.liquidStorage, warpConduitSender.solidStorage);
		WarpConduitStatus.UpdateWarpConduitsOperational(warpConduitSender.gameObject, base.gameObject);
	}

	protected override void OnCleanUp()
	{
		Conduit.GetNetworkManager(this.liquidPortInfo.conduitType).RemoveFromNetworks(this.liquidPort.outputCell, this.liquidPort.networkItem, true);
		if (this.gasPort.portInfo != null)
		{
			Conduit.GetNetworkManager(this.gasPort.portInfo.conduitType).RemoveFromNetworks(this.gasPort.outputCell, this.gasPort.networkItem, true);
		}
		else
		{
			global::Debug.LogWarning("Conduit Receiver gasPort portInfo is null in OnCleanUp");
		}
		Game.Instance.solidConduitSystem.RemoveFromNetworks(this.solidPort.outputCell, this.solidPort.networkItem, true);
		base.OnCleanUp();
	}

	public void OnActivatedChanged(object _)
	{
		if (this.senderGasStorage == null)
		{
			this.FindPartner();
		}
		WarpConduitStatus.UpdateWarpConduitsOperational((this.senderGasStorage != null) ? this.senderGasStorage.gameObject : null, base.gameObject);
	}

	public void SetStorage(Storage gasStorage, Storage liquidStorage, Storage solidStorage)
	{
		this.senderGasStorage = gasStorage;
		this.senderLiquidStorage = liquidStorage;
		this.senderSolidStorage = solidStorage;
		this.gasPort.SetPortInfo(base.gameObject, this.gasPortInfo, gasStorage, 1);
		this.liquidPort.SetPortInfo(base.gameObject, this.liquidPortInfo, liquidStorage, 2);
		this.solidPort.SetPortInfo(base.gameObject, this.solidPortInfo, solidStorage, 3);
		Vector3 position = this.liquidPort.airlock.gameObject.transform.position;
		this.liquidPort.airlock.gameObject.GetComponent<KBatchedAnimController>().transform.position = position + new Vector3(0f, 0f, -0.1f);
		this.liquidPort.airlock.gameObject.GetComponent<KBatchedAnimController>().enabled = false;
		this.liquidPort.airlock.gameObject.GetComponent<KBatchedAnimController>().enabled = true;
	}

	public bool HasSecondaryConduitType(ConduitType type)
	{
		return type == this.gasPortInfo.conduitType || type == this.liquidPortInfo.conduitType || type == this.solidPortInfo.conduitType;
	}

	public CellOffset GetSecondaryConduitOffset(ConduitType type)
	{
		if (type == this.gasPortInfo.conduitType)
		{
			return this.gasPortInfo.offset;
		}
		if (type == this.liquidPortInfo.conduitType)
		{
			return this.liquidPortInfo.offset;
		}
		if (type == this.solidPortInfo.conduitType)
		{
			return this.solidPortInfo.offset;
		}
		return CellOffset.none;
	}

	[SerializeField]
	public ConduitPortInfo liquidPortInfo;

	private WarpConduitReceiver.ConduitPort liquidPort;

	[SerializeField]
	public ConduitPortInfo solidPortInfo;

	private WarpConduitReceiver.ConduitPort solidPort;

	[SerializeField]
	public ConduitPortInfo gasPortInfo;

	private WarpConduitReceiver.ConduitPort gasPort;

	public Storage senderGasStorage;

	public Storage senderLiquidStorage;

	public Storage senderSolidStorage;

	public struct ConduitPort
	{
		public void SetPortInfo(GameObject parent, ConduitPortInfo info, Storage senderStorage, int number)
		{
			this.portInfo = info;
			this.outputCell = Grid.OffsetCell(Grid.PosToCell(parent), this.portInfo.offset);
			if (this.portInfo.conduitType != ConduitType.Solid)
			{
				ConduitDispenser conduitDispenser = parent.AddComponent<ConduitDispenser>();
				conduitDispenser.conduitType = this.portInfo.conduitType;
				conduitDispenser.useSecondaryOutput = true;
				conduitDispenser.alwaysDispense = true;
				conduitDispenser.storage = senderStorage;
				this.dispenser = conduitDispenser;
				IUtilityNetworkMgr networkManager = Conduit.GetNetworkManager(this.portInfo.conduitType);
				this.networkItem = new FlowUtilityNetwork.NetworkItem(this.portInfo.conduitType, Endpoint.Source, this.outputCell, parent);
				networkManager.AddToNetworks(this.outputCell, this.networkItem, true);
			}
			else
			{
				SolidConduitDispenser solidConduitDispenser = parent.AddComponent<SolidConduitDispenser>();
				solidConduitDispenser.storage = senderStorage;
				solidConduitDispenser.alwaysDispense = true;
				solidConduitDispenser.useSecondaryOutput = true;
				this.solidDispenser = solidConduitDispenser;
				this.networkItem = new FlowUtilityNetwork.NetworkItem(ConduitType.Solid, Endpoint.Source, this.outputCell, parent);
				Game.Instance.solidConduitSystem.AddToNetworks(this.outputCell, this.networkItem, true);
			}
			string text = "airlock_" + number.ToString();
			string text2 = "airlock_target_" + number.ToString();
			this.pre = "airlock_" + number.ToString() + "_pre";
			this.loop = "airlock_" + number.ToString() + "_loop";
			this.pst = "airlock_" + number.ToString() + "_pst";
			this.airlock = new MeterController(parent.GetComponent<KBatchedAnimController>(), text2, text, Meter.Offset.Infront, Grid.SceneLayer.NoLayer, new string[] { text2 });
		}

		public bool IsOn()
		{
			if (this.solidDispenser != null)
			{
				return this.solidDispenser.IsDispensing;
			}
			return this.dispenser != null && !this.dispenser.blocked && !this.dispenser.empty;
		}

		public void UpdatePortAnim()
		{
			bool flag = this.IsOn();
			if (flag != this.open)
			{
				this.open = flag;
				if (this.open)
				{
					this.airlock.meterController.Play(this.pre, KAnim.PlayMode.Once, 1f, 0f);
					this.airlock.meterController.Queue(this.loop, KAnim.PlayMode.Loop, 1f, 0f);
					return;
				}
				this.airlock.meterController.Play(this.pst, KAnim.PlayMode.Once, 1f, 0f);
			}
		}

		public ConduitPortInfo portInfo;

		public int outputCell;

		public FlowUtilityNetwork.NetworkItem networkItem;

		public ConduitDispenser dispenser;

		public SolidConduitDispenser solidDispenser;

		public MeterController airlock;

		private bool open;

		private string pre;

		private string loop;

		private string pst;
	}

	public class StatesInstance : GameStateMachine<WarpConduitReceiver.States, WarpConduitReceiver.StatesInstance, WarpConduitReceiver, object>.GameInstance
	{
		public StatesInstance(WarpConduitReceiver master)
			: base(master)
		{
		}
	}

	public class States : GameStateMachine<WarpConduitReceiver.States, WarpConduitReceiver.StatesInstance, WarpConduitReceiver>
	{
		public override void InitializeStates(out StateMachine.BaseState default_state)
		{
			default_state = this.off;
			base.serializable = StateMachine.SerializeType.Both_DEPRECATED;
			this.root.EventHandler(GameHashes.BuildingActivated, delegate(WarpConduitReceiver.StatesInstance smi, object data)
			{
				smi.master.OnActivatedChanged(data);
			});
			this.off.PlayAnim("off").Enter(delegate(WarpConduitReceiver.StatesInstance smi)
			{
				smi.master.gasPort.UpdatePortAnim();
				smi.master.liquidPort.UpdatePortAnim();
				smi.master.solidPort.UpdatePortAnim();
			}).EventTransition(GameHashes.OperationalFlagChanged, this.on, (WarpConduitReceiver.StatesInstance smi) => smi.GetComponent<Operational>().GetFlag(WarpConduitStatus.warpConnectedFlag));
			this.on.DefaultState(this.on.idle).Update(delegate(WarpConduitReceiver.StatesInstance smi, float dt)
			{
				smi.master.gasPort.UpdatePortAnim();
				smi.master.liquidPort.UpdatePortAnim();
				smi.master.solidPort.UpdatePortAnim();
			}, UpdateRate.SIM_1000ms, false);
			this.on.idle.QueueAnim("idle", false, null).ToggleMainStatusItem(Db.Get().BuildingStatusItems.Normal, null).Update(delegate(WarpConduitReceiver.StatesInstance smi, float dt)
			{
				if (smi.master.IsReceiving())
				{
					smi.GoTo(this.on.working);
				}
			}, UpdateRate.SIM_1000ms, false);
			this.on.working.PlayAnim("working_pre").QueueAnim("working_loop", true, null).ToggleMainStatusItem(Db.Get().BuildingStatusItems.Working, null)
				.Update(delegate(WarpConduitReceiver.StatesInstance smi, float dt)
				{
					if (!smi.master.IsReceiving())
					{
						smi.GoTo(this.on.idle);
					}
				}, UpdateRate.SIM_1000ms, false)
				.Exit(delegate(WarpConduitReceiver.StatesInstance smi)
				{
					smi.Play("working_pst", KAnim.PlayMode.Once);
				});
		}

		public GameStateMachine<WarpConduitReceiver.States, WarpConduitReceiver.StatesInstance, WarpConduitReceiver, object>.State off;

		public WarpConduitReceiver.States.onStates on;

		public class onStates : GameStateMachine<WarpConduitReceiver.States, WarpConduitReceiver.StatesInstance, WarpConduitReceiver, object>.State
		{
			public GameStateMachine<WarpConduitReceiver.States, WarpConduitReceiver.StatesInstance, WarpConduitReceiver, object>.State working;

			public GameStateMachine<WarpConduitReceiver.States, WarpConduitReceiver.StatesInstance, WarpConduitReceiver, object>.State idle;
		}
	}
}
