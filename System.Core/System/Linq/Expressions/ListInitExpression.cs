using System;
using System.Collections.ObjectModel;
using System.Reflection.Emit;

namespace System.Linq.Expressions
{
	public sealed class ListInitExpression : Expression
	{
		internal ListInitExpression(NewExpression new_expression, ReadOnlyCollection<ElementInit> initializers)
			: base(ExpressionType.ListInit, new_expression.Type)
		{
			this.new_expression = new_expression;
			this.initializers = initializers;
		}

		public NewExpression NewExpression
		{
			get
			{
				return this.new_expression;
			}
		}

		public ReadOnlyCollection<ElementInit> Initializers
		{
			get
			{
				return this.initializers;
			}
		}

		internal override void Emit(EmitContext ec)
		{
			LocalBuilder localBuilder = ec.EmitStored(this.new_expression);
			ec.EmitCollection(this.initializers, localBuilder);
			ec.EmitLoad(localBuilder);
		}

		private NewExpression new_expression;

		private ReadOnlyCollection<ElementInit> initializers;
	}
}
