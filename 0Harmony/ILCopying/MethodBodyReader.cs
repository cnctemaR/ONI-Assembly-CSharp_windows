using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;

namespace Harmony.ILCopying
{
	public class MethodBodyReader
	{
		public static List<ILInstruction> GetInstructions(ILGenerator generator, MethodBase method)
		{
			bool flag = method == null;
			if (flag)
			{
				throw new ArgumentNullException("Method cannot be null");
			}
			MethodBodyReader methodBodyReader = new MethodBodyReader(method, generator);
			methodBodyReader.DeclareVariables(null);
			methodBodyReader.ReadInstructions();
			return methodBodyReader.ilInstructions;
		}

		public MethodBodyReader(MethodBase method, ILGenerator generator)
		{
			this.generator = generator;
			this.method = method;
			this.module = method.Module;
			MethodBody methodBody = method.GetMethodBody();
			bool flag = methodBody == null;
			if (flag)
			{
				throw new ArgumentException("Method " + method.FullDescription() + " has no body");
			}
			byte[] ilasByteArray = methodBody.GetILAsByteArray();
			bool flag2 = ilasByteArray == null;
			if (flag2)
			{
				throw new ArgumentException("Can not get IL bytes of method " + method.FullDescription());
			}
			this.ilBytes = new ByteBuffer(ilasByteArray);
			this.ilInstructions = new List<ILInstruction>((ilasByteArray.Length + 1) / 2);
			Type declaringType = method.DeclaringType;
			bool isGenericType = declaringType.IsGenericType;
			if (isGenericType)
			{
				try
				{
					this.typeArguments = declaringType.GetGenericArguments();
				}
				catch
				{
					this.typeArguments = null;
				}
			}
			bool isGenericMethod = method.IsGenericMethod;
			if (isGenericMethod)
			{
				try
				{
					this.methodArguments = method.GetGenericArguments();
				}
				catch
				{
					this.methodArguments = null;
				}
			}
			bool flag3 = !method.IsStatic;
			if (flag3)
			{
				this.this_parameter = new MethodBodyReader.ThisParameter(method);
			}
			this.parameters = method.GetParameters();
			this.locals = methodBody.LocalVariables;
			this.exceptions = methodBody.ExceptionHandlingClauses;
		}

		public void ReadInstructions()
		{
			while (this.ilBytes.position < this.ilBytes.buffer.Length)
			{
				int position = this.ilBytes.position;
				ILInstruction ilinstruction = new ILInstruction(this.ReadOpCode(), null)
				{
					offset = position
				};
				this.ReadOperand(ilinstruction);
				this.ilInstructions.Add(ilinstruction);
			}
			this.ResolveBranches();
			this.ParseExceptions();
		}

		public void DeclareVariables(LocalBuilder[] existingVariables)
		{
			bool flag = this.generator == null;
			if (!flag)
			{
				bool flag2 = existingVariables != null;
				if (flag2)
				{
					this.variables = existingVariables;
				}
				else
				{
					this.variables = this.locals.Select<LocalVariableInfo, LocalBuilder>((LocalVariableInfo lvi) => this.generator.DeclareLocal(lvi.LocalType, lvi.IsPinned)).ToArray<LocalBuilder>();
				}
			}
		}

		private void ResolveBranches()
		{
			foreach (ILInstruction ilinstruction in this.ilInstructions)
			{
				OperandType operandType = ilinstruction.opcode.OperandType;
				if (operandType == OperandType.InlineBrTarget)
				{
					goto IL_003B;
				}
				if (operandType != OperandType.InlineSwitch)
				{
					if (operandType == OperandType.ShortInlineBrTarget)
					{
						goto IL_003B;
					}
				}
				else
				{
					int[] array = (int[])ilinstruction.operand;
					ILInstruction[] array2 = new ILInstruction[array.Length];
					for (int i = 0; i < array.Length; i++)
					{
						array2[i] = this.GetInstruction(array[i], false);
					}
					ilinstruction.operand = array2;
				}
				continue;
				IL_003B:
				ilinstruction.operand = this.GetInstruction((int)ilinstruction.operand, false);
			}
		}

