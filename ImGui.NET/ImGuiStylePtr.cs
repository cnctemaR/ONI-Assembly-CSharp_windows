using System;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace ImGuiNET
{
	public struct ImGuiStylePtr
	{
		public unsafe readonly ImGuiStyle* NativePtr { get; }

		public unsafe ImGuiStylePtr(ImGuiStyle* nativePtr)
		{
			this.NativePtr = nativePtr;
		}

		public unsafe ImGuiStylePtr(IntPtr nativePtr)
		{
			this.NativePtr = (ImGuiStyle*)(void*)nativePtr;
		}

		public unsafe static implicit operator ImGuiStylePtr(ImGuiStyle* nativePtr)
		{
			return new ImGuiStylePtr(nativePtr);
		}

		public unsafe static implicit operator ImGuiStyle*(ImGuiStylePtr wrappedPtr)
		{
			return wrappedPtr.NativePtr;
		}

		public static implicit operator ImGuiStylePtr(IntPtr nativePtr)
		{
			return new ImGuiStylePtr(nativePtr);
		}

		public unsafe ref float Alpha
		{
			get
			{
				return Unsafe.AsRef<float>((void*)(&this.NativePtr->Alpha));
			}
		}

		public unsafe ref Vector2 WindowPadding
		{
			get
			{
				return Unsafe.AsRef<Vector2>((void*)(&this.NativePtr->WindowPadding));
			}
		}

		public unsafe ref float WindowRounding
		{
			get
			{
				return Unsafe.AsRef<float>((void*)(&this.NativePtr->WindowRounding));
			}
		}

		public unsafe ref float WindowBorderSize
		{
			get
			{
				return Unsafe.AsRef<float>((void*)(&this.NativePtr->WindowBorderSize));
			}
		}

		public unsafe ref Vector2 WindowMinSize
		{
			get
			{
				return Unsafe.AsRef<Vector2>((void*)(&this.NativePtr->WindowMinSize));
			}
		}

		public unsafe ref Vector2 WindowTitleAlign
		{
			get
			{
				return Unsafe.AsRef<Vector2>((void*)(&this.NativePtr->WindowTitleAlign));
			}
		}

		public unsafe ref ImGuiDir WindowMenuButtonPosition
		{
			get
			{
				return Unsafe.AsRef<ImGuiDir>((void*)(&this.NativePtr->WindowMenuButtonPosition));
			}
		}

		public unsafe ref float ChildRounding
		{
			get
			{
				return Unsafe.AsRef<float>((void*)(&this.NativePtr->ChildRounding));
			}
		}

		public unsafe ref float ChildBorderSize
		{
			get
			{
				return Unsafe.AsRef<float>((void*)(&this.NativePtr->ChildBorderSize));
			}
		}

		public unsafe ref float PopupRounding
		{
			get
			{
				return Unsafe.AsRef<float>((void*)(&this.NativePtr->PopupRounding));
			}
		}

		public unsafe ref float PopupBorderSize
		{
			get
			{
				return Unsafe.AsRef<float>((void*)(&this.NativePtr->PopupBorderSize));
			}
		}

		public unsafe ref Vector2 FramePadding
		{
			get
			{
				return Unsafe.AsRef<Vector2>((void*)(&this.NativePtr->FramePadding));
			}
		}

		public unsafe ref float FrameRounding
		{
			get
			{
				return Unsafe.AsRef<float>((void*)(&this.NativePtr->FrameRounding));
			}
		}

		public unsafe ref float FrameBorderSize
		{
			get
			{
				return Unsafe.AsRef<float>((void*)(&this.NativePtr->FrameBorderSize));
			}
		}

		public unsafe ref Vector2 ItemSpacing
		{
			get
			{
				return Unsafe.AsRef<Vector2>((void*)(&this.NativePtr->ItemSpacing));
			}
		}

		public unsafe ref Vector2 ItemInnerSpacing
		{
			get
			{
				return Unsafe.AsRef<Vector2>((void*)(&this.NativePtr->ItemInnerSpacing));
			}
		}

		public unsafe ref Vector2 CellPadding
		{
			get
			{
				return Unsafe.AsRef<Vector2>((void*)(&this.NativePtr->CellPadding));
			}
		}

		public unsafe ref Vector2 TouchExtraPadding
		{
			get
			{
				return Unsafe.AsRef<Vector2>((void*)(&this.NativePtr->TouchExtraPadding));
			}
		}

		public unsafe ref float IndentSpacing
		{
			get
			{
				return Unsafe.AsRef<float>((void*)(&this.NativePtr->IndentSpacing));
			}
		}

		public unsafe ref float ColumnsMinSpacing
		{
			get
			{
				return Unsafe.AsRef<float>((void*)(&this.NativePtr->ColumnsMinSpacing));
			}
		}

		public unsafe ref float ScrollbarSize
		{
			get
			{
				return Unsafe.AsRef<float>((void*)(&this.NativePtr->ScrollbarSize));
			}
		}

		public unsafe ref float ScrollbarRounding
		{
			get
			{
				return Unsafe.AsRef<float>((void*)(&this.NativePtr->ScrollbarRounding));
			}
		}

		public unsafe ref float GrabMinSize
		{
			get
			{
				return Unsafe.AsRef<float>((void*)(&this.NativePtr->GrabMinSize));
			}
		}

		public unsafe ref float GrabRounding
		{
			get
			{
				return Unsafe.AsRef<float>((void*)(&this.NativePtr->GrabRounding));
			}
		}

		public unsafe ref float LogSliderDeadzone
		{
			get
			{
				return Unsafe.AsRef<float>((void*)(&this.NativePtr->LogSliderDeadzone));
			}
		}

		public unsafe ref float TabRounding
		{
			get
			{
				return Unsafe.AsRef<float>((void*)(&this.NativePtr->TabRounding));
			}
		}

		public unsafe ref float TabBorderSize
		{
			get
			{
				return Unsafe.AsRef<float>((void*)(&this.NativePtr->TabBorderSize));
			}
		}

		public unsafe ref float TabMinWidthForCloseButton
		{
			get
			{
				return Unsafe.AsRef<float>((void*)(&this.NativePtr->TabMinWidthForCloseButton));
			}
		}

		public unsafe ref ImGuiDir ColorButtonPosition
		{
			get
			{
				return Unsafe.AsRef<ImGuiDir>((void*)(&this.NativePtr->ColorButtonPosition));
			}
		}

		public unsafe ref Vector2 ButtonTextAlign
		{
			get
			{
				return Unsafe.AsRef<Vector2>((void*)(&this.NativePtr->ButtonTextAlign));
			}
		}

		public unsafe ref Vector2 SelectableTextAlign
		{
			get
			{
				return Unsafe.AsRef<Vector2>((void*)(&this.NativePtr->SelectableTextAlign));
			}
		}

		public unsafe ref Vector2 DisplayWindowPadding
		{
			get
			{
				return Unsafe.AsRef<Vector2>((void*)(&this.NativePtr->DisplayWindowPadding));
			}
		}

		public unsafe ref Vector2 DisplaySafeAreaPadding
		{
			get
			{
				return Unsafe.AsRef<Vector2>((void*)(&this.NativePtr->DisplaySafeAreaPadding));
			}
		}

		public unsafe ref float MouseCursorScale
		{
			get
			{
				return Unsafe.AsRef<float>((void*)(&this.NativePtr->MouseCursorScale));
			}
		}

		public unsafe ref bool AntiAliasedLines
		{
			get
			{
				return Unsafe.AsRef<bool>((void*)(&this.NativePtr->AntiAliasedLines));
			}
		}

		public unsafe ref bool AntiAliasedLinesUseTex
		{
			get
			{
				return Unsafe.AsRef<bool>((void*)(&this.NativePtr->AntiAliasedLinesUseTex));
			}
		}

		public unsafe ref bool AntiAliasedFill
		{
			get
			{
				return Unsafe.AsRef<bool>((void*)(&this.NativePtr->AntiAliasedFill));
			}
		}

		public unsafe ref float CurveTessellationTol
		{
			get
			{
				return Unsafe.AsRef<float>((void*)(&this.NativePtr->CurveTessellationTol));
			}
		}

		public unsafe ref float CircleTessellationMaxError
		{
			get
			{
				return Unsafe.AsRef<float>((void*)(&this.NativePtr->CircleTessellationMaxError));
			}
		}

		public unsafe RangeAccessor<Vector4> Colors
		{
			get
			{
				return new RangeAccessor<Vector4>((void*)(&this.NativePtr->Colors_0), 55);
			}
		}

		public void Destroy()
		{
			ImGuiNative.ImGuiStyle_destroy(this.NativePtr);
		}

		public void ScaleAllSizes(float scale_factor)
		{
			ImGuiNative.ImGuiStyle_ScaleAllSizes(this.NativePtr, scale_factor);
		}
	}
}
