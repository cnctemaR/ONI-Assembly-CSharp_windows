using System;
using System.Reflection;
using UnityEngine;

namespace NodeEditorFramework
{
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
	public class EventHandlerAttribute : Attribute
	{
		public EventType? handledEvent { get; private set; }

		public int priority { get; private set; }

		public EventHandlerAttribute(EventType eventType, int priorityValue)
		{
			this.handledEvent = new EventType?(eventType);
			this.priority = priorityValue;
		}

		public EventHandlerAttribute(int priorityValue)
		{
			this.handledEvent = null;
			this.priority = priorityValue;
		}

		public EventHandlerAttribute(EventType eventType)
		{
			this.handledEvent = new EventType?(eventType);
			this.priority = 50;
		}

		public EventHandlerAttribute()
		{
			this.handledEvent = null;
		}

		internal static bool AssureValidity(MethodInfo method, EventHandlerAttribute attr)
		{
			if (!method.IsGenericMethod && !method.IsGenericMethodDefinition && (method.ReturnType == null || method.ReturnType == typeof(void)))
			{
				ParameterInfo[] parameters = method.GetParameters();
				if (parameters.Length == 1 && parameters[0].ParameterType == typeof(NodeEditorInputInfo))
				{
					return true;
				}
				global::Debug.LogWarning("Method " + method.Name + " has incorrect signature for EventHandlerAttribute!");
			}
			return false;
		}
	}
}
