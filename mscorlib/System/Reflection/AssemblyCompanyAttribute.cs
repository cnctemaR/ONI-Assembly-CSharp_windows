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
			this.name = company;
		}

		public string Company
		{
			get
			{
				return this.name;
			}
		}

		private string name;
	}
}
