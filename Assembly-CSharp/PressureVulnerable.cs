using System;
using UnityEngine;

public class PressureVulnerable : KMonoBehaviour
{
	public float MassLowWarning
	{
		get
		{
			return this.mass_low_warning;
		}
	}

	public bool IsLethal
	{
		get
		{
			return this.pressureState == PressureVulnerable.AtmosphericPressureState.LethalHigh || this.pressureState == PressureVulnerable.AtmosphericPressureState.LethalLow;
		}
	}

	public bool IsNotNormal
	{
		get
		{
			return this.pressureState != PressureVulnerable.AtmosphericPressureState.Normal;
		}
	}

	public void Configure(float mass_lethal_low, float mass_warning_low)
	{
		this.mass_low_lethal = mass_lethal_low;
		this.mass_low_warning = mass_warning_low;
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		this.handle = GameScheduler.Instance.SchedulePeriodic(base.name, 1f, new Action<object>(this.CheckPressure), null, null, 0f);
		this.CheckPressure(null);
	}

	protected override void OnCleanUp()
	{
		this.handle.Clear();
		base.OnCleanUp();
	}

	private void CheckPressure(object data)
	{
		int num = Grid.PosToCell(base.gameObject);
		if (!Grid.IsValidCell(num))
		{
			return;
		}
		if (!this.IsCellSafe(num))
		{
			float cellMass = this.GetCellMass(num);
			if (cellMass < this.mass_low_lethal)
			{
				this.SetPressureState(PressureVulnerable.AtmosphericPressureState.LethalLow);
			}
			else
			{
				this.SetPressureState(PressureVulnerable.AtmosphericPressureState.WarningLow);
			}
		}
		else
		{
			this.SetPressureState(PressureVulnerable.AtmosphericPressureState.Normal);
		}
	}

	public bool IsCellSafe(int cell)
	{
		return this.GetCellMass(cell) >= this.mass_low_warning;
	}

	private float GetCellMass(int cell)
	{
		return Mathf.Max(Grid.Cell[cell].mass, Grid.Cell[Grid.CellAbove(cell)].mass);
	}

	private void SetPressureState(PressureVulnerable.AtmosphericPressureState newState)
	{
		if (this.pressureState == newState)
		{
			return;
		}
		this.pressureState = newState;
		KSelectable component = base.GetComponent<KSelectable>();
		switch (newState)
		{
		case PressureVulnerable.AtmosphericPressureState.LethalLow:
			this.Trigger(-593125877, null);
			break;
		case PressureVulnerable.AtmosphericPressureState.WarningLow:
			component.AddStatusItem(Db.Get().CreatureStatusItems.AtmosphericPressureTooLow, this);
			this.Trigger(-1175525437, null);
			break;
		case PressureVulnerable.AtmosphericPressureState.Normal:
			component.RemoveStatusItem(Db.Get().CreatureStatusItems.AtmosphericPressureTooLow);
			this.Trigger(-907106982, null);
			break;
		}
	}

	[SerializeField]
	private float mass_low_lethal;

	[SerializeField]
	private float mass_low_warning = 0.15f;

	private PressureVulnerable.AtmosphericPressureState pressureState = PressureVulnerable.AtmosphericPressureState.Normal;

	private SchedulerHandle handle;

	public enum AtmosphericPressureState
	{
		LethalLow,
		WarningLow,
		Normal,
		WarningHigh,
		LethalHigh
	}
}
