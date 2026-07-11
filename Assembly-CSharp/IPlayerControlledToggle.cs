using System;

public interface IPlayerControlledToggle
{
	void ToggledByPlayer();

	bool ToggledOn();

	KSelectable GetSelectable();

	string SideScreenTitleKey { get; }

	bool ToggleRequested { get; set; }
}
