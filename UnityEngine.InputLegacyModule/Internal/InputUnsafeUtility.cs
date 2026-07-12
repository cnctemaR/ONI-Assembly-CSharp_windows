using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine.Internal
{
	[NativeHeader("Runtime/Input/InputBindings.h")]
	internal static class InputUnsafeUtility
	{
		[NativeThrows]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern bool GetKeyString(string name);

		[NativeThrows]
		[RequiredMember]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal unsafe static extern bool GetKeyString__Unmanaged(byte* name, int nameLen);

		[NativeThrows]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern bool GetKeyUpString(string name);

		[NativeThrows]
		[RequiredMember]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal unsafe static extern bool GetKeyUpString__Unmanaged(byte* name, int nameLen);

		[NativeThrows]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern bool GetKeyDownString(string name);

		[NativeThrows]
		[RequiredMember]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal unsafe static extern bool GetKeyDownString__Unmanaged(byte* name, int nameLen);

		[NativeThrows]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern float GetAxis(string axisName);

		[NativeThrows]
		[RequiredMember]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal unsafe static extern float GetAxis__Unmanaged(byte* axisName, int axisNameLen);

		[NativeThrows]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern float GetAxisRaw(string axisName);

		[RequiredMember]
		[NativeThrows]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal unsafe static extern float GetAxisRaw__Unmanaged(byte* axisName, int axisNameLen);

		[NativeThrows]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern bool GetButton(string buttonName);

		[NativeThrows]
		[RequiredMember]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal unsafe static extern bool GetButton__Unmanaged(byte* buttonName, int buttonNameLen);

		[NativeThrows]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern bool GetButtonDown(string buttonName);

		[NativeThrows]
		[RequiredMember]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal unsafe static extern byte GetButtonDown__Unmanaged(byte* buttonName, int buttonNameLen);

		[NativeThrows]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern bool GetButtonUp(string buttonName);

		[RequiredMember]
		[NativeThrows]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal unsafe static extern bool GetButtonUp__Unmanaged(byte* buttonName, int buttonNameLen);
	}
}
