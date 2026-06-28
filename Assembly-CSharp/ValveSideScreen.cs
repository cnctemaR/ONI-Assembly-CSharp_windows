using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class ValveSideScreen : SideScreenContent
{
	protected override void OnSpawn()
	{
		base.OnSpawn();
		this.flowSlider.onReleaseHandle += this.OnReleaseHandle;
		this.flowSlider.onValueChanged.AddListener(new UnityAction<float>(this.UpdateFlowValue));
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
			global::Debug.LogError("The target object does not have a Valve component.", null);
			return;
		}
		this.flowSlider.minValue = 0f;
		this.flowSlider.maxValue = this.targetValve.maxFlow;
		this.minFlowLabel.text = GameUtil.GetFormattedMass(0f, GameUtil.TimeSlice.PerSecond, true, "{0:0.#}");
		this.maxFlowLabel.text = GameUtil.GetFormattedMass(this.targetValve.maxFlow, GameUtil.TimeSlice.PerSecond, true, "{0:0.#}");
		this.currentFlowLabel.text = GameUtil.GetFormattedMass(Mathf.Max(0f, this.targetValve.DesiredFlow), GameUtil.TimeSlice.PerSecond, true, "{0:0.#}");
		this.flowSlider.value = this.targetValve.DesiredFlow;
	}

	private void UpdateFlowValue(float newValue)
	{
		if (this.targetValve == null)
		{
			return;
		}
		this.targetFlow = newValue;
		this.currentFlowLabel.text = GameUtil.GetFormattedMass(newValue, GameUtil.TimeSlice.PerSecond, true, "{0:0.#}");
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

	[Header("Slider")]
	[SerializeField]
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
