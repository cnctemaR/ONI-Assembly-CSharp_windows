using System;

namespace ImGuiNET
{
	public struct ImGuiTextRangePtr
	{
		public unsafe readonly ImGuiTextRange* NativePtr { get; }

		public unsafe ImGuiTextRangePtr(ImGuiTextRange* nativePtr)
		{
			this.NativePtr = nativePtr;
		}

		public unsafe ImGuiTextRangePtr(IntPtr nativePtr)
		{
			this.NativePtr = (ImGuiTextRange*)(void*)nativePtr;
		}

		public unsafe static implicit operator ImGuiTextRangePtr(ImGuiTextRange* nativePtr)
		{
			return new ImGuiTextRangePtr(nativePtr);
		}

		public unsafe static implicit operator ImGuiTextRange*(ImGuiTextRangePtr wrappedPtr)
		{
			return wrappedPtr.NativePtr;
		}

		public static implicit operator ImGuiTextRangePtr(IntPtr nativePtr)
		{
			return new ImGuiTextRangePtr(nativePtr);
		}

		public unsafe IntPtr b
		{
			get
			{
				return (IntPtr)((void*)this.NativePtr->b);
			}
			set
			{
				this.NativePtr->b = (byte*)(void*)value;
			}
		}

		public unsafe IntPtr e
		{
			get
			{
				return (IntPtr)((void*)this.NativePtr->e);
			}
			set
			{
				this.NativePtr->e = (byte*)(void*)value;
			}
		}

		public void Destroy()
		{
			ImGuiNative.ImGuiTextRange_destroy(this.NativePtr);
		}

		public bool empty()
		{
			return ImGuiNative.ImGuiTextRange_empty(this.NativePtr) > 0;
		}

		public unsafe void split(byte separator, out ImVector @out)
		{
			fixed (ImVector* ptr = &@out)
			{
				ImVector* ptr2 = ptr;
				ImGuiNative.ImGuiTextRange_split(this.NativePtr, separator, ptr2);
			}
		}
	}
}
