using System;
using System.Runtime.CompilerServices;

namespace ImGuiNET
{
	public struct ImGuiListClipperPtr
	{
		public unsafe readonly ImGuiListClipper* NativePtr { get; }

		public unsafe ImGuiListClipperPtr(ImGuiListClipper* nativePtr)
		{
			this.NativePtr = nativePtr;
		}

		public unsafe ImGuiListClipperPtr(IntPtr nativePtr)
		{
			this.NativePtr = (ImGuiListClipper*)(void*)nativePtr;
		}

		public unsafe static implicit operator ImGuiListClipperPtr(ImGuiListClipper* nativePtr)
		{
			return new ImGuiListClipperPtr(nativePtr);
		}

		public unsafe static implicit operator ImGuiListClipper*(ImGuiListClipperPtr wrappedPtr)
		{
			return wrappedPtr.NativePtr;
		}

		public static implicit operator ImGuiListClipperPtr(IntPtr nativePtr)
		{
			return new ImGuiListClipperPtr(nativePtr);
		}

		public unsafe ref int DisplayStart
		{
			get
			{
				return Unsafe.AsRef<int>((void*)(&this.NativePtr->DisplayStart));
			}
		}

		public unsafe ref int DisplayEnd
		{
			get
			{
				return Unsafe.AsRef<int>((void*)(&this.NativePtr->DisplayEnd));
			}
		}

		public unsafe ref int ItemsCount
		{
			get
			{
				return Unsafe.AsRef<int>((void*)(&this.NativePtr->ItemsCount));
			}
		}

		public unsafe ref int StepNo
		{
			get
			{
				return Unsafe.AsRef<int>((void*)(&this.NativePtr->StepNo));
			}
		}

		public unsafe ref int ItemsFrozen
		{
			get
			{
				return Unsafe.AsRef<int>((void*)(&this.NativePtr->ItemsFrozen));
			}
		}

		public unsafe ref float ItemsHeight
		{
			get
			{
				return Unsafe.AsRef<float>((void*)(&this.NativePtr->ItemsHeight));
			}
		}

		public unsafe ref float StartPosY
		{
			get
			{
				return Unsafe.AsRef<float>((void*)(&this.NativePtr->StartPosY));
			}
		}

		public void Begin(int items_count)
		{
			float num = -1f;
			ImGuiNative.ImGuiListClipper_Begin(this.NativePtr, items_count, num);
		}

		public void Begin(int items_count, float items_height)
		{
			ImGuiNative.ImGuiListClipper_Begin(this.NativePtr, items_count, items_height);
		}

		public void Destroy()
		{
			ImGuiNative.ImGuiListClipper_destroy(this.NativePtr);
		}

		public void End()
		{
			ImGuiNative.ImGuiListClipper_End(this.NativePtr);
		}

		public bool Step()
		{
			return ImGuiNative.ImGuiListClipper_Step(this.NativePtr) > 0;
		}
	}
}
