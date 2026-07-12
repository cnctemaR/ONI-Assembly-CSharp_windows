using System;
using System.Collections.Generic;
using UnityEngine;

public class QuestCriteria_Equals : QuestCriteria
{
	public QuestCriteria_Equals(Tag id, float[] targetValues, int requiredCount = 1, HashSet<Tag> acceptedTags = null, QuestCriteria.BehaviorFlags flags = QuestCriteria.BehaviorFlags.TrackValues)
		: base(id, targetValues, requiredCount, acceptedTags, flags)
	{
	}

	protected override bool ValueSatisfies_Internal(float current, float target)
	{
		return Mathf.Abs(target - current) <= Mathf.Epsilon;
	}
}
