using System;

internal interface IIlluminationTracker
{
	string GetIlluminationUILabel();

	string GetIlluminationUITooltip();

	bool ShouldIlluminationUICheckboxBeChecked();
}
