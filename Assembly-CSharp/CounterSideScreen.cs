using System;
using STRINGS;
using UnityEngine;

public class CounterSideScreen : SideScreenContent, IRender200ms
{
	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		this.resetButton.onClick += this.ResetCounter;
		this.incrementMaxButton.onClick += this.IncrementMaxCount;
		this.decrementMaxButton.onClick += this.DecrementMaxCount;
		this.incrementModeButton.onClick += this.ToggleMode;
		this.maxCountInput.onEndEdit += delegate
		{
			this.UpdateMaxCountFromTextInput(this.maxCountInput.currentValue);
		};
		this.UpdateCurrentCountLabel(this.targetLogicCounter.currentCount);
	}

	public override bool IsValidForTarget(GameObject target)
	{
		return target.GetComponent<LogicCounter>() != null;
	}

	public override void SetTarget(GameObject target)
	{
		base.SetTarget(target);
		this.targetLogicCounter = target.GetComponent<LogicCounter>();
		this.maxCountInput.SetDisplayValue(this.targetLogicCounter.maxCount.ToString());
		this.maxCountInput.minValue = 0f;
		this.maxCountInput.maxValue = 9f;
		this.incrementModeButton.GetComponentInChildren<LocText>().text = (this.targetLogicCounter.increment ? UI.UISIDESCREENS.COUNTER_SIDE_SCREEN.INCREMENT_MODE : UI.UISIDESCREENS.COUNTER_SIDE_SCREEN.DECREMENT_MODE);
	}

	public void Render200ms(float dt)
	{
		if (this.targetLogicCounter == null)
		{
			return;
		}
		this.UpdateCurrentCountLabel(this.targetLogicCounter.currentCount);
	}

	private void UpdateCurrentCountLabel(int value)
	{
		string text = value.ToString();
		if (value == this.targetLogicCounter.maxCount)
		{
			text = UI.FormatAsAutomationState(text, UI.AutomationState.Active);
		}
		else
		{
			text = UI.FormatAsAutomationState(text, UI.AutomationState.Standby);
		}
		this.currentCount.text = text;
	}

	private void UpdateMaxCountLabel(int value)
	{
		this.maxCountInput.SetAmount((float)value);
	}

	private void UpdateMaxCountFromTextInput(float newValue)
	{
		this.targetLogicCounter.maxCount = (int)newValue;
	}

	private void IncrementMaxCount()
	{
		int num = this.targetLogicCounter.maxCount + 1;
		num = ((num == 10) ? 0 : num);
		this.targetLogicCounter.maxCount = num;
		this.targetLogicCounter.SetCounterState();
		this.targetLogicCounter.UpdateLogicCircuit();
		this.targetLogicCounter.UpdateVisualState(true);
		this.UpdateMaxCountLabel(num);
	}

	private void DecrementMaxCount()
	{
		int num = this.targetLogicCounter.maxCount - 1;
		num = ((num < 0) ? 9 : num);
		this.targetLogicCounter.maxCount = num;
		this.targetLogicCounter.SetCounterState();
		this.targetLogicCounter.UpdateLogicCircuit();
		this.targetLogicCounter.UpdateVisualState(true);
		this.UpdateMaxCountLabel(num);
	}

	private void ResetCounter()
	{
		this.targetLogicCounter.ResetCounter();
	}

	private void ToggleMode()
	{
		this.targetLogicCounter.increment = !this.targetLogicCounter.increment;
		this.incrementModeButton.GetComponentInChildren<LocText>().text = (this.targetLogicCounter.increment ? UI.UISIDESCREENS.COUNTER_SIDE_SCREEN.INCREMENT_MODE : UI.UISIDESCREENS.COUNTER_SIDE_SCREEN.DECREMENT_MODE);
	}

	public LogicCounter targetLogicCounter;

	public KButton resetButton;

	public KButton incrementMaxButton;

	public KButton decrementMaxButton;

	public KButton incrementModeButton;

	public LocText currentCount;

	[SerializeField]
	private KNumberInputField maxCountInput;
}
