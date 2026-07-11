using System;
using System.Dynamic.Utils;

namespace System.Dynamic
{
	public abstract class SetMemberBinder : DynamicMetaObjectBinder
	{
		protected SetMemberBinder(string name, bool ignoreCase)
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

		public sealed override DynamicMetaObject Bind(DynamicMetaObject target, DynamicMetaObject[] args)
		{
			ContractUtils.RequiresNotNull(target, "target");
			ContractUtils.RequiresNotNull(args, "args");
			ContractUtils.Requires(args.Length == 1, "args");
			DynamicMetaObject dynamicMetaObject = args[0];
			ContractUtils.RequiresNotNull(dynamicMetaObject, "args");
			return target.BindSetMember(this, dynamicMetaObject);
		}

		internal sealed override bool IsStandardBinder
		{
			get
			{
				return true;
			}
		}

		public DynamicMetaObject FallbackSetMember(DynamicMetaObject target, DynamicMetaObject value)
		{
			return this.FallbackSetMember(target, value, null);
		}

		public abstract DynamicMetaObject FallbackSetMember(DynamicMetaObject target, DynamicMetaObject value, DynamicMetaObject errorSuggestion);
	}
}
