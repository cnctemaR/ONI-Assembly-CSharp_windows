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
			this.m_conditionString = conditionString;
		}

		public string ConditionString
		{
			get
			{
				return this.m_conditionString;
			}
		}

		private string m_conditionString;
	}
}
