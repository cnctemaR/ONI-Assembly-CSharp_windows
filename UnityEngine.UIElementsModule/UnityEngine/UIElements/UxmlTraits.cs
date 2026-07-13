using System;

namespace UnityEngine.UIElements
{
	[Obsolete("UxmlTraits is deprecated and will be removed. Use UxmlElementAttribute instead.", false)]
	public abstract class UxmlTraits : BaseUxmlTraits
	{
		public virtual void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
		{
		}
	}
}
