using System;
using System.Collections.Generic;

public class IdleTracker : WorldTracker
{
	public IdleTracker(int worldID)
		: base(worldID)
	{
	}

	public override void UpdateData()
	{
		this.objectsOfInterest.Clear();
		int num = 0;
		List<MinionIdentity> worldItems = Components.LiveMinionIdentities.GetWorldItems(base.WorldID, false);
		for (int i = 0; i < worldItems.Count; i++)
		{
			if (worldItems[i].HasTag(GameTags.Idle))
			{
				num++;
				this.objectsOfInterest.Add(worldItems[i].gameObject);
			}
		}
		base.AddPoint((float)num);
	}

	public override string FormatValueString(float value)
	{
		return value.ToString();
	}
}
