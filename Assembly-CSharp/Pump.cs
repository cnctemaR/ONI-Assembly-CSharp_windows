using System;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/Pump")]
public class Pump : KMonoBehaviour, ISim1000ms
{
	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		this.consumer.EnableConsumption(false);
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		this.elapsedTime = 0f;
		this.pumpable = this.UpdateOperational();
		this.dispenser.GetConduitManager().AddConduitUpdater(new Action<float>(this.OnConduitUpdate), ConduitFlowPriority.LastPostUpdate);
	}

	protected override void OnCleanUp()
	{
		this.dispenser.GetConduitManager().RemoveConduitUpdater(new Action<float>(this.OnConduitUpdate));
		base.OnCleanUp();
	}

	public void Sim1000ms(float dt)
	{
		this.elapsedTime += dt;
		if (this.elapsedTime >= 1f)
		{
			this.pumpable = this.UpdateOperational();
			this.elapsedTime = 0f;
		}
		if (this.operational.IsOperational && this.pumpable)
		{
			this.operational.SetActive(true, false);
			return;
		}
		this.operational.SetActive(false, false);
	}

	private bool UpdateOperational()
	{
		Element.State state = Element.State.Vacuum;
		ConduitType conduitType = this.dispenser.conduitType;
		if (conduitType != ConduitType.Gas)
		{
			if (conduitType == ConduitType.Liquid)
			{
				state = Element.State.Liquid;
			}
		}
		else
		{
			state = Element.State.Gas;
		}
		bool flag = this.IsPumpable(state, (int)this.consumer.consumptionRadius);
		StatusItem statusItem = ((state == Element.State.Gas) ? Db.Get().BuildingStatusItems.NoGasElementToPump : Db.Get().BuildingStatusItems.NoLiquidElementToPump);
		this.noElementStatusGuid = this.selectable.ToggleStatusItem(statusItem, this.noElementStatusGuid, !flag, null);
		this.operational.SetFlag(Pump.PumpableFlag, !this.storage.IsFull() && flag);
		return flag;
	}

	private bool IsPumpable(Element.State expected_state, int radius)
	{
		int num = Grid.PosToCell(base.transform.GetPosition());
		for (int i = 0; i < (int)this.consumer.consumptionRadius; i++)
		{
			for (int j = 0; j < (int)this.consumer.consumptionRadius; j++)
			{
				int num2 = num + j + Grid.WidthInCells * i;
				if (Grid.Element[num2].IsState(expected_state))
				{
					return true;
				}
			}
		}
		return false;
	}

	private void OnConduitUpdate(float dt)
	{
		this.conduitBlockedStatusGuid = this.selectable.ToggleStatusItem(Db.Get().BuildingStatusItems.ConduitBlocked, this.conduitBlockedStatusGuid, this.dispenser.blocked, null);
	}

	public ConduitType conduitType
	{
		get
		{
			return this.dispenser.conduitType;
		}
	}

	public static readonly Operational.Flag PumpableFlag = new Operational.Flag("vent", Operational.Flag.Type.Requirement);

	[MyCmpReq]
	private Operational operational;

	[MyCmpGet]
	private KSelectable selectable;

	[MyCmpGet]
	private ElementConsumer consumer;

	[MyCmpGet]
	private ConduitDispenser dispenser;

	[MyCmpGet]
	private Storage storage;

	private const float OperationalUpdateInterval = 1f;

	private float elapsedTime;

	private bool pumpable;

	private Guid conduitBlockedStatusGuid;

	private Guid noElementStatusGuid;
}
