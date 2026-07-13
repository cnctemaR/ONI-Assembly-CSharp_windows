using System;
using System.Collections.Generic;

public class LaunchPadConditions : KMonoBehaviour, IProcessConditionSet
{
	public List<ProcessCondition> GetConditionSet(ProcessCondition.ProcessConditionType conditionType)
	{
		if (conditionType != ProcessCondition.ProcessConditionType.RocketStorage)
		{
			return null;
		}
		return this.conditions;
	}

	public int PopulateConditionSet(ProcessCondition.ProcessConditionType conditionType, List<ProcessCondition> conditions)
	{
		int num = 0;
		if (conditionType == ProcessCondition.ProcessConditionType.RocketStorage)
		{
			conditions.AddRange(this.conditions);
			num += this.conditions.Count;
		}
		return num;
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		this.conditions = new List<ProcessCondition>();
		this.conditions.Add(new TransferCargoCompleteCondition(base.gameObject));
	}

	private List<ProcessCondition> conditions;
}
