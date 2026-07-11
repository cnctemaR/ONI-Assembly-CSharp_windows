using System;

namespace UnityEngine.Experimental.UIElements
{
	public class UxmlFactory<TCreatedType> : UxmlFactory<TCreatedType, VisualElement.UxmlTraits> where TCreatedType : VisualElement
	{
	}
}
