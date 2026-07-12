using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine;

namespace ImGuiNET
{
	public struct ImGuiIO
	{
		public ImGuiConfigFlags ConfigFlags;

		public ImGuiBackendFlags BackendFlags;

		public Vector2 DisplaySize;

		public float DeltaTime;

		public float IniSavingRate;

		public unsafe byte* IniFilename;

		public unsafe byte* LogFilename;

		public float MouseDoubleClickTime;

		public float MouseDoubleClickMaxDist;

		public float MouseDragThreshold;

		[FixedBuffer(typeof(int), 22)]
		public ImGuiIO.<KeyMap>e__FixedBuffer KeyMap;

		public float KeyRepeatDelay;

		public float KeyRepeatRate;

		public unsafe void* UserData;

		public unsafe ImFontAtlas* Fonts;

		public float FontGlobalScale;

		public byte FontAllowUserScaling;

		public unsafe ImFont* FontDefault;

		public Vector2 DisplayFramebufferScale;

		public byte ConfigDockingNoSplit;

		public byte ConfigDockingWithShift;

		public byte ConfigDockingAlwaysTabBar;

		public byte ConfigDockingTransparentPayload;

		public byte ConfigViewportsNoAutoMerge;

		public byte ConfigViewportsNoTaskBarIcon;

		public byte ConfigViewportsNoDecoration;

		public byte ConfigViewportsNoDefaultParent;

		public byte MouseDrawCursor;

		public byte ConfigMacOSXBehaviors;

		public byte ConfigInputTextCursorBlink;

		public byte ConfigDragClickToInputText;

		public byte ConfigWindowsResizeFromEdges;

		public byte ConfigWindowsMoveFromTitleBarOnly;

		public float ConfigMemoryCompactTimer;

		public unsafe byte* BackendPlatformName;

		public unsafe byte* BackendRendererName;

		public unsafe void* BackendPlatformUserData;

		public unsafe void* BackendRendererUserData;

		public unsafe void* BackendLanguageUserData;

		public IntPtr GetClipboardTextFn;

		public IntPtr SetClipboardTextFn;

		public unsafe void* ClipboardUserData;

		public Vector2 MousePos;

		[FixedBuffer(typeof(byte), 5)]
		public ImGuiIO.<MouseDown>e__FixedBuffer MouseDown;

		public float MouseWheel;

		public float MouseWheelH;

		public uint MouseHoveredViewport;

		public byte KeyCtrl;

		public byte KeyShift;

		public byte KeyAlt;

		public byte KeySuper;

		[FixedBuffer(typeof(byte), 512)]
		public ImGuiIO.<KeysDown>e__FixedBuffer KeysDown;

		[FixedBuffer(typeof(float), 21)]
		public ImGuiIO.<NavInputs>e__FixedBuffer NavInputs;

		public byte WantCaptureMouse;

		public byte WantCaptureKeyboard;

		public byte WantTextInput;

		public byte WantSetMousePos;

		public byte WantSaveIniSettings;

		public byte NavActive;

		public byte NavVisible;

		public float Framerate;

		public int MetricsRenderVertices;

		public int MetricsRenderIndices;

		public int MetricsRenderWindows;

		public int MetricsActiveWindows;

		public int MetricsActiveAllocations;

		public Vector2 MouseDelta;

		public ImGuiKeyModFlags KeyMods;

		public Vector2 MousePosPrev;

		public Vector2 MouseClickedPos_0;

		public Vector2 MouseClickedPos_1;

		public Vector2 MouseClickedPos_2;

		public Vector2 MouseClickedPos_3;

		public Vector2 MouseClickedPos_4;

		[FixedBuffer(typeof(double), 5)]
		public ImGuiIO.<MouseClickedTime>e__FixedBuffer MouseClickedTime;

		[FixedBuffer(typeof(byte), 5)]
		public ImGuiIO.<MouseClicked>e__FixedBuffer MouseClicked;

		[FixedBuffer(typeof(byte), 5)]
		public ImGuiIO.<MouseDoubleClicked>e__FixedBuffer MouseDoubleClicked;

		[FixedBuffer(typeof(byte), 5)]
		public ImGuiIO.<MouseReleased>e__FixedBuffer MouseReleased;

		[FixedBuffer(typeof(byte), 5)]
		public ImGuiIO.<MouseDownOwned>e__FixedBuffer MouseDownOwned;

		[FixedBuffer(typeof(byte), 5)]
		public ImGuiIO.<MouseDownWasDoubleClick>e__FixedBuffer MouseDownWasDoubleClick;

		[FixedBuffer(typeof(float), 5)]
		public ImGuiIO.<MouseDownDuration>e__FixedBuffer MouseDownDuration;

		[FixedBuffer(typeof(float), 5)]
		public ImGuiIO.<MouseDownDurationPrev>e__FixedBuffer MouseDownDurationPrev;

		public Vector2 MouseDragMaxDistanceAbs_0;

		public Vector2 MouseDragMaxDistanceAbs_1;

		public Vector2 MouseDragMaxDistanceAbs_2;

