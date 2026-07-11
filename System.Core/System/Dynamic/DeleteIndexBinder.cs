using System;
using System.Dynamic.Utils;

namespace System.Dynamic
{
	public abstract class DeleteIndexBinder : DynamicMetaObjectBinder
	{
		protected DeleteIndexBinder(CallInfo callInfo)
		{
			ContractUtils.RequiresNotNull(callInfo, "callInfo");
			this.CallInfo = callInfo;
		}

		public sealed override Type ReturnType
		{
			get
			{
				return typeof(void);
			}
		}

		public CallInfo CallInfo { get; }

		public sealed override DynamicMetaObject Bind(DynamicMetaObject target, DynamicMetaObject[] args)
		{
			ContractUtils.RequiresNotNull(target, "target");
			ContractUtils.RequiresNotNullItems<DynamicMetaObject>(args, "args");
			return target.BindDeleteIndex(this, args);
		}

		internal sealed override bool IsStandardBinder
		{
			get
			{
				return true;
			}
		}

		public DynamicMetaObject FallbackDeleteIndex(DynamicMetaObject target, DynamicMetaObject[] indexes)
		{
			return this.FallbackDeleteIndex(target, indexes, null);
		}

		public abstract DynamicMetaObject FallbackDeleteIndex(DynamicMetaObject target, DynamicMetaObject[] indexes, DynamicMetaObject errorSuggestion);
	}
}
