using System;
using System.Runtime.CompilerServices;

namespace HarmonyLib
{
	internal class Sandbox
	{
		[MethodImpl(MethodImplOptions.NoInlining)]
		internal Sandbox.SomeStruct_Net GetStruct_Net(IntPtr x, IntPtr y)
		{
			throw new Exception("This method should've been detoured!");
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		internal Sandbox.SomeStruct_Mono GetStruct_Mono(IntPtr x, IntPtr y)
		{
			throw new Exception("This method should've been detoured!");
		}

		internal static void GetStructReplacement_Net(Sandbox self, IntPtr ptr, IntPtr a, IntPtr b)
		{
			Sandbox.hasStructReturnBuffer_Net = a == Sandbox.magicValue && b == Sandbox.magicValue;
		}

		internal static void GetStructReplacement_Mono(Sandbox self, IntPtr ptr, IntPtr a, IntPtr b)
		{
			Sandbox.hasStructReturnBuffer_Mono = a == Sandbox.magicValue && b == Sandbox.magicValue;
		}

		internal static bool hasStructReturnBuffer_Net;

		internal static bool hasStructReturnBuffer_Mono;

		internal static readonly IntPtr magicValue = (IntPtr)305419896;

		internal struct SomeStruct_Net
		{
			private readonly byte b1;

			private readonly byte b2;

			private readonly byte b3;
		}

		internal struct SomeStruct_Mono
		{
			private readonly byte b1;

			private readonly byte b2;

			private readonly byte b3;

			private readonly byte b4;
		}
	}
}
