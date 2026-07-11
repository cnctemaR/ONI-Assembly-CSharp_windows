using System;
using System.Runtime.InteropServices;

namespace System.Reflection
{
	[AttributeUsage(AttributeTargets.Assembly, Inherited = false)]
	[ComVisible(true)]
	public sealed class AssemblyTitleAttribute : Attribute
	{
		public AssemblyTitleAttribute(string title)
		{
			this.name = title;
		}

		public string Title
		{
			get
			{
				return this.name;
			}
		}

		private string name;
	}
}
