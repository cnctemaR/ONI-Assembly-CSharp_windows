using System;
using System.Runtime.CompilerServices;
using System.Text;

namespace ImGuiNET
{
	public struct ImGuiPayloadPtr
	{
		public unsafe readonly ImGuiPayload* NativePtr { get; }

		public unsafe ImGuiPayloadPtr(ImGuiPayload* nativePtr)
		{
			this.NativePtr = nativePtr;
		}

		public unsafe ImGuiPayloadPtr(IntPtr nativePtr)
		{
			this.NativePtr = (ImGuiPayload*)(void*)nativePtr;
		}

		public unsafe static implicit operator ImGuiPayloadPtr(ImGuiPayload* nativePtr)
		{
			return new ImGuiPayloadPtr(nativePtr);
		}

		public unsafe static implicit operator ImGuiPayload*(ImGuiPayloadPtr wrappedPtr)
		{
			return wrappedPtr.NativePtr;
		}

		public static implicit operator ImGuiPayloadPtr(IntPtr nativePtr)
		{
			return new ImGuiPayloadPtr(nativePtr);
		}

		public unsafe IntPtr Data
		{
			get
			{
				return (IntPtr)this.NativePtr->Data;
			}
			set
			{
				this.NativePtr->Data = (void*)value;
			}
		}

		public unsafe ref int DataSize
		{
			get
			{
				return Unsafe.AsRef<int>((void*)(&this.NativePtr->DataSize));
			}
		}

		public unsafe ref uint SourceId
		{
			get
			{
				return Unsafe.AsRef<uint>((void*)(&this.NativePtr->SourceId));
			}
		}

		public unsafe ref uint SourceParentId
		{
			get
			{
				return Unsafe.AsRef<uint>((void*)(&this.NativePtr->SourceParentId));
			}
		}

		public unsafe ref int DataFrameCount
		{
			get
			{
				return Unsafe.AsRef<int>((void*)(&this.NativePtr->DataFrameCount));
			}
		}

		public unsafe RangeAccessor<byte> DataType
		{
			get
			{
				return new RangeAccessor<byte>((void*)(&this.NativePtr->DataType.FixedElementField), 33);
			}
		}

		public unsafe ref bool Preview
		{
			get
			{
				return Unsafe.AsRef<bool>((void*)(&this.NativePtr->Preview));
			}
		}

		public unsafe ref bool Delivery
		{
			get
			{
				return Unsafe.AsRef<bool>((void*)(&this.NativePtr->Delivery));
			}
		}

		public void Clear()
		{
			ImGuiNative.ImGuiPayload_Clear(this.NativePtr);
		}

		public void Destroy()
		{
			ImGuiNative.ImGuiPayload_destroy(this.NativePtr);
		}

		public unsafe bool IsDataType(string type)
		{
			int num = 0;
			byte* ptr;
			if (type != null)
			{
				num = Encoding.UTF8.GetByteCount(type);
				if (num > 2048)
				{
					ptr = Util.Allocate(num + 1);
				}
				else
				{
					ptr = stackalloc byte[(UIntPtr)(num + 1)];
				}
				int utf = Util.GetUtf8(type, ptr, num);
				ptr[utf] = 0;
			}
			else
			{
				ptr = null;
			}
			int num2 = (int)ImGuiNative.ImGuiPayload_IsDataType(this.NativePtr, ptr);
			if (num > 2048)
			{
				Util.Free(ptr);
			}
			return num2 != 0;
		}

		public bool IsDelivery()
		{
			return ImGuiNative.ImGuiPayload_IsDelivery(this.NativePtr) > 0;
		}

		public bool IsPreview()
		{
			return ImGuiNative.ImGuiPayload_IsPreview(this.NativePtr) > 0;
		}
	}
}
