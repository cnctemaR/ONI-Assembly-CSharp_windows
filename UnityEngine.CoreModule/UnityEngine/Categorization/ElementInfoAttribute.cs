using System;

namespace UnityEngine.Categorization
{
	[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
	public sealed class ElementInfoAttribute : Attribute
	{
		public int Order { get; set; } = int.MaxValue;

		public string Name { get; set; } = null;
	}
}
