using System;

namespace System.Reflection
{
	[AttributeUsage(AttributeTargets.Assembly, Inherited = false)]
	public sealed class AssemblyKeyNameAttribute : Attribute
	{
		public AssemblyKeyNameAttribute(string keyName)
		{
			this.KeyName = keyName;
		}

		public string KeyName { get; }
	}
}
