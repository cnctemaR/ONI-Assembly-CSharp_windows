using System;

namespace System.Runtime.Serialization
{
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, Inherited = true, AllowMultiple = true)]
	public sealed class KnownTypeAttribute : Attribute
	{
		public KnownTypeAttribute(string methodName)
		{
			this.method_name = methodName;
		}

		public KnownTypeAttribute(Type type)
		{
			this.type = type;
		}

		public string MethodName
		{
			get
			{
				return this.method_name;
			}
		}

		public Type Type
		{
			get
			{
				return this.type;
			}
		}

		private string method_name;

		private Type type;
	}
}
