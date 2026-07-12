using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Dynamic.Utils;
using System.Runtime.CompilerServices;
using Unity;

namespace System.Linq.Expressions
{
	public class DynamicExpression : Expression, IDynamicExpression, IArgumentProvider
	{
		internal DynamicExpression(Type delegateType, CallSiteBinder binder)
		{
			this.DelegateType = delegateType;
			this.Binder = binder;
		}

		public override bool CanReduce
		{
			get
			{
				return true;
			}
		}

		public override Expression Reduce()
		{
			ConstantExpression constantExpression = Expression.Constant(CallSite.Create(this.DelegateType, this.Binder));
			return Expression.Invoke(Expression.Field(constantExpression, "Target"), this.Arguments.AddFirst(constantExpression));
		}

		internal static DynamicExpression Make(Type returnType, Type delegateType, CallSiteBinder binder, ReadOnlyCollection<Expression> arguments)
		{
			if (returnType == typeof(object))
			{
				return new DynamicExpressionN(delegateType, binder, arguments);
			}
			return new TypedDynamicExpressionN(returnType, delegateType, binder, arguments);
		}

		internal static DynamicExpression Make(Type returnType, Type delegateType, CallSiteBinder binder, Expression arg0)
		{
			if (returnType == typeof(object))
			{
				return new DynamicExpression1(delegateType, binder, arg0);
			}
			return new TypedDynamicExpression1(returnType, delegateType, binder, arg0);
		}

		internal static DynamicExpression Make(Type returnType, Type delegateType, CallSiteBinder binder, Expression arg0, Expression arg1)
		{
			if (returnType == typeof(object))
			{
				return new DynamicExpression2(delegateType, binder, arg0, arg1);
			}
			return new TypedDynamicExpression2(returnType, delegateType, binder, arg0, arg1);
		}

		internal static DynamicExpression Make(Type returnType, Type delegateType, CallSiteBinder binder, Expression arg0, Expression arg1, Expression arg2)
		{
			if (returnType == typeof(object))
			{
				return new DynamicExpression3(delegateType, binder, arg0, arg1, arg2);
			}
			return new TypedDynamicExpression3(returnType, delegateType, binder, arg0, arg1, arg2);
		}

		internal static DynamicExpression Make(Type returnType, Type delegateType, CallSiteBinder binder, Expression arg0, Expression arg1, Expression arg2, Expression arg3)
		{
			if (returnType == typeof(object))
			{
				return new DynamicExpression4(delegateType, binder, arg0, arg1, arg2, arg3);
			}
			return new TypedDynamicExpression4(returnType, delegateType, binder, arg0, arg1, arg2, arg3);
		}

		public override Type Type
		{
			get
			{
				return typeof(object);
			}
		}

		public sealed override ExpressionType NodeType
		{
			get
			{
				return ExpressionType.Dynamic;
			}
		}

		public CallSiteBinder Binder { get; }

		public Type DelegateType { get; }

		public ReadOnlyCollection<Expression> Arguments
		{
			get
			{
				return this.GetOrMakeArguments();
			}
		}

		[ExcludeFromCodeCoverage]
		internal virtual ReadOnlyCollection<Expression> GetOrMakeArguments()
		{
			throw ContractUtils.Unreachable;
		}

		protected internal override Expression Accept(ExpressionVisitor visitor)
		{
			DynamicExpressionVisitor dynamicExpressionVisitor = visitor as DynamicExpressionVisitor;
			if (dynamicExpressionVisitor != null)
			{
				return dynamicExpressionVisitor.VisitDynamic(this);
			}
			return visitor.VisitDynamic(this);
		}

		[ExcludeFromCodeCoverage]
		internal virtual DynamicExpression Rewrite(Expression[] args)
		{
			throw ContractUtils.Unreachable;
		}

		public DynamicExpression Update(IEnumerable<Expression> arguments)
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
			return ExpressionExtension.MakeDynamic(this.DelegateType, this.Binder, arguments);
		}

		[ExcludeFromCodeCoverage]
		internal virtual bool SameArguments(ICollection<Expression> arguments)
		{
			throw ContractUtils.Unreachable;
		}

		[ExcludeFromCodeCoverage]
		Expression IArgumentProvider.GetArgument(int index)
		{
			throw ContractUtils.Unreachable;
		}

		[ExcludeFromCodeCoverage]
		int IArgumentProvider.ArgumentCount
		{
			get
			{
				throw ContractUtils.Unreachable;
			}
		}

		public new static DynamicExpression Dynamic(CallSiteBinder binder, Type returnType, params Expression[] arguments)
		{
			return ExpressionExtension.Dynamic(binder, returnType, arguments);
		}

		public new static DynamicExpression Dynamic(CallSiteBinder binder, Type returnType, IEnumerable<Expression> arguments)
		{
			return ExpressionExtension.Dynamic(binder, returnType, arguments);
		}

		public new static DynamicExpression Dynamic(CallSiteBinder binder, Type returnType, Expression arg0)
		{
			return ExpressionExtension.Dynamic(binder, returnType, arg0);
		}

		public new static DynamicExpression Dynamic(CallSiteBinder binder, Type returnType, Expression arg0, Expression arg1)
		{
			return ExpressionExtension.Dynamic(binder, returnType, arg0, arg1);
		}

		public new static DynamicExpression Dynamic(CallSiteBinder binder, Type returnType, Expression arg0, Expression arg1, Expression arg2)
		{
			return ExpressionExtension.Dynamic(binder, returnType, arg0, arg1, arg2);
		}

		public new static DynamicExpression Dynamic(CallSiteBinder binder, Type returnType, Expression arg0, Expression arg1, Expression arg2, Expression arg3)
		{
			return ExpressionExtension.Dynamic(binder, returnType, arg0, arg1, arg2, arg3);
		}

		public new static DynamicExpression MakeDynamic(Type delegateType, CallSiteBinder binder, IEnumerable<Expression> arguments)
		{
			return ExpressionExtension.MakeDynamic(delegateType, binder, arguments);
		}

		public new static DynamicExpression MakeDynamic(Type delegateType, CallSiteBinder binder, params Expression[] arguments)
		{
			return ExpressionExtension.MakeDynamic(delegateType, binder, arguments);
		}

		public new static DynamicExpression MakeDynamic(Type delegateType, CallSiteBinder binder, Expression arg0)
		{
			return ExpressionExtension.MakeDynamic(delegateType, binder, arg0);
		}

		public new static DynamicExpression MakeDynamic(Type delegateType, CallSiteBinder binder, Expression arg0, Expression arg1)
		{
			return ExpressionExtension.MakeDynamic(delegateType, binder, arg0, arg1);
		}

		public new static DynamicExpression MakeDynamic(Type delegateType, CallSiteBinder binder, Expression arg0, Expression arg1, Expression arg2)
		{
			return ExpressionExtension.MakeDynamic(delegateType, binder, arg0, arg1, arg2);
		}

		public new static DynamicExpression MakeDynamic(Type delegateType, CallSiteBinder binder, Expression arg0, Expression arg1, Expression arg2, Expression arg3)
		{
			return ExpressionExtension.MakeDynamic(delegateType, binder, arg0, arg1, arg2, arg3);
		}

		Expression IDynamicExpression.Rewrite(Expression[] args)
		{
			return this.Rewrite(args);
		}

		object IDynamicExpression.CreateCallSite()
		{
			return CallSite.Create(this.DelegateType, this.Binder);
		}

		internal DynamicExpression()
		{
			global::Unity.ThrowStub.ThrowNotSupportedException();
		}
	}
}
