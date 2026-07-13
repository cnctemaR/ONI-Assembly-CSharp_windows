using System;

namespace UnityEngine.UIElements
{
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
	public class UxmlAttributeAttribute : Attribute
	{
		public UxmlAttributeAttribute()
			: this(null, null)
		{
		}

		public UxmlAttributeAttribute(string name)
			: this(name, null)
		{
		}

		public UxmlAttributeAttribute(string name, params string[] obsoleteNames)
		{
			this.name = name;
			this.obsoleteNames = obsoleteNames;
		}

		public string name;

		public string[] obsoleteNames;
	}
}
