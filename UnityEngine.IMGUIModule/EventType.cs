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
		TouchDown = 30,
		TouchUp,
		TouchMove,
		TouchEnter,
		TouchLeave,
		TouchStationary,
		[Obsolete("Use MouseDown instead (UnityUpgradable) -> MouseDown", true)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		mouseDown = 0,
		[Obsolete("Use MouseUp instead (UnityUpgradable) -> MouseUp", true)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		mouseUp,
		[Obsolete("Use MouseMove instead (UnityUpgradable) -> MouseMove", true)]
		[EditorBrowsable(EditorBrowsableState.Never)]
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
		[EditorBrowsable(EditorBrowsableState.Never)]
		[Obsolete("Use Repaint instead (UnityUpgradable) -> Repaint", true)]
		repaint,
		[EditorBrowsable(EditorBrowsableState.Never)]
		[Obsolete("Use Layout instead (UnityUpgradable) -> Layout", true)]
		layout,
		[EditorBrowsable(EditorBrowsableState.Never)]
		[Obsolete("Use DragUpdated instead (UnityUpgradable) -> DragUpdated", true)]
		dragUpdated,
		[Obsolete("Use DragPerform instead (UnityUpgradable) -> DragPerform", true)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		dragPerform,
		[Obsolete("Use Ignore instead (UnityUpgradable) -> Ignore", true)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		ignore,
		[Obsolete("Use Used instead (UnityUpgradable) -> Used", true)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		used
	}
}
