using System;
using System.Runtime.InteropServices;

namespace System.Reflection
{
	[ComVisible(true)]
	[AttributeUsage(AttributeTargets.Assembly, Inherited = false)]
	public sealed class AssemblyDescriptionAttribute : Attribute
	{
		public AssemblyDescriptionAttribute(string description)
		{
			this.m_description = description;
		}

		public string Description
		{
			get
			{
				return this.m_description;
			}
		}

		private string m_description;
	}
}
