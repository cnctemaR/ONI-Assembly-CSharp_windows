using System;
using System.Runtime.InteropServices;

namespace System.Reflection
{
	[ComVisible(true)]
	[AttributeUsage(AttributeTargets.Assembly, Inherited = false)]
	public sealed class AssemblyInformationalVersionAttribute : Attribute
	{
		public AssemblyInformationalVersionAttribute(string informationalVersion)
		{
			this.m_informationalVersion = informationalVersion;
		}

		public string InformationalVersion
		{
			get
			{
				return this.m_informationalVersion;
			}
		}

		private string m_informationalVersion;
	}
}
