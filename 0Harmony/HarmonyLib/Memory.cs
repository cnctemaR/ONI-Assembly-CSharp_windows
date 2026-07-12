using System;
using System.Reflection;
using System.Reflection.Emit;
using MonoMod.RuntimeDetour;
using MonoMod.Utils;

namespace HarmonyLib
{
	public static class Memory
	{
		public unsafe static void MarkForNoInlining(MethodBase method)
		{
			if (AccessTools.IsMonoRuntime)
			{
				byte* ptr = (byte*)(void*)method.MethodHandle.Value + 2;
				*(short*)ptr = (short)(*(ushort*)ptr | 8);
			}
		}

		public static string DetourMethod(MethodBase original, MethodBase replacement)
		{
			Exception ex;
			long methodStart = Memory.GetMethodStart(original, out ex);
			if (methodStart == 0L)
			{
				return ex.Message;
			}
			Memory.PadShortMethods(original);
			long methodStart2 = Memory.GetMethodStart(replacement, out ex);
			if (methodStart2 == 0L)
			{
				return ex.Message;
			}
			return Memory.WriteJump(methodStart, methodStart2);
		}

		internal static void DetourCompiledMethod(IntPtr originalCodeStart, MethodBase replacement)
		{
			Exception ex;
			long methodStart = Memory.GetMethodStart(replacement, out ex);
			if (methodStart != 0L && ex == null)
			{
				Memory.WriteJump((long)originalCodeStart, methodStart);
			}
		}

		internal static void DetourMethodAndPersist(MethodBase original, MethodBase replacement)
		{
			string text = Memory.DetourMethod(original, replacement);
			if (text != null)
			{
				throw new FormatException("Method " + original.FullDescription() + " cannot be patched. Reason: " + text);
			}
			PatchTools.RememberObject(original, replacement);
		}

		internal static void PadShortMethods(MethodBase method)
		{
			if (Memory.isWindows)
			{
				return;
			}
			MethodBody methodBody = method.GetMethodBody();
			int? num;
			if (methodBody == null)
			{
				num = null;
			}
			else
			{
				byte[] ilasByteArray = methodBody.GetILAsByteArray();
				num = ((ilasByteArray != null) ? new int?(ilasByteArray.Length) : null);
			}
			int? num2 = num;
			int valueOrDefault = num2.GetValueOrDefault();
			if (valueOrDefault == 0)
			{
				return;
			}
			if (valueOrDefault >= 16)
			{
				return;
			}
			DynamicMethodDefinition dynamicMethodDefinition = new DynamicMethodDefinition(string.Format("PadMethod-{0}", Guid.NewGuid()), typeof(void), new Type[0]);
			dynamicMethodDefinition.GetILGenerator().Emit(OpCodes.Ret);
			Exception ex;
			Memory.GetMethodStart(dynamicMethodDefinition.Generate(), out ex);
		}

		public static string WriteJump(long memory, long destination)
		{
			NativeDetourData nativeDetourData = DetourHelper.Native.Create((IntPtr)memory, (IntPtr)destination, null);
			DetourHelper.Native.MakeWritable(nativeDetourData);
			DetourHelper.Native.Apply(nativeDetourData);
			DetourHelper.Native.MakeExecutable(nativeDetourData);
			DetourHelper.Native.FlushICache(nativeDetourData);
			DetourHelper.Native.Free(nativeDetourData);
			return null;
		}

		public static long GetMethodStart(MethodBase method, out Exception exception)
		{
			long num;
			try
			{
				exception = null;
				num = method.Pin<MethodBase>().GetNativeStart().ToInt64();
			}
			catch (Exception ex)
			{
				exception = ex;
				num = 0L;
			}
			return num;
		}

		private static readonly bool isWindows = Environment.OSVersion.Platform.Equals(PlatformID.Win32NT);
	}
}
