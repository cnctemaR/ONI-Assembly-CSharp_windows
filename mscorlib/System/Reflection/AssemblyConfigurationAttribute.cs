using System;
using System.Runtime.InteropServices;

namespace System.Reflection
{
	[ComVisible(true)]
	[AttributeUsage(AttributeTargets.Assembly, Inherited = false)]
	public sealed class AssemblyConfigurationAttribute : Attribute
	{
		public AssemblyConfigurationAttribute(string configuration)
		{
			this.m_configuration = configuration;
		}

		public string Configuration
		{
			get
			{
				return this.m_configuration;
			}
		}

		private string m_configuration;
	}
}
