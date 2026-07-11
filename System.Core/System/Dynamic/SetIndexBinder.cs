using System;
using System.Dynamic.Utils;

namespace System.Dynamic
{
	public abstract class SetIndexBinder : DynamicMetaObjectBinder
	{
		protected SetIndexBinder(CallInfo callInfo)
		{
			ContractUtils.RequiresNotNull(callInfo, "callInfo");
			this.CallInfo = callInfo;
		}

		public sealed override Type ReturnType
		{
			get
			{
				return typeof(object);
			}
		}

		public CallInfo CallInfo { get; }

		public sealed override DynamicMetaObject Bind(DynamicMetaObject target, DynamicMetaObject[] args)
		{
			ContractUtils.RequiresNotNull(target, "target");
			ContractUtils.RequiresNotNull(args, "args");
			ContractUtils.Requires(args.Length >= 2, "args");
			DynamicMetaObject dynamicMetaObject = args[args.Length - 1];
			DynamicMetaObject[] array = args.RemoveLast<DynamicMetaObject>();
			ContractUtils.RequiresNotNull(dynamicMetaObject, "args");
			ContractUtils.RequiresNotNullItems<DynamicMetaObject>(array, "args");
			return target.BindSetIndex(this, array, dynamicMetaObject);
		}

		internal sealed override bool IsStandardBinder
		{
			get
			{
				return true;
			}
		}

		public DynamicMetaObject FallbackSetIndex(DynamicMetaObject target, DynamicMetaObject[] indexes, DynamicMetaObject value)
		{
			return this.FallbackSetIndex(target, indexes, value, null);
		}

		public abstract DynamicMetaObject FallbackSetIndex(DynamicMetaObject target, DynamicMetaObject[] indexes, DynamicMetaObject value, DynamicMetaObject errorSuggestion);
	}
}
