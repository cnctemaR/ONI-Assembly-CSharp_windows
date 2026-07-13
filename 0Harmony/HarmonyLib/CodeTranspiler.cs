using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;

namespace HarmonyLib
{
	internal class CodeTranspiler
	{
		internal CodeTranspiler(List<ILInstruction> ilInstructions)
		{
			this.codeInstructions = ilInstructions.Select<ILInstruction, CodeInstruction>((ILInstruction ilInstruction) => ilInstruction.GetCodeInstruction()).ToList<CodeInstruction>().AsEnumerable<CodeInstruction>();
		}

		internal void Add(MethodInfo transpiler)
		{
			this.transpilers.Add(transpiler);
		}

		internal static object ConvertInstruction(Type type, object instruction, out Dictionary<string, object> unassigned)
		{
			Dictionary<string, object> nonExisting = new Dictionary<string, object>();
			object obj = AccessTools.MakeDeepCopy(instruction, type, delegate(string namePath, Traverse trvSrc, Traverse trvDest)
			{
				object value = trvSrc.GetValue();
				if (!trvDest.FieldExists())
				{
					nonExisting[namePath] = value;
					return null;
				}
				if (namePath == "opcode")
				{
					return CodeTranspiler.ReplaceShortJumps((OpCode)value);
				}
				return value;
			}, "");
			unassigned = nonExisting;
			return obj;
		}

		internal static bool ShouldAddExceptionInfo(object op, int opIndex, List<object> originalInstructions, List<object> newInstructions, Dictionary<object, Dictionary<string, object>> unassignedValues)
		{
			int num = originalInstructions.IndexOf(op);
			if (num == -1)
			{
				return false;
			}
			Dictionary<string, object> unassigned;
			if (!unassignedValues.TryGetValue(op, out unassigned))
			{
				return false;
			}
			object blocksObject;
			if (!unassigned.TryGetValue("blocks", out blocksObject))
			{
				return false;
			}
			List<ExceptionBlock> blocks = blocksObject as List<ExceptionBlock>;
			int num2 = newInstructions.Count<object>((object instr) => instr == op);
			if (num2 <= 1)
			{
				return true;
			}
			ExceptionBlock exceptionBlock = blocks.FirstOrDefault<ExceptionBlock>((ExceptionBlock block) => block.blockType != ExceptionBlockType.EndExceptionBlock);
			ExceptionBlock exceptionBlock2 = blocks.FirstOrDefault<ExceptionBlock>((ExceptionBlock block) => block.blockType == ExceptionBlockType.EndExceptionBlock);
			if (exceptionBlock != null && exceptionBlock2 == null)
			{
				object obj = originalInstructions.Skip<object>(num + 1).FirstOrDefault<object>(delegate(object instr)
				{
					if (!unassignedValues.TryGetValue(instr, out unassigned))
					{
						return false;
					}
					if (!unassigned.TryGetValue("blocks", out blocksObject))
					{
						return false;
					}
					blocks = blocksObject as List<ExceptionBlock>;
					return blocks.Count > 0;
				});
				if (obj != null)
				{
					int num3 = num + 1;
					int num4 = num3 + originalInstructions.Skip<object>(num3).ToList<object>().IndexOf(obj) - 1;
					IEnumerable<object> enumerable = originalInstructions.GetRange(num3, num4 - num3).Intersect<object>(newInstructions);
					obj = newInstructions.Skip<object>(opIndex + 1).FirstOrDefault<object>(delegate(object instr)
					{
						if (!unassignedValues.TryGetValue(instr, out unassigned))
						{
							return false;
						}
						if (!unassigned.TryGetValue("blocks", out blocksObject))
						{
							return false;
						}
						blocks = blocksObject as List<ExceptionBlock>;
						return blocks.Count > 0;
					});
					if (obj != null)
					{
						num3 = opIndex + 1;
						num4 = num3 + newInstructions.Skip<object>(opIndex + 1).ToList<object>().IndexOf(obj) - 1;
						List<object> range = newInstructions.GetRange(num3, num4 - num3);
						List<object> list = enumerable.Except<object>(range).ToList<object>();
						return list.Count == 0;
					}
				}
			}
			if (exceptionBlock == null && exceptionBlock2 != null)
			{
				object obj2 = originalInstructions.GetRange(0, num).LastOrDefault<object>(delegate(object instr)
				{
					if (!unassignedValues.TryGetValue(instr, out unassigned))
					{
						return false;
					}
					if (!unassigned.TryGetValue("blocks", out blocksObject))
					{
						return false;
					}
					blocks = blocksObject as List<ExceptionBlock>;
					return blocks.Count > 0;
				});
				if (obj2 != null)
				{
					int num5 = originalInstructions.GetRange(0, num).LastIndexOf(obj2);
					int num6 = num;
					IEnumerable<object> enumerable2 = originalInstructions.GetRange(num5, num6 - num5).Intersect<object>(newInstructions);
					obj2 = newInstructions.GetRange(0, opIndex).LastOrDefault<object>(delegate(object instr)
					{
						if (!unassignedValues.TryGetValue(instr, out unassigned))
						{
							return false;
						}
						if (!unassigned.TryGetValue("blocks", out blocksObject))
						{
							return false;
						}
						blocks = blocksObject as List<ExceptionBlock>;
						return blocks.Count > 0;
					});
					if (obj2 != null)
					{
						num5 = newInstructions.GetRange(0, opIndex).LastIndexOf(obj2);
						List<object> range2 = newInstructions.GetRange(num5, opIndex - num5);
						IEnumerable<object> enumerable3 = enumerable2.Except<object>(range2);
						return !enumerable3.Any<object>();
					}
				}
			}
			return true;
		}

