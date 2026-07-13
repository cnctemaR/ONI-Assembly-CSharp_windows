using System;

namespace UnityEngine.UIElements
{
	[EventCategory(EventCategory.Style)]
	public class CustomStyleResolvedEvent : EventBase<CustomStyleResolvedEvent>
	{
		static CustomStyleResolvedEvent()
		{
			EventBase<CustomStyleResolvedEvent>.SetCreateFunction(() => new CustomStyleResolvedEvent());
		}

		public ICustomStyle customStyle
		{
			get
			{
				VisualElement elementTarget = base.elementTarget;
				return (elementTarget != null) ? elementTarget.customStyle : null;
			}
		}
	}
}
