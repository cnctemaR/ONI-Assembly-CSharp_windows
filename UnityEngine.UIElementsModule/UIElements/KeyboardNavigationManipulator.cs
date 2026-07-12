using System;
using System.Runtime.CompilerServices;

namespace UnityEngine.UIElements
{
	public class KeyboardNavigationManipulator : Manipulator
	{
		public KeyboardNavigationManipulator(Action<KeyboardNavigationOperation, EventBase> action)
		{
			this.m_Action = action;
		}

		protected override void RegisterCallbacksOnTarget()
		{
			base.target.RegisterCallback<NavigationMoveEvent>(new EventCallback<NavigationMoveEvent>(this.OnNavigationMove), TrickleDown.NoTrickleDown);
			base.target.RegisterCallback<NavigationSubmitEvent>(new EventCallback<NavigationSubmitEvent>(this.OnNavigationSubmit), TrickleDown.NoTrickleDown);
			base.target.RegisterCallback<NavigationCancelEvent>(new EventCallback<NavigationCancelEvent>(this.OnNavigationCancel), TrickleDown.NoTrickleDown);
			base.target.RegisterCallback<KeyDownEvent>(new EventCallback<KeyDownEvent>(this.OnKeyDown), TrickleDown.NoTrickleDown);
		}

		protected override void UnregisterCallbacksFromTarget()
		{
			base.target.UnregisterCallback<NavigationMoveEvent>(new EventCallback<NavigationMoveEvent>(this.OnNavigationMove), TrickleDown.NoTrickleDown);
			base.target.UnregisterCallback<NavigationSubmitEvent>(new EventCallback<NavigationSubmitEvent>(this.OnNavigationSubmit), TrickleDown.NoTrickleDown);
			base.target.UnregisterCallback<NavigationCancelEvent>(new EventCallback<NavigationCancelEvent>(this.OnNavigationCancel), TrickleDown.NoTrickleDown);
			base.target.UnregisterCallback<KeyDownEvent>(new EventCallback<KeyDownEvent>(this.OnKeyDown), TrickleDown.NoTrickleDown);
		}

		internal void OnKeyDown(KeyDownEvent evt)
		{
			KeyboardNavigationManipulator.<>c__DisplayClass4_0 CS$<>8__locals1;
			CS$<>8__locals1.evt = evt;
			KeyboardNavigationOperation keyboardNavigationOperation = KeyboardNavigationManipulator.<OnKeyDown>g__GetOperation|4_0(ref CS$<>8__locals1);
			bool flag = keyboardNavigationOperation > KeyboardNavigationOperation.None;
			if (flag)
			{
				this.Invoke(keyboardNavigationOperation, CS$<>8__locals1.evt);
			}
		}

		private void OnNavigationCancel(NavigationCancelEvent evt)
		{
			this.Invoke(KeyboardNavigationOperation.Cancel, evt);
		}

		private void OnNavigationSubmit(NavigationSubmitEvent evt)
		{
			this.Invoke(KeyboardNavigationOperation.Submit, evt);
		}

		private void OnNavigationMove(NavigationMoveEvent evt)
		{
			switch (evt.direction)
			{
			case NavigationMoveEvent.Direction.Left:
				this.Invoke(KeyboardNavigationOperation.MoveLeft, evt);
				break;
			case NavigationMoveEvent.Direction.Up:
				this.Invoke(KeyboardNavigationOperation.Previous, evt);
				break;
			case NavigationMoveEvent.Direction.Right:
				this.Invoke(KeyboardNavigationOperation.MoveRight, evt);
				break;
			case NavigationMoveEvent.Direction.Down:
				this.Invoke(KeyboardNavigationOperation.Next, evt);
				break;
			}
		}

		private void Invoke(KeyboardNavigationOperation operation, EventBase evt)
		{
			Action<KeyboardNavigationOperation, EventBase> action = this.m_Action;
			if (action != null)
			{
				action(operation, evt);
			}
		}

		[CompilerGenerated]
		internal static KeyboardNavigationOperation <OnKeyDown>g__GetOperation|4_0(ref KeyboardNavigationManipulator.<>c__DisplayClass4_0 A_0)
		{
			KeyCode keyCode = A_0.evt.keyCode;
			KeyCode keyCode2 = keyCode;
			if (keyCode2 != KeyCode.A)
			{
				switch (keyCode2)
				{
				case KeyCode.UpArrow:
				case KeyCode.DownArrow:
				case KeyCode.RightArrow:
				case KeyCode.LeftArrow:
					A_0.evt.StopPropagation();
					break;
				case KeyCode.Home:
					return KeyboardNavigationOperation.Begin;
				case KeyCode.End:
					return KeyboardNavigationOperation.End;
				case KeyCode.PageUp:
					return KeyboardNavigationOperation.PageUp;
				case KeyCode.PageDown:
					return KeyboardNavigationOperation.PageDown;
				}
			}
			else if (A_0.evt.actionKey)
			{
				return KeyboardNavigationOperation.SelectAll;
			}
			return KeyboardNavigationOperation.None;
		}

		private readonly Action<KeyboardNavigationOperation, EventBase> m_Action;
	}
}
