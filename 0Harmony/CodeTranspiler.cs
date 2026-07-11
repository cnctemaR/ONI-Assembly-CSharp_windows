using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using Harmony.ILCopying;

namespace Harmony
{
	public class CodeTranspiler
	{
		public CodeTranspiler(List<ILInstruction> ilInstructions)
		{
			this.codeInstructions = ilInstructions.Select<ILInstruction, CodeInstruction>((ILInstruction ilInstruction) => ilInstruction.GetCodeInstruction()).ToList<CodeInstruction>().AsEnumerable<CodeInstruction>();
		}

		public void Add(MethodInfo transpiler)
		{
			this.transpilers.Add(transpiler);
		}

		[UpgradeToLatestVersion(1)]
		public static object ConvertInstruction(Type type, object op, out Dictionary<string, object> unassigned)
		{
			Dictionary<string, object> nonExisting = new Dictionary<string, object>();
			object obj = AccessTools.MakeDeepCopy(op, type, delegate(string namePath, Traverse trvSrc, Traverse trvDest)
			{
				object value = trvSrc.GetValue();
				bool flag = !trvDest.FieldExists();
				object obj2;
				if (flag)
				{
					nonExisting[namePath] = value;
					obj2 = null;
				}
				else
				{
					bool flag2 = namePath == "opcode";
					if (flag2)
					{
						obj2 = CodeTranspiler.ReplaceShortJumps((OpCode)value);
					}
					else
					{
						obj2 = value;
					}
				}
				return obj2;
			}, "");
			unassigned = nonExisting;
			return obj;
		}

