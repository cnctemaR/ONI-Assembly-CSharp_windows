using System;
using System.Runtime.CompilerServices;

namespace ImGuiNET
{
	public struct ImGuiOnceUponAFramePtr
	{
		public unsafe readonly ImGuiOnceUponAFrame* NativePtr { get; }

		public unsafe ImGuiOnceUponAFramePtr(ImGuiOnceUponAFrame* nativePtr)
		{
			this.NativePtr = nativePtr;
		}

		public unsafe ImGuiOnceUponAFramePtr(IntPtr nativePtr)
		{
			this.NativePtr = (ImGuiOnceUponAFrame*)(void*)nativePtr;
		}

		public unsafe static implicit operator ImGuiOnceUponAFramePtr(ImGuiOnceUponAFrame* nativePtr)
		{
			return new ImGuiOnceUponAFramePtr(nativePtr);
		}

		public unsafe static implicit operator ImGuiOnceUponAFrame*(ImGuiOnceUponAFramePtr wrappedPtr)
		{
			return wrappedPtr.NativePtr;
		}

		public static implicit operator ImGuiOnceUponAFramePtr(IntPtr nativePtr)
		{
			return new ImGuiOnceUponAFramePtr(nativePtr);
		}

		public unsafe ref int RefFrame
		{
			get
			{
				return Unsafe.AsRef<int>((void*)(&this.NativePtr->RefFrame));
			}
		}

		public void Destroy()
		{
			ImGuiNative.ImGuiOnceUponAFrame_destroy(this.NativePtr);
		}
	}
}
