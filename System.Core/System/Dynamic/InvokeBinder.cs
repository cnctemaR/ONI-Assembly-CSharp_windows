using System;
using System.Dynamic.Utils;

namespace System.Dynamic
{
	public abstract class InvokeBinder : DynamicMetaObjectBinder
	{
		protected InvokeBinder(CallInfo callInfo)
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

		public DynamicMetaObject FallbackInvoke(DynamicMetaObject target, DynamicMetaObject[] args)
		{
			return this.FallbackInvoke(target, args, null);
		}

		public abstract DynamicMetaObject FallbackInvoke(DynamicMetaObject target, DynamicMetaObject[] args, DynamicMetaObject errorSuggestion);

		public sealed override DynamicMetaObject Bind(DynamicMetaObject target, DynamicMetaObject[] args)
		{
			ContractUtils.RequiresNotNull(target, "target");
			ContractUtils.RequiresNotNullItems<DynamicMetaObject>(args, "args");
			return target.BindInvoke(this, args);
		}

		internal sealed override bool IsStandardBinder
		{
			get
			{
				return true;
			}
		}
	}
}
