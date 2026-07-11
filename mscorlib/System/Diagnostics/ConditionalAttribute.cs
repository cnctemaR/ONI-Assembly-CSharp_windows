using System;
using System.Runtime.InteropServices;

namespace System.Diagnostics
{
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
	[ComVisible(true)]
	[Serializable]
	public sealed class ConditionalAttribute : Attribute
	{
		public ConditionalAttribute(string conditionString)
		{
			this.myCondition = conditionString;
		}

		public string ConditionString
		{
			get
			{
				return this.myCondition;
			}
		}

		private string myCondition;
	}
}
