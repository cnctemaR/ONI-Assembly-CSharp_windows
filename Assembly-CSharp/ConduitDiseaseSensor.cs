using System;
using KSerialization;
using STRINGS;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class ConduitDiseaseSensor : ConduitThresholdSensor, IThresholdSwitch
{
	protected override void UpdateVisualState(bool force = false)
	{
		if (this.wasOn != this.switchedOn || force)
		{
			this.wasOn = this.switchedOn;
			if (this.switchedOn)
			{
				this.animController.Play(ConduitSensor.ON_ANIMS, KAnim.PlayMode.Loop);
				int num = Grid.PosToCell(this);
				ConduitFlow.ConduitContents contents = Conduit.GetFlowManager(this.conduitType).GetContents(num);
				Color32 color = Color.white;
				if (contents.diseaseIdx != 255)
				{
					color = Db.Get().Diseases[(int)contents.diseaseIdx].overlayColour;
				}
				this.animController.SetSymbolTint(ConduitDiseaseSensor.TINT_SYMBOL, color);
				return;
			}
			this.animController.Play(ConduitSensor.OFF_ANIMS, KAnim.PlayMode.Once);
		}
	}

	public override float CurrentValue
	{
		get
		{
			int num = Grid.PosToCell(this);
			ConduitFlow.ConduitContents contents = Conduit.GetFlowManager(this.conduitType).GetContents(num);
			if (contents.mass > 0f)
			{
				this.lastValue = (float)contents.diseaseCount;
			}
			return this.lastValue;
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
			return 100000f;
		}
	}

	public float GetRangeMinInputField()
	{
		return 0f;
	}

	public float GetRangeMaxInputField()
	{
		return 100000f;
	}

	public LocString Title
	{
		get
		{
			return UI.UISIDESCREENS.THRESHOLD_SWITCH_SIDESCREEN.DISEASE_TITLE;
		}
	}

	public LocString ThresholdValueName
	{
		get
		{
			return UI.UISIDESCREENS.THRESHOLD_SWITCH_SIDESCREEN.DISEASE;
		}
	}

	public string AboveToolTip
	{
		get
		{
			return UI.UISIDESCREENS.THRESHOLD_SWITCH_SIDESCREEN.DISEASE_TOOLTIP_ABOVE;
		}
	}

	public string BelowToolTip
	{
		get
		{
			return UI.UISIDESCREENS.THRESHOLD_SWITCH_SIDESCREEN.DISEASE_TOOLTIP_BELOW;
		}
	}

	public string Format(float value, bool units)
	{
		return GameUtil.GetFormattedInt((float)((int)value), GameUtil.TimeSlice.None);
	}

	public float ProcessedSliderValue(float input)
	{
		return input;
	}

	public float ProcessedInputValue(float input)
	{
		return input;
	}

	public LocString ThresholdValueUnits()
	{
		return UI.UISIDESCREENS.THRESHOLD_SWITCH_SIDESCREEN.DISEASE_UNITS;
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

	private const float rangeMin = 0f;

	private const float rangeMax = 100000f;

	[Serialize]
	private float lastValue;

	private static readonly HashedString TINT_SYMBOL = "germs";
}
