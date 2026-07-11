using System;

namespace UnityEngine
{
	/// <summary>
	///   <para>Types of UnityGUI input and processing events.</para>
	/// </summary>
	public enum EventType
	{
		/// <summary>
		///   <para>Mouse button was pressed.</para>
		/// </summary>
		MouseDown,
		/// <summary>
		///   <para>Mouse button was released.</para>
		/// </summary>
		MouseUp,
		/// <summary>
		///   <para>Mouse was moved (Editor views only).</para>
		/// </summary>
		MouseMove,
		/// <summary>
		///   <para>Mouse was dragged.</para>
		/// </summary>
		MouseDrag,
		/// <summary>
		///   <para>A keyboard key was pressed.</para>
		/// </summary>
		KeyDown,
		/// <summary>
		///   <para>A keyboard key was released.</para>
		/// </summary>
		KeyUp,
		/// <summary>
		///   <para>The scroll wheel was moved.</para>
		/// </summary>
		ScrollWheel,
		/// <summary>
		///   <para>A repaint event. One is sent every frame.</para>
		/// </summary>
		Repaint,
		/// <summary>
		///   <para>A layout event.</para>
		/// </summary>
		Layout,
		/// <summary>
		///   <para>Editor only: drag &amp; drop operation updated.</para>
		/// </summary>
		DragUpdated,
		/// <summary>
		///   <para>Editor only: drag &amp; drop operation performed.</para>
		/// </summary>
		DragPerform,
		/// <summary>
		///   <para>Editor only: drag &amp; drop operation exited.</para>
		/// </summary>
		DragExited = 15,
		/// <summary>
		///   <para>Event should be ignored.</para>
		/// </summary>
		Ignore = 11,
		/// <summary>
		///   <para>Already processed event.</para>
		/// </summary>
		Used,
		/// <summary>
		///   <para>Validates a special command (e.g. copy &amp; paste).</para>
		/// </summary>
		ValidateCommand,
		/// <summary>
		///   <para>Execute a special command (eg. copy &amp; paste).</para>
		/// </summary>
		ExecuteCommand,
		/// <summary>
		///   <para>User has right-clicked (or control-clicked on the mac).</para>
		/// </summary>
		ContextClick = 16,
		/// <summary>
		///   <para>Mouse entered a window (Editor views only).</para>
		/// </summary>
		MouseEnterWindow = 20,
		/// <summary>
		///   <para>Mouse left a window (Editor views only).</para>
		/// </summary>
		MouseLeaveWindow,
		/// <summary>
		///   <para>An event that is called when the mouse is clicked.</para>
		/// </summary>
		[Obsolete("Use MouseDown instead (UnityUpgradable) -> MouseDown", true)]
		mouseDown = 0,
		/// <summary>
		///   <para>An event that is called when the mouse is no longer being clicked.</para>
		/// </summary>
		[Obsolete("Use MouseUp instead (UnityUpgradable) -> MouseUp", true)]
		mouseUp,
		[Obsolete("Use MouseMove instead (UnityUpgradable) -> MouseMove", true)]
		mouseMove,
		/// <summary>
		///   <para>An event that is called when the mouse is clicked and dragged.</para>
		/// </summary>
		[Obsolete("Use MouseDrag instead (UnityUpgradable) -> MouseDrag", true)]
		mouseDrag,
		[Obsolete("Use KeyDown instead (UnityUpgradable) -> KeyDown", true)]
		keyDown,
		[Obsolete("Use KeyUp instead (UnityUpgradable) -> KeyUp", true)]
		keyUp,
		[Obsolete("Use ScrollWheel instead (UnityUpgradable) -> ScrollWheel", true)]
		scrollWheel,
		[Obsolete("Use Repaint instead (UnityUpgradable) -> Repaint", true)]
		repaint,
		[Obsolete("Use Layout instead (UnityUpgradable) -> Layout", true)]
		layout,
		[Obsolete("Use DragUpdated instead (UnityUpgradable) -> DragUpdated", true)]
		dragUpdated,
		[Obsolete("Use DragPerform instead (UnityUpgradable) -> DragPerform", true)]
		dragPerform,
		[Obsolete("Use Ignore instead (UnityUpgradable) -> Ignore", true)]
		ignore,
		[Obsolete("Use Used instead (UnityUpgradable) -> Used", true)]
		used
	}
}
