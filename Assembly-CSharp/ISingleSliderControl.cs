using System;

public interface ISingleSliderControl
{
	float SingleSliderPercent { get; set; }

	string SliderTitleKey { get; }

	string SliderTooltipKey { get; }
}
