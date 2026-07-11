using System;

namespace UnityEngine.Experimental.UIElements
{
	public interface IKeyboardEvent
	{
		/// <summary>
		///   <para>Flag set holding the pressed modifier keys (Alt, Control, Shift, Windows/Command).</para>
		/// </summary>
		EventModifiers modifiers { get; }

		/// <summary>
		///   <para>The character.</para>
		/// </summary>
		char character { get; }

		/// <summary>
		///   <para>The key code.</para>
		/// </summary>
		KeyCode keyCode { get; }

		/// <summary>
		///   <para>Return true if the Shift key is pressed.</para>
		/// </summary>
		bool shiftKey { get; }

		/// <summary>
		///   <para>Return true if the Control key is pressed.</para>
		/// </summary>
		bool ctrlKey { get; }

		/// <summary>
		///   <para>Return true if the Windows/Command key is pressed.</para>
		/// </summary>
		bool commandKey { get; }

		/// <summary>
		///   <para>Return true if the Alt key is pressed.</para>
		/// </summary>
		bool altKey { get; }
	}
}
