using System;
using System.Runtime.CompilerServices;

namespace ImGuiNET
{
	public struct ImGuiTableColumnSortSpecsPtr
	{
		public unsafe readonly ImGuiTableColumnSortSpecs* NativePtr { get; }

		public unsafe ImGuiTableColumnSortSpecsPtr(ImGuiTableColumnSortSpecs* nativePtr)
		{
			this.NativePtr = nativePtr;
		}

		public unsafe ImGuiTableColumnSortSpecsPtr(IntPtr nativePtr)
		{
			this.NativePtr = (ImGuiTableColumnSortSpecs*)(void*)nativePtr;
		}

		public unsafe static implicit operator ImGuiTableColumnSortSpecsPtr(ImGuiTableColumnSortSpecs* nativePtr)
		{
			return new ImGuiTableColumnSortSpecsPtr(nativePtr);
		}

		public unsafe static implicit operator ImGuiTableColumnSortSpecs*(ImGuiTableColumnSortSpecsPtr wrappedPtr)
		{
			return wrappedPtr.NativePtr;
		}

		public static implicit operator ImGuiTableColumnSortSpecsPtr(IntPtr nativePtr)
		{
			return new ImGuiTableColumnSortSpecsPtr(nativePtr);
		}

		public unsafe ref uint ColumnUserID
		{
			get
			{
				return Unsafe.AsRef<uint>((void*)(&this.NativePtr->ColumnUserID));
			}
		}

		public unsafe ref short ColumnIndex
		{
			get
			{
				return Unsafe.AsRef<short>((void*)(&this.NativePtr->ColumnIndex));
			}
		}

		public unsafe ref short SortOrder
		{
			get
			{
				return Unsafe.AsRef<short>((void*)(&this.NativePtr->SortOrder));
			}
		}

		public unsafe ref ImGuiSortDirection SortDirection
		{
			get
			{
				return Unsafe.AsRef<ImGuiSortDirection>((void*)(&this.NativePtr->SortDirection));
			}
		}

		public void Destroy()
		{
			ImGuiNative.ImGuiTableColumnSortSpecs_destroy(this.NativePtr);
		}
	}
}
