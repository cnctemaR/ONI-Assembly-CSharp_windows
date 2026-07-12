using System;

public class ChoreCountTracker : WorldTracker
{
	public ChoreCountTracker(int worldID, ChoreGroup group)
		: base(worldID)
	{
		this.choreGroup = group;
	}

	public override void UpdateData()
	{
		float num = 0f;
		foreach (Chore chore in GlobalChoreProvider.Instance.chores)
		{
			if (chore != null && !chore.target.Equals(null) && !(chore.gameObject == null) && chore.gameObject.GetMyWorldId() == base.WorldID)
			{
				ChoreGroup[] groups = chore.choreType.groups;
				for (int i = 0; i < groups.Length; i++)
				{
					if (groups[i] == this.choreGroup)
					{
						num += 1f;
						break;
					}
				}
			}
		}
		base.AddPoint(num);
	}

	public override string FormatValueString(float value)
	{
		return value.ToString();
	}

	public ChoreGroup choreGroup;
}
