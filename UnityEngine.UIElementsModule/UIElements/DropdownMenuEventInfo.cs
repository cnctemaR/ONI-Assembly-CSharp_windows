using System;

namespace UnityEngine.UIElements
{
	public class DropdownMenuEventInfo
	{
		public EventModifiers modifiers { get; }

		public Vector2 mousePosition { get; }

		public Vector2 localMousePosition { get; }

		private char character { get; }

		private KeyCode keyCode { get; }

		public DropdownMenuEventInfo(EventBase e)
		{
			IMouseEvent mouseEvent = e as IMouseEvent;
			bool flag = mouseEvent != null;
			if (flag)
			{
				this.mousePosition = mouseEvent.mousePosition;
				this.localMousePosition = mouseEvent.localMousePosition;
				this.modifiers = mouseEvent.modifiers;
				this.character = '\0';
				this.keyCode = KeyCode.None;
			}
			else
			{
				IKeyboardEvent keyboardEvent = e as IKeyboardEvent;
				bool flag2 = keyboardEvent != null;
				if (flag2)
				{
					this.character = keyboardEvent.character;
					this.keyCode = keyboardEvent.keyCode;
					this.modifiers = keyboardEvent.modifiers;
					this.mousePosition = Vector2.zero;
					this.localMousePosition = Vector2.zero;
				}
			}
		}
	}
}
