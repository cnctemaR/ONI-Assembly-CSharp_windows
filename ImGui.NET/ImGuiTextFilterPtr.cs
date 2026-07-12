using System;
using System.Runtime.CompilerServices;
using System.Text;

namespace ImGuiNET
{
	public struct ImGuiTextFilterPtr
	{
		public unsafe readonly ImGuiTextFilter* NativePtr { get; }

		public unsafe ImGuiTextFilterPtr(ImGuiTextFilter* nativePtr)
		{
			this.NativePtr = nativePtr;
		}

		public unsafe ImGuiTextFilterPtr(IntPtr nativePtr)
		{
			this.NativePtr = (ImGuiTextFilter*)(void*)nativePtr;
		}

		public unsafe static implicit operator ImGuiTextFilterPtr(ImGuiTextFilter* nativePtr)
		{
			return new ImGuiTextFilterPtr(nativePtr);
		}

		public unsafe static implicit operator ImGuiTextFilter*(ImGuiTextFilterPtr wrappedPtr)
		{
			return wrappedPtr.NativePtr;
		}

		public static implicit operator ImGuiTextFilterPtr(IntPtr nativePtr)
		{
			return new ImGuiTextFilterPtr(nativePtr);
		}

		public unsafe RangeAccessor<byte> InputBuf
		{
			get
			{
				return new RangeAccessor<byte>((void*)(&this.NativePtr->InputBuf.FixedElementField), 256);
			}
		}

		public unsafe ImPtrVector<ImGuiTextRangePtr> Filters
		{
			get
			{
				return new ImPtrVector<ImGuiTextRangePtr>(this.NativePtr->Filters, Unsafe.SizeOf<ImGuiTextRange>());
			}
		}

		public unsafe ref int CountGrep
		{
			get
			{
				return Unsafe.AsRef<int>((void*)(&this.NativePtr->CountGrep));
			}
		}

		public void Build()
		{
			ImGuiNative.ImGuiTextFilter_Build(this.NativePtr);
		}

		public void Clear()
		{
			ImGuiNative.ImGuiTextFilter_Clear(this.NativePtr);
		}

		public void Destroy()
		{
			ImGuiNative.ImGuiTextFilter_destroy(this.NativePtr);
		}

		public unsafe bool Draw()
		{
			int byteCount = Encoding.UTF8.GetByteCount("Filter(inc,-exc)");
			byte* ptr;
			if (byteCount > 2048)
			{
				ptr = Util.Allocate(byteCount + 1);
			}
			else
			{
				ptr = stackalloc byte[(UIntPtr)(byteCount + 1)];
			}
			int utf = Util.GetUtf8("Filter(inc,-exc)", ptr, byteCount);
			ptr[utf] = 0;
			float num = 0f;
			int num2 = (int)ImGuiNative.ImGuiTextFilter_Draw(this.NativePtr, ptr, num);
			if (byteCount > 2048)
			{
				Util.Free(ptr);
			}
			return num2 != 0;
		}

		public unsafe bool Draw(string label)
		{
			int num = 0;
			byte* ptr;
			if (label != null)
			{
				num = Encoding.UTF8.GetByteCount(label);
				if (num > 2048)
				{
					ptr = Util.Allocate(num + 1);
				}
				else
				{
					ptr = stackalloc byte[(UIntPtr)(num + 1)];
				}
				int utf = Util.GetUtf8(label, ptr, num);
				ptr[utf] = 0;
			}
			else
			{
				ptr = null;
			}
			float num2 = 0f;
			int num3 = (int)ImGuiNative.ImGuiTextFilter_Draw(this.NativePtr, ptr, num2);
			if (num > 2048)
			{
				Util.Free(ptr);
			}
			return num3 != 0;
		}

		public unsafe bool Draw(string label, float width)
		{
			int num = 0;
			byte* ptr;
			if (label != null)
			{
				num = Encoding.UTF8.GetByteCount(label);
				if (num > 2048)
				{
					ptr = Util.Allocate(num + 1);
				}
				else
				{
					ptr = stackalloc byte[(UIntPtr)(num + 1)];
				}
				int utf = Util.GetUtf8(label, ptr, num);
				ptr[utf] = 0;
			}
			else
			{
				ptr = null;
			}
			int num2 = (int)ImGuiNative.ImGuiTextFilter_Draw(this.NativePtr, ptr, width);
			if (num > 2048)
			{
				Util.Free(ptr);
			}
			return num2 != 0;
		}

		public bool IsActive()
		{
			return ImGuiNative.ImGuiTextFilter_IsActive(this.NativePtr) > 0;
		}

		public unsafe bool PassFilter(string text)
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
			int num2 = (int)ImGuiNative.ImGuiTextFilter_PassFilter(this.NativePtr, ptr, ptr2);
			if (num > 2048)
			{
				Util.Free(ptr);
			}
			return num2 != 0;
		}
	}
}
