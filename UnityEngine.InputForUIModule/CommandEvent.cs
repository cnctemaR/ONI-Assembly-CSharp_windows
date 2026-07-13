using System;
using Unity.IntegerTime;
using UnityEngine.Bindings;

namespace UnityEngine.InputForUI
{
	[VisibleToOtherModules(new string[] { "UnityEngine.UIElementsModule" })]
	internal struct CommandEvent : IEventProperties
	{
		public DiscreteTime timestamp { readonly get; set; }

		public EventSource eventSource { readonly get; set; }

		public uint playerId { readonly get; set; }

		public EventModifiers eventModifiers { readonly get; set; }

		public override string ToString()
		{
			return string.Format("{0} {1}", this.type, this.command);
		}

		public CommandEvent.Type type;

		public CommandEvent.Command command;

		public enum Type
		{
			Validate = 1,
			Execute
		}

		public enum Command
		{
			Invalid,
			Cut,
			Copy,
			Paste,
			SelectAll,
			DeselectAll,
			InvertSelection,
			Duplicate,
			Rename,
			Delete,
			SoftDelete,
			Find,
			SelectChildren,
			SelectPrefabRoot,
			UndoRedoPerformed,
			OnLostFocus,
			NewKeyboardFocus,
			ModifierKeysChanged,
			EyeDropperUpdate,
			EyeDropperClicked,
			EyeDropperCancelled,
			ColorPickerChanged,
			FrameSelected,
			FrameSelectedWithLock
		}
	}
}
