using System;

namespace UnityEngine.Experimental.UIElements
{
	public interface IUxmlAttributes
	{
		bool TryGetAttributeValue(string attributeName, out string value);
	}
}
