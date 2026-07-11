using System;
using System.Runtime.InteropServices;

namespace System.Reflection
{
	[ComVisible(true)]
	[AttributeUsage(AttributeTargets.Assembly, Inherited = false)]
	public sealed class AssemblyCultureAttribute : Attribute
	{
		public AssemblyCultureAttribute(string culture)
		{
			this.name = culture;
		}

		public string Culture
		{
			get
			{
				return this.name;
			}
		}

		private string name;
	}
}