		private void ParseExceptions()
		{
			foreach (ExceptionHandlingClause exceptionHandlingClause in this.exceptions)
			{
				int tryOffset = exceptionHandlingClause.TryOffset;
				int num = exceptionHandlingClause.TryOffset + exceptionHandlingClause.TryLength - 1;
				int handlerOffset = exceptionHandlingClause.HandlerOffset;
				int num2 = exceptionHandlingClause.HandlerOffset + exceptionHandlingClause.HandlerLength - 1;
				ILInstruction instruction = this.GetInstruction(tryOffset, false);
				instruction.blocks.Add(new ExceptionBlock(ExceptionBlockType.BeginExceptionBlock, null));
				ILInstruction instruction2 = this.GetInstruction(num2, true);
				instruction2.blocks.Add(new ExceptionBlock(ExceptionBlockType.EndExceptionBlock, null));
				switch (exceptionHandlingClause.Flags)
				{
				case ExceptionHandlingClauseOptions.Clause:
				{
					ILInstruction instruction3 = this.GetInstruction(handlerOffset, false);
					instruction3.blocks.Add(new ExceptionBlock(ExceptionBlockType.BeginCatchBlock, exceptionHandlingClause.CatchType));
					break;
				}
				case ExceptionHandlingClauseOptions.Filter:
				{
					ILInstruction instruction4 = this.GetInstruction(exceptionHandlingClause.FilterOffset, false);
					instruction4.blocks.Add(new ExceptionBlock(ExceptionBlockType.BeginExceptFilterBlock, null));
					break;
				}
				case ExceptionHandlingClauseOptions.Finally:
				{
					ILInstruction instruction5 = this.GetInstruction(handlerOffset, false);
					instruction5.blocks.Add(new ExceptionBlock(ExceptionBlockType.BeginFinallyBlock, null));
					break;
				}
				case ExceptionHandlingClauseOptions.Fault:
				{
					ILInstruction instruction6 = this.GetInstruction(handlerOffset, false);
					instruction6.blocks.Add(new ExceptionBlock(ExceptionBlockType.BeginFaultBlock, null));
					break;
				}
				}
			}
		}

