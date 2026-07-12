using System;
using System.Runtime.CompilerServices;

namespace ImGuiNET
{
	public struct ImGuiWindowClassPtr
	{
		public unsafe readonly ImGuiWindowClass* NativePtr { get; }

		public unsafe ImGuiWindowClassPtr(ImGuiWindowClass* nativePtr)
		{
			this.NativePtr = nativePtr;
		}

		public unsafe ImGuiWindowClassPtr(IntPtr nativePtr)
		{
			this.NativePtr = (ImGuiWindowClass*)(void*)nativePtr;
		}

		public unsafe static implicit operator ImGuiWindowClassPtr(ImGuiWindowClass* nativePtr)
		{
			return new ImGuiWindowClassPtr(nativePtr);
		}

		public unsafe static implicit operator ImGuiWindowClass*(ImGuiWindowClassPtr wrappedPtr)
		{
			return wrappedPtr.NativePtr;
		}

		public static implicit operator ImGuiWindowClassPtr(IntPtr nativePtr)
		{
			return new ImGuiWindowClassPtr(nativePtr);
		}

		public unsafe ref uint ClassId
		{
			get
			{
				return Unsafe.AsRef<uint>((void*)(&this.NativePtr->ClassId));
			}
		}

		public unsafe ref uint ParentViewportId
		{
			get
			{
				return Unsafe.AsRef<uint>((void*)(&this.NativePtr->ParentViewportId));
			}
		}

		public unsafe ref ImGuiViewportFlags ViewportFlagsOverrideSet
		{
			get
			{
				return Unsafe.AsRef<ImGuiViewportFlags>((void*)(&this.NativePtr->ViewportFlagsOverrideSet));
			}
		}

		public unsafe ref ImGuiViewportFlags ViewportFlagsOverrideClear
		{
			get
			{
				return Unsafe.AsRef<ImGuiViewportFlags>((void*)(&this.NativePtr->ViewportFlagsOverrideClear));
			}
		}

		public unsafe ref ImGuiTabItemFlags TabItemFlagsOverrideSet
		{
			get
			{
				return Unsafe.AsRef<ImGuiTabItemFlags>((void*)(&this.NativePtr->TabItemFlagsOverrideSet));
			}
		}

		public unsafe ref ImGuiDockNodeFlags DockNodeFlagsOverrideSet
		{
			get
			{
				return Unsafe.AsRef<ImGuiDockNodeFlags>((void*)(&this.NativePtr->DockNodeFlagsOverrideSet));
			}
		}

		public unsafe ref ImGuiDockNodeFlags DockNodeFlagsOverrideClear
		{
			get
			{
				return Unsafe.AsRef<ImGuiDockNodeFlags>((void*)(&this.NativePtr->DockNodeFlagsOverrideClear));
			}
		}

		public unsafe ref bool DockingAlwaysTabBar
		{
			get
			{
				return Unsafe.AsRef<bool>((void*)(&this.NativePtr->DockingAlwaysTabBar));
			}
		}

		public unsafe ref bool DockingAllowUnclassed
		{
			get
			{
				return Unsafe.AsRef<bool>((void*)(&this.NativePtr->DockingAllowUnclassed));
			}
		}

		public void Destroy()
		{
			ImGuiNative.ImGuiWindowClass_destroy(this.NativePtr);
		}
	}
}
