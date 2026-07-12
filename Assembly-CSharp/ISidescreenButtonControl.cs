using System;

public interface ISidescreenButtonControl
{
	string SidescreenButtonText { get; }

	string SidescreenButtonTooltip { get; }

	void SetButtonTextOverride(ButtonMenuTextOverride textOverride);

	bool SidescreenEnabled();

	bool SidescreenButtonInteractable();

	void OnSidescreenButtonPressed();

	int HorizontalGroupID();

	int ButtonSideScreenSortOrder();
}
