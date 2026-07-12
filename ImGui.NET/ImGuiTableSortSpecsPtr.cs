using System;
using System.Runtime.CompilerServices;

namespace ImGuiNET
{
	public struct ImGuiTableSortSpecsPtr
	{
		public unsafe readonly ImGuiTableSortSpecs* NativePtr { get; }

		public unsafe ImGuiTableSortSpecsPtr(ImGuiTableSortSpecs* nativePtr)
		{
			this.NativePtr = nativePtr;
		}

		public unsafe ImGuiTableSortSpecsPtr(IntPtr nativePtr)
		{
			this.NativePtr = (ImGuiTableSortSpecs*)(void*)nativePtr;
		}

		public unsafe static implicit operator ImGuiTableSortSpecsPtr(ImGuiTableSortSpecs* nativePtr)
		{
			return new ImGuiTableSortSpecsPtr(nativePtr);
		}

		public unsafe static implicit operator ImGuiTableSortSpecs*(ImGuiTableSortSpecsPtr wrappedPtr)
		{
			return wrappedPtr.NativePtr;
		}

		public static implicit operator ImGuiTableSortSpecsPtr(IntPtr nativePtr)
		{
			return new ImGuiTableSortSpecsPtr(nativePtr);
		}

		public unsafe ref int SpecsCount
		{
			get
			{
				return Unsafe.AsRef<int>((void*)(&this.NativePtr->SpecsCount));
			}
		}

		public unsafe ref bool SpecsDirty
		{
			get
			{
				return Unsafe.AsRef<bool>((void*)(&this.NativePtr->SpecsDirty));
			}
		}

		public unsafe RangeAccessor<ImGuiTableColumnSortSpecs> Specs
		{
			get
			{
				return new RangeAccessor<ImGuiTableColumnSortSpecs>((void*)this.NativePtr->Specs, this.NativePtr->SpecsCount);
			}
		}

		public void Destroy()
		{
			ImGuiNative.ImGuiTableSortSpecs_destroy(this.NativePtr);
		}
	}
}
