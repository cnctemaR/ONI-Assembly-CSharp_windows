using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Dynamic.Utils;
using System.Threading;

namespace System.Linq.Expressions
{
	[DebuggerTypeProxy(typeof(Expression.BlockExpressionProxy))]
	public class BlockExpression : Expression
	{
		public ReadOnlyCollection<Expression> Expressions
		{
			get
			{
				return this.GetOrMakeExpressions();
			}
		}

		public ReadOnlyCollection<ParameterExpression> Variables
		{
			get
			{
				return this.GetOrMakeVariables();
			}
		}

		public Expression Result
		{
			get
			{
				return this.GetExpression(this.ExpressionCount - 1);
			}
		}

		internal BlockExpression()
		{
		}

		protected internal override Expression Accept(ExpressionVisitor visitor)
		{
			return visitor.VisitBlock(this);
		}

		public sealed override ExpressionType NodeType
		{
			get
			{
				return ExpressionType.Block;
			}
		}

		public override Type Type
		{
			get
			{
				return this.GetExpression(this.ExpressionCount - 1).Type;
			}
		}

		public BlockExpression Update(IEnumerable<ParameterExpression> variables, IEnumerable<Expression> expressions)
		{
			if (expressions != null)
			{
				ICollection<ParameterExpression> collection;
				if (variables == null)
				{
					collection = null;
				}
				else
				{
					collection = variables as ICollection<ParameterExpression>;
					if (collection == null)
					{
						collection = (variables = variables.ToReadOnly<ParameterExpression>());
					}
				}
				if (this.SameVariables(collection))
				{
					ICollection<Expression> collection2 = expressions as ICollection<Expression>;
					if (collection2 == null)
					{
						collection2 = (expressions = expressions.ToReadOnly<Expression>());
					}
					if (this.SameExpressions(collection2))
					{
						return this;
					}
				}
			}
			return Expression.Block(this.Type, variables, expressions);
		}

		internal virtual bool SameVariables(ICollection<ParameterExpression> variables)
		{
			return variables == null || variables.Count == 0;
		}

		[ExcludeFromCodeCoverage]
		internal virtual bool SameExpressions(ICollection<Expression> expressions)
		{
			throw ContractUtils.Unreachable;
		}

		[ExcludeFromCodeCoverage]
		internal virtual Expression GetExpression(int index)
		{
			throw ContractUtils.Unreachable;
		}

		[ExcludeFromCodeCoverage]
		internal virtual int ExpressionCount
		{
			get
			{
				throw ContractUtils.Unreachable;
			}
		}

		[ExcludeFromCodeCoverage]
		internal virtual ReadOnlyCollection<Expression> GetOrMakeExpressions()
		{
			throw ContractUtils.Unreachable;
		}

		internal virtual ReadOnlyCollection<ParameterExpression> GetOrMakeVariables()
		{
			return EmptyReadOnlyCollection<ParameterExpression>.Instance;
		}

		[ExcludeFromCodeCoverage]
		internal virtual BlockExpression Rewrite(ReadOnlyCollection<ParameterExpression> variables, Expression[] args)
		{
			throw ContractUtils.Unreachable;
		}

		internal static ReadOnlyCollection<Expression> ReturnReadOnlyExpressions(BlockExpression provider, ref object collection)
		{
			Expression expression = collection as Expression;
			if (expression != null)
			{
				Interlocked.CompareExchange(ref collection, new ReadOnlyCollection<Expression>(new BlockExpressionList(provider, expression)), expression);
			}
			return (ReadOnlyCollection<Expression>)collection;
		}
	}
}
