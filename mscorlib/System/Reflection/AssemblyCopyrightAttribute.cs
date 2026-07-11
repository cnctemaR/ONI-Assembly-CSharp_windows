using System;
using System.Runtime.InteropServices;

namespace System.Reflection
{
	[ComVisible(true)]
	[AttributeUsage(AttributeTargets.Assembly, Inherited = false)]
	public sealed class AssemblyCopyrightAttribute : Attribute
	{
		public AssemblyCopyrightAttribute(string copyright)
		{
			this.name = copyright;
		}

		public string Copyright
		{
			get
			{
				return this.name;
			}
		}

		private string name;
	}
}