		public static bool ShouldAddExceptionInfo(object op, int opIndex, List<object> originalInstructions, List<object> newInstructions, Dictionary<object, Dictionary<string, object>> unassignedValues)
		{
			int num = originalInstructions.IndexOf(op);
			bool flag = num == -1;
			bool flag2;
			if (flag)
			{
				flag2 = false;
			}
			else
			{
				Dictionary<string, object> unassigned = null;
				bool flag3 = !unassignedValues.TryGetValue(op, out unassigned);
				if (flag3)
				{
					flag2 = false;
				}
				else
				{
					object blocksObject;
					bool flag4 = !unassigned.TryGetValue("blocks", out blocksObject);
					if (flag4)
					{
						flag2 = false;
					}
					else
					{
						List<ExceptionBlock> blocks = blocksObject as List<ExceptionBlock>;
						int num2 = newInstructions.Count<object>((object instr) => instr == op);
						bool flag5 = num2 <= 1;
						if (flag5)
						{
							flag2 = true;
						}
						else
						{
							ExceptionBlock exceptionBlock = blocks.FirstOrDefault<ExceptionBlock>((ExceptionBlock block) => block.blockType != ExceptionBlockType.EndExceptionBlock);
							ExceptionBlock exceptionBlock2 = blocks.FirstOrDefault<ExceptionBlock>((ExceptionBlock block) => block.blockType == ExceptionBlockType.EndExceptionBlock);
							bool flag6 = exceptionBlock != null && exceptionBlock2 == null;
							if (flag6)
							{
								object obj = originalInstructions.Skip<object>(num + 1).FirstOrDefault<object>(delegate(object instr)
								{
									bool flag12 = !unassignedValues.TryGetValue(instr, out unassigned);
									bool flag13;
									if (flag12)
									{
										flag13 = false;
									}
									else
									{
										bool flag14 = !unassigned.TryGetValue("blocks", out blocksObject);
										if (flag14)
										{
											flag13 = false;
										}
										else
										{
											blocks = blocksObject as List<ExceptionBlock>;
											flag13 = blocks.Count<ExceptionBlock>() > 0;
										}
									}
									return flag13;
								});
								bool flag7 = obj != null;
								if (flag7)
								{
									int num3 = num + 1;
									int num4 = num3 + originalInstructions.Skip<object>(num3).ToList<object>().IndexOf(obj) - 1;
									IEnumerable<object> enumerable = originalInstructions.GetRange(num3, num4 - num3).Intersect<object>(newInstructions);
									obj = newInstructions.Skip<object>(opIndex + 1).FirstOrDefault<object>(delegate(object instr)
									{
										bool flag15 = !unassignedValues.TryGetValue(instr, out unassigned);
										bool flag16;
										if (flag15)
										{
											flag16 = false;
										}
										else
										{
											bool flag17 = !unassigned.TryGetValue("blocks", out blocksObject);
											if (flag17)
											{
												flag16 = false;
											}
											else
											{
												blocks = blocksObject as List<ExceptionBlock>;
												flag16 = blocks.Count<ExceptionBlock>() > 0;
											}
										}
										return flag16;
									});
									bool flag8 = obj != null;
									if (flag8)
									{
										num3 = opIndex + 1;
										num4 = num3 + newInstructions.Skip<object>(opIndex + 1).ToList<object>().IndexOf(obj) - 1;
										List<object> range = newInstructions.GetRange(num3, num4 - num3);
										List<object> list = enumerable.Except<object>(range).ToList<object>();
										return list.Count<object>() == 0;
									}
								}
							}
							bool flag9 = exceptionBlock == null && exceptionBlock2 != null;
							if (flag9)
							{
								object obj2 = originalInstructions.GetRange(0, num).LastOrDefault<object>(delegate(object instr)
								{
									bool flag18 = !unassignedValues.TryGetValue(instr, out unassigned);
									bool flag19;
									if (flag18)
									{
										flag19 = false;
									}
									else
									{
										bool flag20 = !unassigned.TryGetValue("blocks", out blocksObject);
										if (flag20)
										{
											flag19 = false;
										}
										else
										{
											blocks = blocksObject as List<ExceptionBlock>;
											flag19 = blocks.Count<ExceptionBlock>() > 0;
										}
									}
									return flag19;
								});
								bool flag10 = obj2 != null;
								if (flag10)
								{
									int num5 = originalInstructions.GetRange(0, num).LastIndexOf(obj2);
									int num6 = num;
									IEnumerable<object> enumerable2 = originalInstructions.GetRange(num5, num6 - num5).Intersect<object>(newInstructions);
									obj2 = newInstructions.GetRange(0, opIndex).LastOrDefault<object>(delegate(object instr)
									{
										bool flag21 = !unassignedValues.TryGetValue(instr, out unassigned);
										bool flag22;
										if (flag21)
										{
											flag22 = false;
										}
										else
										{
											bool flag23 = !unassigned.TryGetValue("blocks", out blocksObject);
											if (flag23)
											{
												flag22 = false;
											}
											else
											{
												blocks = blocksObject as List<ExceptionBlock>;
												flag22 = blocks.Count<ExceptionBlock>() > 0;
											}
										}
										return flag22;
									});
									bool flag11 = obj2 != null;
									if (flag11)
									{
										num5 = newInstructions.GetRange(0, opIndex).LastIndexOf(obj2);
										List<object> range2 = newInstructions.GetRange(num5, opIndex - num5);
										IEnumerable<object> enumerable3 = enumerable2.Except<object>(range2);
										return enumerable3.Count<object>() == 0;
									}
								}
							}
							flag2 = true;
						}
					}
				}
			}
			return flag2;
		}

