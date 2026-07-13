using System;

namespace UnityEngine.UIElements
{
	[AttributeUsage(AttributeTargets.Class)]
	internal class EventCategoryAttribute : Attribute
	{
		public EventCategoryAttribute(EventCategory category)
		{
			this.category = category;
		}

		internal EventCategory category;
	}
}
