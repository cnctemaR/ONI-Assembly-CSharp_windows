using System;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace ImGuiNET
{
	public struct ImGuiSizeCallbackDataPtr
	{
		public unsafe readonly ImGuiSizeCallbackData* NativePtr { get; }

		public unsafe ImGuiSizeCallbackDataPtr(ImGuiSizeCallbackData* nativePtr)
		{
			this.NativePtr = nativePtr;
		}

		public unsafe ImGuiSizeCallbackDataPtr(IntPtr nativePtr)
		{
			this.NativePtr = (ImGuiSizeCallbackData*)(void*)nativePtr;
		}

		public unsafe static implicit operator ImGuiSizeCallbackDataPtr(ImGuiSizeCallbackData* nativePtr)
		{
			return new ImGuiSizeCallbackDataPtr(nativePtr);
		}

		public unsafe static implicit operator ImGuiSizeCallbackData*(ImGuiSizeCallbackDataPtr wrappedPtr)
		{
			return wrappedPtr.NativePtr;
		}

		public static implicit operator ImGuiSizeCallbackDataPtr(IntPtr nativePtr)
		{
			return new ImGuiSizeCallbackDataPtr(nativePtr);
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

		public unsafe ref Vector2 Pos
		{
			get
			{
				return Unsafe.AsRef<Vector2>((void*)(&this.NativePtr->Pos));
			}
		}

		public unsafe ref Vector2 CurrentSize
		{
			get
			{
				return Unsafe.AsRef<Vector2>((void*)(&this.NativePtr->CurrentSize));
			}
		}

		public unsafe ref Vector2 DesiredSize
		{
			get
			{
				return Unsafe.AsRef<Vector2>((void*)(&this.NativePtr->DesiredSize));
			}
		}
	}
}
