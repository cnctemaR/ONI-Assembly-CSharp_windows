using System;
using System.Runtime.CompilerServices;

namespace UnityEngine.UIElements
{
	internal class EventInterpreter : IEventInterpreter
	{
		public virtual bool IsActivationEvent(EventBase evt)
		{
			bool flag = evt.eventTypeId == EventBase<KeyDownEvent>.TypeId();
			bool flag2;
			if (flag)
			{
				KeyDownEvent keyDownEvent = (KeyDownEvent)evt;
				flag2 = keyDownEvent.keyCode == KeyCode.KeypadEnter || keyDownEvent.keyCode == KeyCode.Return;
			}
			else
			{
				flag2 = false;
			}
			return flag2;
		}

		public virtual bool IsCancellationEvent(EventBase evt)
		{
			bool flag = evt.eventTypeId == EventBase<KeyDownEvent>.TypeId();
			bool flag2;
			if (flag)
			{
				KeyDownEvent keyDownEvent = (KeyDownEvent)evt;
				flag2 = keyDownEvent.keyCode == KeyCode.Escape;
			}
			else
			{
				flag2 = false;
			}
			return flag2;
		}

		public virtual bool IsNavigationEvent(EventBase evt, out NavigationDirection direction)
		{
			bool flag = evt.eventTypeId == EventBase<KeyDownEvent>.TypeId();
			bool flag2;
			if (flag)
			{
				flag2 = (direction = this.GetNavigationDirection((KeyDownEvent)evt)) > NavigationDirection.None;
			}
			else
			{
				direction = NavigationDirection.None;
				flag2 = false;
			}
			return flag2;
		}

		private NavigationDirection GetNavigationDirection(KeyDownEvent keyDownEvent)
		{
			EventInterpreter.<>c__DisplayClass4_0 CS$<>8__locals1;
			CS$<>8__locals1.keyDownEvent = keyDownEvent;
			KeyCode keyCode = CS$<>8__locals1.keyDownEvent.keyCode;
			KeyCode keyCode2 = keyCode;
			NavigationDirection navigationDirection;
			if (keyCode2 != KeyCode.Tab)
			{
				switch (keyCode2)
				{
				case KeyCode.UpArrow:
					return NavigationDirection.Up;
				case KeyCode.DownArrow:
					return NavigationDirection.Down;
				case KeyCode.RightArrow:
					return NavigationDirection.Right;
				case KeyCode.LeftArrow:
					return NavigationDirection.Left;
				case KeyCode.Home:
					return NavigationDirection.Home;
				case KeyCode.End:
					return NavigationDirection.End;
				case KeyCode.PageUp:
					return NavigationDirection.PageUp;
				case KeyCode.PageDown:
					return NavigationDirection.PageDown;
				}
				navigationDirection = NavigationDirection.None;
			}
			else
			{
				navigationDirection = (EventInterpreter.<GetNavigationDirection>g__Shift|4_0(ref CS$<>8__locals1) ? NavigationDirection.Previous : NavigationDirection.Next);
			}
			return navigationDirection;
		}

		[CompilerGenerated]
		internal static bool <GetNavigationDirection>g__Shift|4_0(ref EventInterpreter.<>c__DisplayClass4_0 A_0)
		{
			return (A_0.keyDownEvent.modifiers & EventModifiers.Shift) > EventModifiers.None;
		}

		internal static readonly EventInterpreter s_Instance = new EventInterpreter();
	}
}
