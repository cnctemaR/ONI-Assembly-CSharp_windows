using System;

namespace UnityEngine.UIElements
{
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
	public class UxmlTypeReferenceAttribute : PropertyAttribute
	{
		public UxmlTypeReferenceAttribute()
			: this(null)
		{
		}

		public UxmlTypeReferenceAttribute(Type baseType)
		{
			this.baseType = baseType;
		}

		public Type baseType;
	}
}
