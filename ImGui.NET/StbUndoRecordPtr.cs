using System;
using System.Runtime.CompilerServices;

namespace ImGuiNET
{
	public struct StbUndoRecordPtr
	{
		public unsafe readonly StbUndoRecord* NativePtr { get; }

		public unsafe StbUndoRecordPtr(StbUndoRecord* nativePtr)
		{
			this.NativePtr = nativePtr;
		}

		public unsafe StbUndoRecordPtr(IntPtr nativePtr)
		{
			this.NativePtr = (StbUndoRecord*)(void*)nativePtr;
		}

		public unsafe static implicit operator StbUndoRecordPtr(StbUndoRecord* nativePtr)
		{
			return new StbUndoRecordPtr(nativePtr);
		}

		public unsafe static implicit operator StbUndoRecord*(StbUndoRecordPtr wrappedPtr)
		{
			return wrappedPtr.NativePtr;
		}

		public static implicit operator StbUndoRecordPtr(IntPtr nativePtr)
		{
			return new StbUndoRecordPtr(nativePtr);
		}

		public unsafe ref int where
		{
			get
			{
				return Unsafe.AsRef<int>((void*)(&this.NativePtr->where));
			}
		}

		public unsafe ref int insert_length
		{
			get
			{
				return Unsafe.AsRef<int>((void*)(&this.NativePtr->insert_length));
			}
		}

		public unsafe ref int delete_length
		{
			get
			{
				return Unsafe.AsRef<int>((void*)(&this.NativePtr->delete_length));
			}
		}

		public unsafe ref int char_storage
		{
			get
			{
				return Unsafe.AsRef<int>((void*)(&this.NativePtr->char_storage));
			}
		}
	}
}
