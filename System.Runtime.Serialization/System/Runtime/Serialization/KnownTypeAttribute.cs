using System;

namespace System.Runtime.Serialization
{
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, Inherited = true, AllowMultiple = true)]
	public sealed class KnownTypeAttribute : Attribute
	{
		private KnownTypeAttribute()
		{
		}

		public KnownTypeAttribute(Type type)
		{
			this.type = type;
		}

		public KnownTypeAttribute(string methodName)
		{
			this.methodName = methodName;
		}

		public string MethodName
		{
			get
			{
				return this.methodName;
			}
		}

		public Type Type
		{
			get
			{
				return this.type;
			}
		}

		private string methodName;

		private Type type;
	}
}
