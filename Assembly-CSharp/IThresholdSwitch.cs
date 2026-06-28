using System;

public interface IThresholdSwitch
{
	float Threshold { get; set; }

	bool ActivateAboveThreshold { get; set; }

	float CurrentValue { get; }

	float RangeMin { get; }

	float RangeMax { get; }

	LocString ThresholdValueName { get; }

	string Format(float value);

	string AboveToolTip { get; }

	string BelowToolTip { get; }

	bool IsConnected();
}
