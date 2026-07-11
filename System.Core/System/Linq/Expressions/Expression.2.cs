using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Dynamic.Utils;
using System.Linq.Expressions.Compiler;
using System.Runtime.CompilerServices;
using Unity;

namespace System.Linq.Expressions
{
	public class Expression<TDelegate> : LambdaExpression
	{
		internal Expression(Expression body)
			: base(body)
		{
		}

		internal sealed override Type TypeCore
		{
			get
			{
				return typeof(TDelegate);
			}
		}

		internal override Type PublicType
		{
			get
			{
				return typeof(Expression<TDelegate>);
			}
		}

		public new TDelegate Compile()
		{
			return this.Compile(false);
		}

		public new TDelegate Compile(bool preferInterpretation)
		{
			return (TDelegate)((object)LambdaCompiler.Compile(this));
		}

		public Expression<TDelegate> Update(Expression body, IEnumerable<ParameterExpression> parameters)
		{
			if (body == base.Body)
			{
				ICollection<ParameterExpression> collection;
				if (parameters == null)
				{
					collection = null;
				}
				else
				{
					collection = parameters as ICollection<ParameterExpression>;
					if (collection == null)
					{
						collection = (parameters = parameters.ToReadOnly<ParameterExpression>());
					}
				}
				if (this.SameParameters(collection))
				{
					return this;
				}
			}
			return Expression.Lambda<TDelegate>(body, base.Name, base.TailCall, parameters);
		}

		[ExcludeFromCodeCoverage]
		internal virtual bool SameParameters(ICollection<ParameterExpression> parameters)
		{
			throw ContractUtils.Unreachable;
		}

		[ExcludeFromCodeCoverage]
		internal virtual Expression<TDelegate> Rewrite(Expression body, ParameterExpression[] parameters)
		{
			throw ContractUtils.Unreachable;
		}

		protected internal override Expression Accept(ExpressionVisitor visitor)
		{
			return visitor.VisitLambda<TDelegate>(this);
		}

		internal override LambdaExpression Accept(StackSpiller spiller)
		{
			return spiller.Rewrite<TDelegate>(this);
		}

		internal static Expression<TDelegate> Create(Expression body, string name, bool tailCall, IReadOnlyList<ParameterExpression> parameters)
		{
			if (name != null || tailCall)
			{
				return new FullExpression<TDelegate>(body, name, tailCall, parameters);
			}
			switch (parameters.Count)
			{
			case 0:
				return new Expression0<TDelegate>(body);
			case 1:
				return new Expression1<TDelegate>(body, parameters[0]);
			case 2:
				return new Expression2<TDelegate>(body, parameters[0], parameters[1]);
			case 3:
				return new Expression3<TDelegate>(body, parameters[0], parameters[1], parameters[2]);
			default:
				return new ExpressionN<TDelegate>(body, parameters);
			}
		}

		public new TDelegate Compile(DebugInfoGenerator debugInfoGenerator)
		{
			return this.Compile();
		}

		internal Expression()
		{
			global::Unity.ThrowStub.ThrowNotSupportedException();
		}
	}
}
