using System;
using KSerialization;
using STRINGS;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class LogicCritterCountSensor : Switch, ISaveLoadable, ISim200ms, IIntSliderControl, ISliderControl
{
	public string SliderTitleKey
	{
		get
		{
			return "STRINGS.UI.UISIDESCREENS.CRITTER_COUNT_SIDE_SCREEN.TITLE";
		}
	}

	public string SliderUnits
	{
		get
		{
			return UI.UNITSUFFIXES.CRITTERS;
		}
	}

	public int SliderDecimalPlaces(int index)
	{
		return 0;
	}

	public float GetSliderMin(int index)
	{
		return 0f;
	}

	public float GetSliderMax(int index)
	{
		return 64f;
	}

	public float GetSliderValue(int index)
	{
		return (float)this.countThreshold;
	}

	public void SetSliderValue(float value, int index)
	{
		this.countThreshold = Mathf.RoundToInt(value);
	}

	public string GetSliderTooltipKey(int index)
	{
		return "STRINGS.UI.UISIDESCREENS.CRITTER_COUNT_SIDE_SCREEN.TOOLTIP";
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		this.selectable = base.GetComponent<KSelectable>();
		base.Subscribe<LogicCritterCountSensor>(-905833192, LogicCritterCountSensor.OnCopySettingsDelegate);
	}

	private void OnCopySettings(object data)
	{
		GameObject gameObject = (GameObject)data;
		LogicCritterCountSensor component = gameObject.GetComponent<LogicCritterCountSensor>();
		if (component != null)
		{
			this.countThreshold = component.countThreshold;
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
			int num = roomOfGameObject.cavity.creatures.Count;
			if (this.countEggs)
			{
				num += roomOfGameObject.cavity.eggs.Count;
			}
			this.SetState(num > this.countThreshold);
			if (this.selectable.HasStatusItem(Db.Get().BuildingStatusItems.NotInAnyRoom))
			{
				this.selectable.RemoveStatusItem(this.roomStatusGUID, false);
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
		LogicPorts component = base.GetComponent<LogicPorts>();
		component.SendSignal(LogicSwitch.PORT_ID, (!this.switchedOn) ? 0 : 1);
	}

	private void UpdateVisualState(bool force = false)
	{
		if (this.wasOn != this.switchedOn || force)
		{
			this.wasOn = this.switchedOn;
			KBatchedAnimController component = base.GetComponent<KBatchedAnimController>();
			component.Play((!this.switchedOn) ? "on_pst" : "on_pre", KAnim.PlayMode.Once, 1f, 0f);
			component.Queue((!this.switchedOn) ? "off" : "on", KAnim.PlayMode.Once, 1f, 0f);
		}
	}

	private bool wasOn;

	private bool countEggs = true;

	[Serialize]
	public int countThreshold;

	private KSelectable selectable;

	private Guid roomStatusGUID;

	[MyCmpAdd]
	private CopyBuildingSettings copyBuildingSettings;

	private static readonly EventSystem.IntraObjectHandler<LogicCritterCountSensor> OnCopySettingsDelegate = new EventSystem.IntraObjectHandler<LogicCritterCountSensor>(delegate(LogicCritterCountSensor component, object data)
	{
		component.OnCopySettings(data);
	});
}
