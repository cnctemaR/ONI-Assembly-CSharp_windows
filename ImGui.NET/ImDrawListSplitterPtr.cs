using System;
using System.Runtime.CompilerServices;

namespace ImGuiNET
{
	public struct ImDrawListSplitterPtr
	{
		public unsafe readonly ImDrawListSplitter* NativePtr { get; }

		public unsafe ImDrawListSplitterPtr(ImDrawListSplitter* nativePtr)
		{
			this.NativePtr = nativePtr;
		}

		public unsafe ImDrawListSplitterPtr(IntPtr nativePtr)
		{
			this.NativePtr = (ImDrawListSplitter*)(void*)nativePtr;
		}

		public unsafe static implicit operator ImDrawListSplitterPtr(ImDrawListSplitter* nativePtr)
		{
			return new ImDrawListSplitterPtr(nativePtr);
		}

		public unsafe static implicit operator ImDrawListSplitter*(ImDrawListSplitterPtr wrappedPtr)
		{
			return wrappedPtr.NativePtr;
		}

		public static implicit operator ImDrawListSplitterPtr(IntPtr nativePtr)
		{
			return new ImDrawListSplitterPtr(nativePtr);
		}

		public unsafe ref int _Current
		{
			get
			{
				return Unsafe.AsRef<int>((void*)(&this.NativePtr->_Current));
			}
		}

		public unsafe ref int _Count
		{
			get
			{
				return Unsafe.AsRef<int>((void*)(&this.NativePtr->_Count));
			}
		}

		public unsafe ImPtrVector<ImDrawChannelPtr> _Channels
		{
			get
			{
				return new ImPtrVector<ImDrawChannelPtr>(this.NativePtr->_Channels, Unsafe.SizeOf<ImDrawChannel>());
			}
		}

		public void Clear()
		{
			ImGuiNative.ImDrawListSplitter_Clear(this.NativePtr);
		}

		public void ClearFreeMemory()
		{
			ImGuiNative.ImDrawListSplitter_ClearFreeMemory(this.NativePtr);
		}

		public void Destroy()
		{
			ImGuiNative.ImDrawListSplitter_destroy(this.NativePtr);
		}

		public unsafe void Merge(ImDrawListPtr draw_list)
		{
			ImDrawList* nativePtr = draw_list.NativePtr;
			ImGuiNative.ImDrawListSplitter_Merge(this.NativePtr, nativePtr);
		}

		public unsafe void SetCurrentChannel(ImDrawListPtr draw_list, int channel_idx)
		{
			ImDrawList* nativePtr = draw_list.NativePtr;
			ImGuiNative.ImDrawListSplitter_SetCurrentChannel(this.NativePtr, nativePtr, channel_idx);
		}

		public unsafe void Split(ImDrawListPtr draw_list, int count)
		{
			ImDrawList* nativePtr = draw_list.NativePtr;
			ImGuiNative.ImDrawListSplitter_Split(this.NativePtr, nativePtr, count);
		}
	}
}
