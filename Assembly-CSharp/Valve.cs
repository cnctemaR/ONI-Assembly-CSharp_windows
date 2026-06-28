using System;
using KSerialization;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class Valve : Workable, ISaveLoadable
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
		this.synchronizeAnims = false;
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		Building component = base.GetComponent<Building>();
		this.inputCell = component.GetUtilityInputCell();
		this.outputCell = component.GetUtilityOutputCell();
		Conduit.GetFlowManager(this.conduitType).AddConduitUpdater(new Action<float>(this.ConduitUpdate), ConduitFlow.Priority.Default);
		this.ChangeFlow(this.desiredFlow);
		this.UpdateAnim();
		this.OnCmpEnable();
	}

	protected override void OnCleanUp()
	{
		Conduit.GetFlowManager(this.conduitType).RemoveConduitUpdater(new Action<float>(this.ConduitUpdate));
		base.OnCleanUp();
	}

	private void ConduitUpdate(float dt)
	{
		ConduitFlow flowManager = Conduit.GetFlowManager(this.conduitType);
		ConduitFlow.Conduit conduit = flowManager.GetConduit(this.inputCell);
		ConduitFlow.Conduit conduit2 = flowManager.GetConduit(this.outputCell);
		if (conduit == null || conduit2 == null)
		{
			this.UpdateAnim();
			return;
		}
		ConduitFlow.ConduitContents contents = conduit.GetContents();
		float num = Mathf.Min(contents.mass, this.currentFlow * dt);
		if (num > 0f)
		{
			float num2 = flowManager.AddElement(this.outputCell, contents.element, num, contents.temperature, contents.diseaseIdx, contents.diseaseCount);
			this.flowAccumulator.Accumulate(num2);
			if (num2 > 0f)
			{
				flowManager.RemoveElement(this.inputCell, num2);
			}
		}
		this.UpdateAnim();
	}

	public void ChangeFlow(float amount)
	{
		this.desiredFlow = Mathf.Clamp(amount, 0f, this.maxFlow);
		KSelectable component = base.GetComponent<KSelectable>();
		component.ToggleStatusItem(Db.Get().BuildingStatusItems.PumpingLiquidOrGas, this.desiredFlow >= 0f, this.flowAccumulator);
		if (DebugHandler.InstantBuildMode)
		{
			this.UpdateFlow();
		}
		else
		{
			if (this.desiredFlow == this.currentFlow)
			{
				if (this.chore != null)
				{
					this.chore.Cancel("desiredFlow == currentFlow");
					this.chore = null;
				}
				component.RemoveStatusItem(Db.Get().BuildingStatusItems.ValveRequest, false);
				return;
			}
			if (this.chore == null)
			{
				component.AddStatusItem(Db.Get().BuildingStatusItems.ValveRequest, this);
				this.chore = new WorkChore<Valve>(Db.Get().ChoreTypes.Toggle, this, null, true, null, null, null, true, null, false, default(Tag), null, false, true, true);
			}
		}
	}

	protected override void OnCompleteWork(Worker worker)
	{
		base.OnCompleteWork(worker);
		this.UpdateFlow();
	}

	public void UpdateFlow()
	{
		this.currentFlow = this.desiredFlow;
		this.UpdateAnim();
		if (this.chore != null)
		{
			this.chore.Cancel("forced complete");
		}
		this.chore = null;
		base.GetComponent<KSelectable>().RemoveStatusItem(Db.Get().BuildingStatusItems.ValveRequest, false);
	}

	private void UpdateAnim()
	{
		float avgRate = this.flowAccumulator.AvgRate;
		if (avgRate > 0f)
		{
			for (int i = 0; i < this.animFlowRanges.Length; i++)
			{
				if (avgRate <= this.animFlowRanges[i].minFlow)
				{
					if (this.curFlowIdx != i)
					{
						this.curFlowIdx = i;
						this.controller.Play(this.animFlowRanges[i].animName, (avgRate > 0f) ? KAnim.PlayMode.Loop : KAnim.PlayMode.Once, 1f, 0f);
					}
					break;
				}
			}
		}
		else
		{
			this.controller.Play("off", KAnim.PlayMode.Once, 1f, 0f);
		}
	}

	[SerializeField]
	public ConduitType conduitType;

	[SerializeField]
	public float smallAmount;

	[SerializeField]
	public float largeAmount;

	[SerializeField]
	public float maxFlow = 0.5f;

	[MyCmpAdd]
	protected UserMenu userMenu;

	[MyCmpGet]
	private KBatchedAnimController controller;

	private Accumulator flowAccumulator;

	private int curFlowIdx = -1;

	private int inputCell;

	private int outputCell;

	[SerializeField]
	public Valve.AnimRangeInfo[] animFlowRanges;

	[Serialize]
	private float currentFlow;

	[Serialize]
	private float desiredFlow = 0.5f;

	private Chore chore;

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
