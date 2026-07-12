using System;
using System.Runtime.CompilerServices;

namespace ImGuiNET
{
	public struct ImDrawChannelPtr
	{
		public unsafe readonly ImDrawChannel* NativePtr { get; }

		public unsafe ImDrawChannelPtr(ImDrawChannel* nativePtr)
		{
			this.NativePtr = nativePtr;
		}

		public unsafe ImDrawChannelPtr(IntPtr nativePtr)
		{
			this.NativePtr = (ImDrawChannel*)(void*)nativePtr;
		}

		public unsafe static implicit operator ImDrawChannelPtr(ImDrawChannel* nativePtr)
		{
			return new ImDrawChannelPtr(nativePtr);
		}

		public unsafe static implicit operator ImDrawChannel*(ImDrawChannelPtr wrappedPtr)
		{
			return wrappedPtr.NativePtr;
		}

		public static implicit operator ImDrawChannelPtr(IntPtr nativePtr)
		{
			return new ImDrawChannelPtr(nativePtr);
		}

		public unsafe ImPtrVector<ImDrawCmdPtr> _CmdBuffer
		{
			get
			{
				return new ImPtrVector<ImDrawCmdPtr>(this.NativePtr->_CmdBuffer, Unsafe.SizeOf<ImDrawCmd>());
			}
		}

		public unsafe ImVector<ushort> _IdxBuffer
		{
			get
			{
				return new ImVector<ushort>(this.NativePtr->_IdxBuffer);
			}
		}
	}
}
