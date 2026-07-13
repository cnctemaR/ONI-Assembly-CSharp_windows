using System;

namespace UnityEngine.UIElements
{
	[AttributeUsage(AttributeTargets.Class, Inherited = false)]
	public class UxmlElementAttribute : Attribute
	{
		public UxmlElementAttribute()
			: this(null)
		{
		}

		public UxmlElementAttribute(string uxmlName)
			: this(uxmlName, null)
		{
		}

		public UxmlElementAttribute(string uxmlName, params Type[] supportedTypes)
		{
			this.name = uxmlName;
			this.supportedChildTypes = supportedTypes;
		}

		public readonly string name;

		public LibraryVisibility visibility = LibraryVisibility.Default;

		public string libraryPath;

		internal readonly Type[] supportedChildTypes;
	}
}
