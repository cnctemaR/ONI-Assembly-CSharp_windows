using System;
using System.Collections.Generic;

public class QuestCriteria_LessOrEqual : QuestCriteria
{
	public QuestCriteria_LessOrEqual(Tag id, float[] targetValues, int requiredCount = 1, HashSet<Tag> acceptedTags = null, QuestCriteria.BehaviorFlags flags = QuestCriteria.BehaviorFlags.TrackValues)
		: base(id, targetValues, requiredCount, acceptedTags, flags)
	{
	}

	protected override bool ValueSatisfies_Internal(float current, float target)
	{
		return current <= target;
	}
}
