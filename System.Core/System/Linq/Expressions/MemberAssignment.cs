using System;
using System.Reflection;

namespace System.Linq.Expressions
{
	public sealed class MemberAssignment : MemberBinding
	{
		internal MemberAssignment(MemberInfo member, Expression expression)
			: base(MemberBindingType.Assignment, member)
		{
			this._expression = expression;
		}

		public Expression Expression
		{
			get
			{
				return this._expression;
			}
		}

		public MemberAssignment Update(Expression expression)
		{
			if (expression == this.Expression)
			{
				return this;
			}
			return Expression.Bind(base.Member, expression);
		}

		internal override void ValidateAsDefinedHere(int index)
		{
		}

		private readonly Expression _expression;
	}
}