		public void FinalizeILCodes(List<MethodInfo> transpilers, List<Label> endLabels, List<ExceptionBlock> endBlocks)
		{
			bool flag = this.generator == null;
			if (!flag)
			{
				Label label;
				foreach (ILInstruction ilinstruction in this.ilInstructions)
				{
					OperandType operandType = ilinstruction.opcode.OperandType;
					if (operandType == OperandType.InlineBrTarget)
					{
						goto IL_00E7;
					}
					if (operandType != OperandType.InlineSwitch)
					{
						if (operandType == OperandType.ShortInlineBrTarget)
						{
							goto IL_00E7;
						}
					}
					else
					{
						ILInstruction[] array = ilinstruction.operand as ILInstruction[];
						bool flag2 = array != null;
						if (flag2)
						{
							List<Label> list = new List<Label>();
							foreach (ILInstruction ilinstruction2 in array)
							{
								label = this.generator.DefineLabel();
								ilinstruction2.labels.Add(label);
								list.Add(label);
							}
							ilinstruction.argument = list.ToArray();
						}
					}
					continue;
					IL_00E7:
					ILInstruction ilinstruction3 = ilinstruction.operand as ILInstruction;
					bool flag3 = ilinstruction3 != null;
					if (flag3)
					{
						Label label2 = this.generator.DefineLabel();
						ilinstruction3.labels.Add(label2);
						ilinstruction.argument = label2;
					}
				}
				CodeTranspiler codeTranspiler = new CodeTranspiler(this.ilInstructions);
				transpilers.Do<MethodInfo>(delegate(MethodInfo transpiler)
				{
					codeTranspiler.Add(transpiler);
				});
				List<CodeInstruction> result = codeTranspiler.GetResult(this.generator, this.method);
				for (;;)
				{
					CodeInstruction codeInstruction2 = result.LastOrDefault<CodeInstruction>();
					bool flag4 = codeInstruction2 == null || codeInstruction2.opcode != OpCodes.Ret;
					if (flag4)
					{
						break;
					}
					endLabels.AddRange(codeInstruction2.labels);
					result.RemoveAt(result.Count - 1);
				}
				int idx = 0;
				Action<Label> <>9__2;
				Action<ExceptionBlock> <>9__3;
				Action<ExceptionBlock> <>9__4;
				result.Do<CodeInstruction>(delegate(CodeInstruction codeInstruction)
				{
					IEnumerable<Label> labels = codeInstruction.labels;
					Action<Label> action;
					if ((action = <>9__2) == null)
					{
						action = (<>9__2 = delegate(Label label)
						{
							Emitter.MarkLabel(this.generator, label);
						});
					}
					labels.Do<Label>(action);
					IEnumerable<ExceptionBlock> blocks = codeInstruction.blocks;
					Action<ExceptionBlock> action2;
					if ((action2 = <>9__3) == null)
					{
						action2 = (<>9__3 = delegate(ExceptionBlock block)
						{
							Label? label5;
							Emitter.MarkBlockBefore(this.generator, block, out label5);
						});
					}
					blocks.Do<ExceptionBlock>(action2);
					OpCode opCode = codeInstruction.opcode;
					object obj = codeInstruction.operand;
					bool flag5 = opCode == OpCodes.Ret;
					if (flag5)
					{
						Label label3 = this.generator.DefineLabel();
						opCode = OpCodes.Br;
						obj = label3;
						endLabels.Add(label3);
					}
					OpCode opCode2;
					bool flag6 = MethodBodyReader.shortJumps.TryGetValue(opCode, out opCode2);
					if (flag6)
					{
						opCode = opCode2;
					}
					bool flag7 = true;
					bool flag8 = flag7;
					if (flag8)
					{
						OperandType operandType2 = opCode.OperandType;
						if (operandType2 != OperandType.InlineNone)
						{
							if (operandType2 != OperandType.InlineSig)
							{
								bool flag9 = obj == null;
								if (flag9)
								{
									throw new Exception("Wrong null argument: " + codeInstruction);
								}
								MethodInfo methodInfo = this.EmitMethodForType(obj.GetType());
								bool flag10 = methodInfo == null;
								if (flag10)
								{
									throw new Exception(string.Concat(new object[]
									{
										"Unknown Emit argument type ",
										obj.GetType(),
										" in ",
										codeInstruction
									}));
								}
								bool debug = HarmonyInstance.DEBUG;
								if (debug)
								{
									FileLog.LogBuffered(string.Concat(new object[]
									{
										Emitter.CodePos(this.generator),
										opCode,
										" ",
										Emitter.FormatArgument(obj)
									}));
								}
								methodInfo.Invoke(this.generator, new object[] { opCode, obj });
							}
							else
							{
								bool flag11 = obj == null;
								if (flag11)
								{
									throw new Exception("Wrong null argument: " + codeInstruction);
								}
								bool flag12 = !(obj is int);
								if (flag12)
								{
									throw new Exception(string.Concat(new object[]
									{
										"Wrong Emit argument type ",
										obj.GetType(),
										" in ",
										codeInstruction
									}));
								}
								Emitter.Emit(this.generator, opCode, (int)obj);
							}
						}
						else
						{
							Emitter.Emit(this.generator, opCode);
						}
					}
					IEnumerable<ExceptionBlock> blocks2 = codeInstruction.blocks;
					Action<ExceptionBlock> action3;
					if ((action3 = <>9__4) == null)
					{
						action3 = (<>9__4 = delegate(ExceptionBlock block)
						{
							Emitter.MarkBlockAfter(this.generator, block);
						});
					}
					blocks2.Do<ExceptionBlock>(action3);
					int idx2 = idx;
					idx = idx2 + 1;
				});
			}
		}

		private static void GetMemberInfoValue(MemberInfo info, out object result)
		{
			result = null;
			MemberTypes memberType = info.MemberType;
			if (memberType <= MemberTypes.Method)
			{
				switch (memberType)
				{
				case MemberTypes.Constructor:
					result = (ConstructorInfo)info;
					break;
				case MemberTypes.Event:
					result = (EventInfo)info;
					break;
				case MemberTypes.Constructor | MemberTypes.Event:
					break;
				case MemberTypes.Field:
					result = (FieldInfo)info;
					break;
				default:
					if (memberType == MemberTypes.Method)
					{
						result = (MethodInfo)info;
					}
					break;
				}
			}
			else if (memberType != MemberTypes.Property)
			{
				if (memberType == MemberTypes.TypeInfo || memberType == MemberTypes.NestedType)
				{
					result = (Type)info;
				}
			}
			else
			{
				result = (PropertyInfo)info;
			}
		}

