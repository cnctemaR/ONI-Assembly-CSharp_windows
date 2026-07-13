using System;

namespace UnityEngine.UIElements
{
	[Obsolete("UxmlFactory<TCreatedType> is deprecated and will be removed. Use UxmlElementAttribute instead.", false)]
	public class UxmlFactory<TCreatedType> : UxmlFactory<TCreatedType, VisualElement.UxmlTraits> where TCreatedType : VisualElement, new()
	{
	}
}
