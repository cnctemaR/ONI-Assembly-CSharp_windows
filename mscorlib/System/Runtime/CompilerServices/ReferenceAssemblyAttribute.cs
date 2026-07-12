using System;

namespace System.Runtime.CompilerServices
{
	[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = false)]
	[Serializable]
	public sealed class ReferenceAssemblyAttribute : Attribute
	{
		public ReferenceAssemblyAttribute()
		{
		}

		public ReferenceAssemblyAttribute(string description)
		{
			this.Description = description;
		}

		public string Description { get; }
	}
}
