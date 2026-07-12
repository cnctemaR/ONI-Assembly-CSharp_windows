using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace ImGuiNET
{
	public struct ImDrawDataPtr
	{
		public unsafe readonly ImDrawData* NativePtr { get; }

		public unsafe ImDrawDataPtr(ImDrawData* nativePtr)
		{
			this.NativePtr = nativePtr;
		}

		public unsafe ImDrawDataPtr(IntPtr nativePtr)
		{
			this.NativePtr = (ImDrawData*)(void*)nativePtr;
		}

		public unsafe static implicit operator ImDrawDataPtr(ImDrawData* nativePtr)
		{
			return new ImDrawDataPtr(nativePtr);
		}

		public unsafe static implicit operator ImDrawData*(ImDrawDataPtr wrappedPtr)
		{
			return wrappedPtr.NativePtr;
		}

		public static implicit operator ImDrawDataPtr(IntPtr nativePtr)
		{
			return new ImDrawDataPtr(nativePtr);
		}

		public unsafe ref bool Valid
		{
			get
			{
				return Unsafe.AsRef<bool>((void*)(&this.NativePtr->Valid));
			}
		}

		public unsafe ref int CmdListsCount
		{
			get
			{
				return Unsafe.AsRef<int>((void*)(&this.NativePtr->CmdListsCount));
			}
		}

		public unsafe ref int TotalIdxCount
		{
			get
			{
				return Unsafe.AsRef<int>((void*)(&this.NativePtr->TotalIdxCount));
			}
		}

		public unsafe ref int TotalVtxCount
		{
			get
			{
				return Unsafe.AsRef<int>((void*)(&this.NativePtr->TotalVtxCount));
			}
		}

		public unsafe IntPtr CmdLists
		{
			get
			{
				return (IntPtr)((void*)this.NativePtr->CmdLists);
			}
			set
			{
				this.NativePtr->CmdLists = (ImDrawList**)(void*)value;
			}
		}

		public unsafe ref Vector2 DisplayPos
		{
			get
			{
				return Unsafe.AsRef<Vector2>((void*)(&this.NativePtr->DisplayPos));
			}
		}

		public unsafe ref Vector2 DisplaySize
		{
			get
			{
				return Unsafe.AsRef<Vector2>((void*)(&this.NativePtr->DisplaySize));
			}
		}

		public unsafe ref Vector2 FramebufferScale
		{
			get
			{
				return Unsafe.AsRef<Vector2>((void*)(&this.NativePtr->FramebufferScale));
			}
		}

		public unsafe ImGuiViewportPtr OwnerViewport
		{
			get
			{
				return new ImGuiViewportPtr(this.NativePtr->OwnerViewport);
			}
		}

		public void Clear()
		{
			ImGuiNative.ImDrawData_Clear(this.NativePtr);
		}

		public void DeIndexAllBuffers()
		{
			ImGuiNative.ImDrawData_DeIndexAllBuffers(this.NativePtr);
		}

		public void Destroy()
		{
			ImGuiNative.ImDrawData_destroy(this.NativePtr);
		}

		public void ScaleClipRects(Vector2 fb_scale)
		{
			ImGuiNative.ImDrawData_ScaleClipRects(this.NativePtr, fb_scale);
		}

		public unsafe RangePtrAccessor<ImDrawListPtr> CmdListsRange
		{
			get
			{
				return new RangePtrAccessor<ImDrawListPtr>(this.CmdLists.ToPointer(), *this.CmdListsCount);
			}
		}
	}
}
