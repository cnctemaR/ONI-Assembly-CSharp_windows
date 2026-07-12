using System;

namespace System.ComponentModel.Composition
{
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Interface, AllowMultiple = true, Inherited = false)]
	public sealed class ExportMetadataAttribute : Attribute
	{
		public ExportMetadataAttribute(string name, object value)
		{
			this.Name = name ?? string.Empty;
			this.Value = value;
		}

		public string Name { get; private set; }

		public object Value { get; private set; }

		public bool IsMultiple { get; set; }
	}
}
