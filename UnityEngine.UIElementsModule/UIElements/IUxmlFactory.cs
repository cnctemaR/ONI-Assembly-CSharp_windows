using System;

namespace UnityEngine.UIElements
{
	public interface IUxmlFactory : IBaseUxmlFactory
	{
		VisualElement Create(IUxmlAttributes bag, CreationContext cc);
	}
}
