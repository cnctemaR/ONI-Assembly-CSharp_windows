using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace HarmonyLib
{
	internal class MethodCopier
	{
		internal MethodCopier(MethodBase fromMethod, ILGenerator toILGenerator, LocalBuilder[] existingVariables = null)
		{
			if (fromMethod == null)
			{
				throw new ArgumentNullException("fromMethod");
			}
			this.reader = new MethodBodyReader(fromMethod, toILGenerator);
			this.reader.DeclareVariables(existingVariables);
			this.reader.ReadInstructions();
		}

		internal void SetDebugging(bool debug)
		{
			this.reader.SetDebugging(debug);
		}

		internal void SetArgumentShift(bool useShift)
		{
			this.reader.SetArgumentShift(useShift);
		}

		internal void AddTranspiler(MethodInfo transpiler)
		{
			this.transpilers.Add(transpiler);
		}

		internal List<CodeInstruction> Finalize(Emitter emitter, List<Label> endLabels, out bool hasReturnCode)
		{
			return this.reader.FinalizeILCodes(emitter, this.transpilers, endLabels, out hasReturnCode);
		}

		internal static List<CodeInstruction> GetInstructions(ILGenerator generator, MethodBase method, int maxTranspilers)
		{
			if (generator == null)
			{
				throw new ArgumentNullException("generator");
			}
			if (method == null)
			{
				throw new ArgumentNullException("method");
			}
			LocalBuilder[] array = MethodPatcher.DeclareLocalVariables(generator, method);
			bool flag = StructReturnBuffer.NeedsFix(method);
			MethodCopier methodCopier = new MethodCopier(method, generator, array);
			methodCopier.SetArgumentShift(flag);
			Patches patchInfo = Harmony.GetPatchInfo(method);
			if (patchInfo != null)
			{
				List<MethodInfo> sortedPatchMethods = PatchFunctions.GetSortedPatchMethods(method, patchInfo.Transpilers.ToArray<Patch>(), false);
				int num = 0;
				while (num < maxTranspilers && num < sortedPatchMethods.Count)
				{
					methodCopier.AddTranspiler(sortedPatchMethods[num]);
					num++;
				}
			}
			bool flag2;
			return methodCopier.Finalize(null, null, out flag2);
		}

		private readonly MethodBodyReader reader;

		private readonly List<MethodInfo> transpilers = new List<MethodInfo>();
	}
}
