using System;
using STRINGS;
using UnityEngine;

public class DisinfectThresholdDiagram : MonoBehaviour
{
	private void Start()
	{
		this.inputField.minValue = 0f;
		this.inputField.maxValue = (float)DisinfectThresholdDiagram.MAX_VALUE;
		this.inputField.currentValue = (float)SaveGame.Instance.minGermCountForDisinfect;
		this.inputField.SetDisplayValue(SaveGame.Instance.minGermCountForDisinfect.ToString());
		this.inputField.onEndEdit += delegate
		{
			this.ReceiveValueFromInput(this.inputField.currentValue);
		};
		this.inputField.decimalPlaces = 1;
		this.inputField.Activate();
		this.slider.minValue = 0f;
		this.slider.maxValue = (float)DisinfectThresholdDiagram.MAX_VALUE;
		this.slider.value = (float)SaveGame.Instance.minGermCountForDisinfect;
		this.slider.onReleaseHandle += this.OnReleaseHandle;
		this.slider.onDrag += delegate
		{
			this.ReceiveValueFromSlider(this.slider.value);
		};
		this.slider.onPointerDown += delegate
		{
			this.ReceiveValueFromSlider(this.slider.value);
		};
		this.slider.onMove += delegate
		{
			this.ReceiveValueFromSlider(this.slider.value);
			this.OnReleaseHandle();
		};
		this.unitsLabel.SetText(UI.OVERLAYS.DISEASE.DISINFECT_THRESHOLD_DIAGRAM.UNITS);
		this.minLabel.SetText(0.ToString());
		this.maxLabel.SetText(DisinfectThresholdDiagram.MAX_VALUE.ToString());
		this.thresholdPrefix.SetText(UI.OVERLAYS.DISEASE.DISINFECT_THRESHOLD_DIAGRAM.THRESHOLD_PREFIX);
		this.toolTip.OnToolTip = () => UI.OVERLAYS.DISEASE.DISINFECT_THRESHOLD_DIAGRAM.TOOLTIP.ToString().Replace("{NumberOfGerms}", SaveGame.Instance.minGermCountForDisinfect.ToString());
	}

	private void OnReleaseHandle()
	{
		float num = (float)((int)this.slider.value);
		SaveGame.Instance.minGermCountForDisinfect = (int)num;
		this.inputField.SetDisplayValue(num.ToString());
	}

	private void ReceiveValueFromSlider(float new_value)
	{
		SaveGame.Instance.minGermCountForDisinfect = (int)new_value;
		this.inputField.SetDisplayValue(new_value.ToString());
	}

	private void ReceiveValueFromInput(float new_value)
	{
		this.slider.value = new_value;
		SaveGame.Instance.minGermCountForDisinfect = (int)new_value;
	}

	[SerializeField]
	private KNumberInputField inputField;

	[SerializeField]
	private KSlider slider;

	[SerializeField]
	private LocText minLabel;

	[SerializeField]
	private LocText maxLabel;

	[SerializeField]
	private LocText unitsLabel;

	[SerializeField]
	private LocText thresholdPrefix;

	[SerializeField]
	private ToolTip toolTip;

	private static int MAX_VALUE = 100000;
}
