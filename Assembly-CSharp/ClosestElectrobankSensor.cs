using System;
using System.Collections.Generic;

public class ClosestElectrobankSensor : ClosestPickupableSensor<Electrobank>
{
	public ClosestElectrobankSensor(Sensors sensors, bool shouldStartActive)
		: base(sensors, GameTags.ChargedPortableBattery, shouldStartActive)
	{
		this.bionicIncompatiobleElectrobankTags = new Tag[GameTags.BionicIncompatibleBatteries.Count];
		GameTags.BionicIncompatibleBatteries.CopyTo(this.bionicIncompatiobleElectrobankTags, 0);
	}

	public override HashSet<Tag> GetForbbidenTags()
	{
		HashSet<Tag> forbbidenTags = base.GetForbbidenTags();
		if (this.bionicIncompatiobleElectrobankTags != null && this.bionicIncompatiobleElectrobankTags.Length != 0)
		{
			HashSet<Tag> hashSet = forbbidenTags;
			foreach (Tag tag in this.bionicIncompatiobleElectrobankTags)
			{
				if (!forbbidenTags.Contains(tag))
				{
					hashSet.Add(tag);
				}
			}
			return hashSet;
		}
		return forbbidenTags;
	}

	private Tag[] bionicIncompatiobleElectrobankTags;
}
