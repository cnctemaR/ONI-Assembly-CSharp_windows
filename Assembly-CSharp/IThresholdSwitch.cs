using System;

public interface IThresholdSwitch
{
	float Threshold { get; set; }

	bool ActivateAboveThreshold { get; set; }

	float CurrentValue { get; }

	float RangeMin { get; }

	float RangeMax { get; }

	float GetRangeMinInputField();

	float GetRangeMaxInputField();

	LocString ThresholdValueName { get; }

	LocString ThresholdValueUnits();

	string Format(float value, bool units);

	string AboveToolTip { get; }

	string BelowToolTip { get; }

	bool IsConnected();

	float ProcessedSliderValue(float input);

	float ProcessedInputValue(float input);
}
