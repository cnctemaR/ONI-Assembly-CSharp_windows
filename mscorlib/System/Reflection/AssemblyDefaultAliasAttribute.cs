using System;
using System.Runtime.InteropServices;

namespace System.Reflection
{
	[ComVisible(true)]
	[AttributeUsage(AttributeTargets.Assembly, Inherited = false)]
	public sealed class AssemblyDefaultAliasAttribute : Attribute
	{
		public AssemblyDefaultAliasAttribute(string defaultAlias)
		{
			this.name = defaultAlias;
		}

		public string DefaultAlias
		{
			get
			{
				return this.name;
			}
		}

		private string name;
	}
}