		public static IEnumerable ConvertInstructionsAndUnassignedValues(Type type, IEnumerable enumerable, out Dictionary<object, Dictionary<string, object>> unassignedValues)
		{
			Assembly assembly = type.GetGenericTypeDefinition().Assembly;
			Type type2 = assembly.GetType(typeof(List<>).FullName);
			Type type3 = type.GetGenericArguments()[0];
			Type type4 = assembly.GetType(type2.MakeGenericType(new Type[] { type3 }).FullName);
			object obj = Activator.CreateInstance(type4);
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

		[UpgradeToLatestVersion(1)]
		public static IEnumerable ConvertToOurInstructions(IEnumerable instructions, List<object> originalInstructions, Dictionary<object, Dictionary<string, object>> unassignedValues)
		{
			Type codeInstructionType = (from type in (from frame in new StackTrace().GetFrames()
					select frame.GetMethod()).OfType<MethodInfo>().Select<MethodInfo, Type>(delegate(MethodInfo method)
				{
					Type returnType = method.ReturnType;
					bool flag3 = !returnType.IsGenericType;
					Type type3;
					if (flag3)
					{
						type3 = null;
					}
					else
					{
						Type[] genericArguments = returnType.GetGenericArguments();
						bool flag4 = genericArguments.Length != 1;
						if (flag4)
						{
							type3 = null;
						}
						else
						{
							Type type2 = genericArguments[0];
							type3 = ((type2.FullName == typeof(CodeInstruction).FullName) ? type2 : null);
						}
					}
					return type3;
				})
				where type != null
				select type).First<Type>();
			List<object> newInstructions = instructions.Cast<object>().ToList<object>();
			int index = -1;
			foreach (object op in newInstructions)
			{
				int num = index;
				index = num + 1;
				object elementTo = AccessTools.MakeDeepCopy(op, codeInstructionType, null, "");
				Dictionary<string, object> fields;
				bool flag = unassignedValues.TryGetValue(op, out fields);
				if (flag)
				{
					bool addExceptionInfo = CodeTranspiler.ShouldAddExceptionInfo(op, index, originalInstructions, newInstructions, unassignedValues);
					Traverse trv = Traverse.Create(elementTo);
					foreach (KeyValuePair<string, object> field in fields)
					{
						bool flag2 = addExceptionInfo || field.Key != "blocks";
						if (flag2)
						{
							trv.Field(field.Key).SetValue(field.Value);
						}
						field = default(KeyValuePair<string, object>);
					}
					Dictionary<string, object>.Enumerator enumerator2 = default(Dictionary<string, object>.Enumerator);
					trv = null;
				}
				yield return elementTo;
				elementTo = null;
				fields = null;
				op = null;
			}
			List<object>.Enumerator enumerator = default(List<object>.Enumerator);
			yield break;
			yield break;
		}

		public static IEnumerable ConvertToGeneralInstructions(MethodInfo transpiler, IEnumerable enumerable, out Dictionary<object, Dictionary<string, object>> unassignedValues)
		{
			Type type = (from p in transpiler.GetParameters()
				select p.ParameterType).FirstOrDefault<Type>((Type t) => t.IsGenericType && t.GetGenericTypeDefinition().Name.StartsWith("IEnumerable"));
			return CodeTranspiler.ConvertInstructionsAndUnassignedValues(type, enumerable, out unassignedValues);
		}

		public static List<object> GetTranspilerCallParameters(ILGenerator generator, MethodInfo transpiler, MethodBase method, IEnumerable instructions)
		{
			List<object> parameter = new List<object>();
			(from param in transpiler.GetParameters()
				select param.ParameterType).Do<Type>(delegate(Type type)
			{
				bool flag = type.IsAssignableFrom(typeof(ILGenerator));
				if (flag)
				{
					parameter.Add(generator);
				}
				else
				{
					bool flag2 = type.IsAssignableFrom(typeof(MethodBase));
					if (flag2)
					{
						parameter.Add(method);
					}
					else
					{
						parameter.Add(instructions);
					}
				}
			});
			return parameter;
		}

		public List<CodeInstruction> GetResult(ILGenerator generator, MethodBase method)
		{
			IEnumerable instructions = this.codeInstructions;
			this.transpilers.ForEach(delegate(MethodInfo transpiler)
			{
				Dictionary<object, Dictionary<string, object>> dictionary;
				instructions = CodeTranspiler.ConvertToGeneralInstructions(transpiler, instructions, out dictionary);
				List<object> list = new List<object>();
				list.AddRange(instructions.Cast<object>());
				List<object> transpilerCallParameters = CodeTranspiler.GetTranspilerCallParameters(generator, transpiler, method, instructions);
				instructions = transpiler.Invoke(null, transpilerCallParameters.ToArray()) as IEnumerable;
				instructions = CodeTranspiler.ConvertToOurInstructions(instructions, list, dictionary);
			});
			return instructions.Cast<CodeInstruction>().ToList<CodeInstruction>();
		}

		private static OpCode ReplaceShortJumps(OpCode opcode)
		{
			foreach (KeyValuePair<OpCode, OpCode> keyValuePair in CodeTranspiler.allJumpCodes)
			{
				bool flag = opcode == keyValuePair.Key;
				if (flag)
				{
					return keyValuePair.Value;
				}
			}
			return opcode;
		}

		private IEnumerable<CodeInstruction> codeInstructions;

		private List<MethodInfo> transpilers = new List<MethodInfo>();

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
	}
}
