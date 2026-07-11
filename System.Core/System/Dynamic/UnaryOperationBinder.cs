using System;
using System.Dynamic.Utils;
using System.Linq.Expressions;

namespace System.Dynamic
{
	public abstract class UnaryOperationBinder : DynamicMetaObjectBinder
	{
		protected UnaryOperationBinder(ExpressionType operation)
		{
			ContractUtils.Requires(UnaryOperationBinder.OperationIsValid(operation), "operation");
			this.Operation = operation;
		}

		public sealed override Type ReturnType
		{
			get
			{
				ExpressionType operation = this.Operation;
				if (operation - ExpressionType.IsTrue <= 1)
				{
					return typeof(bool);
				}
				return typeof(object);
			}
		}

		public ExpressionType Operation { get; }

		public DynamicMetaObject FallbackUnaryOperation(DynamicMetaObject target)
		{
			return this.FallbackUnaryOperation(target, null);
		}

		public abstract DynamicMetaObject FallbackUnaryOperation(DynamicMetaObject target, DynamicMetaObject errorSuggestion);

		public sealed override DynamicMetaObject Bind(DynamicMetaObject target, DynamicMetaObject[] args)
		{
			ContractUtils.RequiresNotNull(target, "target");
			ContractUtils.Requires(args == null || args.Length == 0, "args");
			return target.BindUnaryOperation(this);
		}

		internal sealed override bool IsStandardBinder
		{
			get
			{
				return true;
			}
		}

		internal static bool OperationIsValid(ExpressionType operation)
		{
			if (operation <= ExpressionType.Decrement)
			{
				if (operation - ExpressionType.Negate > 1 && operation != ExpressionType.Not && operation != ExpressionType.Decrement)
				{
					return false;
				}
			}
			else if (operation != ExpressionType.Extension && operation != ExpressionType.Increment && operation - ExpressionType.OnesComplement > 2)
			{
				return false;
			}
			return true;
		}
	}
}
