using System;
using System.Runtime.InteropServices;

namespace System.Reflection
{
	[AttributeUsage(AttributeTargets.Assembly, Inherited = false)]
	[ComVisible(true)]
	public sealed class AssemblyKeyNameAttribute : Attribute
	{
		public AssemblyKeyNameAttribute(string keyName)
		{
			this.name = keyName;
		}

		public string KeyName
		{
			get
			{
				return this.name;
			}
		}

		private string name;
	}
}
