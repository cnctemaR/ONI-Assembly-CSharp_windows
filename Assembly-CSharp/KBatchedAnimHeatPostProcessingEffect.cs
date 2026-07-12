using System;
using UnityEngine;

public class KBatchedAnimHeatPostProcessingEffect : KMonoBehaviour
{
	public float HeatProduction
	{
		get
		{
			return this.heatProduction;
		}
	}

	public bool IsHeatProductionEnoughToShowEffect
	{
		get
		{
			return this.HeatProduction >= 1f;
		}
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		this.animController.postProcessingEffectsAllowed |= KAnimConverter.PostProcessingEffects.TemperatureOverlay;
	}

	public void SetHeatBeingProducedValue(float heat)
	{
		this.heatProduction = heat;
		this.RefreshEffectVisualState();
	}

	public void RefreshEffectVisualState()
	{
		if (base.enabled && this.IsHeatProductionEnoughToShowEffect)
		{
			this.SetParameterValue(1f);
			return;
		}
		this.SetParameterValue(0f);
	}

	private void SetParameterValue(float value)
	{
		if (this.animController != null)
		{
			this.animController.postProcessingParameters = value;
		}
	}

	protected override void OnCmpEnable()
	{
		this.RefreshEffectVisualState();
	}

	protected override void OnCmpDisable()
	{
		this.RefreshEffectVisualState();
	}

	private void Update()
	{
		int num = Mathf.FloorToInt(Time.timeSinceLevelLoad / 1f);
		if (num != this.loopsPlayed)
		{
			this.loopsPlayed = num;
			this.OnNewLoopReached();
		}
	}

	private void OnNewLoopReached()
	{
		if (OverlayScreen.Instance != null && OverlayScreen.Instance.mode == OverlayModes.Temperature.ID && this.IsHeatProductionEnoughToShowEffect)
		{
			Vector3 position = base.transform.GetPosition();
			string sound = GlobalAssets.GetSound("Temperature_Heat_Emission", false);
			position.z = 0f;
			SoundEvent.EndOneShot(SoundEvent.BeginOneShot(sound, position, 1f, false));
		}
	}

	public const float SHOW_EFFECT_HEAT_TRESHOLD = 1f;

	private const float DISABLING_VALUE = 0f;

	private const float ENABLING_VALUE = 1f;

	private float heatProduction;

	public const float ANIM_DURATION = 1f;

	private int loopsPlayed;

	[MyCmpGet]
	private KBatchedAnimController animController;
}
