using System;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace HarmonyLib
{
	internal class FaultBlockRewriter
	{
		private static int FindMatchingBeginException(List<CodeInstruction> rewritten)
		{
			int i = rewritten.Count - 1;
			int num = 0;
			while (i >= 0)
			{
				if (rewritten[i].HasBlock(ExceptionBlockType.EndExceptionBlock))
				{
					num++;
				}
				if (rewritten[i].HasBlock(ExceptionBlockType.BeginExceptionBlock))
				{
					if (num == 0)
					{
						return i;
					}
					num--;
				}
				i--;
			}
			return -1;
		}

		private static int FindMatchingEndException(List<CodeInstruction> source, int start)
		{
			int i = start;
			int num = 0;
			while (i < source.Count)
			{
				if (source[i].HasBlock(ExceptionBlockType.BeginExceptionBlock))
				{
					num++;
				}
				if (source[i].HasBlock(ExceptionBlockType.EndExceptionBlock))
				{
					if (num == 0)
					{
						return i;
					}
					num--;
				}
				i++;
			}
			return -1;
		}

		private static CodeInstruction CloneWithoutFaultMarker(CodeInstruction original)
		{
			CodeInstruction codeInstruction = new CodeInstruction(original);
			codeInstruction.blocks.RemoveAll((ExceptionBlock b) => b.blockType == ExceptionBlockType.BeginFaultBlock);
			return codeInstruction;
		}

		internal static List<CodeInstruction> Rewrite(List<CodeInstruction> instructions, ILGenerator generator)
		{
			if (instructions == null)
			{
				throw new ArgumentNullException("instructions");
			}
			if (generator == null)
			{
				throw new ArgumentNullException("generator");
			}
			int i = 0;
			List<CodeInstruction> list = new List<CodeInstruction>(instructions.Count * 2);
			while (i < instructions.Count)
			{
				CodeInstruction codeInstruction = instructions[i];
				if (!codeInstruction.HasBlock(ExceptionBlockType.BeginFaultBlock))
				{
					list.Add(new CodeInstruction(codeInstruction));
					i++;
				}
				else
				{
					int num = FaultBlockRewriter.FindMatchingBeginException(list);
					int num2 = FaultBlockRewriter.FindMatchingEndException(instructions, i + 1);
					if (num < 0 || num2 < 0)
					{
						throw new InvalidOperationException("Unbalanced exception markers – cannot rewrite.");
					}
					List<CodeInstruction> list2 = new List<CodeInstruction>();
					for (int j = i; j < num2; j++)
					{
						list2.Add(FaultBlockRewriter.CloneWithoutFaultMarker(instructions[j]));
					}
					i = num2 + 1;
					LocalBuilder localBuilder = generator.DeclareLocal(typeof(bool));
					Label label = generator.DefineLabel();
					list.AddRange(new <>z__ReadOnlyArray<CodeInstruction>(new CodeInstruction[]
					{
						Code.Nop.WithBlocks(new ExceptionBlock[]
						{
							new ExceptionBlock(ExceptionBlockType.BeginCatchBlock, typeof(object))
						}),
						Code.Pop,
						Code.Ldc_I4_1,
						Code.Stloc[localBuilder.LocalIndex, null],
						Code.Rethrow,
						Code.Nop.WithBlocks(new ExceptionBlock[]
						{
							new ExceptionBlock(ExceptionBlockType.BeginFinallyBlock, null)
						}),
						Code.Ldloc[localBuilder.LocalIndex, null],
						Code.Brfalse_S[label, null],
						Code.Nop.WithLabels(new Label[] { label }),
						Code.Nop.WithBlocks(new ExceptionBlock[]
						{
							new ExceptionBlock(ExceptionBlockType.EndExceptionBlock, null)
						})
					}));
				}
			}
			return list;
		}
	}
}