		internal static IEnumerable ConvertInstructionsAndUnassignedValues(Type type, IEnumerable enumerable, out Dictionary<object, Dictionary<string, object>> unassignedValues)
		{
			Assembly assembly = type.GetGenericTypeDefinition().Assembly;
			Type type2 = assembly.GetType(typeof(List<>).FullName);
			Type type3 = type.GetGenericArguments()[0];
			Type type4 = type2.MakeGenericType(new Type[] { type3 });
			Type type5 = assembly.GetType(type4.FullName);
			object obj = Activator.CreateInstance(type5);
			MethodInfo method = obj.GetType().GetMethod("Add");
			unassignedValues = new Dictionary<object, Dictionary<string, object>>();
			foreach (object obj2 in enumerable)
			{
				Dictionary<string, object> dictionary;
				object obj3 = CodeTranspiler.ConvertInstruction(type3, obj2, out dictionary);
				unassignedValues.Add(obj3, dictionary);
				method.Invoke(obj, new object[] { obj3 });
			}
			return obj as IEnumerable;
		}

		internal static IEnumerable ConvertToOurInstructions(IEnumerable instructions, Type codeInstructionType, List<object> originalInstructions, Dictionary<object, Dictionary<string, object>> unassignedValues)
		{
			CodeTranspiler.<ConvertToOurInstructions>d__7 <ConvertToOurInstructions>d__ = new CodeTranspiler.<ConvertToOurInstructions>d__7(-2);
			<ConvertToOurInstructions>d__.<>3__instructions = instructions;
			<ConvertToOurInstructions>d__.<>3__codeInstructionType = codeInstructionType;
			<ConvertToOurInstructions>d__.<>3__originalInstructions = originalInstructions;
			<ConvertToOurInstructions>d__.<>3__unassignedValues = unassignedValues;
			return <ConvertToOurInstructions>d__;
		}

		private static bool IsCodeInstructionsParameter(Type type)
		{
			return type.IsGenericType && type.GetGenericTypeDefinition().Name.StartsWith("IEnumerable", StringComparison.Ordinal);
		}

		internal static IEnumerable ConvertToGeneralInstructions(MethodInfo transpiler, IEnumerable enumerable, out Dictionary<object, Dictionary<string, object>> unassignedValues)
		{
			IEnumerable<Type> enumerable2 = from p in transpiler.GetParameters()
				select p.ParameterType;
			Func<Type, bool> func;
			if ((func = CodeTranspiler.<>O.<0>__IsCodeInstructionsParameter) == null)
			{
				func = (CodeTranspiler.<>O.<0>__IsCodeInstructionsParameter = new Func<Type, bool>(CodeTranspiler.IsCodeInstructionsParameter));
			}
			Type type = enumerable2.FirstOrDefault<Type>(func);
			if (type == typeof(IEnumerable<CodeInstruction>))
			{
				unassignedValues = null;
				IList<CodeInstruction> list;
				if ((list = enumerable as IList<CodeInstruction>) == null)
				{
					List<CodeInstruction> list2 = new List<CodeInstruction>();
					list2.AddRange((enumerable as IEnumerable<CodeInstruction>) ?? enumerable.Cast<CodeInstruction>());
					list = list2;
				}
				return list;
			}
			return CodeTranspiler.ConvertInstructionsAndUnassignedValues(type, enumerable, out unassignedValues);
		}

