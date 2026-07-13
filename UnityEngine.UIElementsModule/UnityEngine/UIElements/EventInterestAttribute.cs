using System;

namespace UnityEngine.UIElements
{
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
	public class EventInterestAttribute : Attribute
	{
		public EventInterestAttribute(params Type[] eventTypes)
		{
			this.eventTypes = eventTypes;
		}

		public EventInterestAttribute(EventInterestOptions interests)
		{
			this.categoryFlags = (EventCategoryFlags)interests;
		}

		internal EventInterestAttribute(EventInterestOptionsInternal interests)
		{
			this.categoryFlags = (EventCategoryFlags)interests;
		}

		internal Type[] eventTypes;

		internal EventCategoryFlags categoryFlags = EventCategoryFlags.None;
	}
}
