using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine
{
	[NativeType("Modules/Marshalling/MarshallingTests.h")]
	internal class InvokeTests
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool TestInvokeBool(bool arg);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern sbyte TestInvokeSByte(sbyte arg);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern byte TestInvokeByte(byte arg);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern char TestInvokeChar(char arg);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern short TestInvokeShort(short arg);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern ushort TestInvokeUShort(ushort arg);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern int TestInvokeInt(int arg);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern uint TestInvokeUInt(uint arg);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern long TestInvokeLong(long arg);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern ulong TestInvokeULong(ulong arg);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern IntPtr TestInvokeIntPtr(IntPtr arg);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern UIntPtr TestInvokeUIntPtr(UIntPtr arg);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern float TestInvokeFloat(float arg);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern double TestInvokeDouble(double arg);

		[RequiredByNativeCode(Optional = true)]
		[RequiredMember]
		private static bool InvokeBool(bool arg)
		{
			return arg;
		}

		[RequiredByNativeCode(Optional = true)]
		[RequiredMember]
		private static sbyte InvokeSByte(sbyte arg)
		{
			return arg;
		}

		[RequiredMember]
		[RequiredByNativeCode(Optional = true)]
		private static byte InvokeByte(byte arg)
		{
			return arg;
		}

		[RequiredMember]
		[RequiredByNativeCode(Optional = true)]
		private static char InvokeChar(char arg)
		{
			return arg;
		}

		[RequiredMember]
		[RequiredByNativeCode(Optional = true)]
		private static short InvokeShort(short arg)
		{
			return arg;
		}

		[RequiredMember]
		[RequiredByNativeCode(Optional = true)]
		private static ushort InvokeUShort(ushort arg)
		{
			return arg;
		}

		[RequiredMember]
		[RequiredByNativeCode(Optional = true)]
		private static int InvokeInt(int arg)
		{
			return arg;
		}

		[RequiredMember]
		[RequiredByNativeCode(Optional = true)]
		private static uint InvokeUInt(uint arg)
		{
			return arg;
		}

		[RequiredMember]
		[RequiredByNativeCode(Optional = true)]
		private static long InvokeLong(long arg)
		{
			return arg;
		}

		[RequiredMember]
		[RequiredByNativeCode(Optional = true)]
		private static ulong InvokeULong(ulong arg)
		{
			return arg;
		}

		[RequiredMember]
		[RequiredByNativeCode(Optional = true)]
		private static IntPtr InvokeIntPtr(IntPtr arg)
		{
			return arg;
		}

		[RequiredMember]
		[RequiredByNativeCode(Optional = true)]
		private static UIntPtr InvokeUIntPtr(UIntPtr arg)
		{
			return arg;
		}

		[RequiredMember]
		[RequiredByNativeCode(Optional = true)]
		private static float InvokeFloat(float arg)
		{
			return arg;
		}

		[RequiredMember]
		[RequiredByNativeCode(Optional = true)]
		private static double InvokeDouble(double arg)
		{
			return arg;
		}
	}
}
