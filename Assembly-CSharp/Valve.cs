using System;
using KSerialization;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class Valve : Workable, ISaveLoadable
{
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

	public float MaxFlow
	{
		get
		{
			return this.valveBase.MaxFlow;
		}
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		base.SetOffsetTable(OffsetGroups.InvertedStandardTable);
		this.synchronizeAnims = false;
		this.valveBase.CurrentFlow = this.valveBase.MaxFlow;
		this.desiredFlow = this.valveBase.MaxFlow;
	}

	protected override void OnSpawn()
	{
		this.ChangeFlow(this.desiredFlow);
		base.OnSpawn();
	}

	public void ChangeFlow(float amount)
	{
		this.desiredFlow = Mathf.Clamp(amount, 0f, this.valveBase.MaxFlow);
		KSelectable component = base.GetComponent<KSelectable>();
		component.ToggleStatusItem(Db.Get().BuildingStatusItems.PumpingLiquidOrGas, this.desiredFlow >= 0f, this.valveBase.AccumulatorHandle);
		if (DebugHandler.InstantBuildMode)
		{
			this.UpdateFlow();
		}
		else
		{
			if (this.desiredFlow == this.valveBase.CurrentFlow)
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
				this.chore = new WorkChore<Valve>(Db.Get().ChoreTypes.Toggle, this, null, null, true, null, null, null, true, null, false, null, false, true, true, PriorityScreen.PriorityClass.basic, 0, false);
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
		this.valveBase.CurrentFlow = this.desiredFlow;
		this.valveBase.UpdateAnim();
		if (this.chore != null)
		{
			this.chore.Cancel("forced complete");
		}
		this.chore = null;
		base.GetComponent<KSelectable>().RemoveStatusItem(Db.Get().BuildingStatusItems.ValveRequest, false);
	}

	[MyCmpReq]
	private ValveBase valveBase;

	[Serialize]
	private float desiredFlow = 0.5f;

	private Chore chore;
}
