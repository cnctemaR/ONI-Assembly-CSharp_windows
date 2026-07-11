using System;
using System.Runtime.InteropServices;

namespace System.Diagnostics
{
	[AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Enum | AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Delegate, AllowMultiple = true)]
	[ComVisible(true)]
	public sealed class DebuggerDisplayAttribute : Attribute
	{
		public DebuggerDisplayAttribute(string value)
		{
			if (value == null)
			{
				value = string.Empty;
			}
			this.value = value;
			this.type = string.Empty;
			this.name = string.Empty;
		}

		public string Value
		{
			get
			{
				return this.value;
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
				if (value == null)
				{
					throw new ArgumentNullException("value");
				}
				this.target_type = value;
				this.target_type_name = this.target_type.AssemblyQualifiedName;
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

		public string Type
		{
			get
			{
				return this.type;
			}
			set
			{
				this.type = value;
			}
		}

		public string Name
		{
			get
			{
				return this.name;
			}
			set
			{
				this.name = value;
			}
		}

		private string value;

		private string type;

		private string name;

		private string target_type_name;

		private Type target_type;
	}
}
