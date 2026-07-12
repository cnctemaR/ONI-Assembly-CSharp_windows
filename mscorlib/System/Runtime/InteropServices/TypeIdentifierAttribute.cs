using System;

namespace System.Runtime.InteropServices
{
	[ComVisible(false)]
	[AttributeUsage(AttributeTargets.Struct | AttributeTargets.Enum | AttributeTargets.Interface | AttributeTargets.Delegate, AllowMultiple = false, Inherited = false)]
	public sealed class TypeIdentifierAttribute : Attribute
	{
		public TypeIdentifierAttribute()
		{
		}

		public TypeIdentifierAttribute(string scope, string identifier)
		{
			this.Scope_ = scope;
			this.Identifier_ = identifier;
		}

		public string Scope
		{
			get
			{
				return this.Scope_;
			}
		}

		public string Identifier
		{
			get
			{
				return this.Identifier_;
			}
		}

		internal string Scope_;

		internal string Identifier_;
	}
}
