using System;
using System.Collections.Generic;

public interface IMissileSelectionInterface
{
	bool AmmunitionIsAllowed(Tag tag);

	bool IsAnyCosmicBlastShotAllowed();

	void ChangeAmmunition(Tag tag, bool allowed);

	void OnRowToggleClick();

	List<Tag> GetValidAmmunitionTags();
}
