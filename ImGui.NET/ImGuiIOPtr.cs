using System;
using System.Runtime.CompilerServices;
using System.Text;
using UnityEngine;

namespace ImGuiNET
{
	public struct ImGuiIOPtr
	{
		public unsafe readonly ImGuiIO* NativePtr { get; }

		public unsafe ImGuiIOPtr(ImGuiIO* nativePtr)
		{
			this.NativePtr = nativePtr;
		}

		public unsafe ImGuiIOPtr(IntPtr nativePtr)
		{
			this.NativePtr = (ImGuiIO*)(void*)nativePtr;
		}

		public unsafe static implicit operator ImGuiIOPtr(ImGuiIO* nativePtr)
		{
			return new ImGuiIOPtr(nativePtr);
		}

		public unsafe static implicit operator ImGuiIO*(ImGuiIOPtr wrappedPtr)
		{
			return wrappedPtr.NativePtr;
		}

		public static implicit operator ImGuiIOPtr(IntPtr nativePtr)
		{
			return new ImGuiIOPtr(nativePtr);
		}

		public unsafe ref ImGuiConfigFlags ConfigFlags
		{
			get
			{
				return Unsafe.AsRef<ImGuiConfigFlags>((void*)(&this.NativePtr->ConfigFlags));
			}
		}

		public unsafe ref ImGuiBackendFlags BackendFlags
		{
			get
			{
				return Unsafe.AsRef<ImGuiBackendFlags>((void*)(&this.NativePtr->BackendFlags));
			}
		}

		public unsafe ref Vector2 DisplaySize
		{
			get
			{
				return Unsafe.AsRef<Vector2>((void*)(&this.NativePtr->DisplaySize));
			}
		}

		public unsafe ref float DeltaTime
		{
			get
			{
				return Unsafe.AsRef<float>((void*)(&this.NativePtr->DeltaTime));
			}
		}

		public unsafe ref float IniSavingRate
		{
			get
			{
				return Unsafe.AsRef<float>((void*)(&this.NativePtr->IniSavingRate));
			}
		}

		public unsafe NullTerminatedString IniFilename
		{
			get
			{
				return new NullTerminatedString(this.NativePtr->IniFilename);
			}
		}

		public unsafe NullTerminatedString LogFilename
		{
			get
			{
				return new NullTerminatedString(this.NativePtr->LogFilename);
			}
		}

		public unsafe ref float MouseDoubleClickTime
		{
			get
			{
				return Unsafe.AsRef<float>((void*)(&this.NativePtr->MouseDoubleClickTime));
			}
		}

		public unsafe ref float MouseDoubleClickMaxDist
		{
			get
			{
				return Unsafe.AsRef<float>((void*)(&this.NativePtr->MouseDoubleClickMaxDist));
			}
		}

		public unsafe ref float MouseDragThreshold
		{
			get
			{
				return Unsafe.AsRef<float>((void*)(&this.NativePtr->MouseDragThreshold));
			}
		}

		public unsafe RangeAccessor<int> KeyMap
		{
			get
			{
				return new RangeAccessor<int>((void*)(&this.NativePtr->KeyMap.FixedElementField), 22);
			}
		}

		public unsafe ref float KeyRepeatDelay
		{
			get
			{
				return Unsafe.AsRef<float>((void*)(&this.NativePtr->KeyRepeatDelay));
			}
		}

