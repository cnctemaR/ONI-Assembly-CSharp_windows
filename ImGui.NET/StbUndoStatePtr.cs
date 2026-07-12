using System;
using System.Runtime.CompilerServices;

namespace ImGuiNET
{
	public struct StbUndoStatePtr
	{
		public unsafe readonly StbUndoState* NativePtr { get; }

		public unsafe StbUndoStatePtr(StbUndoState* nativePtr)
		{
			this.NativePtr = nativePtr;
		}

		public unsafe StbUndoStatePtr(IntPtr nativePtr)
		{
			this.NativePtr = (StbUndoState*)(void*)nativePtr;
		}

		public unsafe static implicit operator StbUndoStatePtr(StbUndoState* nativePtr)
		{
			return new StbUndoStatePtr(nativePtr);
		}

		public unsafe static implicit operator StbUndoState*(StbUndoStatePtr wrappedPtr)
		{
			return wrappedPtr.NativePtr;
		}

		public static implicit operator StbUndoStatePtr(IntPtr nativePtr)
		{
			return new StbUndoStatePtr(nativePtr);
		}

		public unsafe RangeAccessor<StbUndoRecord> undo_rec
		{
			get
			{
				return new RangeAccessor<StbUndoRecord>((void*)(&this.NativePtr->undo_rec_0), 99);
			}
		}

		public unsafe RangeAccessor<ushort> undo_char
		{
			get
			{
				return new RangeAccessor<ushort>((void*)(&this.NativePtr->undo_char.FixedElementField), 999);
			}
		}

		public unsafe ref short undo_point
		{
			get
			{
				return Unsafe.AsRef<short>((void*)(&this.NativePtr->undo_point));
			}
		}

		public unsafe ref short redo_point
		{
			get
			{
				return Unsafe.AsRef<short>((void*)(&this.NativePtr->redo_point));
			}
		}

		public unsafe ref int undo_char_point
		{
			get
			{
				return Unsafe.AsRef<int>((void*)(&this.NativePtr->undo_char_point));
			}
		}

		public unsafe ref int redo_char_point
		{
			get
			{
				return Unsafe.AsRef<int>((void*)(&this.NativePtr->redo_char_point));
			}
		}
	}
}
