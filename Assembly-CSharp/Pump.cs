using System;

public class Pump : KMonoBehaviour
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
	}

	private void SimUpdate(float dt)
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
		}
		else
		{
			this.selectable.RemoveStatusItem(Db.Get().BuildingStatusItems.PumpingLiquidOrGas, false);
			this.operational.SetActive(false, false);
		}
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
		bool flag = !this.storage.IsFull() && this.IsPumpable(state, (int)this.consumer.consumptionRadius);
		this.operational.SetFlag(Pump.PumpableFlag, flag);
		StatusItem statusItem = ((state != Element.State.Gas) ? Db.Get().BuildingStatusItems.NoLiquidElementToPump : Db.Get().BuildingStatusItems.NoGasElementToPump);
		this.selectable.ToggleStatusItem(statusItem, !flag, null);
		return flag;
	}

	private bool IsPumpable(Element.State expected_state, int radius)
	{
		int num = Grid.PosToCell(this.transform.position);
		for (int i = 0; i < (int)this.consumer.consumptionRadius; i++)
		{
			for (int j = 0; j < (int)this.consumer.consumptionRadius; j++)
			{
				int num2 = num + j + Grid.WidthInCells * i;
				bool flag = Grid.Element[num2].IsState(expected_state);
				if (flag)
				{
					return true;
				}
			}
		}
		return false;
	}

	private const float OperationalUpdateInterval = 1f;

	public static Operational.Flag PumpableFlag = new Operational.Flag("vent", Operational.Flag.Type.Requirement);

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

	private float elapsedTime;

	private bool pumpable;
}
