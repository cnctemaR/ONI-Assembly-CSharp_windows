using System;
using System.Runtime.InteropServices;

namespace System.Reflection
{
	[AttributeUsage(AttributeTargets.Assembly, Inherited = false)]
	[ComVisible(true)]
	public sealed class AssemblyCompanyAttribute : Attribute
	{
		public AssemblyCompanyAttribute(string company)
		{
			this.m_company = company;
		}

		public string Company
		{
			get
			{
				return this.m_company;
			}
		}

		private string m_company;
	}
}
