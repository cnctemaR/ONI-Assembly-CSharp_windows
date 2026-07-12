using System;
using System.Runtime.CompilerServices;

namespace ImGuiNET
{
	public struct STB_TexteditStatePtr
	{
		public unsafe readonly STB_TexteditState* NativePtr { get; }

		public unsafe STB_TexteditStatePtr(STB_TexteditState* nativePtr)
		{
			this.NativePtr = nativePtr;
		}

		public unsafe STB_TexteditStatePtr(IntPtr nativePtr)
		{
			this.NativePtr = (STB_TexteditState*)(void*)nativePtr;
		}

		public unsafe static implicit operator STB_TexteditStatePtr(STB_TexteditState* nativePtr)
		{
			return new STB_TexteditStatePtr(nativePtr);
		}

		public unsafe static implicit operator STB_TexteditState*(STB_TexteditStatePtr wrappedPtr)
		{
			return wrappedPtr.NativePtr;
		}

		public static implicit operator STB_TexteditStatePtr(IntPtr nativePtr)
		{
			return new STB_TexteditStatePtr(nativePtr);
		}

		public unsafe ref int cursor
		{
			get
			{
				return Unsafe.AsRef<int>((void*)(&this.NativePtr->cursor));
			}
		}

		public unsafe ref int select_start
		{
			get
			{
				return Unsafe.AsRef<int>((void*)(&this.NativePtr->select_start));
			}
		}

		public unsafe ref int select_end
		{
			get
			{
				return Unsafe.AsRef<int>((void*)(&this.NativePtr->select_end));
			}
		}

		public unsafe ref byte insert_mode
		{
			get
			{
				return Unsafe.AsRef<byte>((void*)(&this.NativePtr->insert_mode));
			}
		}

		public unsafe ref int row_count_per_page
		{
			get
			{
				return Unsafe.AsRef<int>((void*)(&this.NativePtr->row_count_per_page));
			}
		}

		public unsafe ref byte cursor_at_end_of_line
		{
			get
			{
				return Unsafe.AsRef<byte>((void*)(&this.NativePtr->cursor_at_end_of_line));
			}
		}

		public unsafe ref byte initialized
		{
			get
			{
				return Unsafe.AsRef<byte>((void*)(&this.NativePtr->initialized));
			}
		}

		public unsafe ref byte has_preferred_x
		{
			get
			{
				return Unsafe.AsRef<byte>((void*)(&this.NativePtr->has_preferred_x));
			}
		}

		public unsafe ref byte single_line
		{
			get
			{
				return Unsafe.AsRef<byte>((void*)(&this.NativePtr->single_line));
			}
		}

		public unsafe ref byte padding1
		{
			get
			{
				return Unsafe.AsRef<byte>((void*)(&this.NativePtr->padding1));
			}
		}

		public unsafe ref byte padding2
		{
			get
			{
				return Unsafe.AsRef<byte>((void*)(&this.NativePtr->padding2));
			}
		}

		public unsafe ref byte padding3
		{
			get
			{
				return Unsafe.AsRef<byte>((void*)(&this.NativePtr->padding3));
			}
		}

		public unsafe ref float preferred_x
		{
			get
			{
				return Unsafe.AsRef<float>((void*)(&this.NativePtr->preferred_x));
			}
		}

		public unsafe ref StbUndoState undostate
		{
			get
			{
				return Unsafe.AsRef<StbUndoState>((void*)(&this.NativePtr->undostate));
			}
		}
	}
}
