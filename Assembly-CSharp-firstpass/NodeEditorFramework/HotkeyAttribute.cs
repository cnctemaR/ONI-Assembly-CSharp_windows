using System;
using System.Reflection;
using UnityEngine;

namespace NodeEditorFramework
{
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
	public class HotkeyAttribute : Attribute
	{
		public KeyCode handledHotKey { get; private set; }

		public EventModifiers? modifiers { get; private set; }

		public EventType? limitingEventType { get; private set; }

		public int priority { get; private set; }

		public HotkeyAttribute(KeyCode handledKey)
		{
			this.handledHotKey = handledKey;
			this.modifiers = null;
			this.limitingEventType = null;
			this.priority = 50;
		}

		public HotkeyAttribute(KeyCode handledKey, EventModifiers eventModifiers)
		{
			this.handledHotKey = handledKey;
			this.modifiers = new EventModifiers?(eventModifiers);
			this.limitingEventType = null;
			this.priority = 50;
		}

		public HotkeyAttribute(KeyCode handledKey, EventType LimitEventType)
		{
			this.handledHotKey = handledKey;
			this.modifiers = null;
			this.limitingEventType = new EventType?(LimitEventType);
			this.priority = 50;
		}

		public HotkeyAttribute(KeyCode handledKey, EventType LimitEventType, int priorityValue)
		{
			this.handledHotKey = handledKey;
			this.modifiers = null;
			this.limitingEventType = new EventType?(LimitEventType);
			this.priority = priorityValue;
		}

		public HotkeyAttribute(KeyCode handledKey, EventModifiers eventModifiers, EventType LimitEventType)
		{
			this.handledHotKey = handledKey;
			this.modifiers = new EventModifiers?(eventModifiers);
			this.limitingEventType = new EventType?(LimitEventType);
			this.priority = 50;
		}

		internal static bool AssureValidity(MethodInfo method, HotkeyAttribute attr)
		{
			if (!method.IsGenericMethod && !method.IsGenericMethodDefinition && (method.ReturnType == null || method.ReturnType == typeof(void)))
			{
				ParameterInfo[] parameters = method.GetParameters();
				if (parameters.Length == 1 && parameters[0].ParameterType.IsAssignableFrom(typeof(NodeEditorInputInfo)))
				{
					return true;
				}
				global::Debug.LogWarning("Method " + method.Name + " has incorrect signature for HotkeyAttribute!");
			}
			return false;
		}
	}
}
