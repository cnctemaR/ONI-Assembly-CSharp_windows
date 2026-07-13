using System;
using System.Reflection;

namespace UnityEngine.UIElements
{
	internal readonly struct UxmlDescription
	{
		public UxmlDescription(string uxmlName, string cSharpName, string overriddenCSharpName, FieldInfo serializedField, string[] obsoleteNames = null)
		{
			this.uxmlName = uxmlName;
			this.cSharpName = cSharpName;
			this.overriddenCSharpName = overriddenCSharpName;
			this.serializedField = serializedField;
			this.serializedFieldAttributeFlags = serializedField.DeclaringType.GetField(serializedField.Name + "_UxmlAttributeFlags", BindingFlags.Instance | BindingFlags.NonPublic);
			this.fieldType = ((serializedField.GetCustomAttribute<UxmlTypeReferenceAttribute>() != null) ? typeof(Type) : serializedField.FieldType);
			this.obsoleteNames = obsoleteNames;
		}

		public UxmlDescription(FieldInfo serializedField, UxmlAttributeNames names, string overriddenCSharpName)
		{
			this.uxmlName = names.uxmlName;
			this.cSharpName = names.fieldName;
			this.overriddenCSharpName = overriddenCSharpName;
			this.serializedField = serializedField;
			this.serializedFieldAttributeFlags = serializedField.DeclaringType.GetField(serializedField.Name + "_UxmlAttributeFlags", BindingFlags.Instance | BindingFlags.NonPublic);
			this.fieldType = ((null != names.typeReference) ? typeof(Type) : serializedField.FieldType);
			this.obsoleteNames = names.obsoleteNames;
		}

		public readonly string uxmlName;

		public readonly string cSharpName;

		public readonly string overriddenCSharpName;

		public readonly FieldInfo serializedField;

		public readonly FieldInfo serializedFieldAttributeFlags;

		public readonly Type fieldType;

		public readonly string[] obsoleteNames;
	}
}
