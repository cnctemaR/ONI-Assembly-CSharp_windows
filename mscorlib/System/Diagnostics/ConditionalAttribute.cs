using System;

namespace System.Diagnostics
{
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
	[Serializable]
	public sealed class ConditionalAttribute : Attribute
	{
		public ConditionalAttribute(string conditionString)
		{
			this.ConditionString = conditionString;
		}

		public string ConditionString { get; }
	}
}
