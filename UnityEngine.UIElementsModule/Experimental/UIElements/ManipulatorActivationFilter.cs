using System;

namespace UnityEngine.Experimental.UIElements
{
	/// <summary>
	///   <para>Used by manipulators to match events against their requirements.</para>
	/// </summary>
	public struct ManipulatorActivationFilter
	{
		/// <summary>
		///   <para>Returns true if the current mouse event satisfies the activation requirements.</para>
		/// </summary>
		/// <param name="e">The mouse event.</param>
		/// <returns>
		///   <para>True if the event matches the requirements.</para>
		/// </returns>
		public bool Matches(IMouseEvent e)
		{
			bool flag = this.clickCount == 0 || e.clickCount >= this.clickCount;
			return this.button == (MouseButton)e.button && this.HasModifiers(e) && flag;
		}

		private bool HasModifiers(IMouseEvent e)
		{
			return ((this.modifiers & EventModifiers.Alt) == EventModifiers.None || e.altKey) && ((this.modifiers & EventModifiers.Alt) != EventModifiers.None || !e.altKey) && ((this.modifiers & EventModifiers.Control) == EventModifiers.None || e.ctrlKey) && ((this.modifiers & EventModifiers.Control) != EventModifiers.None || !e.ctrlKey) && ((this.modifiers & EventModifiers.Shift) == EventModifiers.None || e.shiftKey) && ((this.modifiers & EventModifiers.Shift) != EventModifiers.None || !e.shiftKey) && ((this.modifiers & EventModifiers.Command) == EventModifiers.None || e.commandKey) && ((this.modifiers & EventModifiers.Command) != EventModifiers.None || !e.commandKey);
		}

		/// <summary>
		///   <para>The button that activates the manipulation.</para>
		/// </summary>
		public MouseButton button;

		/// <summary>
		///   <para>Any modifier keys (ie. ctrl, alt, ...) that are needed to activate the manipulation.</para>
		/// </summary>
		public EventModifiers modifiers;

		/// <summary>
		///   <para>Number of mouse clicks required to activate the manipulator.</para>
		/// </summary>
		public int clickCount;
	}
}
