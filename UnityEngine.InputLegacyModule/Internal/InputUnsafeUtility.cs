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
		internal unsafe static bool GetKeyString(string name)
		{
			bool keyString_Injected;
			try
			{
				ManagedSpanWrapper managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(name, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = name.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				keyString_Injected = InputUnsafeUtility.GetKeyString_Injected(ref managedSpanWrapper);
			}
			finally
			{
				char* ptr = null;
			}
			return keyString_Injected;
		}

		[NativeThrows]
		[RequiredMember]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal unsafe static extern bool GetKeyString__Unmanaged(byte* name, int nameLen);

		[NativeThrows]
		internal unsafe static bool GetKeyUpString(string name)
		{
			bool keyUpString_Injected;
			try
			{
				ManagedSpanWrapper managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(name, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = name.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				keyUpString_Injected = InputUnsafeUtility.GetKeyUpString_Injected(ref managedSpanWrapper);
			}
			finally
			{
				char* ptr = null;
			}
			return keyUpString_Injected;
		}

		[NativeThrows]
		[RequiredMember]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal unsafe static extern bool GetKeyUpString__Unmanaged(byte* name, int nameLen);

		[NativeThrows]
		internal unsafe static bool GetKeyDownString(string name)
		{
			bool keyDownString_Injected;
			try
			{
				ManagedSpanWrapper managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(name, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = name.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				keyDownString_Injected = InputUnsafeUtility.GetKeyDownString_Injected(ref managedSpanWrapper);
			}
			finally
			{
				char* ptr = null;
			}
			return keyDownString_Injected;
		}

		[NativeThrows]
		[RequiredMember]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal unsafe static extern bool GetKeyDownString__Unmanaged(byte* name, int nameLen);

		[NativeThrows]
		internal unsafe static float GetAxis(string axisName)
		{
			float axis_Injected;
			try
			{
				ManagedSpanWrapper managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(axisName, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = axisName.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				axis_Injected = InputUnsafeUtility.GetAxis_Injected(ref managedSpanWrapper);
			}
			finally
			{
				char* ptr = null;
			}
			return axis_Injected;
		}

		[NativeThrows]
		[RequiredMember]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal unsafe static extern float GetAxis__Unmanaged(byte* axisName, int axisNameLen);

		[NativeThrows]
		internal unsafe static float GetAxisRaw(string axisName)
		{
			float axisRaw_Injected;
			try
			{
				ManagedSpanWrapper managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(axisName, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = axisName.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				axisRaw_Injected = InputUnsafeUtility.GetAxisRaw_Injected(ref managedSpanWrapper);
			}
			finally
			{
				char* ptr = null;
			}
			return axisRaw_Injected;
		}

		[NativeThrows]
		[RequiredMember]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal unsafe static extern float GetAxisRaw__Unmanaged(byte* axisName, int axisNameLen);

		[NativeThrows]
		internal unsafe static bool GetButton(string buttonName)
		{
			bool button_Injected;
			try
			{
				ManagedSpanWrapper managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(buttonName, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = buttonName.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				button_Injected = InputUnsafeUtility.GetButton_Injected(ref managedSpanWrapper);
			}
			finally
			{
				char* ptr = null;
			}
			return button_Injected;
		}

		[NativeThrows]
		[RequiredMember]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal unsafe static extern bool GetButton__Unmanaged(byte* buttonName, int buttonNameLen);

		[NativeThrows]
		internal unsafe static bool GetButtonDown(string buttonName)
		{
			bool buttonDown_Injected;
			try
			{
				ManagedSpanWrapper managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(buttonName, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = buttonName.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				buttonDown_Injected = InputUnsafeUtility.GetButtonDown_Injected(ref managedSpanWrapper);
			}
			finally
			{
				char* ptr = null;
			}
			return buttonDown_Injected;
		}

		[NativeThrows]
		[RequiredMember]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal unsafe static extern byte GetButtonDown__Unmanaged(byte* buttonName, int buttonNameLen);

		[NativeThrows]
		internal unsafe static bool GetButtonUp(string buttonName)
		{
			bool buttonUp_Injected;
			try
			{
				ManagedSpanWrapper managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(buttonName, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = buttonName.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				buttonUp_Injected = InputUnsafeUtility.GetButtonUp_Injected(ref managedSpanWrapper);
			}
			finally
			{
				char* ptr = null;
			}
			return buttonUp_Injected;
		}

		[RequiredMember]
		[NativeThrows]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal unsafe static extern bool GetButtonUp__Unmanaged(byte* buttonName, int buttonNameLen);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool GetKeyString_Injected(ref ManagedSpanWrapper name);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool GetKeyUpString_Injected(ref ManagedSpanWrapper name);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool GetKeyDownString_Injected(ref ManagedSpanWrapper name);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float GetAxis_Injected(ref ManagedSpanWrapper axisName);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float GetAxisRaw_Injected(ref ManagedSpanWrapper axisName);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool GetButton_Injected(ref ManagedSpanWrapper buttonName);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool GetButtonDown_Injected(ref ManagedSpanWrapper buttonName);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool GetButtonUp_Injected(ref ManagedSpanWrapper buttonName);
	}
}
