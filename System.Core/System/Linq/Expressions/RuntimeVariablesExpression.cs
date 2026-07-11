using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Dynamic.Utils;
using System.Runtime.CompilerServices;
using Unity;

namespace System.Linq.Expressions
{
	[DebuggerTypeProxy(typeof(Expression.RuntimeVariablesExpressionProxy))]
	public sealed class RuntimeVariablesExpression : Expression
	{
		internal RuntimeVariablesExpression(ReadOnlyCollection<ParameterExpression> variables)
		{
			this.Variables = variables;
		}

		public sealed override Type Type
		{
			get
			{
				return typeof(IRuntimeVariables);
			}
		}

		public sealed override ExpressionType NodeType
		{
			get
			{
				return ExpressionType.RuntimeVariables;
			}
		}

		public ReadOnlyCollection<ParameterExpression> Variables { get; }

		protected internal override Expression Accept(ExpressionVisitor visitor)
		{
			return visitor.VisitRuntimeVariables(this);
		}

		public RuntimeVariablesExpression Update(IEnumerable<ParameterExpression> variables)
		{
			if (variables != null && ExpressionUtils.SameElements<ParameterExpression>(ref variables, this.Variables))
			{
				return this;
			}
			return Expression.RuntimeVariables(variables);
		}

		internal RuntimeVariablesExpression()
		{
			global::Unity.ThrowStub.ThrowNotSupportedException();
		}
	}
}
