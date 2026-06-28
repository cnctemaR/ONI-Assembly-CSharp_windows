using System;
using System.Runtime.InteropServices;

namespace System.Reflection
{
	[AttributeUsage(AttributeTargets.Assembly, Inherited = false)]
	[ComVisible(true)]
	public sealed class AssemblyVersionAttribute : Attribute
	{
		public AssemblyVersionAttribute(string version)
		{
			this.name = version;
		}

		public string Version
		{
			get
			{
				return this.name;
			}
		}

		private string name;
	}
}
