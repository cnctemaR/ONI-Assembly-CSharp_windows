using System;

namespace UnityEngine.UIElements
{
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, Inherited = false)]
	public class UxmlObjectReferenceAttribute : Attribute
	{
		public UxmlObjectReferenceAttribute()
			: this(null, null)
		{
		}

		public UxmlObjectReferenceAttribute(string uxmlName)
			: this(uxmlName, null)
		{
			bool flag = uxmlName == "null";
			if (flag)
			{
				throw new ArgumentException("UxmlObjectReferenceAttribute name cannot be \"null\".");
			}
		}

		public UxmlObjectReferenceAttribute(string uxmlName, params Type[] acceptedTypes)
		{
			this.name = uxmlName;
			this.types = acceptedTypes;
		}

		public string name;

		public Type[] types;
	}
}
