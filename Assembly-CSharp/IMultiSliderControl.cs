using System;

public interface IMultiSliderControl
{
	string SidescreenTitleKey { get; }

	bool SidescreenEnabled();

	ISliderControl[] sliderControls { get; }
}
