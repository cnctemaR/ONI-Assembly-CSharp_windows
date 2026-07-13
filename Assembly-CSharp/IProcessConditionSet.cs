using System;
using System.Collections.Generic;

public interface IProcessConditionSet
{
	List<ProcessCondition> GetConditionSet(ProcessCondition.ProcessConditionType conditionType);

	int PopulateConditionSet(ProcessCondition.ProcessConditionType conditionType, List<ProcessCondition> conditions);
}