		private void ReadOperand(ILInstruction instruction)
		{
			switch (instruction.opcode.OperandType)
			{
			case OperandType.InlineBrTarget:
			{
				int num = this.ilBytes.ReadInt32();
				instruction.operand = num + this.ilBytes.position;
				return;
			}
			case OperandType.InlineField:
			{
				int num2 = this.ilBytes.ReadInt32();
				instruction.operand = this.module.ResolveField(num2, this.typeArguments, this.methodArguments);
				instruction.argument = (FieldInfo)instruction.operand;
				return;
			}
			case OperandType.InlineI:
			{
				int num3 = this.ilBytes.ReadInt32();
				instruction.operand = num3;
				instruction.argument = (int)instruction.operand;
				return;
			}
			case OperandType.InlineI8:
			{
				long num4 = this.ilBytes.ReadInt64();
				instruction.operand = num4;
				instruction.argument = (long)instruction.operand;
				return;
			}
			case OperandType.InlineMethod:
			{
				int num5 = this.ilBytes.ReadInt32();
				instruction.operand = this.module.ResolveMethod(num5, this.typeArguments, this.methodArguments);
				bool flag = instruction.operand is ConstructorInfo;
				if (flag)
				{
					instruction.argument = (ConstructorInfo)instruction.operand;
				}
				else
				{
					instruction.argument = (MethodInfo)instruction.operand;
				}
				return;
			}
			case OperandType.InlineNone:
				instruction.argument = null;
				return;
			case OperandType.InlineR:
			{
				double num6 = this.ilBytes.ReadDouble();
				instruction.operand = num6;
				instruction.argument = (double)instruction.operand;
				return;
			}
			case OperandType.InlineSig:
			{
				int num7 = this.ilBytes.ReadInt32();
				byte[] array = this.module.ResolveSignature(num7);
				instruction.operand = array;
				instruction.argument = array;
				Debugger.Log(0, "TEST", "METHOD " + this.method.FullDescription() + "\n");
				Debugger.Log(0, "TEST", "Signature = " + array.Select<byte, string>((byte b) => string.Format("0x{0:x02}", b)).Aggregate<string>((string a, string b) => a + " " + b) + "\n");
				Debugger.Break();
				return;
			}
			case OperandType.InlineString:
			{
				int num8 = this.ilBytes.ReadInt32();
				instruction.operand = this.module.ResolveString(num8);
				instruction.argument = (string)instruction.operand;
				return;
			}
			case OperandType.InlineSwitch:
			{
				int num9 = this.ilBytes.ReadInt32();
				int num10 = this.ilBytes.position + 4 * num9;
				int[] array2 = new int[num9];
				for (int i = 0; i < num9; i++)
				{
					array2[i] = this.ilBytes.ReadInt32() + num10;
				}
				instruction.operand = array2;
				return;
			}
			case OperandType.InlineTok:
			{
				int num11 = this.ilBytes.ReadInt32();
				instruction.operand = this.module.ResolveMember(num11, this.typeArguments, this.methodArguments);
				MethodBodyReader.GetMemberInfoValue((MemberInfo)instruction.operand, out instruction.argument);
				return;
			}
			case OperandType.InlineType:
			{
				int num12 = this.ilBytes.ReadInt32();
				instruction.operand = this.module.ResolveType(num12, this.typeArguments, this.methodArguments);
				instruction.argument = (Type)instruction.operand;
				return;
			}
			case OperandType.InlineVar:
			{
				short num13 = this.ilBytes.ReadInt16();
				bool flag2 = MethodBodyReader.TargetsLocalVariable(instruction.opcode);
				if (flag2)
				{
					LocalVariableInfo localVariable = this.GetLocalVariable((int)num13);
					bool flag3 = localVariable == null;
					if (flag3)
					{
						instruction.argument = num13;
					}
					else
					{
						instruction.operand = localVariable;
						instruction.argument = this.variables[localVariable.LocalIndex];
					}
				}
				else
				{
					instruction.operand = this.GetParameter((int)num13);
					instruction.argument = num13;
				}
				return;
			}
			case OperandType.ShortInlineBrTarget:
			{
				sbyte b5 = (sbyte)this.ilBytes.ReadByte();
				instruction.operand = (int)b5 + this.ilBytes.position;
				return;
			}
			case OperandType.ShortInlineI:
			{
				bool flag4 = instruction.opcode == OpCodes.Ldc_I4_S;
				if (flag4)
				{
					sbyte b2 = (sbyte)this.ilBytes.ReadByte();
					instruction.operand = b2;
					instruction.argument = (sbyte)instruction.operand;
				}
				else
				{
					byte b3 = this.ilBytes.ReadByte();
					instruction.operand = b3;
					instruction.argument = (byte)instruction.operand;
				}
				return;
			}
			case OperandType.ShortInlineR:
			{
				float num14 = this.ilBytes.ReadSingle();
				instruction.operand = num14;
				instruction.argument = (float)instruction.operand;
				return;
			}
			case OperandType.ShortInlineVar:
			{
				byte b4 = this.ilBytes.ReadByte();
				bool flag5 = MethodBodyReader.TargetsLocalVariable(instruction.opcode);
				if (flag5)
				{
					LocalVariableInfo localVariable2 = this.GetLocalVariable((int)b4);
					bool flag6 = localVariable2 == null;
					if (flag6)
					{
						instruction.argument = b4;
					}
					else
					{
						instruction.operand = localVariable2;
						instruction.argument = this.variables[localVariable2.LocalIndex];
					}
				}
				else
				{
					instruction.operand = this.GetParameter((int)b4);
					instruction.argument = b4;
				}
				return;
			}
			}
			throw new NotSupportedException();
		}

