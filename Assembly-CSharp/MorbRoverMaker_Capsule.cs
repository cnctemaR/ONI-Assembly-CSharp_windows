using System;
using UnityEngine;

public class MorbRoverMaker_Capsule : KMonoBehaviour
{
	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		this.MorbDevelopment_Meter = new MeterController(this.buildingAnimCtr, "meter_morb_target", "meter_morb", Meter.Offset.UserSpecified, Grid.SceneLayer.BuildingBack, Array.Empty<string>());
		this.GermMeter = new MeterController(this.buildingAnimCtr, "meter_germs_target", "meter_germs", Meter.Offset.UserSpecified, Grid.SceneLayer.BuildingBack, Array.Empty<string>());
		this.MorbDevelopment_Capsule_Meter = new MeterController(this.buildingAnimCtr, "meter_capsule_target", "meter_capsule", Meter.Offset.UserSpecified, Grid.SceneLayer.BuildingBack, Array.Empty<string>());
		this.MorbDevelopment_Capsule_Meter.meterController.onAnimComplete += this.OnGermAddedAnimationComplete;
	}

	private void OnGermAddedAnimationComplete(HashedString animName)
	{
		if (animName == MorbRoverMaker_Capsule.MORB_CAPSULE_METER_PUMP_ANIM_NAME)
		{
			this.MorbDevelopment_Capsule_Meter.meterController.Play("meter_capsule", KAnim.PlayMode.Once, 1f, 0f);
		}
	}

	public void PlayPumpGermsAnimation()
	{
		if (this.MorbDevelopment_Capsule_Meter.meterController.currentAnim != MorbRoverMaker_Capsule.MORB_CAPSULE_METER_PUMP_ANIM_NAME)
		{
			this.MorbDevelopment_Capsule_Meter.meterController.Play(MorbRoverMaker_Capsule.MORB_CAPSULE_METER_PUMP_ANIM_NAME, KAnim.PlayMode.Once, 1f, 0f);
		}
	}

	public void SetMorbDevelopmentProgress(float morbDevelopmentProgress)
	{
		global::Debug.Assert(true, "MORB PHASES COUNT needs to be larger than 0");
		string text = "meter_morb_" + (1 + Mathf.FloorToInt(morbDevelopmentProgress * 4f)).ToString();
		if (this.MorbDevelopment_Meter.meterController.currentAnim != text)
		{
			this.MorbDevelopment_Meter.meterController.Play(text, KAnim.PlayMode.Loop, 1f, 0f);
		}
	}

	public void SetGermMeterProgress(float progress)
	{
		this.GermMeter.SetPositionPercent(progress);
	}

	public const byte MORB_PHASES_COUNT = 5;

	public const byte MORB_FIRST_PHASE_INDEX = 1;

	private const string GERM_METER_TARGET_NAME = "meter_germs_target";

	private const string GERM_METER_ANIMATION_NAME = "meter_germs";

	private const string MORB_METER_TARGET_NAME = "meter_morb_target";

	private const string MORB_METER_ANIMATION_NAME = "meter_morb";

	private const string MORB_CAPSULE_METER_TARGET_NAME = "meter_capsule_target";

	private const string MORB_CAPSULE_METER_ANIMATION_NAME = "meter_capsule";

	private static HashedString MORB_CAPSULE_METER_PUMP_ANIM_NAME = new HashedString("germ_pump");

	[MyCmpGet]
	private KBatchedAnimController buildingAnimCtr;

	private MeterController MorbDevelopment_Meter;

	private MeterController MorbDevelopment_Capsule_Meter;

	private MeterController GermMeter;
}
