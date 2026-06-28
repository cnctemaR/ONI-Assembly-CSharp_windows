using System;

namespace UnityEngineInternal
{
	[AttributeUsage(AttributeTargets.Method)]
	[Serializable]
	public class TypeInferenceRuleAttribute : Attribute
	{
		public TypeInferenceRuleAttribute(TypeInferenceRules rule)
			: this(rule.ToString())
		{
		}

		public TypeInferenceRuleAttribute(string rule)
		{
			this._rule = rule;
		}

		public override string ToString()
		{
			return this._rule;
		}

		private readonly string _rule;
	}
}
