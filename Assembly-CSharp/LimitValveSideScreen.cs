using System;
using STRINGS;
using UnityEngine;

public class LimitValveSideScreen : SideScreenContent
{
	protected override void OnSpawn()
	{
		base.OnSpawn();
		this.resetButton.onClick += this.ResetCounter;
		this.limitSlider.onReleaseHandle += this.OnReleaseHandle;
		this.limitSlider.onDrag += delegate
		{
			this.ReceiveValueFromSlider(this.limitSlider.value);
		};
		this.limitSlider.onPointerDown += delegate
		{
			this.ReceiveValueFromSlider(this.limitSlider.value);
		};
		this.limitSlider.onMove += delegate
		{
			this.ReceiveValueFromSlider(this.limitSlider.value);
			this.OnReleaseHandle();
		};
		this.numberInput.onEndEdit += delegate
		{
			this.ReceiveValueFromInput(this.numberInput.currentValue);
		};
		this.numberInput.decimalPlaces = 3;
	}

	public void OnReleaseHandle()
	{
		this.targetLimitValve.Limit = this.targetLimit;
	}

	public override bool IsValidForTarget(GameObject target)
	{
		return target.GetComponent<LimitValve>() != null;
	}

	public override void SetTarget(GameObject target)
	{
		bool flag = true;
		if (this.targetLimitValve != null && this.targetLimitValve.gameObject == target)
		{
			flag = false;
		}
		this.targetLimitValve = target.GetComponent<LimitValve>();
		if (this.targetLimitValve == null)
		{
			global::Debug.LogError("The target object does not have a LimitValve component.");
			return;
		}
		if (this.targetLimitValveSubHandle != -1)
		{
			base.Unsubscribe(this.targetLimitValveSubHandle);
		}
		this.targetLimitValveSubHandle = this.targetLimitValve.Subscribe(-1722241721, new Action<object>(this.UpdateAmountLabel));
		this.limitSlider.minValue = 0f;
		this.limitSlider.maxValue = 100f;
		this.limitSlider.SetRanges(this.targetLimitValve.GetRanges());
		this.limitSlider.value = this.limitSlider.GetPercentageFromValue(this.targetLimitValve.Limit);
		this.numberInput.minValue = 0f;
		this.numberInput.maxValue = this.targetLimitValve.maxLimitKg;
		this.numberInput.Activate();
		if (this.targetLimitValve.displayUnitsInsteadOfMass)
		{
			this.minLimitLabel.text = GameUtil.GetFormattedUnits(0f, GameUtil.TimeSlice.None, true, "");
			this.maxLimitLabel.text = GameUtil.GetFormattedUnits(this.targetLimitValve.maxLimitKg, GameUtil.TimeSlice.None, true, "");
			if (flag)
			{
				this.numberInput.SetDisplayValue(GameUtil.GetFormattedUnits(Mathf.Max(0f, this.targetLimitValve.Limit), GameUtil.TimeSlice.None, false, LimitValveSideScreen.FLOAT_FORMAT));
			}
			this.unitsLabel.text = UI.UNITSUFFIXES.UNITS;
			this.toolTip.enabled = true;
			this.toolTip.SetSimpleTooltip(UI.UISIDESCREENS.LIMIT_VALVE_SIDE_SCREEN.SLIDER_TOOLTIP_UNITS);
		}
		else
		{
			this.minLimitLabel.text = GameUtil.GetFormattedMass(0f, GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.Kilogram, true, "{0:0.#}");
			this.maxLimitLabel.text = GameUtil.GetFormattedMass(this.targetLimitValve.maxLimitKg, GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.Kilogram, true, "{0:0.#}");
			if (flag)
			{
				this.numberInput.SetDisplayValue(GameUtil.GetFormattedMass(Mathf.Max(0f, this.targetLimitValve.Limit), GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.Kilogram, false, LimitValveSideScreen.FLOAT_FORMAT));
			}
			this.unitsLabel.text = GameUtil.GetCurrentMassUnit(false);
			this.toolTip.enabled = false;
		}
		this.UpdateAmountLabel(null);
	}

	private void UpdateAmountLabel(object _ = null)
	{
		if (this.targetLimitValve.displayUnitsInsteadOfMass)
		{
			string formattedUnits = GameUtil.GetFormattedUnits(this.targetLimitValve.Amount, GameUtil.TimeSlice.None, true, LimitValveSideScreen.FLOAT_FORMAT);
			this.amountLabel.text = string.Format(UI.UISIDESCREENS.LIMIT_VALVE_SIDE_SCREEN.AMOUNT, formattedUnits);
			return;
		}
		string formattedMass = GameUtil.GetFormattedMass(this.targetLimitValve.Amount, GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.Kilogram, true, LimitValveSideScreen.FLOAT_FORMAT);
		this.amountLabel.text = string.Format(UI.UISIDESCREENS.LIMIT_VALVE_SIDE_SCREEN.AMOUNT, formattedMass);
	}

	private void ResetCounter()
	{
		this.targetLimitValve.ResetAmount();
	}

	private void ReceiveValueFromSlider(float sliderPercentage)
	{
		float num = this.limitSlider.GetValueForPercentage(sliderPercentage);
		num = (float)Mathf.RoundToInt(num);
		this.UpdateLimitValue(num);
	}

	private void ReceiveValueFromInput(float input)
	{
		this.UpdateLimitValue(input);
		this.targetLimitValve.Limit = this.targetLimit;
	}

	private void UpdateLimitValue(float newValue)
	{
		this.targetLimit = newValue;
		this.limitSlider.value = this.limitSlider.GetPercentageFromValue(newValue);
		if (this.targetLimitValve.displayUnitsInsteadOfMass)
		{
			this.numberInput.SetDisplayValue(GameUtil.GetFormattedUnits(newValue, GameUtil.TimeSlice.None, false, LimitValveSideScreen.FLOAT_FORMAT));
			return;
		}
		this.numberInput.SetDisplayValue(GameUtil.GetFormattedMass(newValue, GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.Kilogram, false, LimitValveSideScreen.FLOAT_FORMAT));
	}

	public static readonly string FLOAT_FORMAT = "{0:0.#####}";

	private LimitValve targetLimitValve;

	[Header("State")]
	[SerializeField]
	private LocText amountLabel;

	[SerializeField]
	private KButton resetButton;

	[Header("Slider")]
	[SerializeField]
	private NonLinearSlider limitSlider;

	[SerializeField]
	private LocText minLimitLabel;

	[SerializeField]
	private LocText maxLimitLabel;

	[SerializeField]
	private ToolTip toolTip;

	[Header("Input Field")]
	[SerializeField]
	private KNumberInputField numberInput;

	[SerializeField]
	private LocText unitsLabel;

	private float targetLimit;

	private int targetLimitValveSubHandle = -1;
}
