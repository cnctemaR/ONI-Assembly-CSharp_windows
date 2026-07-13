using System;
using System.Collections.Generic;
using System.Reflection;

namespace UnityEngine.UIElements
{
	internal static class EventInterestReflectionUtils
	{
		internal static void GetDefaultEventInterests(Type elementType, out int defaultActionCategories, out int defaultActionAtTargetCategories, out int handleEventTrickleDownCategories, out int handleEventBubbleUpCategories)
		{
			EventInterestReflectionUtils.DefaultEventInterests defaultEventInterests;
			bool flag = !EventInterestReflectionUtils.s_DefaultEventInterests.TryGetValue(elementType, out defaultEventInterests);
			if (flag)
			{
				Type baseType = elementType.BaseType;
				bool flag2 = baseType != null;
				if (flag2)
				{
					EventInterestReflectionUtils.GetDefaultEventInterests(baseType, out defaultEventInterests.DefaultActionCategories, out defaultEventInterests.DefaultActionAtTargetCategories, out defaultEventInterests.HandleEventTrickleDownCategories, out defaultEventInterests.HandleEventBubbleUpCategories);
				}
				defaultEventInterests.DefaultActionCategories |= EventInterestReflectionUtils.ComputeDefaultEventInterests(elementType, "ExecuteDefaultAction") | EventInterestReflectionUtils.ComputeDefaultEventInterests(elementType, "ExecuteDefaultActionDisabled");
				defaultEventInterests.DefaultActionAtTargetCategories |= EventInterestReflectionUtils.ComputeDefaultEventInterests(elementType, "ExecuteDefaultActionAtTarget") | EventInterestReflectionUtils.ComputeDefaultEventInterests(elementType, "ExecuteDefaultActionDisabledAtTarget");
				defaultEventInterests.HandleEventTrickleDownCategories |= EventInterestReflectionUtils.ComputeDefaultEventInterests(elementType, "HandleEventTrickleDown") | EventInterestReflectionUtils.ComputeDefaultEventInterests(elementType, "HandleEventTrickleDownDisabled");
				defaultEventInterests.HandleEventBubbleUpCategories |= EventInterestReflectionUtils.ComputeDefaultEventInterests(elementType, "HandleEventBubbleUp") | EventInterestReflectionUtils.ComputeDefaultEventInterests(elementType, "HandleEventBubbleUpDisabled");
				EventInterestReflectionUtils.s_DefaultEventInterests.Add(elementType, defaultEventInterests);
			}
			defaultActionCategories = defaultEventInterests.DefaultActionCategories;
			defaultActionAtTargetCategories = defaultEventInterests.DefaultActionAtTargetCategories;
			handleEventTrickleDownCategories = defaultEventInterests.HandleEventTrickleDownCategories;
			handleEventBubbleUpCategories = defaultEventInterests.HandleEventBubbleUpCategories;
		}

		private static int ComputeDefaultEventInterests(Type elementType, string methodName)
		{
			MethodInfo method = elementType.GetMethod(methodName, BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			bool flag = method == null;
			int num;
			if (flag)
			{
				num = 0;
			}
			else
			{
				bool flag2 = false;
				int num2 = 0;
				object[] customAttributes = method.GetCustomAttributes(typeof(EventInterestAttribute), false);
				foreach (EventInterestAttribute eventInterestAttribute in customAttributes)
				{
					flag2 = true;
					bool flag3 = eventInterestAttribute.eventTypes != null;
					if (flag3)
					{
						foreach (Type type in eventInterestAttribute.eventTypes)
						{
							num2 |= 1 << (int)EventInterestReflectionUtils.GetEventCategory(type);
						}
					}
					num2 |= (int)eventInterestAttribute.categoryFlags;
				}
				num = (flag2 ? num2 : (-1));
			}
			return num;
		}

		internal static EventCategory GetEventCategory(Type eventType)
		{
			EventCategory category;
			bool flag = EventInterestReflectionUtils.s_EventCategories.TryGetValue(eventType, out category);
			EventCategory eventCategory;
			if (flag)
			{
				eventCategory = category;
			}
			else
			{
				object[] customAttributes = eventType.GetCustomAttributes(typeof(EventCategoryAttribute), true);
				object[] array = customAttributes;
				int num = 0;
				if (num >= array.Length)
				{
					throw new ArgumentOutOfRangeException("eventType", "Type must derive from EventBase<T>");
				}
				EventCategoryAttribute eventCategoryAttribute = (EventCategoryAttribute)array[num];
				category = eventCategoryAttribute.category;
				EventInterestReflectionUtils.s_EventCategories.Add(eventType, category);
				eventCategory = category;
			}
			return eventCategory;
		}

		private static readonly Dictionary<Type, EventInterestReflectionUtils.DefaultEventInterests> s_DefaultEventInterests = new Dictionary<Type, EventInterestReflectionUtils.DefaultEventInterests>();

		private static readonly Dictionary<Type, EventCategory> s_EventCategories = new Dictionary<Type, EventCategory>();

		private struct DefaultEventInterests
		{
			public int DefaultActionCategories;

			public int DefaultActionAtTargetCategories;

			public int HandleEventTrickleDownCategories;

			public int HandleEventBubbleUpCategories;
		}
	}
}
