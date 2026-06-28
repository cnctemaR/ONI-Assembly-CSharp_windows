using System;
using KSerialization;
using STRINGS;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class LogicTimeOfDaySensor : Switch, ISaveLoadable, IDualSliderControl, ISliderControl
{
	public string SliderTitleKey
	{
		get
		{
			return "STRINGS.UI.UISIDESCREENS.TIME_OF_DAY_SIDE_SCREEN.TITLE";
		}
	}

	public string SliderUnits
	{
		get
		{
			return UI.UNITSUFFIXES.PERCENT;
		}
	}

	public float GetSliderMin(int index)
	{
		return 0f;
	}

	public float GetSliderMax(int index)
	{
		return 100f;
	}

	public float GetSliderValue(int index)
	{
		return (index != 0) ? (this.maxTime * 100f) : (this.minTime * 100f);
	}

	public void SetSliderValue(float percent, int index)
	{
		if (index == 0)
		{
			this.minTime = percent / 100f;
		}
		else
		{
			this.maxTime = percent / 100f;
		}
	}

	public string GetSliderTooltipKey(int index)
	{
		return "STRINGS.UI.UISIDESCREENS.TIME_OF_DAY_SIDE_SCREEN.TOOLTIP";
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		base.OnToggle += this.OnSwitchToggled;
		this.UpdateLogicCircuit();
		this.UpdateVisualState(true);
		this.wasOn = this.switchedOn;
	}

	private void SimUpdate(float dt)
	{
		float currentDayAsPercentage = GameClock.Instance.GetCurrentDayAsPercentage();
		bool flag = false;
		if (this.minTime > this.maxTime)
		{
			if (currentDayAsPercentage > this.minTime || currentDayAsPercentage < this.maxTime)
			{
				flag = true;
			}
		}
		else if (currentDayAsPercentage > this.minTime && currentDayAsPercentage < this.maxTime)
		{
			flag = true;
		}
		this.SetState(flag);
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

	[SerializeField]
	[Serialize]
	private float minTime;

	[SerializeField]
	[Serialize]
	private float maxTime = 1f;

	private bool wasOn;
}
