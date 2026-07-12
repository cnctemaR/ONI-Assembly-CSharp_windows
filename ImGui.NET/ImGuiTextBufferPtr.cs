using System;
using System.Text;

namespace ImGuiNET
{
	public struct ImGuiTextBufferPtr
	{
		public unsafe readonly ImGuiTextBuffer* NativePtr { get; }

		public unsafe ImGuiTextBufferPtr(ImGuiTextBuffer* nativePtr)
		{
			this.NativePtr = nativePtr;
		}

		public unsafe ImGuiTextBufferPtr(IntPtr nativePtr)
		{
			this.NativePtr = (ImGuiTextBuffer*)(void*)nativePtr;
		}

		public unsafe static implicit operator ImGuiTextBufferPtr(ImGuiTextBuffer* nativePtr)
		{
			return new ImGuiTextBufferPtr(nativePtr);
		}

		public unsafe static implicit operator ImGuiTextBuffer*(ImGuiTextBufferPtr wrappedPtr)
		{
			return wrappedPtr.NativePtr;
		}

		public static implicit operator ImGuiTextBufferPtr(IntPtr nativePtr)
		{
			return new ImGuiTextBufferPtr(nativePtr);
		}

		public unsafe ImVector<byte> Buf
		{
			get
			{
				return new ImVector<byte>(this.NativePtr->Buf);
			}
		}

		public unsafe void append(string str)
		{
			int num = 0;
			byte* ptr;
			if (str != null)
			{
				num = Encoding.UTF8.GetByteCount(str);
				if (num > 2048)
				{
					ptr = Util.Allocate(num + 1);
				}
				else
				{
					ptr = stackalloc byte[(UIntPtr)(num + 1)];
				}
				int utf = Util.GetUtf8(str, ptr, num);
				ptr[utf] = 0;
			}
			else
			{
				ptr = null;
			}
			byte* ptr2 = null;
			ImGuiNative.ImGuiTextBuffer_append(this.NativePtr, ptr, ptr2);
			if (num > 2048)
			{
				Util.Free(ptr);
			}
		}

		public unsafe void appendf(string fmt)
		{
			int num = 0;
			byte* ptr;
			if (fmt != null)
			{
				num = Encoding.UTF8.GetByteCount(fmt);
				if (num > 2048)
				{
					ptr = Util.Allocate(num + 1);
				}
				else
				{
					ptr = stackalloc byte[(UIntPtr)(num + 1)];
				}
				int utf = Util.GetUtf8(fmt, ptr, num);
				ptr[utf] = 0;
			}
			else
			{
				ptr = null;
			}
			ImGuiNative.ImGuiTextBuffer_appendf(this.NativePtr, ptr);
			if (num > 2048)
			{
				Util.Free(ptr);
			}
		}

		public string begin()
		{
			return Util.StringFromPtr(ImGuiNative.ImGuiTextBuffer_begin(this.NativePtr));
		}

		public string c_str()
		{
			return Util.StringFromPtr(ImGuiNative.ImGuiTextBuffer_c_str(this.NativePtr));
		}

		public void clear()
		{
			ImGuiNative.ImGuiTextBuffer_clear(this.NativePtr);
		}

		public void Destroy()
		{
			ImGuiNative.ImGuiTextBuffer_destroy(this.NativePtr);
		}

		public bool empty()
		{
			return ImGuiNative.ImGuiTextBuffer_empty(this.NativePtr) > 0;
		}

		public string end()
		{
			return Util.StringFromPtr(ImGuiNative.ImGuiTextBuffer_end(this.NativePtr));
		}

		public void reserve(int capacity)
		{
			ImGuiNative.ImGuiTextBuffer_reserve(this.NativePtr, capacity);
		}

		public int size()
		{
			return ImGuiNative.ImGuiTextBuffer_size(this.NativePtr);
		}
	}
}
