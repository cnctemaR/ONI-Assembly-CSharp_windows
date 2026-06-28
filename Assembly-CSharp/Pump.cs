using System;

public class Pump : KMonoBehaviour
{
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
			this.selectable.RemoveStatusItem(Db.Get().BuildingStatusItems.PumpingLiquidOrGas);
			this.operational.SetActive(false, false);
		}
	}

	private bool UpdateOperational()
	{
		bool flag = false;
		int num = Grid.PosToCell(this.transform.position);
		ConduitType conduitType = this.dispenser.conduitType;
		if (conduitType != ConduitType.Gas)
		{
			if (conduitType == ConduitType.Liquid)
			{
				flag = Grid.Element[num].IsLiquid;
				this.operational.SetFlag(Pump.PumpableFlag, flag);
				this.selectable.ToggleStatusItem(Db.Get().BuildingStatusItems.NoLiquidElementToPump, !flag, null);
			}
		}
		else
		{
			flag = Grid.Element[num].IsGas;
			this.operational.SetFlag(Pump.PumpableFlag, flag);
			this.selectable.ToggleStatusItem(Db.Get().BuildingStatusItems.NoGasElementToPump, !flag, null);
		}
		return flag;
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

	private float elapsedTime;

	private bool pumpable;
}