		private ILInstruction GetInstruction(int offset, bool isEndOfInstruction)
		{
			int num = this.ilInstructions.Count - 1;
			bool flag = offset < 0 || offset > this.ilInstructions[num].offset;
			if (flag)
			{
				throw new Exception(string.Concat(new object[]
				{
					"Instruction offset ",
					offset,
					" is outside valid range 0 - ",
					this.ilInstructions[num].offset
				}));
			}
			int i = 0;
			int num2 = num;
			while (i <= num2)
			{
				int num3 = i + (num2 - i) / 2;
				ILInstruction ilinstruction = this.ilInstructions[num3];
				if (isEndOfInstruction)
				{
					bool flag2 = offset == ilinstruction.offset + ilinstruction.GetSize() - 1;
					if (flag2)
					{
						return ilinstruction;
					}
				}
				else
				{
					bool flag3 = offset == ilinstruction.offset;
					if (flag3)
					{
						return ilinstruction;
					}
				}
				bool flag4 = offset < ilinstruction.offset;
				if (flag4)
				{
					num2 = num3 - 1;
				}
				else
				{
					i = num3 + 1;
				}
			}
			throw new Exception("Cannot find instruction for " + offset.ToString("X4"));
		}

		private static bool TargetsLocalVariable(OpCode opcode)
		{
			return opcode.Name.Contains("loc");
		}

		private LocalVariableInfo GetLocalVariable(int index)
		{
			IList<LocalVariableInfo> list = this.locals;
			return (list != null) ? list[index] : null;
		}

		private ParameterInfo GetParameter(int index)
		{
			bool flag = index == 0;
			ParameterInfo parameterInfo;
			if (flag)
			{
				parameterInfo = this.this_parameter;
			}
			else
			{
				parameterInfo = this.parameters[index - 1];
			}
			return parameterInfo;
		}

		private OpCode ReadOpCode()
		{
			byte b = this.ilBytes.ReadByte();
			return (b != 254) ? MethodBodyReader.one_byte_opcodes[(int)b] : MethodBodyReader.two_bytes_opcodes[(int)this.ilBytes.ReadByte()];
		}

