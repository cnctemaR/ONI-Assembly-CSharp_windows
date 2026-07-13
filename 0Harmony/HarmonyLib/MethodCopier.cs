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
			this.reader.GenerateInstructions();
		}

		internal MethodCopier(MethodCreatorConfig config)
		{
			if (config.MethodBase == null)
			{
				throw new ArgumentNullException("config.methodbase");
			}
			this.reader = new MethodBodyReader(config.MethodBase, config.il);
			this.reader.DeclareVariables(config.originalVariables);
			this.reader.GenerateInstructions();
			this.reader.SetDebugging(config.debug);
		}

		internal void AddTranspiler(MethodInfo transpiler)
		{
			this.transpilers.Add(transpiler);
		}

		internal List<CodeInstruction> Finalize(bool stripLastReturn, out bool hasReturnCode, out bool methodEndsInDeadCode, List<Label> endLabels)
		{
			return this.reader.FinalizeILCodes(this.transpilers, stripLastReturn, out hasReturnCode, out methodEndsInDeadCode, endLabels);
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
			LocalBuilder[] array = MethodPatcherTools.DeclareOriginalLocalVariables(generator, method);
			MethodCopier methodCopier = new MethodCopier(method, generator, array);
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
			bool flag;
			bool flag2;
			return methodCopier.Finalize(false, out flag, out flag2, null);
		}

		private readonly MethodBodyReader reader;

		private readonly List<MethodInfo> transpilers = new List<MethodInfo>();
	}
}
