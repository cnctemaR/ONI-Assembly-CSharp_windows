using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Dynamic.Utils;
using System.Reflection;
using Unity;

namespace System.Linq.Expressions
{
	[DebuggerTypeProxy(typeof(Expression.MethodCallExpressionProxy))]
	public class MethodCallExpression : Expression, IArgumentProvider
	{
		internal MethodCallExpression(MethodInfo method)
		{
			this.Method = method;
		}

		internal virtual Expression GetInstance()
		{
			return null;
		}

		public sealed override ExpressionType NodeType
		{
			get
			{
				return ExpressionType.Call;
			}
		}

		public sealed override Type Type
		{
			get
			{
				return this.Method.ReturnType;
			}
		}

		public MethodInfo Method { get; }

		public Expression Object
		{
			get
			{
				return this.GetInstance();
			}
		}

		public ReadOnlyCollection<Expression> Arguments
		{
			get
			{
				return this.GetOrMakeArguments();
			}
		}

		public MethodCallExpression Update(Expression @object, IEnumerable<Expression> arguments)
		{
			if (@object == this.Object)
			{
				ICollection<Expression> collection;
				if (arguments == null)
				{
					collection = null;
				}
				else
				{
					collection = arguments as ICollection<Expression>;
					if (collection == null)
					{
						collection = (arguments = arguments.ToReadOnly<Expression>());
					}
				}
				if (this.SameArguments(collection))
				{
					return this;
				}
			}
			return Expression.Call(@object, this.Method, arguments);
		}

		[ExcludeFromCodeCoverage]
		internal virtual bool SameArguments(ICollection<Expression> arguments)
		{
			throw ContractUtils.Unreachable;
		}

		[ExcludeFromCodeCoverage]
		internal virtual ReadOnlyCollection<Expression> GetOrMakeArguments()
		{
			throw ContractUtils.Unreachable;
		}

		protected internal override Expression Accept(ExpressionVisitor visitor)
		{
			return visitor.VisitMethodCall(this);
		}

		[ExcludeFromCodeCoverage]
		internal virtual MethodCallExpression Rewrite(Expression instance, IReadOnlyList<Expression> args)
		{
			throw ContractUtils.Unreachable;
		}

		[ExcludeFromCodeCoverage]
		public virtual Expression GetArgument(int index)
		{
			throw ContractUtils.Unreachable;
		}

		[ExcludeFromCodeCoverage]
		public virtual int ArgumentCount
		{
			get
			{
				throw ContractUtils.Unreachable;
			}
		}

		internal MethodCallExpression()
		{
			global::Unity.ThrowStub.ThrowNotSupportedException();
		}
	}
}
