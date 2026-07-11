using System;
using System.Runtime.InteropServices;

namespace System.Reflection
{
	[AttributeUsage(AttributeTargets.Assembly, Inherited = false)]
	[ComVisible(true)]
	public sealed class AssemblyConfigurationAttribute : Attribute
	{
		public AssemblyConfigurationAttribute(string configuration)
		{
			this.name = configuration;
		}

		public string Configuration
		{
			get
			{
				return this.name;
			}
		}

		private string name;
	}
}
