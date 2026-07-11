using System;
using System.Dynamic.Utils;

namespace System.Dynamic
{
	public abstract class ConvertBinder : DynamicMetaObjectBinder
	{
		protected ConvertBinder(Type type, bool @explicit)
		{
			ContractUtils.RequiresNotNull(type, "type");
			this.Type = type;
			this.Explicit = @explicit;
		}

		public Type Type { get; }

		public bool Explicit { get; }

		public DynamicMetaObject FallbackConvert(DynamicMetaObject target)
		{
			return this.FallbackConvert(target, null);
		}

		public abstract DynamicMetaObject FallbackConvert(DynamicMetaObject target, DynamicMetaObject errorSuggestion);

		public sealed override DynamicMetaObject Bind(DynamicMetaObject target, DynamicMetaObject[] args)
		{
			ContractUtils.RequiresNotNull(target, "target");
			ContractUtils.Requires(args == null || args.Length == 0, "args");
			return target.BindConvert(this);
		}

		internal sealed override bool IsStandardBinder
		{
			get
			{
				return true;
			}
		}

		public sealed override Type ReturnType
		{
			get
			{
				return this.Type;
			}
		}
	}
}
