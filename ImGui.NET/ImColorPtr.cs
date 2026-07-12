using System;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace ImGuiNET
{
	public struct ImColorPtr
	{
		public unsafe readonly ImColor* NativePtr { get; }

		public unsafe ImColorPtr(ImColor* nativePtr)
		{
			this.NativePtr = nativePtr;
		}

		public unsafe ImColorPtr(IntPtr nativePtr)
		{
			this.NativePtr = (ImColor*)(void*)nativePtr;
		}

		public unsafe static implicit operator ImColorPtr(ImColor* nativePtr)
		{
			return new ImColorPtr(nativePtr);
		}

		public unsafe static implicit operator ImColor*(ImColorPtr wrappedPtr)
		{
			return wrappedPtr.NativePtr;
		}

		public static implicit operator ImColorPtr(IntPtr nativePtr)
		{
			return new ImColorPtr(nativePtr);
		}

		public unsafe ref Vector4 Value
		{
			get
			{
				return Unsafe.AsRef<Vector4>((void*)(&this.NativePtr->Value));
			}
		}

		public void Destroy()
		{
			ImGuiNative.ImColor_destroy(this.NativePtr);
		}

		public unsafe ImColor HSV(float h, float s, float v)
		{
			float num = 1f;
			ImColor imColor;
			ImGuiNative.ImColor_HSV(&imColor, h, s, v, num);
			return imColor;
		}

		public unsafe ImColor HSV(float h, float s, float v, float a)
		{
			ImColor imColor;
			ImGuiNative.ImColor_HSV(&imColor, h, s, v, a);
			return imColor;
		}

		public void SetHSV(float h, float s, float v)
		{
			float num = 1f;
			ImGuiNative.ImColor_SetHSV(this.NativePtr, h, s, v, num);
		}

		public void SetHSV(float h, float s, float v, float a)
		{
			ImGuiNative.ImColor_SetHSV(this.NativePtr, h, s, v, a);
		}
	}
}
