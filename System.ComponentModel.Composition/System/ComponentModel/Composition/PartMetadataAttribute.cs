using System;

namespace System.ComponentModel.Composition
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
	public sealed class PartMetadataAttribute : Attribute
	{
		public PartMetadataAttribute(string name, object value)
		{
			this.Name = name ?? string.Empty;
			this.Value = value;
		}

		public string Name { get; private set; }

		public object Value { get; private set; }
	}
}
