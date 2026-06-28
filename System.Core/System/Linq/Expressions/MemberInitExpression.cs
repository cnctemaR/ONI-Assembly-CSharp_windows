using System;
using System.Collections.ObjectModel;
using System.Reflection.Emit;

namespace System.Linq.Expressions
{
	public sealed class MemberInitExpression : Expression
	{
		internal MemberInitExpression(NewExpression new_expression, ReadOnlyCollection<MemberBinding> bindings)
			: base(ExpressionType.MemberInit, new_expression.Type)
		{
			this.new_expression = new_expression;
			this.bindings = bindings;
		}

		public NewExpression NewExpression
		{
			get
			{
				return this.new_expression;
			}
		}

		public ReadOnlyCollection<MemberBinding> Bindings
		{
			get
			{
				return this.bindings;
			}
		}

		internal override void Emit(EmitContext ec)
		{
			LocalBuilder localBuilder = ec.EmitStored(this.new_expression);
			ec.EmitCollection(this.bindings, localBuilder);
			ec.EmitLoad(localBuilder);
		}

		private NewExpression new_expression;

		private ReadOnlyCollection<MemberBinding> bindings;
	}
}
