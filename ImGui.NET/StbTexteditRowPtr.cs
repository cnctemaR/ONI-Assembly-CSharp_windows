using System;
using System.Runtime.CompilerServices;

namespace ImGuiNET
{
	public struct StbTexteditRowPtr
	{
		public unsafe readonly StbTexteditRow* NativePtr { get; }

		public unsafe StbTexteditRowPtr(StbTexteditRow* nativePtr)
		{
			this.NativePtr = nativePtr;
		}

		public unsafe StbTexteditRowPtr(IntPtr nativePtr)
		{
			this.NativePtr = (StbTexteditRow*)(void*)nativePtr;
		}

		public unsafe static implicit operator StbTexteditRowPtr(StbTexteditRow* nativePtr)
		{
			return new StbTexteditRowPtr(nativePtr);
		}

		public unsafe static implicit operator StbTexteditRow*(StbTexteditRowPtr wrappedPtr)
		{
			return wrappedPtr.NativePtr;
		}

		public static implicit operator StbTexteditRowPtr(IntPtr nativePtr)
		{
			return new StbTexteditRowPtr(nativePtr);
		}

		public unsafe ref float x0
		{
			get
			{
				return Unsafe.AsRef<float>((void*)(&this.NativePtr->x0));
			}
		}

		public unsafe ref float x1
		{
			get
			{
				return Unsafe.AsRef<float>((void*)(&this.NativePtr->x1));
			}
		}

		public unsafe ref float baseline_y_delta
		{
			get
			{
				return Unsafe.AsRef<float>((void*)(&this.NativePtr->baseline_y_delta));
			}
		}

		public unsafe ref float ymin
		{
			get
			{
				return Unsafe.AsRef<float>((void*)(&this.NativePtr->ymin));
			}
		}

		public unsafe ref float ymax
		{
			get
			{
				return Unsafe.AsRef<float>((void*)(&this.NativePtr->ymax));
			}
		}

		public unsafe ref int num_chars
		{
			get
			{
				return Unsafe.AsRef<int>((void*)(&this.NativePtr->num_chars));
			}
		}
	}
}
