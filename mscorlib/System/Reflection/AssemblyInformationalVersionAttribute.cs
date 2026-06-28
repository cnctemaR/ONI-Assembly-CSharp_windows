using System;
using System.Runtime.InteropServices;

namespace System.Reflection
{
	[AttributeUsage(AttributeTargets.Assembly, Inherited = false)]
	[ComVisible(true)]
	public sealed class AssemblyInformationalVersionAttribute : Attribute
	{
		public AssemblyInformationalVersionAttribute(string informationalVersion)
		{
			this.name = informationalVersion;
		}

		public string InformationalVersion
		{
			get
			{
				return this.name;
			}
		}

		private string name;
	}
}
