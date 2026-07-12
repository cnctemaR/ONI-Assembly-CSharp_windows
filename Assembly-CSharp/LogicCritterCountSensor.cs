using System;
using KSerialization;
using STRINGS;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class LogicCritterCountSensor : Switch, ISaveLoadable, IThresholdSwitch, ISim200ms
{
	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		this.selectable = base.GetComponent<KSelectable>();
		base.Subscribe<LogicCritterCountSensor>(-905833192, LogicCritterCountSensor.OnCopySettingsDelegate);
	}

	private void OnCopySettings(object data)
	{
		LogicCritterCountSensor component = ((GameObject)data).GetComponent<LogicCritterCountSensor>();
		if (component != null)
		{
			this.countThreshold = component.countThreshold;
			this.activateOnGreaterThan = component.activateOnGreaterThan;
			this.countCritters = component.countCritters;
			this.countEggs = component.countEggs;
		}
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		base.OnToggle += this.OnSwitchToggled;
		this.UpdateLogicCircuit();
		this.UpdateVisualState(true);
		this.wasOn = this.switchedOn;
	}

	public void Sim200ms(float dt)
	{
		Room roomOfGameObject = Game.Instance.roomProber.GetRoomOfGameObject(base.gameObject);
		if (roomOfGameObject != null)
		{
			this.currentCount = 0;
			if (this.countCritters)
			{
				this.currentCount += roomOfGameObject.cavity.creatures.Count;
			}
			if (this.countEggs)
			{
				this.currentCount += roomOfGameObject.cavity.eggs.Count;
			}
			bool flag = (this.activateOnGreaterThan ? (this.currentCount > this.countThreshold) : (this.currentCount < this.countThreshold));
			this.SetState(flag);
			if (this.selectable.HasStatusItem(Db.Get().BuildingStatusItems.NotInAnyRoom))
			{
				this.selectable.RemoveStatusItem(this.roomStatusGUID, false);
				return;
			}
		}
		else
		{
			if (!this.selectable.HasStatusItem(Db.Get().BuildingStatusItems.NotInAnyRoom))
			{
				this.roomStatusGUID = this.selectable.AddStatusItem(Db.Get().BuildingStatusItems.NotInAnyRoom, null);
			}
			this.SetState(false);
		}
	}

	private void OnSwitchToggled(bool toggled_on)
	{
		this.UpdateLogicCircuit();
		this.UpdateVisualState(false);
	}

	private void UpdateLogicCircuit()
	{
		base.GetComponent<LogicPorts>().SendSignal(LogicSwitch.PORT_ID, this.switchedOn ? 1 : 0);
	}

	private void UpdateVisualState(bool force = false)
	{
		if (this.wasOn != this.switchedOn || force)
		{
			this.wasOn = this.switchedOn;
			KBatchedAnimController component = base.GetComponent<KBatchedAnimController>();
			component.Play(this.switchedOn ? "on_pre" : "on_pst", KAnim.PlayMode.Once, 1f, 0f);
			component.Queue(this.switchedOn ? "on" : "off", KAnim.PlayMode.Once, 1f, 0f);
		}
	}

	protected override void UpdateSwitchStatus()
	{
		StatusItem statusItem = (this.switchedOn ? Db.Get().BuildingStatusItems.LogicSensorStatusActive : Db.Get().BuildingStatusItems.LogicSensorStatusInactive);
		base.GetComponent<KSelectable>().SetStatusItem(Db.Get().StatusItemCategories.Power, statusItem, null);
	}

	public float Threshold
	{
		get
		{
			return (float)this.countThreshold;
		}
		set
		{
			this.countThreshold = (int)value;
		}
	}

	public bool ActivateAboveThreshold
	{
		get
		{
			return this.activateOnGreaterThan;
		}
		set
		{
			this.activateOnGreaterThan = value;
		}
	}

	public float CurrentValue
	{
		get
		{
			return (float)this.currentCount;
		}
	}

	public float RangeMin
	{
		get
		{
			return 0f;
		}
	}

	public float RangeMax
	{
		get
		{
			return 64f;
		}
	}

	public float GetRangeMinInputField()
	{
		return this.RangeMin;
	}

	public float GetRangeMaxInputField()
	{
		return this.RangeMax;
	}

	public LocString Title
	{
		get
		{
			return UI.UISIDESCREENS.CRITTER_COUNT_SIDE_SCREEN.TITLE;
		}
	}

	public LocString ThresholdValueName
	{
		get
		{
			return UI.UISIDESCREENS.CRITTER_COUNT_SIDE_SCREEN.VALUE_NAME;
		}
	}

	public string AboveToolTip
	{
		get
		{
			return UI.UISIDESCREENS.CRITTER_COUNT_SIDE_SCREEN.TOOLTIP_ABOVE;
		}
	}

	public string BelowToolTip
	{
		get
		{
			return UI.UISIDESCREENS.CRITTER_COUNT_SIDE_SCREEN.TOOLTIP_BELOW;
		}
	}

	public string Format(float value, bool units)
	{
		return value.ToString();
	}

	public float ProcessedSliderValue(float input)
	{
		return Mathf.Round(input);
	}

	public float ProcessedInputValue(float input)
	{
		return Mathf.Round(input);
	}

	public LocString ThresholdValueUnits()
	{
		return "";
	}

	public ThresholdScreenLayoutType LayoutType
	{
		get
		{
			return ThresholdScreenLayoutType.SliderBar;
		}
	}

	public int IncrementScale
	{
		get
		{
			return 1;
		}
	}

	public NonLinearSlider.Range[] GetRanges
	{
		get
		{
			return NonLinearSlider.GetDefaultRange(this.RangeMax);
		}
	}

	private bool wasOn;

	[Serialize]
	public bool countEggs = true;

	[Serialize]
	public bool countCritters = true;

	[Serialize]
	public int countThreshold;

	[Serialize]
	public bool activateOnGreaterThan = true;

	[Serialize]
	private int currentCount;

	private KSelectable selectable;

	private Guid roomStatusGUID;

	[MyCmpAdd]
	private CopyBuildingSettings copyBuildingSettings;

	private static readonly EventSystem.IntraObjectHandler<LogicCritterCountSensor> OnCopySettingsDelegate = new EventSystem.IntraObjectHandler<LogicCritterCountSensor>(delegate(LogicCritterCountSensor component, object data)
	{
		component.OnCopySettings(data);
	});
}
