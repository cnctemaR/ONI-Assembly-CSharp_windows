using System;

namespace UnityEngine.Experimental.UIElements
{
	/// <summary>
	///   <para>UxmlFactory specialization for classes that derive from VisualElement and that shares its traits, VisualElementTraits.</para>
	/// </summary>
	public class UxmlFactory<TCreatedType> : UxmlFactory<TCreatedType, VisualElement.VisualElementUxmlTraits> where TCreatedType : VisualElement
	{
	}
}
