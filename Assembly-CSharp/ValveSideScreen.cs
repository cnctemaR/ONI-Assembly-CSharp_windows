using System;
using System.Collections;
using STRINGS;
using UnityEngine;

public class ValveSideScreen : SideScreenContent
{
	protected override void OnSpawn()
	{
		base.OnSpawn();
		this.unitsLabel.text = GameUtil.AddTimeSliceText(UI.UNITSUFFIXES.MASS.GRAM, GameUtil.TimeSlice.PerSecond);
		this.flowSlider.onReleaseHandle += this.OnReleaseHandle;
		this.flowSlider.onDrag += delegate
		{
			this.ReceiveValueFromSlider(this.flowSlider.value);
		};
		this.flowSlider.onPointerDown += delegate
		{
			this.ReceiveValueFromSlider(this.flowSlider.value);
		};
		this.flowSlider.onMove += delegate
		{
			this.ReceiveValueFromSlider(this.flowSlider.value);
			this.OnReleaseHandle();
		};
		this.numberInput.onEndEdit += delegate
		{
			this.ReceiveValueFromInput(this.numberInput.currentValue);
		};
		this.numberInput.decimalPlaces = 1;
	}

	public void OnReleaseHandle()
	{
		this.targetValve.ChangeFlow(this.targetFlow);
	}

	public override bool IsValidForTarget(GameObject target)
	{
		return target.GetComponent<Valve>() != null;
	}

	public override void SetTarget(GameObject target)
	{
		this.targetValve = target.GetComponent<Valve>();
		if (this.targetValve == null)
		{
			global::Debug.LogError("The target object does not have a Valve component.");
			return;
		}
		this.flowSlider.minValue = 0f;
		this.flowSlider.maxValue = this.targetValve.MaxFlow;
		this.flowSlider.value = this.targetValve.DesiredFlow;
		this.minFlowLabel.text = GameUtil.GetFormattedMass(0f, GameUtil.TimeSlice.PerSecond, GameUtil.MetricMassFormat.Gram, true, "{0:0.#}");
		this.maxFlowLabel.text = GameUtil.GetFormattedMass(this.targetValve.MaxFlow, GameUtil.TimeSlice.PerSecond, GameUtil.MetricMassFormat.Gram, true, "{0:0.#}");
		this.numberInput.minValue = 0f;
		this.numberInput.maxValue = this.targetValve.MaxFlow * 1000f;
		this.numberInput.SetDisplayValue(GameUtil.GetFormattedMass(Mathf.Max(0f, this.targetValve.DesiredFlow), GameUtil.TimeSlice.PerSecond, GameUtil.MetricMassFormat.Gram, false, "{0:0.#####}"));
		this.numberInput.Activate();
	}

	private void ReceiveValueFromSlider(float newValue)
	{
		newValue = Mathf.Round(newValue * 1000f) / 1000f;
		this.UpdateFlowValue(newValue);
	}

	private void ReceiveValueFromInput(float input)
	{
		float num = input / 1000f;
		this.UpdateFlowValue(num);
		this.targetValve.ChangeFlow(this.targetFlow);
	}

	private void UpdateFlowValue(float newValue)
	{
		this.targetFlow = newValue;
		this.flowSlider.value = newValue;
		this.numberInput.SetDisplayValue(GameUtil.GetFormattedMass(newValue, GameUtil.TimeSlice.PerSecond, GameUtil.MetricMassFormat.Gram, false, "{0:0.#####}"));
	}

	private IEnumerator SettingDelay(float delay)
	{
		float startTime = Time.realtimeSinceStartup;
		float currentTime = startTime;
		while (currentTime < startTime + delay)
		{
			currentTime += Time.unscaledDeltaTime;
			yield return new WaitForEndOfFrame();
		}
		this.OnReleaseHandle();
		yield break;
	}

	public override void OnKeyDown(KButtonEvent e)
	{
		global::Debug.Log("ValveSideScreen OnKeyDown");
		if (this.isEditing)
		{
			e.Consumed = true;
		}
		else
		{
			base.OnKeyDown(e);
		}
	}

	private Valve targetValve;

	[Header("Slider")]
	[SerializeField]
	private KSlider flowSlider;

	[SerializeField]
	private LocText minFlowLabel;

	[SerializeField]
	private LocText maxFlowLabel;

	[Header("Input Field")]
	[SerializeField]
	private KNumberInputField numberInput;

	[SerializeField]
	private LocText unitsLabel;

	private bool isEditing;

	private float targetFlow;
}
