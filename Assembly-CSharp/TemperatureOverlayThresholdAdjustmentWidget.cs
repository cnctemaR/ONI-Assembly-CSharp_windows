using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class TemperatureOverlayThresholdAdjustmentWidget : KMonoBehaviour
{
	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		this.scrollbar.onValueChanged.AddListener(new UnityAction<float>(this.OnValueChanged));
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		this.scrollbar.size = TemperatureOverlayThresholdAdjustmentWidget.temperatureWindowSize / TemperatureOverlayThresholdAdjustmentWidget.maxTemperatureRange;
		this.scrollbar.value = this.KelvinToScrollPercentage(SaveGame.Instance.relativeTemperatureOverlaySliderValue);
		this.defaultButton.onClick += this.OnDefaultPressed;
	}

	private void OnValueChanged(float data)
	{
		this.SetUserConfig(data);
	}

	private float KelvinToScrollPercentage(float kelvin)
	{
		kelvin -= TemperatureOverlayThresholdAdjustmentWidget.minimumSelectionTemperature;
		if (kelvin < 1f)
		{
			kelvin = 1f;
		}
		return Mathf.Clamp01(kelvin / TemperatureOverlayThresholdAdjustmentWidget.maxTemperatureRange);
	}

	private void SetUserConfig(float scrollPercentage)
	{
		float num = TemperatureOverlayThresholdAdjustmentWidget.minimumSelectionTemperature + TemperatureOverlayThresholdAdjustmentWidget.maxTemperatureRange * scrollPercentage;
		float num2 = num - TemperatureOverlayThresholdAdjustmentWidget.temperatureWindowSize / 2f;
		float num3 = num + TemperatureOverlayThresholdAdjustmentWidget.temperatureWindowSize / 2f;
		SimDebugView.Instance.user_temperatureThresholds[0] = num2;
		SimDebugView.Instance.user_temperatureThresholds[1] = num3;
		this.scrollBarRangeCenterText.SetText(GameUtil.GetFormattedTemperature(num, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute, true, true));
		this.scrollBarRangeLowText.SetText(GameUtil.GetFormattedTemperature((float)Mathf.RoundToInt(num2), GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute, true, true));
		this.scrollBarRangeHighText.SetText(GameUtil.GetFormattedTemperature((float)Mathf.RoundToInt(num3), GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute, true, true));
		SaveGame.Instance.relativeTemperatureOverlaySliderValue = num;
	}

	private void OnDefaultPressed()
	{
		this.scrollbar.value = this.KelvinToScrollPercentage(294.15f);
	}

	public const float DEFAULT_TEMPERATURE = 294.15f;

	[SerializeField]
	private Scrollbar scrollbar;

	[SerializeField]
	private LocText scrollBarRangeLowText;

	[SerializeField]
	private LocText scrollBarRangeCenterText;

	[SerializeField]
	private LocText scrollBarRangeHighText;

	[SerializeField]
	private KButton defaultButton;

	private static float maxTemperatureRange = 700f;

	private static float temperatureWindowSize = 200f;

	private static float minimumSelectionTemperature = TemperatureOverlayThresholdAdjustmentWidget.temperatureWindowSize / 2f;
}
