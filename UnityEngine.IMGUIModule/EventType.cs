using System;
using System.ComponentModel;

namespace UnityEngine
{
	public enum EventType
	{
		MouseDown,
		MouseUp,
		MouseMove,
		MouseDrag,
		KeyDown,
		KeyUp,
		ScrollWheel,
		Repaint,
		Layout,
		DragUpdated,
		DragPerform,
		DragExited = 15,
		Ignore = 11,
		Used,
		ValidateCommand,
		ExecuteCommand,
		ContextClick = 16,
		MouseEnterWindow = 20,
		MouseLeaveWindow,
		[EditorBrowsable(EditorBrowsableState.Never)]
		[Obsolete("Use MouseDown instead (UnityUpgradable) -> MouseDown", true)]
		mouseDown = 0,
		[Obsolete("Use MouseUp instead (UnityUpgradable) -> MouseUp", true)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		mouseUp,
		[EditorBrowsable(EditorBrowsableState.Never)]
		[Obsolete("Use MouseMove instead (UnityUpgradable) -> MouseMove", true)]
		mouseMove,
		[Obsolete("Use MouseDrag instead (UnityUpgradable) -> MouseDrag", true)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		mouseDrag,
		[EditorBrowsable(EditorBrowsableState.Never)]
		[Obsolete("Use KeyDown instead (UnityUpgradable) -> KeyDown", true)]
		keyDown,
		[EditorBrowsable(EditorBrowsableState.Never)]
		[Obsolete("Use KeyUp instead (UnityUpgradable) -> KeyUp", true)]
		keyUp,
		[EditorBrowsable(EditorBrowsableState.Never)]
		[Obsolete("Use ScrollWheel instead (UnityUpgradable) -> ScrollWheel", true)]
		scrollWheel,
		[Obsolete("Use Repaint instead (UnityUpgradable) -> Repaint", true)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		repaint,
		[EditorBrowsable(EditorBrowsableState.Never)]
		[Obsolete("Use Layout instead (UnityUpgradable) -> Layout", true)]
		layout,
		[Obsolete("Use DragUpdated instead (UnityUpgradable) -> DragUpdated", true)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		dragUpdated,
		[EditorBrowsable(EditorBrowsableState.Never)]
		[Obsolete("Use DragPerform instead (UnityUpgradable) -> DragPerform", true)]
		dragPerform,
		[EditorBrowsable(EditorBrowsableState.Never)]
		[Obsolete("Use Ignore instead (UnityUpgradable) -> Ignore", true)]
		ignore,
		[Obsolete("Use Used instead (UnityUpgradable) -> Used", true)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		used
	}
}
