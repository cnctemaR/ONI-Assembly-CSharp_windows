using System;
using System.Runtime.InteropServices;

namespace System.Diagnostics
{
	[ComVisible(true)]
	[AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = true)]
	public sealed class DebuggerTypeProxyAttribute : Attribute
	{
		public DebuggerTypeProxyAttribute(string typeName)
		{
			this.proxy_type_name = typeName;
		}

		public DebuggerTypeProxyAttribute(Type type)
		{
			this.proxy_type_name = type.Name;
		}

		public string ProxyTypeName
		{
			get
			{
				return this.proxy_type_name;
			}
		}

		public Type Target
		{
			get
			{
				return this.target_type;
			}
			set
			{
				this.target_type = value;
				this.target_type_name = this.target_type.Name;
			}
		}

		public string TargetTypeName
		{
			get
			{
				return this.target_type_name;
			}
			set
			{
				this.target_type_name = value;
			}
		}

		private string proxy_type_name;

		private string target_type_name;

		private Type target_type;
	}
}