		public unsafe ref float KeyRepeatRate
		{
			get
			{
				return Unsafe.AsRef<float>((void*)(&this.NativePtr->KeyRepeatRate));
			}
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

		public unsafe ImFontAtlasPtr Fonts
		{
			get
			{
				return new ImFontAtlasPtr(this.NativePtr->Fonts);
			}
		}

		public unsafe ref float FontGlobalScale
		{
			get
			{
				return Unsafe.AsRef<float>((void*)(&this.NativePtr->FontGlobalScale));
			}
		}

		public unsafe ref bool FontAllowUserScaling
		{
			get
			{
				return Unsafe.AsRef<bool>((void*)(&this.NativePtr->FontAllowUserScaling));
			}
		}

		public unsafe ImFontPtr FontDefault
		{
			get
			{
				return new ImFontPtr(this.NativePtr->FontDefault);
			}
		}

		public unsafe ref Vector2 DisplayFramebufferScale
		{
			get
			{
				return Unsafe.AsRef<Vector2>((void*)(&this.NativePtr->DisplayFramebufferScale));
			}
		}

		public unsafe ref bool ConfigDockingNoSplit
		{
			get
			{
				return Unsafe.AsRef<bool>((void*)(&this.NativePtr->ConfigDockingNoSplit));
			}
		}

		public unsafe ref bool ConfigDockingWithShift
		{
			get
			{
				return Unsafe.AsRef<bool>((void*)(&this.NativePtr->ConfigDockingWithShift));
			}
		}

		public unsafe ref bool ConfigDockingAlwaysTabBar
		{
			get
			{
				return Unsafe.AsRef<bool>((void*)(&this.NativePtr->ConfigDockingAlwaysTabBar));
			}
		}

		public unsafe ref bool ConfigDockingTransparentPayload
		{
			get
			{
				return Unsafe.AsRef<bool>((void*)(&this.NativePtr->ConfigDockingTransparentPayload));
			}
		}

		public unsafe ref bool ConfigViewportsNoAutoMerge
		{
			get
			{
				return Unsafe.AsRef<bool>((void*)(&this.NativePtr->ConfigViewportsNoAutoMerge));
			}
		}

		public unsafe ref bool ConfigViewportsNoTaskBarIcon
		{
			get
			{
				return Unsafe.AsRef<bool>((void*)(&this.NativePtr->ConfigViewportsNoTaskBarIcon));
			}
		}

		public unsafe ref bool ConfigViewportsNoDecoration
		{
			get
			{
				return Unsafe.AsRef<bool>((void*)(&this.NativePtr->ConfigViewportsNoDecoration));
			}
		}

		public unsafe ref bool ConfigViewportsNoDefaultParent
		{
			get
			{
				return Unsafe.AsRef<bool>((void*)(&this.NativePtr->ConfigViewportsNoDefaultParent));
			}
		}

		public unsafe ref bool MouseDrawCursor
		{
			get
			{
				return Unsafe.AsRef<bool>((void*)(&this.NativePtr->MouseDrawCursor));
			}
		}

		public unsafe ref bool ConfigMacOSXBehaviors
		{
			get
			{
				return Unsafe.AsRef<bool>((void*)(&this.NativePtr->ConfigMacOSXBehaviors));
			}
		}

		public unsafe ref bool ConfigInputTextCursorBlink
		{
			get
			{
				return Unsafe.AsRef<bool>((void*)(&this.NativePtr->ConfigInputTextCursorBlink));
			}
		}

		public unsafe ref bool ConfigDragClickToInputText
		{
			get
			{
				return Unsafe.AsRef<bool>((void*)(&this.NativePtr->ConfigDragClickToInputText));
			}
		}

		public unsafe ref bool ConfigWindowsResizeFromEdges
		{
			get
			{
				return Unsafe.AsRef<bool>((void*)(&this.NativePtr->ConfigWindowsResizeFromEdges));
			}
		}

		public unsafe ref bool ConfigWindowsMoveFromTitleBarOnly
		{
			get
			{
				return Unsafe.AsRef<bool>((void*)(&this.NativePtr->ConfigWindowsMoveFromTitleBarOnly));
			}
		}

		public unsafe ref float ConfigMemoryCompactTimer
		{
			get
			{
				return Unsafe.AsRef<float>((void*)(&this.NativePtr->ConfigMemoryCompactTimer));
			}
		}

		public unsafe NullTerminatedString BackendPlatformName
		{
			get
			{
				return new NullTerminatedString(this.NativePtr->BackendPlatformName);
			}
		}

		public unsafe NullTerminatedString BackendRendererName
		{
			get
			{
				return new NullTerminatedString(this.NativePtr->BackendRendererName);
			}
		}

		public unsafe IntPtr BackendPlatformUserData
		{
			get
			{
				return (IntPtr)this.NativePtr->BackendPlatformUserData;
			}
			set
			{
				this.NativePtr->BackendPlatformUserData = (void*)value;
			}
		}

		public unsafe IntPtr BackendRendererUserData
		{
			get
			{
				return (IntPtr)this.NativePtr->BackendRendererUserData;
			}
			set
			{
				this.NativePtr->BackendRendererUserData = (void*)value;
			}
		}

		public unsafe IntPtr BackendLanguageUserData
		{
			get
			{
				return (IntPtr)this.NativePtr->BackendLanguageUserData;
			}
			set
			{
				this.NativePtr->BackendLanguageUserData = (void*)value;
			}
		}

		public unsafe ref IntPtr GetClipboardTextFn
		{
			get
			{
				return Unsafe.AsRef<IntPtr>((void*)(&this.NativePtr->GetClipboardTextFn));
			}
		}

		public unsafe ref IntPtr SetClipboardTextFn
		{
			get
			{
				return Unsafe.AsRef<IntPtr>((void*)(&this.NativePtr->SetClipboardTextFn));
			}
		}

		public unsafe IntPtr ClipboardUserData
		{
			get
			{
				return (IntPtr)this.NativePtr->ClipboardUserData;
			}
			set
			{
				this.NativePtr->ClipboardUserData = (void*)value;
			}
		}

		public unsafe ref Vector2 MousePos
		{
			get
			{
				return Unsafe.AsRef<Vector2>((void*)(&this.NativePtr->MousePos));
			}
		}

		public unsafe RangeAccessor<bool> MouseDown
		{
			get
			{
				return new RangeAccessor<bool>((void*)(&this.NativePtr->MouseDown.FixedElementField), 5);
			}
		}

		public unsafe ref float MouseWheel
		{
			get
			{
				return Unsafe.AsRef<float>((void*)(&this.NativePtr->MouseWheel));
			}
		}

		public unsafe ref float MouseWheelH
		{
			get
			{
				return Unsafe.AsRef<float>((void*)(&this.NativePtr->MouseWheelH));
			}
		}

		public unsafe ref uint MouseHoveredViewport
		{
			get
			{
				return Unsafe.AsRef<uint>((void*)(&this.NativePtr->MouseHoveredViewport));
			}
		}

		public unsafe ref bool KeyCtrl
		{
			get
			{
				return Unsafe.AsRef<bool>((void*)(&this.NativePtr->KeyCtrl));
			}
		}

		public unsafe ref bool KeyShift
		{
			get
			{
				return Unsafe.AsRef<bool>((void*)(&this.NativePtr->KeyShift));
			}
		}

		public unsafe ref bool KeyAlt
		{
			get
			{
				return Unsafe.AsRef<bool>((void*)(&this.NativePtr->KeyAlt));
			}
		}

		public unsafe ref bool KeySuper
		{
			get
			{
				return Unsafe.AsRef<bool>((void*)(&this.NativePtr->KeySuper));
			}
		}

		public unsafe RangeAccessor<bool> KeysDown
		{
			get
			{
				return new RangeAccessor<bool>((void*)(&this.NativePtr->KeysDown.FixedElementField), 512);
			}
		}

		public unsafe RangeAccessor<float> NavInputs
		{
			get
			{
				return new RangeAccessor<float>((void*)(&this.NativePtr->NavInputs.FixedElementField), 21);
			}
		}

		public unsafe ref bool WantCaptureMouse
		{
			get
			{
				return Unsafe.AsRef<bool>((void*)(&this.NativePtr->WantCaptureMouse));
			}
		}

		public unsafe ref bool WantCaptureKeyboard
		{
			get
			{
				return Unsafe.AsRef<bool>((void*)(&this.NativePtr->WantCaptureKeyboard));
			}
		}

		public unsafe ref bool WantTextInput
		{
			get
			{
				return Unsafe.AsRef<bool>((void*)(&this.NativePtr->WantTextInput));
			}
		}

		public unsafe ref bool WantSetMousePos
		{
			get
			{
				return Unsafe.AsRef<bool>((void*)(&this.NativePtr->WantSetMousePos));
			}
		}

		public unsafe ref bool WantSaveIniSettings
		{
			get
			{
				return Unsafe.AsRef<bool>((void*)(&this.NativePtr->WantSaveIniSettings));
			}
		}

		public unsafe ref bool NavActive
		{
			get
			{
				return Unsafe.AsRef<bool>((void*)(&this.NativePtr->NavActive));
			}
		}

		public unsafe ref bool NavVisible
		{
			get
			{
				return Unsafe.AsRef<bool>((void*)(&this.NativePtr->NavVisible));
			}
		}

		public unsafe ref float Framerate
		{
			get
			{
				return Unsafe.AsRef<float>((void*)(&this.NativePtr->Framerate));
			}
		}

		public unsafe ref int MetricsRenderVertices
		{
			get
			{
				return Unsafe.AsRef<int>((void*)(&this.NativePtr->MetricsRenderVertices));
			}
		}

		public unsafe ref int MetricsRenderIndices
		{
			get
			{
				return Unsafe.AsRef<int>((void*)(&this.NativePtr->MetricsRenderIndices));
			}
		}

		public unsafe ref int MetricsRenderWindows
		{
			get
			{
				return Unsafe.AsRef<int>((void*)(&this.NativePtr->MetricsRenderWindows));
			}
		}

		public unsafe ref int MetricsActiveWindows
		{
			get
			{
				return Unsafe.AsRef<int>((void*)(&this.NativePtr->MetricsActiveWindows));
			}
		}

		public unsafe ref int MetricsActiveAllocations
		{
			get
			{
				return Unsafe.AsRef<int>((void*)(&this.NativePtr->MetricsActiveAllocations));
			}
		}

		public unsafe ref Vector2 MouseDelta
		{
			get
			{
				return Unsafe.AsRef<Vector2>((void*)(&this.NativePtr->MouseDelta));
			}
		}

		public unsafe ref ImGuiKeyModFlags KeyMods
		{
			get
			{
				return Unsafe.AsRef<ImGuiKeyModFlags>((void*)(&this.NativePtr->KeyMods));
			}
		}

		public unsafe ref Vector2 MousePosPrev
		{
			get
			{
				return Unsafe.AsRef<Vector2>((void*)(&this.NativePtr->MousePosPrev));
			}
		}

		public unsafe RangeAccessor<Vector2> MouseClickedPos
		{
			get
			{
				return new RangeAccessor<Vector2>((void*)(&this.NativePtr->MouseClickedPos_0), 5);
			}
		}

		public unsafe RangeAccessor<double> MouseClickedTime
		{
			get
			{
				return new RangeAccessor<double>((void*)(&this.NativePtr->MouseClickedTime.FixedElementField), 5);
			}
		}

		public unsafe RangeAccessor<bool> MouseClicked
		{
			get
			{
				return new RangeAccessor<bool>((void*)(&this.NativePtr->MouseClicked.FixedElementField), 5);
			}
		}

		public unsafe RangeAccessor<bool> MouseDoubleClicked
		{
			get
			{
				return new RangeAccessor<bool>((void*)(&this.NativePtr->MouseDoubleClicked.FixedElementField), 5);
			}
		}

		public unsafe RangeAccessor<bool> MouseReleased
		{
			get
			{
				return new RangeAccessor<bool>((void*)(&this.NativePtr->MouseReleased.FixedElementField), 5);
			}
		}

		public unsafe RangeAccessor<bool> MouseDownOwned
		{
			get
			{
				return new RangeAccessor<bool>((void*)(&this.NativePtr->MouseDownOwned.FixedElementField), 5);
			}
		}

		public unsafe RangeAccessor<bool> MouseDownWasDoubleClick
		{
			get
			{
				return new RangeAccessor<bool>((void*)(&this.NativePtr->MouseDownWasDoubleClick.FixedElementField), 5);
			}
		}

		public unsafe RangeAccessor<float> MouseDownDuration
		{
			get
			{
				return new RangeAccessor<float>((void*)(&this.NativePtr->MouseDownDuration.FixedElementField), 5);
			}
		}

		public unsafe RangeAccessor<float> MouseDownDurationPrev
		{
			get
			{
				return new RangeAccessor<float>((void*)(&this.NativePtr->MouseDownDurationPrev.FixedElementField), 5);
			}
		}

		public unsafe RangeAccessor<Vector2> MouseDragMaxDistanceAbs
		{
			get
			{
				return new RangeAccessor<Vector2>((void*)(&this.NativePtr->MouseDragMaxDistanceAbs_0), 5);
			}
		}

		public unsafe RangeAccessor<float> MouseDragMaxDistanceSqr
		{
			get
			{
				return new RangeAccessor<float>((void*)(&this.NativePtr->MouseDragMaxDistanceSqr.FixedElementField), 5);
			}
		}

		public unsafe RangeAccessor<float> KeysDownDuration
		{
			get
			{
				return new RangeAccessor<float>((void*)(&this.NativePtr->KeysDownDuration.FixedElementField), 512);
			}
		}

		public unsafe RangeAccessor<float> KeysDownDurationPrev
		{
			get
			{
				return new RangeAccessor<float>((void*)(&this.NativePtr->KeysDownDurationPrev.FixedElementField), 512);
			}
		}

		public unsafe RangeAccessor<float> NavInputsDownDuration
		{
			get
			{
				return new RangeAccessor<float>((void*)(&this.NativePtr->NavInputsDownDuration.FixedElementField), 21);
			}
		}

		public unsafe RangeAccessor<float> NavInputsDownDurationPrev
		{
			get
			{
				return new RangeAccessor<float>((void*)(&this.NativePtr->NavInputsDownDurationPrev.FixedElementField), 21);
			}
		}

		public unsafe ref float PenPressure
		{
			get
			{
				return Unsafe.AsRef<float>((void*)(&this.NativePtr->PenPressure));
			}
		}

		public unsafe ref ushort InputQueueSurrogate
		{
			get
			{
				return Unsafe.AsRef<ushort>((void*)(&this.NativePtr->InputQueueSurrogate));
			}
		}

		public unsafe ImVector<ushort> InputQueueCharacters
		{
			get
			{
				return new ImVector<ushort>(this.NativePtr->InputQueueCharacters);
			}
		}

		public void AddInputCharacter(uint c)
		{
			ImGuiNative.ImGuiIO_AddInputCharacter(this.NativePtr, c);
		}

		public unsafe void AddInputCharactersUTF8(string str)
		{
			int num = 0;
			byte* ptr;
			if (str != null)
			{
				num = Encoding.UTF8.GetByteCount(str);
				if (num > 2048)
				{
					ptr = Util.Allocate(num + 1);
				}
				else
				{
					ptr = stackalloc byte[(UIntPtr)(num + 1)];
				}
				int utf = Util.GetUtf8(str, ptr, num);
				ptr[utf] = 0;
			}
			else
			{
				ptr = null;
			}
			ImGuiNative.ImGuiIO_AddInputCharactersUTF8(this.NativePtr, ptr);
			if (num > 2048)
			{
				Util.Free(ptr);
			}
		}

		public void AddInputCharacterUTF16(ushort c)
		{
			ImGuiNative.ImGuiIO_AddInputCharacterUTF16(this.NativePtr, c);
		}

		public void ClearInputCharacters()
		{
			ImGuiNative.ImGuiIO_ClearInputCharacters(this.NativePtr);
		}

		public void Destroy()
		{
			ImGuiNative.ImGuiIO_destroy(this.NativePtr);
		}
	}
}
