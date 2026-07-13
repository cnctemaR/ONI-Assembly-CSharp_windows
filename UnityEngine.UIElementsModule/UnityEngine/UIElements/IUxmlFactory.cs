using System;

namespace UnityEngine.UIElements
{
	[Obsolete("IUxmlFactory is deprecated and will be removed. Use UxmlElementAttribute instead.", false)]
	public interface IUxmlFactory : IBaseUxmlFactory
	{
		VisualElement Create(IUxmlAttributes bag, CreationContext cc);
	}
}
