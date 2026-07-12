using System;

namespace UnityEngine.UIElements
{
	public class CustomStyleResolvedEvent : EventBase<CustomStyleResolvedEvent>
	{
		public ICustomStyle customStyle
		{
			get
			{
				VisualElement visualElement = base.target as VisualElement;
				return (visualElement != null) ? visualElement.customStyle : null;
			}
		}
	}
}
