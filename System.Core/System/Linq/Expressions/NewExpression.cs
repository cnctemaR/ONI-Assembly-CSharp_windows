using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Dynamic.Utils;
using System.Reflection;

namespace System.Linq.Expressions
{
	[DebuggerTypeProxy(typeof(Expression.NewExpressionProxy))]
	public class NewExpression : Expression, IArgumentProvider
	{
		internal NewExpression(ConstructorInfo constructor, IReadOnlyList<Expression> arguments, ReadOnlyCollection<MemberInfo> members)
		{
			this.Constructor = constructor;
			this._arguments = arguments;
			this.Members = members;
		}

		public override Type Type
		{
			get
			{
				return this.Constructor.DeclaringType;
			}
		}

		public sealed override ExpressionType NodeType
		{
			get
			{
				return ExpressionType.New;
			}
		}

		public ConstructorInfo Constructor { get; }

		public ReadOnlyCollection<Expression> Arguments
		{
			get
			{
				return ExpressionUtils.ReturnReadOnly<Expression>(ref this._arguments);
			}
		}

		public Expression GetArgument(int index)
		{
			return this._arguments[index];
		}

		public int ArgumentCount
		{
			get
			{
				return this._arguments.Count;
			}
		}

		public ReadOnlyCollection<MemberInfo> Members { get; }

		protected internal override Expression Accept(ExpressionVisitor visitor)
		{
			return visitor.VisitNew(this);
		}

		public NewExpression Update(IEnumerable<Expression> arguments)
		{
			if (ExpressionUtils.SameElements<Expression>(ref arguments, this.Arguments))
			{
				return this;
			}
			if (this.Members == null)
			{
				return Expression.New(this.Constructor, arguments);
			}
			return Expression.New(this.Constructor, arguments, this.Members);
		}

		private IReadOnlyList<Expression> _arguments;
	}
}
