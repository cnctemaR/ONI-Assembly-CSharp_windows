using System;
using System.Dynamic.Utils;

namespace System.Dynamic
{
	public abstract class GetMemberBinder : DynamicMetaObjectBinder
	{
		protected GetMemberBinder(string name, bool ignoreCase)
		{
			ContractUtils.RequiresNotNull(name, "name");
			this.Name = name;
			this.IgnoreCase = ignoreCase;
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

		public DynamicMetaObject FallbackGetMember(DynamicMetaObject target)
		{
			return this.FallbackGetMember(target, null);
		}

		public abstract DynamicMetaObject FallbackGetMember(DynamicMetaObject target, DynamicMetaObject errorSuggestion);

		public sealed override DynamicMetaObject Bind(DynamicMetaObject target, DynamicMetaObject[] args)
		{
			ContractUtils.RequiresNotNull(target, "target");
			ContractUtils.Requires(args == null || args.Length == 0, "args");
			return target.BindGetMember(this);
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
