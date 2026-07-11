using System;

namespace UnityEngine.Experimental.UIElements
{
	public class UxmlChildElementDescription
	{
		public UxmlChildElementDescription(Type t)
		{
			this.elementName = t.Name;
			this.elementNamespace = t.Namespace;
		}

		public string elementName { get; protected set; }

		public string elementNamespace { get; protected set; }
	}
}
