using System;
using System.Runtime.InteropServices;

namespace System.Reflection
{
	[AttributeUsage(AttributeTargets.Assembly, Inherited = false)]
	[ComVisible(true)]
	public sealed class AssemblyCopyrightAttribute : Attribute
	{
		public AssemblyCopyrightAttribute(string copyright)
		{
			this.m_copyright = copyright;
		}

		public string Copyright
		{
			get
			{
				return this.m_copyright;
			}
		}

		private string m_copyright;
	}
}
