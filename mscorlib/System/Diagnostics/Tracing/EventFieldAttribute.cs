using System;

namespace System.Diagnostics.Tracing
{
	[AttributeUsage(AttributeTargets.Property)]
	public class EventFieldAttribute : Attribute
	{
		public EventFieldTags Tags { get; set; }

		internal string Name { get; set; }

		public EventFieldFormat Format { get; set; }
	}
}
