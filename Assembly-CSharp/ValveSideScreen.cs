using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class ValveSideScreen : SideScreenContent
{
	protected override void OnSpawn()
	{
		base.OnSpawn();
		this.flowSlider.maxValue = 10f;
		this.flowSlider.onReleaseHandle += this.OnReleaseHandle;
		this.flowSlider.onValueChanged.AddListener(new UnityAction<float>(this.UpdateFlowValue));
		this.minFlowLabel.SetText(GameUtil.GetFormattedMass(0f, GameUtil.TimeSlice.PerSecond, true, "F1"));
		this.maxFlowLabel.SetText(GameUtil.GetFormattedMass(10f, GameUtil.TimeSlice.PerSecond, true, "F1"));
	}

	public void OnReleaseHandle()
	{
		this.targetValve.ChangeFlow(this.targetFlow);
	}

	public override void SetTarget(GameObject target)
	{
		this.targetValve = target.GetComponent<Valve>();
		if (this.targetValve == null)
		{
			Debug.LogError("The target object does not have a Valve component.");
			return;
		}
		this.flowSlider.minValue = this.targetValve.minFlow;
		this.flowSlider.maxValue = this.targetValve.maxFlow;
		this.minFlowLabel.SetText(GameUtil.GetFormattedMass(this.targetValve.minFlow, GameUtil.TimeSlice.PerSecond, true, "F1"));
		this.maxFlowLabel.SetText(GameUtil.GetFormattedMass(this.targetValve.maxFlow, GameUtil.TimeSlice.PerSecond, true, "F1"));
		this.currentFlowLabel.text = GameUtil.GetFormattedMass(Mathf.Max(0f, this.targetValve.DesiredFlow), GameUtil.TimeSlice.PerSecond, true, "F1");
		this.flowSlider.value = this.targetValve.DesiredFlow;
	}

	private void UpdateFlowValue(float newValue)
	{
		if (this.targetValve == null)
		{
			return;
		}
		this.targetFlow = newValue;
		this.currentFlowLabel.text = GameUtil.GetFormattedMass(newValue, GameUtil.TimeSlice.PerSecond, true, "F1");
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

	private Valve targetValve;

	[SerializeField]
	[Header("Slider")]
	private KSlider flowSlider;

	[SerializeField]
	[Header("Labels")]
	private LocText currentFlowLabel;

	[SerializeField]
	private LocText minFlowLabel;

	[SerializeField]
	private LocText maxFlowLabel;

	private float targetFlow;
}
