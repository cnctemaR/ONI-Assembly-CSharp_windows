using System;
using UnityEngine;

public class EnvironmentConsumer : KMonoBehaviour
{
	public bool IsCorrectEnvironment
	{
		get
		{
			return this.operational.GetFlag(EnvironmentConsumer.CorrectEnvironmentFlag);
		}
		set
		{
			this.operational.SetFlag(EnvironmentConsumer.CorrectEnvironmentFlag, value);
		}
	}

	public float Efficiency
	{
		get
		{
			return BuildingDef.GetEnergyEfficiency(this.primaryElement.Element, 0.0125f);
		}
	}

	public EnvironmentConsumer.PowerState powerState { get; private set; }

	protected override void OnPrefabInit()
	{
		this.IsCorrectEnvironment = false;
		this.cell = Grid.PosToCell(this.transform.position);
		this.Subscribe(-1643076535, new EventSystem.EventHandler(this.OnRotated));
	}

	private void OnRotated(object data)
	{
		Rotatable rotatable = (Rotatable)data;
		CellOffset rotatedCellOffset = rotatable.GetRotatedCellOffset(this.offset);
		this.cell = Grid.PosToCell(this.transform.position);
		this.cell = Grid.OffsetCell(this.cell, rotatedCellOffset);
	}

	private void SimUpdate(float dt)
	{
		Element element = Grid.Element[this.cell];
		if (element.IsGas)
		{
			if (element.id == this.consumesElement || element.lowTempTransitionTarget == this.consumesElement)
			{
				this.IsCorrectEnvironment = true;
				EnvironmentConsumer.PowerState powerState;
				if (element.lowTempTransitionTarget == this.consumesElement)
				{
					powerState = EnvironmentConsumer.PowerState.OverPowered;
				}
				else
				{
					EnvironmentConsumer.PowerState powerState2 = EnvironmentConsumer.PowerState.Powered;
					this.powerState = powerState2;
					powerState = powerState2;
				}
				this.powerState = powerState;
			}
		}
		else if (element.IsLiquid)
		{
			if (element.id == this.consumesElement || element.highTempTransitionTarget == this.consumesElement)
			{
				this.IsCorrectEnvironment = true;
				this.powerState = ((element.highTempTransitionTarget != this.consumesElement) ? EnvironmentConsumer.PowerState.Powered : EnvironmentConsumer.PowerState.UnderPowered);
			}
		}
		else
		{
			this.IsCorrectEnvironment = false;
			this.powerState = EnvironmentConsumer.PowerState.UnPowered;
		}
		if (this.operational.IsActive)
		{
			Debug.Assert(false, "jcheng: need to replace this file");
			float num = 0f;
			SimMessages.ModifyMass(this.cell, num, CellEventLogger.Instance.EnvironmentConsumerFixedUpdate, Grid.Temperature[this.cell], Grid.Element[this.cell].id);
		}
	}

	private const int SimUpdateSortKey = 1001;

	[MyCmpReq]
	private Building building;

	[MyCmpReq]
	private PrimaryElement primaryElement;

	[MyCmpGet]
	private Upgradable upgradable;

	[MyCmpReq]
	private Operational operational;

	[MyCmpGet]
	private Rotatable rotatable;

	[SerializeField]
	public Orientation offset;

	public static Operational.Flag CorrectEnvironmentFlag = new Operational.Flag("correct_environment", Operational.Flag.Type.Requirement);

	[HashedEnum]
	[SerializeField]
	public SimHashes consumesElement;

	private int cell;

	public enum PowerState
	{
		UnPowered,
		Powered,
		OverPowered,
		UnderPowered
	}
}