		private MethodInfo EmitMethodForType(Type type)
		{
			foreach (KeyValuePair<Type, MethodInfo> keyValuePair in MethodBodyReader.emitMethods)
			{
				bool flag = keyValuePair.Key == type;
				if (flag)
				{
					return keyValuePair.Value;
				}
			}
			foreach (KeyValuePair<Type, MethodInfo> keyValuePair2 in MethodBodyReader.emitMethods)
			{
				bool flag2 = keyValuePair2.Key.IsAssignableFrom(type);
				if (flag2)
				{
					return keyValuePair2.Value;
				}
			}
			return null;
		}

		[MethodImpl(MethodImplOptions.Synchronized)]
		static MethodBodyReader()
		{
			FieldInfo[] fields = typeof(OpCodes).GetFields(BindingFlags.Static | BindingFlags.Public);
			foreach (FieldInfo fieldInfo in fields)
			{
				OpCode opCode = (OpCode)fieldInfo.GetValue(null);
				bool flag = opCode.OpCodeType == OpCodeType.Nternal;
				if (!flag)
				{
					bool flag2 = opCode.Size == 1;
					if (flag2)
					{
						MethodBodyReader.one_byte_opcodes[(int)opCode.Value] = opCode;
					}
					else
					{
						MethodBodyReader.two_bytes_opcodes[(int)(opCode.Value & 255)] = opCode;
					}
				}
			}
			MethodBodyReader.emitMethods = new Dictionary<Type, MethodInfo>();
			typeof(ILGenerator).GetMethods().ToList<MethodInfo>().Do<MethodInfo>(delegate(MethodInfo method)
			{
				bool flag3 = method.Name != "Emit";
				if (!flag3)
				{
					ParameterInfo[] array2 = method.GetParameters();
					bool flag4 = array2.Length != 2;
					if (!flag4)
					{
						Type[] array3 = array2.Select<ParameterInfo, Type>((ParameterInfo p) => p.ParameterType).ToArray<Type>();
						bool flag5 = array3[0] != typeof(OpCode);
						if (!flag5)
						{
							MethodBodyReader.emitMethods[array3[1]] = method;
						}
					}
				}
			});
		}

		private readonly ILGenerator generator;

		private readonly MethodBase method;

		private readonly Module module;

		private readonly Type[] typeArguments;

		private readonly Type[] methodArguments;

		private readonly ByteBuffer ilBytes;

		private readonly ParameterInfo this_parameter;

		private readonly ParameterInfo[] parameters;

		private readonly IList<LocalVariableInfo> locals;

		private readonly IList<ExceptionHandlingClause> exceptions;

		private List<ILInstruction> ilInstructions;

		private LocalBuilder[] variables;

		private static Dictionary<OpCode, OpCode> shortJumps = new Dictionary<OpCode, OpCode>
		{
			{
				OpCodes.Leave_S,
				OpCodes.Leave
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
				OpCodes.Beq_S,
				OpCodes.Beq
			},
			{
				OpCodes.Bge_S,
				OpCodes.Bge
			},
			{
				OpCodes.Bgt_S,
				OpCodes.Bgt
			},
			{
				OpCodes.Ble_S,
				OpCodes.Ble
			},
			{
				OpCodes.Blt_S,
				OpCodes.Blt
			},
			{
				OpCodes.Bne_Un_S,
				OpCodes.Bne_Un
			},
			{
				OpCodes.Bge_Un_S,
				OpCodes.Bge_Un
			},
			{
				OpCodes.Bgt_Un_S,
				OpCodes.Bgt_Un
			},
			{
				OpCodes.Ble_Un_S,
				OpCodes.Ble_Un
			},
			{
				OpCodes.Br_S,
				OpCodes.Br
			},
			{
				OpCodes.Blt_Un_S,
				OpCodes.Blt_Un
			}
		};

		private static readonly OpCode[] one_byte_opcodes = new OpCode[225];

		private static readonly OpCode[] two_bytes_opcodes = new OpCode[31];

		private static readonly Dictionary<Type, MethodInfo> emitMethods;

		private class ThisParameter : ParameterInfo
		{
			public ThisParameter(MethodBase method)
			{
				this.MemberImpl = method;
				this.ClassImpl = method.DeclaringType;
				this.NameImpl = "this";
				this.PositionImpl = -1;
			}
		}
	}
}
