using System;
using UnityEngine.Bindings;

namespace UnityEngine.UIElements.StyleSheets.Syntax
{
	[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
	internal class Expression
	{
		public Expression(ExpressionType type)
		{
			this.type = type;
			this.combinator = ExpressionCombinator.None;
			this.multiplier = new ExpressionMultiplier(ExpressionMultiplierType.None);
			this.subExpressions = null;
			this.keyword = null;
		}

		public ExpressionType type;

		public ExpressionMultiplier multiplier;

		public DataType dataType;

		public ExpressionCombinator combinator;

		public Expression[] subExpressions;

		public float min;

		public float max;

		public string keyword;
	}
}
