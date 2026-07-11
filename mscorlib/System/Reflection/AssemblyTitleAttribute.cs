using System;
using System.Runtime.InteropServices;

namespace System.Reflection
{
	[ComVisible(true)]
	[AttributeUsage(AttributeTargets.Assembly, Inherited = false)]
	public sealed class AssemblyTitleAttribute : Attribute
	{
		public AssemblyTitleAttribute(string title)
		{
			this.m_title = title;
		}

		public string Title
		{
			get
			{
				return this.m_title;
			}
		}

		private string m_title;
	}
}
