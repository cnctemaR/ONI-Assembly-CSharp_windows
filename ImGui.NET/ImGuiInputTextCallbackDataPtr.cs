using System;
using System.Runtime.CompilerServices;
using System.Text;

namespace ImGuiNET
{
	public struct ImGuiInputTextCallbackDataPtr
	{
		public unsafe readonly ImGuiInputTextCallbackData* NativePtr { get; }

		public unsafe ImGuiInputTextCallbackDataPtr(ImGuiInputTextCallbackData* nativePtr)
		{
			this.NativePtr = nativePtr;
		}

		public unsafe ImGuiInputTextCallbackDataPtr(IntPtr nativePtr)
		{
			this.NativePtr = (ImGuiInputTextCallbackData*)(void*)nativePtr;
		}

		public unsafe static implicit operator ImGuiInputTextCallbackDataPtr(ImGuiInputTextCallbackData* nativePtr)
		{
			return new ImGuiInputTextCallbackDataPtr(nativePtr);
		}

		public unsafe static implicit operator ImGuiInputTextCallbackData*(ImGuiInputTextCallbackDataPtr wrappedPtr)
		{
			return wrappedPtr.NativePtr;
		}

		public static implicit operator ImGuiInputTextCallbackDataPtr(IntPtr nativePtr)
		{
			return new ImGuiInputTextCallbackDataPtr(nativePtr);
		}

		public unsafe ref ImGuiInputTextFlags EventFlag
		{
			get
			{
				return Unsafe.AsRef<ImGuiInputTextFlags>((void*)(&this.NativePtr->EventFlag));
			}
		}

		public unsafe ref ImGuiInputTextFlags Flags
		{
			get
			{
				return Unsafe.AsRef<ImGuiInputTextFlags>((void*)(&this.NativePtr->Flags));
			}
		}

		public unsafe IntPtr UserData
		{
			get
			{
				return (IntPtr)this.NativePtr->UserData;
			}
			set
			{
				this.NativePtr->UserData = (void*)value;
			}
		}

		public unsafe ref ushort EventChar
		{
			get
			{
				return Unsafe.AsRef<ushort>((void*)(&this.NativePtr->EventChar));
			}
		}

		public unsafe ref ImGuiKey EventKey
		{
			get
			{
				return Unsafe.AsRef<ImGuiKey>((void*)(&this.NativePtr->EventKey));
			}
		}

		public unsafe IntPtr Buf
		{
			get
			{
				return (IntPtr)((void*)this.NativePtr->Buf);
			}
			set
			{
				this.NativePtr->Buf = (byte*)(void*)value;
			}
		}

		public unsafe ref int BufTextLen
		{
			get
			{
				return Unsafe.AsRef<int>((void*)(&this.NativePtr->BufTextLen));
			}
		}

		public unsafe ref int BufSize
		{
			get
			{
				return Unsafe.AsRef<int>((void*)(&this.NativePtr->BufSize));
			}
		}

		public unsafe ref bool BufDirty
		{
			get
			{
				return Unsafe.AsRef<bool>((void*)(&this.NativePtr->BufDirty));
			}
		}

		public unsafe ref int CursorPos
		{
			get
			{
				return Unsafe.AsRef<int>((void*)(&this.NativePtr->CursorPos));
			}
		}

		public unsafe ref int SelectionStart
		{
			get
			{
				return Unsafe.AsRef<int>((void*)(&this.NativePtr->SelectionStart));
			}
		}

		public unsafe ref int SelectionEnd
		{
			get
			{
				return Unsafe.AsRef<int>((void*)(&this.NativePtr->SelectionEnd));
			}
		}

		public void ClearSelection()
		{
			ImGuiNative.ImGuiInputTextCallbackData_ClearSelection(this.NativePtr);
		}

		public void DeleteChars(int pos, int bytes_count)
		{
			ImGuiNative.ImGuiInputTextCallbackData_DeleteChars(this.NativePtr, pos, bytes_count);
		}

		public void Destroy()
		{
			ImGuiNative.ImGuiInputTextCallbackData_destroy(this.NativePtr);
		}

		public bool HasSelection()
		{
			return ImGuiNative.ImGuiInputTextCallbackData_HasSelection(this.NativePtr) > 0;
		}

		public unsafe void InsertChars(int pos, string text)
		{
			int num = 0;
			byte* ptr;
			if (text != null)
			{
				num = Encoding.UTF8.GetByteCount(text);
				if (num > 2048)
				{
					ptr = Util.Allocate(num + 1);
				}
				else
				{
					ptr = stackalloc byte[(UIntPtr)(num + 1)];
				}
				int utf = Util.GetUtf8(text, ptr, num);
				ptr[utf] = 0;
			}
			else
			{
				ptr = null;
			}
			byte* ptr2 = null;
			ImGuiNative.ImGuiInputTextCallbackData_InsertChars(this.NativePtr, pos, ptr, ptr2);
			if (num > 2048)
			{
				Util.Free(ptr);
			}
		}

		public void SelectAll()
		{
			ImGuiNative.ImGuiInputTextCallbackData_SelectAll(this.NativePtr);
		}
	}
}
