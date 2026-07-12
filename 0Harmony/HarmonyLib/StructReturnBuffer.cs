using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using MonoMod.Utils;

namespace HarmonyLib
{
	internal class StructReturnBuffer
	{
		private static int SizeOf(Type type)
		{
			Dictionary<Type, int> dictionary = StructReturnBuffer.sizes;
			int num;
			lock (dictionary)
			{
				int managedSize;
				if (StructReturnBuffer.sizes.TryGetValue(type, out managedSize))
				{
					num = managedSize;
				}
				else
				{
					managedSize = type.GetManagedSize();
					StructReturnBuffer.sizes.Add(type, managedSize);
					num = managedSize;
				}
			}
			return num;
		}

		internal static bool NeedsFix(MethodBase method)
		{
			Type returnedType = AccessTools.GetReturnedType(method);
			if (!AccessTools.IsStruct(returnedType))
			{
				return false;
			}
			if (!AccessTools.IsMonoRuntime && method.IsStatic)
			{
				return false;
			}
			int num = StructReturnBuffer.SizeOf(returnedType);
			return !StructReturnBuffer.specialSizes.Contains(num) && StructReturnBuffer.HasStructReturnBuffer();
		}

		private static bool HasStructReturnBuffer()
		{
			object obj;
			if (AccessTools.IsMonoRuntime)
			{
				obj = StructReturnBuffer.hasTestResult_Mono_lock;
				lock (obj)
				{
					if (!StructReturnBuffer.hasTestResult_Mono)
					{
						Sandbox.hasStructReturnBuffer_Mono = false;
						new StructReturnBuffer();
						MethodBase methodBase = AccessTools.DeclaredMethod(typeof(Sandbox), "GetStruct_Mono", null, null);
						MethodInfo methodInfo = AccessTools.DeclaredMethod(typeof(Sandbox), "GetStructReplacement_Mono", null, null);
						Memory.DetourMethod(methodBase, methodInfo);
						new Sandbox().GetStruct_Mono(Sandbox.magicValue, Sandbox.magicValue);
						StructReturnBuffer.hasTestResult_Mono = true;
					}
				}
				return Sandbox.hasStructReturnBuffer_Mono;
			}
			obj = StructReturnBuffer.hasTestResult_Net_lock;
			lock (obj)
			{
				if (!StructReturnBuffer.hasTestResult_Net)
				{
					Sandbox.hasStructReturnBuffer_Net = false;
					new StructReturnBuffer();
					MethodBase methodBase2 = AccessTools.DeclaredMethod(typeof(Sandbox), "GetStruct_Net", null, null);
					MethodInfo methodInfo2 = AccessTools.DeclaredMethod(typeof(Sandbox), "GetStructReplacement_Net", null, null);
					Memory.DetourMethod(methodBase2, methodInfo2);
					new Sandbox().GetStruct_Net(Sandbox.magicValue, Sandbox.magicValue);
					StructReturnBuffer.hasTestResult_Net = true;
				}
			}
			return Sandbox.hasStructReturnBuffer_Net;
		}

		internal static void ResetCaches()
		{
			Dictionary<Type, int> dictionary = StructReturnBuffer.sizes;
			lock (dictionary)
			{
				StructReturnBuffer.sizes.Clear();
			}
			object obj = StructReturnBuffer.hasTestResult_Mono_lock;
			lock (obj)
			{
				StructReturnBuffer.hasTestResult_Mono = false;
			}
			obj = StructReturnBuffer.hasTestResult_Net_lock;
			lock (obj)
			{
				StructReturnBuffer.hasTestResult_Net = false;
			}
		}

		internal static void ArgumentShifter(List<CodeInstruction> instructions, bool shiftArgZero)
		{
			foreach (CodeInstruction codeInstruction in instructions)
			{
				if (codeInstruction.opcode == OpCodes.Ldarg_3)
				{
					codeInstruction.opcode = OpCodes.Ldarg;
					codeInstruction.operand = 4;
				}
				else if (codeInstruction.opcode == OpCodes.Ldarg_2)
				{
					codeInstruction.opcode = OpCodes.Ldarg_3;
				}
				else if (codeInstruction.opcode == OpCodes.Ldarg_1)
				{
					codeInstruction.opcode = OpCodes.Ldarg_2;
				}
				else if (shiftArgZero && codeInstruction.opcode == OpCodes.Ldarg_0)
				{
					codeInstruction.opcode = OpCodes.Ldarg_1;
				}
				else if (codeInstruction.opcode == OpCodes.Ldarg || codeInstruction.opcode == OpCodes.Ldarg_S || codeInstruction.opcode == OpCodes.Ldarga || codeInstruction.opcode == OpCodes.Ldarga_S || codeInstruction.opcode == OpCodes.Starg || codeInstruction.opcode == OpCodes.Starg_S)
				{
					short num = Convert.ToInt16(codeInstruction.operand);
					if (num > 0 || shiftArgZero)
					{
						codeInstruction.operand = (int)(num + 1);
					}
				}
			}
		}

		private static readonly Dictionary<Type, int> sizes = new Dictionary<Type, int>();

		private static readonly HashSet<int> specialSizes = new HashSet<int> { 1, 2, 4, 8 };

		internal static bool hasTestResult_Mono;

		private static readonly object hasTestResult_Mono_lock = new object();

		internal static bool hasTestResult_Net;

		private static readonly object hasTestResult_Net_lock = new object();
	}
}