		internal static List<object> GetTranspilerCallParameters(ILGenerator generator, MethodInfo transpiler, MethodBase method, IEnumerable instructions)
		{
			List<object> parameter = new List<object>();
			(from param in transpiler.GetParameters()
				select param.ParameterType).Do<Type>(delegate(Type type)
			{
				if (type.IsAssignableFrom(typeof(ILGenerator)))
				{
					parameter.Add(generator);
					return;
				}
				if (type.IsAssignableFrom(typeof(MethodBase)))
				{
					parameter.Add(method);
					return;
				}
				if (CodeTranspiler.IsCodeInstructionsParameter(type))
				{
					parameter.Add(instructions);
				}
			});
			return parameter;
		}

		internal List<CodeInstruction> GetResult(ILGenerator generator, MethodBase method)
		{
			IEnumerable instructions = this.codeInstructions;
			this.transpilers.ForEach(delegate(MethodInfo transpiler)
			{
				Dictionary<object, Dictionary<string, object>> dictionary;
				instructions = CodeTranspiler.ConvertToGeneralInstructions(transpiler, instructions, out dictionary);
				List<object> list = null;
				if (dictionary != null)
				{
					list = instructions.Cast<object>().ToList<object>();
				}
				List<object> transpilerCallParameters = CodeTranspiler.GetTranspilerCallParameters(generator, transpiler, method, instructions);
				IEnumerable enumerable = transpiler.Invoke(null, transpilerCallParameters.ToArray()) as IEnumerable;
				if (enumerable != null)
				{
					instructions = enumerable;
				}
				if (dictionary != null)
				{
					instructions = CodeTranspiler.ConvertToOurInstructions(instructions, typeof(CodeInstruction), list, dictionary);
				}
			});
			return (instructions as List<CodeInstruction>) ?? instructions.Cast<CodeInstruction>().ToList<CodeInstruction>();
		}

		private static OpCode ReplaceShortJumps(OpCode opcode)
		{
			foreach (KeyValuePair<OpCode, OpCode> keyValuePair in CodeTranspiler.allJumpCodes)
			{
				if (opcode == keyValuePair.Key)
				{
					return keyValuePair.Value;
				}
			}
			return opcode;
		}

		private readonly IEnumerable<CodeInstruction> codeInstructions;

		private readonly List<MethodInfo> transpilers = new List<MethodInfo>();

		private static readonly Dictionary<OpCode, OpCode> allJumpCodes = new Dictionary<OpCode, OpCode>
		{
			{
				OpCodes.Beq_S,
				OpCodes.Beq
			},
			{
				OpCodes.Bge_S,
				OpCodes.Bge
			},
			{
				OpCodes.Bge_Un_S,
				OpCodes.Bge_Un
			},
			{
				OpCodes.Bgt_S,
				OpCodes.Bgt
			},
			{
				OpCodes.Bgt_Un_S,
				OpCodes.Bgt_Un
			},
			{
				OpCodes.Ble_S,
				OpCodes.Ble
			},
			{
				OpCodes.Ble_Un_S,
				OpCodes.Ble_Un
			},
			{
				OpCodes.Blt_S,
				OpCodes.Blt
			},
			{
				OpCodes.Blt_Un_S,
				OpCodes.Blt_Un
			},
			{
				OpCodes.Bne_Un_S,
				OpCodes.Bne_Un
			},
			{
				OpCodes.Brfalse_S,
				OpCodes.Brfalse
			},
			{
				OpCodes.Brtrue_S,
				OpCodes.Brtrue
			},
			{
				OpCodes.Br_S,
				OpCodes.Br
			},
			{
				OpCodes.Leave_S,
				OpCodes.Leave
			}
		};

		[CompilerGenerated]
		private static class <>O
		{
			public static Func<Type, bool> <0>__IsCodeInstructionsParameter;
		}
	}
}
