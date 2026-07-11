using System;

namespace UnityEngine.Experimental.UIElements
{
	public struct ManipulatorActivationFilter
	{
		public bool Matches(IMouseEvent e)
		{
			bool flag = this.clickCount == 0 || e.clickCount >= this.clickCount;
			return this.button == (MouseButton)e.button && this.HasModifiers(e) && flag;
		}

		private bool HasModifiers(IMouseEvent e)
		{
			return ((this.modifiers & EventModifiers.Alt) == EventModifiers.None || e.altKey) && ((this.modifiers & EventModifiers.Alt) != EventModifiers.None || !e.altKey) && ((this.modifiers & EventModifiers.Control) == EventModifiers.None || e.ctrlKey) && ((this.modifiers & EventModifiers.Control) != EventModifiers.None || !e.ctrlKey) && ((this.modifiers & EventModifiers.Shift) == EventModifiers.None || e.shiftKey) && ((this.modifiers & EventModifiers.Shift) != EventModifiers.None || !e.shiftKey) && ((this.modifiers & EventModifiers.Command) == EventModifiers.None || e.commandKey) && ((this.modifiers & EventModifiers.Command) != EventModifiers.None || !e.commandKey);
		}

		public MouseButton button;

		public EventModifiers modifiers;

		public int clickCount;
	}
}
