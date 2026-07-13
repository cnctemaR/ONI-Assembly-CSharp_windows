using System;

namespace UnityEngine.UIElements
{
	public readonly struct UxmlAttributeNames
	{
		public UxmlAttributeNames(string fieldName, string uxmlName, Type typeReference = null, params string[] obsoleteNames)
		{
			this.fieldName = fieldName;
			this.uxmlName = uxmlName;
			this.obsoleteNames = obsoleteNames ?? Array.Empty<string>();
			this.typeReference = typeReference;
		}

		public readonly string fieldName;

		public readonly string uxmlName;

		public readonly Type typeReference;

		public readonly string[] obsoleteNames;
	}
}
