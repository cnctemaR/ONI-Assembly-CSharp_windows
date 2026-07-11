using System;
using Klei.AI;
using KSerialization;
using STRINGS;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class LogicDiseaseSensor : Switch, ISaveLoadable, IThresholdSwitch, ISim200ms
{
	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		base.Subscribe<LogicDiseaseSensor>(-905833192, LogicDiseaseSensor.OnCopySettingsDelegate);
	}

	private void OnCopySettings(object data)
	{
		GameObject gameObject = (GameObject)data;
		LogicDiseaseSensor component = gameObject.GetComponent<LogicDiseaseSensor>();
		if (component != null)
		{
			this.Threshold = component.Threshold;
			this.ActivateAboveThreshold = component.ActivateAboveThreshold;
		}
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		this.animController = base.GetComponent<KBatchedAnimController>();
		base.OnToggle += this.OnSwitchToggled;
		this.UpdateLogicCircuit();
		this.UpdateVisualState(true);
		this.wasOn = this.switchedOn;
	}

	public void Sim200ms(float dt)
	{
		if (this.sampleIdx < 8)
		{
			int num = Grid.PosToCell(this);
			if (Grid.Mass[num] > 0f)
			{
				this.samples[this.sampleIdx] = Grid.DiseaseCount[num];
				this.sampleIdx++;
			}
			return;
		}
		this.sampleIdx = 0;
		float currentValue = this.CurrentValue;
		if (this.activateAboveThreshold)
		{
			if ((currentValue > this.threshold && !base.IsSwitchedOn) || (currentValue <= this.threshold && base.IsSwitchedOn))
			{
				this.Toggle();
			}
		}
		else if ((currentValue > this.threshold && base.IsSwitchedOn) || (currentValue <= this.threshold && !base.IsSwitchedOn))
		{
			this.Toggle();
		}
		this.animController.SetSymbolVisiblity(LogicDiseaseSensor.TINT_SYMBOL, currentValue > 0f);
	}

	private void OnSwitchToggled(bool toggled_on)
	{
		this.UpdateLogicCircuit();
		this.UpdateVisualState(false);
	}

	public float Threshold
	{
		get
		{
			return this.threshold;
		}
		set
		{
			this.threshold = value;
		}
	}

	public bool ActivateAboveThreshold
	{
		get
		{
			return this.activateAboveThreshold;
		}
		set
		{
			this.activateAboveThreshold = value;
		}
	}

	public float CurrentValue
	{
		get
		{
			float num = 0f;
			for (int i = 0; i < 8; i++)
			{
				num += (float)this.samples[i];
			}
			return num / 8f;
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
			return 100;
		}
	}

	public NonLinearSlider.Range[] GetRanges
	{
		get
		{
			return NonLinearSlider.GetDefaultRange(this.RangeMax);
		}
	}

	private void UpdateLogicCircuit()
	{
		base.GetComponent<LogicPorts>().SendSignal(LogicSwitch.PORT_ID, (!this.switchedOn) ? 0 : 1);
	}

	private void UpdateVisualState(bool force = false)
	{
		if (this.wasOn != this.switchedOn || force)
		{
			this.wasOn = this.switchedOn;
			if (this.switchedOn)
			{
				this.animController.Play(LogicDiseaseSensor.ON_ANIMS, KAnim.PlayMode.Loop);
				int num = Grid.PosToCell(this);
				byte b = Grid.DiseaseIdx[num];
				Color32 color = Color.white;
				if (b != 255)
				{
					Disease disease = Db.Get().Diseases[(int)b];
					color = disease.overlayColour;
				}
				this.animController.SetSymbolTint(LogicDiseaseSensor.TINT_SYMBOL, color);
			}
			else
			{
				this.animController.Play(LogicDiseaseSensor.OFF_ANIMS, KAnim.PlayMode.Once);
			}
		}
	}

	public LocString Title
	{
		get
		{
			return UI.UISIDESCREENS.THRESHOLD_SWITCH_SIDESCREEN.DISEASE_TITLE;
		}
	}

	[SerializeField]
	[Serialize]
	private float threshold;

	[SerializeField]
	[Serialize]
	private bool activateAboveThreshold = true;

	private KBatchedAnimController animController;

	private bool wasOn;

	private const float rangeMin = 0f;

	private const float rangeMax = 100000f;

	private const int WINDOW_SIZE = 8;

	private int[] samples = new int[8];

	private int sampleIdx;

	[MyCmpAdd]
	private CopyBuildingSettings copyBuildingSettings;

	private static readonly EventSystem.IntraObjectHandler<LogicDiseaseSensor> OnCopySettingsDelegate = new EventSystem.IntraObjectHandler<LogicDiseaseSensor>(delegate(LogicDiseaseSensor component, object data)
	{
		component.OnCopySettings(data);
	});

	private static readonly HashedString[] ON_ANIMS = new HashedString[] { "on_pre", "on_loop" };

	private static readonly HashedString[] OFF_ANIMS = new HashedString[] { "on_pst", "off" };

	private static readonly HashedString TINT_SYMBOL = "germs";
}
