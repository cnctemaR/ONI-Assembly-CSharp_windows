using System;
using UnityEngine;

[Serializable]
public class SliderSet
{
	public void SetupSlider(int index)
	{
		this.index = index;
		this.valueSlider.onReleaseHandle += delegate
		{
			this.valueSlider.value = Mathf.Round(this.valueSlider.value * 10f) / 10f;
			this.ReceiveValueFromSlider();
		};
		this.valueSlider.onDrag += delegate
		{
			this.ReceiveValueFromSlider();
		};
		this.valueSlider.onMove += delegate
		{
			this.ReceiveValueFromSlider();
		};
		this.valueSlider.onPointerDown += delegate
		{
			this.ReceiveValueFromSlider();
		};
		this.numberInput.onEndEdit += delegate
		{
			this.ReceiveValueFromInput();
		};
	}

	public void SetTarget(ISliderControl target)
	{
		this.target = target;
		ToolTip component = this.valueSlider.handleRect.GetComponent<ToolTip>();
		if (component != null)
		{
			component.SetSimpleTooltip(Strings.Get(target.GetSliderTooltipKey(this.index)));
		}
		this.unitsLabel.text = target.SliderUnits;
		this.minLabel.text = target.GetSliderMin(this.index) + target.SliderUnits;
		this.maxLabel.text = target.GetSliderMax(this.index) + target.SliderUnits;
		this.numberInput.minValue = target.GetSliderMin(this.index);
		this.numberInput.maxValue = target.GetSliderMax(this.index);
		this.valueSlider.minValue = target.GetSliderMin(this.index);
		this.valueSlider.maxValue = target.GetSliderMax(this.index);
		this.valueSlider.value = target.GetSliderValue(this.index);
		this.SetValue(target.GetSliderValue(this.index));
		if (this.index == 0)
		{
			this.numberInput.Activate();
		}
	}

	private void ReceiveValueFromSlider()
	{
		this.SetValue(this.valueSlider.value);
	}

	private void ReceiveValueFromInput()
	{
		float num = Mathf.Round(this.numberInput.currentValue * 10f) / 10f;
		this.valueSlider.value = num;
		this.SetValue(num);
	}

	private void SetValue(float value)
	{
		float num = value;
		if (num > this.target.GetSliderMax(this.index))
		{
			num = this.target.GetSliderMax(this.index);
		}
		else if (num < this.target.GetSliderMin(this.index))
		{
			num = this.target.GetSliderMin(this.index);
		}
		this.UpdateLabel(num);
		this.target.SetSliderValue(num, this.index);
	}

	private void UpdateLabel(float value)
	{
		float num = Mathf.Round(value * 10f) / 10f;
		this.numberInput.SetDisplayValue(num.ToString());
	}

	public KSlider valueSlider;

	public KNumberInputField numberInput;

	public LocText unitsLabel;

	public LocText minLabel;

	public LocText maxLabel;

	[NonSerialized]
	public int index;

	private ISliderControl target;
}
