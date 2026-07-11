using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace Harmony
{
	public static class Transpilers
	{
		public static IEnumerable<CodeInstruction> MethodReplacer(this IEnumerable<CodeInstruction> instructions, MethodBase from, MethodBase to)
		{
			bool flag = from == null;
			if (flag)
			{
				throw new ArgumentException("Unexpected null argument", "from");
			}
			bool flag2 = to == null;
			if (flag2)
			{
				throw new ArgumentException("Unexpected null argument", "to");
			}
			foreach (CodeInstruction instruction in instructions)
			{
				MethodBase method = instruction.operand as MethodBase;
				bool flag3 = method == from;
				if (flag3)
				{
					instruction.opcode = (to.IsConstructor ? OpCodes.Newobj : OpCodes.Call);
					instruction.operand = to;
				}
				yield return instruction;
				method = null;
				instruction = null;
			}
			IEnumerator<CodeInstruction> enumerator = null;
			yield break;
			yield break;
		}

		public static IEnumerable<CodeInstruction> DebugLogger(this IEnumerable<CodeInstruction> instructions, string text)
		{
			yield return new CodeInstruction(OpCodes.Ldstr, text);
			yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(FileLog), "Log", null, null));
			foreach (CodeInstruction instruction in instructions)
			{
				yield return instruction;
				instruction = null;
			}
			IEnumerator<CodeInstruction> enumerator = null;
			yield break;
			yield break;
		}
	}
}