		public Vector2 MouseDragMaxDistanceAbs_3;

		public Vector2 MouseDragMaxDistanceAbs_4;

		[FixedBuffer(typeof(float), 5)]
		public ImGuiIO.<MouseDragMaxDistanceSqr>e__FixedBuffer MouseDragMaxDistanceSqr;

		[FixedBuffer(typeof(float), 512)]
		public ImGuiIO.<KeysDownDuration>e__FixedBuffer KeysDownDuration;

		[FixedBuffer(typeof(float), 512)]
		public ImGuiIO.<KeysDownDurationPrev>e__FixedBuffer KeysDownDurationPrev;

		[FixedBuffer(typeof(float), 21)]
		public ImGuiIO.<NavInputsDownDuration>e__FixedBuffer NavInputsDownDuration;

		[FixedBuffer(typeof(float), 21)]
		public ImGuiIO.<NavInputsDownDurationPrev>e__FixedBuffer NavInputsDownDurationPrev;

		public float PenPressure;

		public ushort InputQueueSurrogate;

		public ImVector InputQueueCharacters;

		[CompilerGenerated]
		[UnsafeValueType]
		[StructLayout(LayoutKind.Sequential, Size = 88)]
		public struct <KeyMap>e__FixedBuffer
		{
			public int FixedElementField;
		}

		[CompilerGenerated]
		[UnsafeValueType]
		[StructLayout(LayoutKind.Sequential, Size = 512)]
		public struct <KeysDown>e__FixedBuffer
		{
			public byte FixedElementField;
		}

		[CompilerGenerated]
		[UnsafeValueType]
		[StructLayout(LayoutKind.Sequential, Size = 2048)]
		public struct <KeysDownDuration>e__FixedBuffer
		{
			public float FixedElementField;
		}

		[CompilerGenerated]
		[UnsafeValueType]
		[StructLayout(LayoutKind.Sequential, Size = 2048)]
		public struct <KeysDownDurationPrev>e__FixedBuffer
		{
			public float FixedElementField;
		}

		[CompilerGenerated]
		[UnsafeValueType]
		[StructLayout(LayoutKind.Sequential, Size = 5)]
		public struct <MouseClicked>e__FixedBuffer
		{
			public byte FixedElementField;
		}

		[CompilerGenerated]
		[UnsafeValueType]
		[StructLayout(LayoutKind.Sequential, Size = 40)]
		public struct <MouseClickedTime>e__FixedBuffer
		{
			public double FixedElementField;
		}

		[CompilerGenerated]
		[UnsafeValueType]
		[StructLayout(LayoutKind.Sequential, Size = 5)]
		public struct <MouseDoubleClicked>e__FixedBuffer
		{
			public byte FixedElementField;
		}

		[CompilerGenerated]
		[UnsafeValueType]
		[StructLayout(LayoutKind.Sequential, Size = 5)]
		public struct <MouseDown>e__FixedBuffer
		{
			public byte FixedElementField;
		}

		[CompilerGenerated]
		[UnsafeValueType]
		[StructLayout(LayoutKind.Sequential, Size = 20)]
		public struct <MouseDownDuration>e__FixedBuffer
		{
			public float FixedElementField;
		}

		[CompilerGenerated]
		[UnsafeValueType]
		[StructLayout(LayoutKind.Sequential, Size = 20)]
		public struct <MouseDownDurationPrev>e__FixedBuffer
		{
			public float FixedElementField;
		}

		[CompilerGenerated]
		[UnsafeValueType]
		[StructLayout(LayoutKind.Sequential, Size = 5)]
		public struct <MouseDownOwned>e__FixedBuffer
		{
			public byte FixedElementField;
		}

		[CompilerGenerated]
		[UnsafeValueType]
		[StructLayout(LayoutKind.Sequential, Size = 5)]
		public struct <MouseDownWasDoubleClick>e__FixedBuffer
		{
			public byte FixedElementField;
		}

		[CompilerGenerated]
		[UnsafeValueType]
		[StructLayout(LayoutKind.Sequential, Size = 20)]
		public struct <MouseDragMaxDistanceSqr>e__FixedBuffer
		{
			public float FixedElementField;
		}

		[CompilerGenerated]
		[UnsafeValueType]
		[StructLayout(LayoutKind.Sequential, Size = 5)]
		public struct <MouseReleased>e__FixedBuffer
		{
			public byte FixedElementField;
		}

		[CompilerGenerated]
		[UnsafeValueType]
		[StructLayout(LayoutKind.Sequential, Size = 84)]
		public struct <NavInputs>e__FixedBuffer
		{
			public float FixedElementField;
		}

		[CompilerGenerated]
		[UnsafeValueType]
		[StructLayout(LayoutKind.Sequential, Size = 84)]
		public struct <NavInputsDownDuration>e__FixedBuffer
		{
			public float FixedElementField;
		}

		[CompilerGenerated]
		[UnsafeValueType]
		[StructLayout(LayoutKind.Sequential, Size = 84)]
		public struct <NavInputsDownDurationPrev>e__FixedBuffer
		{
			public float FixedElementField;
		}
	}
}
