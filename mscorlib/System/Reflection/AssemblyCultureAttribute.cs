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
			this.m_culture = culture;
		}

		public string Culture
		{
			get
			{
				return this.m_culture;
			}
		}

		private string m_culture;
	}
}
