using System;
using System.Dynamic.Utils;

namespace System.Dynamic
{
	public abstract class InvokeMemberBinder : DynamicMetaObjectBinder
	{
		protected InvokeMemberBinder(string name, bool ignoreCase, CallInfo callInfo)
		{
			ContractUtils.RequiresNotNull(name, "name");
			ContractUtils.RequiresNotNull(callInfo, "callInfo");
			this.Name = name;
			this.IgnoreCase = ignoreCase;
			this.CallInfo = callInfo;
		}

		public sealed override Type ReturnType
		{
			get
			{
				return typeof(object);
			}
		}

		public string Name { get; }

		public bool IgnoreCase { get; }

		public CallInfo CallInfo { get; }

		public sealed override DynamicMetaObject Bind(DynamicMetaObject target, DynamicMetaObject[] args)
		{
			ContractUtils.RequiresNotNull(target, "target");
			ContractUtils.RequiresNotNullItems<DynamicMetaObject>(args, "args");
			return target.BindInvokeMember(this, args);
		}

		internal sealed override bool IsStandardBinder
		{
			get
			{
				return true;
			}
		}

		public DynamicMetaObject FallbackInvokeMember(DynamicMetaObject target, DynamicMetaObject[] args)
		{
			return this.FallbackInvokeMember(target, args, null);
		}

		public abstract DynamicMetaObject FallbackInvokeMember(DynamicMetaObject target, DynamicMetaObject[] args, DynamicMetaObject errorSuggestion);

		public abstract DynamicMetaObject FallbackInvoke(DynamicMetaObject target, DynamicMetaObject[] args, DynamicMetaObject errorSuggestion);
	}
}
