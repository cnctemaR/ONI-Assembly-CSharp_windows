using System;
using System.Collections.Generic;
using KSerialization;
using STRINGS;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class LogicMassSensor : Switch, ISaveLoadable, IThresholdSwitch
{
	protected override void OnSpawn()
	{
		base.OnSpawn();
		this.UpdateVisualState(true);
		int num = Grid.CellAbove(this.NaturalBuildingCell());
		this.solidChangedEntry = GameScenePartitioner.Instance.Add("LogicMassSensor.SolidChanged", base.gameObject, num, GameScenePartitioner.Instance.solidChangedLayer, new Action<object>(this.OnSolidChanged));
		this.pickupablesChangedEntry = GameScenePartitioner.Instance.Add("LogicMassSensor.PickupablesChanged", base.gameObject, num, GameScenePartitioner.Instance.pickupablesChangedLayer, new Action<object>(this.OnPickupablesChanged));
		this.floorSwitchActivatorChangedEntry = GameScenePartitioner.Instance.Add("LogicMassSensor.SwitchActivatorChanged", base.gameObject, num, GameScenePartitioner.Instance.floorSwitchActivatorChangedLayer, new Action<object>(this.OnActivatorsChanged));
		base.OnToggle += this.SwitchToggled;
	}

	protected override void OnCleanUp()
	{
		if (this.solidChangedEntry != null)
		{
			this.solidChangedEntry.Release();
		}
		if (this.pickupablesChangedEntry != null)
		{
			this.pickupablesChangedEntry.Release();
		}
		if (this.floorSwitchActivatorChangedEntry != null)
		{
			this.floorSwitchActivatorChangedEntry.Release();
		}
		base.OnCleanUp();
	}

	private void Update()
	{
		this.toggleCooldown = Mathf.Max(0f, this.toggleCooldown - Time.deltaTime);
		if (this.toggleCooldown == 0f)
		{
			float currentValue = this.CurrentValue;
			bool flag = ((!this.activateAboveThreshold) ? (currentValue < this.threshold) : (currentValue > this.threshold));
			if (flag != base.IsSwitchedOn)
			{
				this.Toggle();
				this.toggleCooldown = 0.15f;
			}
			this.UpdateVisualState(false);
		}
	}

	private void OnSolidChanged(object data)
	{
		int num = Grid.CellAbove(this.NaturalBuildingCell());
		if (Grid.Solid[num])
		{
			this.massSolid = Grid.Cell[num].mass;
		}
		else
		{
			this.massSolid = 0f;
		}
	}

	private void OnPickupablesChanged(object data)
	{
		float num = 0f;
		int num2 = Grid.CellAbove(this.NaturalBuildingCell());
		List<ScenePartitionerEntry> list = ListPool<ScenePartitionerEntry, GameScenePartitioner>.Allocate();
		GameScenePartitioner.Instance.GatherEntries(Grid.CellToXY(num2).x, Grid.CellToXY(num2).y, 1, 1, GameScenePartitioner.Instance.pickupablesLayer, list);
		for (int i = 0; i < list.Count; i++)
		{
			Pickupable pickupable = list[i].obj as Pickupable;
			if (!(pickupable == null))
			{
				if (!pickupable.wasAbsorbed)
				{
					num += pickupable.PrimaryElement.Mass;
				}
			}
		}
		ListPool<ScenePartitionerEntry, GameScenePartitioner>.Free(list);
		this.massPickupables = num;
	}

	private void OnActivatorsChanged(object data)
	{
		float num = 0f;
		int num2 = Grid.CellAbove(this.NaturalBuildingCell());
		List<ScenePartitionerEntry> list = ListPool<ScenePartitionerEntry, GameScenePartitioner>.Allocate();
		GameScenePartitioner.Instance.GatherEntries(Grid.CellToXY(num2).x, Grid.CellToXY(num2).y, 1, 1, GameScenePartitioner.Instance.floorSwitchActivatorLayer, list);
		for (int i = 0; i < list.Count; i++)
		{
			FloorSwitchActivator floorSwitchActivator = list[i].obj as FloorSwitchActivator;
			if (!(floorSwitchActivator == null))
			{
				num += floorSwitchActivator.PrimaryElement.Mass;
			}
		}
		ListPool<ScenePartitionerEntry, GameScenePartitioner>.Free(list);
		this.massActivators = num;
	}

	public LocString Title
	{
		get
		{
			return UI.UISIDESCREENS.THRESHOLD_SWITCH_SIDESCREEN.TITLE;
		}
	}

	public float Threshold
	{
		get
		{
			return this.threshold;
		}
		set
		{
			this.threshold = value;
		}
	}

	public bool ActivateAboveThreshold
	{
		get
		{
			return this.activateAboveThreshold;
		}
		set
		{
			this.activateAboveThreshold = value;
		}
	}

	public float CurrentValue
	{
		get
		{
			return this.massSolid + this.massPickupables + this.massActivators;
		}
	}

	public float RangeMin
	{
		get
		{
			return this.rangeMin;
		}
	}

	public float RangeMax
	{
		get
		{
			return this.rangeMax;
		}
	}

	public float GetRangeMinInputField()
	{
		return this.rangeMin;
	}

	public float GetRangeMaxInputField()
	{
		return this.rangeMax;
	}

	public LocString ThresholdValueName
	{
		get
		{
			return UI.UISIDESCREENS.THRESHOLD_SWITCH_SIDESCREEN.PRESSURE;
		}
	}

	public string AboveToolTip
	{
		get
		{
			return UI.UISIDESCREENS.THRESHOLD_SWITCH_SIDESCREEN.PRESSURE_TOOLTIP_ABOVE;
		}
	}

	public string BelowToolTip
	{
		get
		{
			return UI.UISIDESCREENS.THRESHOLD_SWITCH_SIDESCREEN.PRESSURE_TOOLTIP_BELOW;
		}
	}

	public string Format(float value, bool units)
	{
		GameUtil.MetricMassFormat metricMassFormat = GameUtil.MetricMassFormat.Kilogram;
		return GameUtil.GetFormattedMass(value, GameUtil.TimeSlice.None, metricMassFormat, units, "{0:0.#}");
	}

	public float ProcessedSliderValue(float input)
	{
		input = Mathf.Round(input);
		return input;
	}

	public float ProcessedInputValue(float input)
	{
		return input;
	}

	public LocString ThresholdValueUnits()
	{
		LocString locString = null;
		GameUtil.MassUnit massUnit = GameUtil.massUnit;
		if (massUnit != GameUtil.MassUnit.Kilograms)
		{
			if (massUnit == GameUtil.MassUnit.Pounds)
			{
				locString = UI.UNITSUFFIXES.MASS.POUND;
			}
		}
		else
		{
			locString = UI.UNITSUFFIXES.MASS.KILOGRAM;
		}
		return locString;
	}

	private void SwitchToggled(bool toggled_on)
	{
		base.GetComponent<LogicPorts>().SendSignal(LogicSwitch.PORT_ID, (!toggled_on) ? 0 : 1);
	}

	private void UpdateVisualState(bool force = false)
	{
		bool flag = this.CurrentValue > this.threshold;
		if (flag != this.was_pressed || this.was_on != base.IsSwitchedOn || force)
		{
			KBatchedAnimController component = base.GetComponent<KBatchedAnimController>();
			if (flag)
			{
				if (force)
				{
					component.Play((!base.IsSwitchedOn) ? "off_down" : "on_down", KAnim.PlayMode.Once, 1f, 0f);
				}
				else
				{
					component.Play((!base.IsSwitchedOn) ? "off_down_pre" : "on_down_pre", KAnim.PlayMode.Once, 1f, 0f);
					component.Queue((!base.IsSwitchedOn) ? "off_down" : "on_down", KAnim.PlayMode.Once, 1f, 0f);
				}
			}
			else if (force)
			{
				component.Play((!base.IsSwitchedOn) ? "off_up" : "on_up", KAnim.PlayMode.Once, 1f, 0f);
			}
			else
			{
				component.Play((!base.IsSwitchedOn) ? "off_up_pre" : "on_up_pre", KAnim.PlayMode.Once, 1f, 0f);
				component.Queue((!base.IsSwitchedOn) ? "off_up" : "on_up", KAnim.PlayMode.Once, 1f, 0f);
			}
			this.was_pressed = flag;
			this.was_on = base.IsSwitchedOn;
		}
	}

	[SerializeField]
	[Serialize]
	private float threshold;

	[SerializeField]
	[Serialize]
	private bool activateAboveThreshold = true;

	[MyCmpGet]
	private LogicPorts logicPorts;

	private bool was_pressed;

	private bool was_on;

	public float rangeMin;

	public float rangeMax = 1f;

	[Serialize]
	private float massSolid;

	[Serialize]
	private float massPickupables;

	[Serialize]
	private float massActivators;

	private const float MIN_TOGGLE_TIME = 0.15f;

	private float toggleCooldown = 0.15f;

	private GameScenePartitionerEntry solidChangedEntry;

	private GameScenePartitionerEntry pickupablesChangedEntry;

	private GameScenePartitionerEntry floorSwitchActivatorChangedEntry;
}
