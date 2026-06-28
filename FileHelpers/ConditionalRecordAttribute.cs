using System;

namespace FileHelpers
{
	[AttributeUsage(AttributeTargets.Class)]
	public sealed class ConditionalRecordAttribute : Attribute
	{
		public RecordCondition Condition { get; private set; }

		public string ConditionSelector { get; private set; }

		public ConditionalRecordAttribute(RecordCondition condition, string conditionSelector)
		{
			this.Condition = condition;
			this.ConditionSelector = conditionSelector;
			ExHelper.CheckNullOrEmpty(conditionSelector, "conditionSelector");
		}
	}
}
