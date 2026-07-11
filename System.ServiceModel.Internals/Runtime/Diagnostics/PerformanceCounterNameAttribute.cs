using System;

namespace System.Runtime.Diagnostics
{
	[AttributeUsage(AttributeTargets.Field, Inherited = false)]
	internal sealed class PerformanceCounterNameAttribute : Attribute
	{
		public PerformanceCounterNameAttribute(string name)
		{
			this.Name = name;
		}

		public string Name { get; set; }
	}
}
