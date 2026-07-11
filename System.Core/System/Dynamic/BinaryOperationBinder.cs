using System;
using System.Dynamic.Utils;
using System.Linq.Expressions;

namespace System.Dynamic
{
	public abstract class BinaryOperationBinder : DynamicMetaObjectBinder
	{
		protected BinaryOperationBinder(ExpressionType operation)
		{
			ContractUtils.Requires(BinaryOperationBinder.OperationIsValid(operation), "operation");
			this.Operation = operation;
		}

		public sealed override Type ReturnType
		{
			get
			{
				return typeof(object);
			}
		}

		public ExpressionType Operation { get; }

		public DynamicMetaObject FallbackBinaryOperation(DynamicMetaObject target, DynamicMetaObject arg)
		{
			return this.FallbackBinaryOperation(target, arg, null);
		}

		public abstract DynamicMetaObject FallbackBinaryOperation(DynamicMetaObject target, DynamicMetaObject arg, DynamicMetaObject errorSuggestion);

		public sealed override DynamicMetaObject Bind(DynamicMetaObject target, DynamicMetaObject[] args)
		{
			ContractUtils.RequiresNotNull(target, "target");
			ContractUtils.RequiresNotNull(args, "args");
			ContractUtils.Requires(args.Length == 1, "args");
			DynamicMetaObject dynamicMetaObject = args[0];
			ContractUtils.RequiresNotNull(dynamicMetaObject, "args");
			return target.BindBinaryOperation(this, dynamicMetaObject);
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
			if (operation <= ExpressionType.Multiply)
			{
				if (operation != ExpressionType.Add && operation != ExpressionType.And)
				{
					switch (operation)
					{
					case ExpressionType.Divide:
					case ExpressionType.Equal:
					case ExpressionType.ExclusiveOr:
					case ExpressionType.GreaterThan:
					case ExpressionType.GreaterThanOrEqual:
					case ExpressionType.LeftShift:
					case ExpressionType.LessThan:
					case ExpressionType.LessThanOrEqual:
					case ExpressionType.Modulo:
					case ExpressionType.Multiply:
						break;
					case ExpressionType.Invoke:
					case ExpressionType.Lambda:
					case ExpressionType.ListInit:
					case ExpressionType.MemberAccess:
					case ExpressionType.MemberInit:
						return false;
					default:
						return false;
					}
				}
			}
			else
			{
				switch (operation)
				{
				case ExpressionType.NotEqual:
				case ExpressionType.Or:
				case ExpressionType.Power:
				case ExpressionType.RightShift:
				case ExpressionType.Subtract:
					break;
				case ExpressionType.OrElse:
				case ExpressionType.Parameter:
				case ExpressionType.Quote:
					return false;
				default:
					if (operation != ExpressionType.Extension && operation - ExpressionType.AddAssign > 10)
					{
						return false;
					}
					break;
				}
			}
			return true;
		}
	}
}
