using System;

namespace System.Reflection
{
	[AttributeUsage(AttributeTargets.Assembly, Inherited = false)]
	public sealed class AssemblyTitleAttribute : Attribute
	{
		public AssemblyTitleAttribute(string title)
		{
			this.Title = title;
		}

		public string Title { get; }
	}
}
