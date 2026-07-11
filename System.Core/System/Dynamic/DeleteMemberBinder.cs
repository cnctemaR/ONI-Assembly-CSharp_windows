using System;
using System.Dynamic.Utils;

namespace System.Dynamic
{
	public abstract class DeleteMemberBinder : DynamicMetaObjectBinder
	{
		protected DeleteMemberBinder(string name, bool ignoreCase)
		{
			ContractUtils.RequiresNotNull(name, "name");
			this.Name = name;
			this.IgnoreCase = ignoreCase;
		}

		public string Name { get; }

		public bool IgnoreCase { get; }

		public sealed override Type ReturnType
		{
			get
			{
				return typeof(void);
			}
		}

		public DynamicMetaObject FallbackDeleteMember(DynamicMetaObject target)
		{
			return this.FallbackDeleteMember(target, null);
		}

		public abstract DynamicMetaObject FallbackDeleteMember(DynamicMetaObject target, DynamicMetaObject errorSuggestion);

		public sealed override DynamicMetaObject Bind(DynamicMetaObject target, DynamicMetaObject[] args)
		{
			ContractUtils.RequiresNotNull(target, "target");
			ContractUtils.Requires(args == null || args.Length == 0, "args");
			return target.BindDeleteMember(this);
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
