using System;
using KSerialization;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class Valve : BuildingWorkable, ISaveLoadableJson
{
	public float MaxFlow
	{
		get
		{
			return this.maxFlow;
		}
	}

	public float QueuedMaxFlow
	{
		get
		{
			return (this.chore == null) ? (-1f) : this.desiredFlow;
		}
	}

	public float DesiredFlow
	{
		get
		{
			return this.desiredFlow;
		}
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		this.currentFlow = this.maxFlow;
		this.desiredFlow = this.maxFlow;
		base.SetOffsetTable(OffsetGroups.InvertedStandardTable);
		this.flowAccumulator = new Accumulator("Flow", this, 3f);
		this.outputConduitHandle = HandleVector<ConduitFlow.BuildingConduit>.InvalidHandle;
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		base.GetComponent<ConduitConsumer>().isConsuming = false;
		Building component = base.GetComponent<Building>();
		this.inputCell = component.GetUtilityInputCell();
		this.outputCell = component.GetUtilityOutputCell();
		ConduitFlow conduitFlowManager = Game.Instance.GetConduitFlowManager(this.type);
		conduitFlowManager.AddConduitUpdater(new Action<float>(this.ConduitUpdate), 0);
		this.outputConduit = new ConduitFlow.BuildingConduit(conduitFlowManager);
		this.outputConduit.cell = this.outputCell;
		conduitFlowManager.AddBuildingConduit(this.outputConduit);
		IUtilityNetworkMgr networkManager = Game.Instance.GetNetworkManager(this.type);
		if (this.itemInput != null)
		{
			networkManager.RemoveFromNetworks(this.itemInput.Cell, this.itemInput);
		}
		if (this.itemOutput != null)
		{
			networkManager.RemoveFromNetworks(this.itemOutput.Cell, this.itemOutput);
		}
		this.itemInput = new FlowUtilityNetwork.NetworkItem(this.type, Vent.Endpoint.Sink, this.inputCell, 1000, null);
		this.itemOutput = new FlowUtilityNetwork.NetworkItem(this.type, Vent.Endpoint.Source, this.outputCell, 1000, null);
		Vent[] components = base.GetComponents<Vent>();
		foreach (Vent vent in components)
		{
			if (vent != null && vent.endpointType == Vent.Endpoint.Source)
			{
				vent.SortKey = 1000;
			}
		}
		networkManager.AddToNetworks(this.inputCell, this.itemInput);
		networkManager.AddToNetworks(this.outputCell, this.itemOutput);
		this.ChangeFlow(this.desiredFlow);
		this.UpdateAnim();
		this.OnCmpEnable();
	}

	protected override void OnCleanUp()
	{
		ConduitFlow conduitFlowManager = Game.Instance.GetConduitFlowManager(this.type);
		conduitFlowManager.RemoveConduitUpdater(new Action<float>(this.ConduitUpdate));
		conduitFlowManager.RemoveBuildingConduit(this.outputConduitHandle);
		IUtilityNetworkMgr networkManager = Game.Instance.GetNetworkManager(this.type);
		networkManager.RemoveFromNetworks(this.itemInput.Cell, this.itemInput);
		networkManager.RemoveFromNetworks(this.itemOutput.Cell, this.itemOutput);
	}

	private void ConduitUpdate(float dt)
	{
		ConduitFlow conduitFlowManager = Game.Instance.GetConduitFlowManager(this.type);
		ConduitFlow.Conduit conduit = conduitFlowManager.GetConduit(this.inputCell);
		if (conduit == null)
		{
			return;
		}
		if (this.outputConduit.GetContents().element == SimHashes.Vacuum)
		{
			ConduitFlow.ConduitContents contents = conduit.GetContents();
			float num = Mathf.Min(contents.mass, this.desiredFlow * dt);
			this.flowAccumulator.Accumulate(num);
			if (num > 0f)
			{
				ConduitFlow.ConduitContents conduitContents = contents;
				conduitContents.mass = num;
				this.outputConduit.SetContents(conduitContents);
				conduitFlowManager.RemoveElement(this.inputCell, num);
			}
		}
	}

	public void ChangeFlow(float amount)
	{
		this.desiredFlow = Mathf.Max(0f, Mathf.Min(this.maxFlow, amount));
		base.GetComponent<KSelectable>().SetStatusItem(Db.Get().StatusItemCategories.Main, Db.Get().BuildingStatusItems.Valve, this);
		base.GetComponent<KSelectable>().AddStatusItem(Db.Get().BuildingStatusItems.PumpingLiquidOrGas, this.flowAccumulator);
		if (this.desiredFlow == this.currentFlow)
		{
			if (this.chore != null)
			{
				this.chore.Cancel("desiredFlow == currentFlow");
				this.chore = null;
			}
			return;
		}
		base.GetComponent<KSelectable>().AddStatusItem(Db.Get().BuildingStatusItems.ValveRequest, this);
		if (DebugHandler.InstantBuildMode)
		{
			this.UpdateFlow();
		}
		else if (this.chore == null)
		{
			this.chore = new WorkChore<Valve>(Db.Get().ChoreTypes.Toggle, this, null, true, null, null, null, true, null, true, default(Tag), null, false, true);
		}
	}

	protected override void OnCompleteWork(Worker worker)
	{
		base.OnCompleteWork(worker);
		this.UpdateFlow();
	}

	public void UpdateFlow()
	{
		base.GetComponent<KSelectable>().RemoveStatusItem(Db.Get().BuildingStatusItems.ValveRequest);
		this.currentFlow = this.desiredFlow;
		this.UpdateAnim();
		if (this.chore != null)
		{
			this.chore.Cancel("forced complete");
		}
		this.chore = null;
	}

	private void UpdateAnim()
	{
		float avgFlowRate = this.flowAccumulator.AvgFlowRate;
		for (int i = this.animFlowRanges.Length - 1; i >= 0; i--)
		{
			if (this.animFlowRanges[i].minFlow <= avgFlowRate)
			{
				if (this.curFlowIdx != i)
				{
					this.curFlowIdx = i;
					this.controller.Play(this.animFlowRanges[i].animName, (avgFlowRate > 0f) ? KAnim.PlayMode.Loop : KAnim.PlayMode.Once, 1f, 0f);
				}
				break;
			}
		}
	}

	protected override void OnCmpEnable()
	{
		base.OnCmpEnable();
		if (base.isSpawned && !this.updateHandle.IsValid)
		{
			this.updateHandle = GameScheduler.Instance.SchedulePeriodic("ValveFlowUpdate", 1.5f, new Action<object>(Valve.UpdateAnimationCB), this, null, 0f);
		}
	}

	protected override void OnCmpDisable()
	{
		if (this.updateHandle.IsValid)
		{
			this.updateHandle.Clear();
		}
		base.OnCmpDisable();
	}

	private static void UpdateAnimationCB(object instance)
	{
		Valve valve = (Valve)instance;
		valve.UpdateAnim();
	}

	private const float UpdateInterval = 1.5f;

	[SerializeField]
	public Vent.Transfer type;

	[SerializeField]
	public float smallAmount;

	[SerializeField]
	public float largeAmount;

	[SerializeField]
	[Serialize]
	public float maxFlow = 0.5f;

	[SerializeField]
	[Serialize]
	public float minFlow = 0.05f;

	[Serialize]
	private float desiredFlow = 0.5f;

	[MyCmpAdd]
	protected UserMenu userMenu;

	[MyCmpGet]
	private KBatchedAnimController controller;

	private Accumulator flowAccumulator;

	private SchedulerHandle updateHandle;

	private int curFlowIdx = -1;

	[SerializeField]
	public Valve.AnimRangeInfo[] animFlowRanges;

	private int inputCell;

	private int outputCell;

	private FlowUtilityNetwork.NetworkItem itemInput;

	private FlowUtilityNetwork.NetworkItem itemOutput;

	public float currentFlow;

	private Chore chore;

	[Serialize]
	private ConduitFlow.BuildingConduit outputConduit;

	private HandleVector<ConduitFlow.BuildingConduit>.Handle outputConduitHandle;

	[Serializable]
	public struct AnimRangeInfo
	{
		public AnimRangeInfo(float min_flow, string anim_name)
		{
			this.minFlow = min_flow;
			this.animName = anim_name;
		}

		public float minFlow;

		public string animName;
	}
}
