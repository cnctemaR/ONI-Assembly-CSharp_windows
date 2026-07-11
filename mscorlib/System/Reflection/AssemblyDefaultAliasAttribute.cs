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
			this.m_defaultAlias = defaultAlias;
		}

		public string DefaultAlias
		{
			get
			{
				return this.m_defaultAlias;
			}
		}

		private string m_defaultAlias;
	}
}
