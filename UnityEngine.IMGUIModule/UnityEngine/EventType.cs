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
		[EditorBrowsable(EditorBrowsableState.Never)]
		[Obsolete("Use MouseDrag instead (UnityUpgradable) -> MouseDrag", true)]
		mouseDrag,
		[Obsolete("Use KeyDown instead (UnityUpgradable) -> KeyDown", true)]
		[EditorBrowsable(EditorBrowsableState.Never)]
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
		[Obsolete("Use Layout instead (UnityUpgradable) -> Layout", true)]
		[EditorBrowsable(EditorBrowsableState.Never)]
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
		[EditorBrowsable(EditorBrowsableState.Never)]
		[Obsolete("Use Used instead (UnityUpgradable) -> Used", true)]
		used
	}
}
