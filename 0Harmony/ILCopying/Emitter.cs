using System;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.InteropServices;

namespace Harmony.ILCopying
{
	public static class Emitter
	{
		public static string CodePos(ILGenerator il)
		{
			int num = (int)Emitter.codeLenGetter(il);
			return string.Format("L_{0:x4}: ", num);
		}

		public static void LogIL(ILGenerator il, OpCode opCode, object argument)
		{
			bool debug = HarmonyInstance.DEBUG;
			if (debug)
			{
				string text = Emitter.FormatArgument(argument);
				string text2 = ((text.Length > 0) ? " " : "");
				FileLog.LogBuffered(string.Format("{0}{1}{2}{3}", new object[]
				{
					Emitter.CodePos(il),
					opCode,
					text2,
					text
				}));
			}
		}

		public static void LogLocalVariable(ILGenerator il, LocalBuilder variable)
		{
			bool debug = HarmonyInstance.DEBUG;
			if (debug)
			{
				LocalBuilder[] array = ((Emitter.localsGetter != null) ? ((LocalBuilder[])Emitter.localsGetter(il)) : null);
				bool flag = array != null && array.Length != 0;
				int num;
				if (flag)
				{
					num = array.Length;
				}
				else
				{
					num = (int)Emitter.localCountGetter(il);
				}
				string text = string.Format("{0}Local var {1}: {2}{3}", new object[]
				{
					Emitter.CodePos(il),
					num - 1,
					variable.LocalType.FullName,
					variable.IsPinned ? "(pinned)" : ""
				});
				FileLog.LogBuffered(text);
			}
		}

		public static string FormatArgument(object argument)
		{
			bool flag = argument == null;
			string text;
			if (flag)
			{
				text = "NULL";
			}
			else
			{
				Type type = argument.GetType();
				bool flag2 = type == typeof(string);
				if (flag2)
				{
					text = "\"" + argument + "\"";
				}
				else
				{
					bool flag3 = type == typeof(Label);
					if (flag3)
					{
						text = "Label" + ((Label)argument).GetHashCode();
					}
					else
					{
						bool flag4 = type == typeof(Label[]);
						if (flag4)
						{
							text = "Labels" + string.Join(",", ((Label[])argument).Select<Label, string>((Label l) => l.GetHashCode().ToString()).ToArray<string>());
						}
						else
						{
							bool flag5 = type == typeof(LocalBuilder);
							if (flag5)
							{
								text = string.Concat(new object[]
								{
									((LocalBuilder)argument).LocalIndex,
									" (",
									((LocalBuilder)argument).LocalType,
									")"
								});
							}
							else
							{
								text = argument.ToString().Trim();
							}
						}
					}
				}
			}
			return text;
		}

		public static void MarkLabel(ILGenerator il, Label label)
		{
			bool debug = HarmonyInstance.DEBUG;
			if (debug)
			{
				FileLog.LogBuffered(Emitter.CodePos(il) + Emitter.FormatArgument(label));
			}
			il.MarkLabel(label);
		}

		public static void MarkBlockBefore(ILGenerator il, ExceptionBlock block, out Label? label)
		{
			label = null;
			switch (block.blockType)
			{
			case ExceptionBlockType.BeginExceptionBlock:
			{
				bool debug = HarmonyInstance.DEBUG;
				if (debug)
				{
					FileLog.LogBuffered(".try");
					FileLog.LogBuffered("{");
					FileLog.ChangeIndent(1);
				}
				label = new Label?(il.BeginExceptionBlock());
				break;
			}
			case ExceptionBlockType.BeginCatchBlock:
			{
				bool debug2 = HarmonyInstance.DEBUG;
				if (debug2)
				{
					Emitter.LogIL(il, OpCodes.Leave, new LeaveTry());
					FileLog.ChangeIndent(-1);
					FileLog.LogBuffered("} // end try");
					FileLog.LogBuffered(".catch " + block.catchType);
					FileLog.LogBuffered("{");
					FileLog.ChangeIndent(1);
				}
				il.BeginCatchBlock(block.catchType);
				break;
			}
			case ExceptionBlockType.BeginExceptFilterBlock:
			{
				bool debug3 = HarmonyInstance.DEBUG;
				if (debug3)
				{
					Emitter.LogIL(il, OpCodes.Leave, new LeaveTry());
					FileLog.ChangeIndent(-1);
					FileLog.LogBuffered("} // end try");
					FileLog.LogBuffered(".filter");
					FileLog.LogBuffered("{");
					FileLog.ChangeIndent(1);
				}
				il.BeginExceptFilterBlock();
				break;
			}
			case ExceptionBlockType.BeginFaultBlock:
			{
				bool debug4 = HarmonyInstance.DEBUG;
				if (debug4)
				{
					Emitter.LogIL(il, OpCodes.Leave, new LeaveTry());
					FileLog.ChangeIndent(-1);
					FileLog.LogBuffered("} // end try");
					FileLog.LogBuffered(".fault");
					FileLog.LogBuffered("{");
					FileLog.ChangeIndent(1);
				}
				il.BeginFaultBlock();
				break;
			}
			case ExceptionBlockType.BeginFinallyBlock:
			{
				bool debug5 = HarmonyInstance.DEBUG;
				if (debug5)
				{
					Emitter.LogIL(il, OpCodes.Leave, new LeaveTry());
					FileLog.ChangeIndent(-1);
					FileLog.LogBuffered("} // end try");
					FileLog.LogBuffered(".finally");
					FileLog.LogBuffered("{");
					FileLog.ChangeIndent(1);
				}
				il.BeginFinallyBlock();
				break;
			}
			}
		}

		public static void MarkBlockAfter(ILGenerator il, ExceptionBlock block)
		{
			bool flag = block.blockType == ExceptionBlockType.EndExceptionBlock;
			if (flag)
			{
				bool debug = HarmonyInstance.DEBUG;
				if (debug)
				{
					Emitter.LogIL(il, OpCodes.Leave, new LeaveTry());
					FileLog.ChangeIndent(-1);
					FileLog.LogBuffered("} // end handler");
				}
				il.EndExceptionBlock();
			}
		}

		public static void Emit(ILGenerator il, OpCode opcode)
		{
			bool debug = HarmonyInstance.DEBUG;
			if (debug)
			{
				FileLog.LogBuffered(Emitter.CodePos(il) + opcode);
			}
			il.Emit(opcode);
		}

		public static void Emit(ILGenerator il, OpCode opcode, LocalBuilder local)
		{
			Emitter.LogIL(il, opcode, local);
			il.Emit(opcode, local);
		}

		public static void Emit(ILGenerator il, OpCode opcode, FieldInfo field)
		{
			Emitter.LogIL(il, opcode, field);
			il.Emit(opcode, field);
		}

		public static void Emit(ILGenerator il, OpCode opcode, Label[] labels)
		{
			Emitter.LogIL(il, opcode, labels);
			il.Emit(opcode, labels);
		}

		public static void Emit(ILGenerator il, OpCode opcode, Label label)
		{
			Emitter.LogIL(il, opcode, label);
			il.Emit(opcode, label);
		}

		public static void Emit(ILGenerator il, OpCode opcode, string str)
		{
			Emitter.LogIL(il, opcode, str);
			il.Emit(opcode, str);
		}

		public static void Emit(ILGenerator il, OpCode opcode, float arg)
		{
			Emitter.LogIL(il, opcode, arg);
			il.Emit(opcode, arg);
		}

		public static void Emit(ILGenerator il, OpCode opcode, byte arg)
		{
			Emitter.LogIL(il, opcode, arg);
			il.Emit(opcode, arg);
		}

		public static void Emit(ILGenerator il, OpCode opcode, sbyte arg)
		{
			Emitter.LogIL(il, opcode, arg);
			il.Emit(opcode, arg);
		}

		public static void Emit(ILGenerator il, OpCode opcode, double arg)
		{
			Emitter.LogIL(il, opcode, arg);
			il.Emit(opcode, arg);
		}

		public static void Emit(ILGenerator il, OpCode opcode, int arg)
		{
			Emitter.LogIL(il, opcode, arg);
			il.Emit(opcode, arg);
		}

		public static void Emit(ILGenerator il, OpCode opcode, MethodInfo meth)
		{
			Emitter.LogIL(il, opcode, meth);
			il.Emit(opcode, meth);
		}

		public static void Emit(ILGenerator il, OpCode opcode, short arg)
		{
			Emitter.LogIL(il, opcode, arg);
			il.Emit(opcode, arg);
		}

		public static void Emit(ILGenerator il, OpCode opcode, SignatureHelper signature)
		{
			Emitter.LogIL(il, opcode, signature);
			il.Emit(opcode, signature);
		}

		public static void Emit(ILGenerator il, OpCode opcode, ConstructorInfo con)
		{
			Emitter.LogIL(il, opcode, con);
			il.Emit(opcode, con);
		}

		public static void Emit(ILGenerator il, OpCode opcode, Type cls)
		{
			Emitter.LogIL(il, opcode, cls);
			il.Emit(opcode, cls);
		}

		public static void Emit(ILGenerator il, OpCode opcode, long arg)
		{
			Emitter.LogIL(il, opcode, arg);
			il.Emit(opcode, arg);
		}

		public static void EmitCall(ILGenerator il, OpCode opcode, MethodInfo methodInfo, Type[] optionalParameterTypes)
		{
			bool debug = HarmonyInstance.DEBUG;
			if (debug)
			{
				FileLog.LogBuffered(string.Format("{0}Call {1} {2} {3}", new object[]
				{
					Emitter.CodePos(il),
					opcode,
					methodInfo,
					optionalParameterTypes
				}));
			}
			il.EmitCall(opcode, methodInfo, optionalParameterTypes);
		}

		public static void EmitCalli(ILGenerator il, OpCode opcode, CallingConvention unmanagedCallConv, Type returnType, Type[] parameterTypes)
		{
			bool debug = HarmonyInstance.DEBUG;
			if (debug)
			{
				FileLog.LogBuffered(string.Format("{0}Calli {1} {2} {3} {4}", new object[]
				{
					Emitter.CodePos(il),
					opcode,
					unmanagedCallConv,
					returnType,
					parameterTypes
				}));
			}
			il.EmitCalli(opcode, unmanagedCallConv, returnType, parameterTypes);
		}

		public static void EmitCalli(ILGenerator il, OpCode opcode, CallingConventions callingConvention, Type returnType, Type[] parameterTypes, Type[] optionalParameterTypes)
		{
			bool debug = HarmonyInstance.DEBUG;
			if (debug)
			{
				FileLog.LogBuffered(string.Format("{0}Calli {1} {2} {3} {4} {5}", new object[]
				{
					Emitter.CodePos(il),
					opcode,
					callingConvention,
					returnType,
					parameterTypes,
					optionalParameterTypes
				}));
			}
			il.EmitCalli(opcode, callingConvention, returnType, parameterTypes, optionalParameterTypes);
		}

		private static readonly GetterHandler codeLenGetter = FastAccess.CreateFieldGetter(typeof(ILGenerator), new string[] { "code_len", "m_length" });

		private static readonly GetterHandler localsGetter = FastAccess.CreateFieldGetter(typeof(ILGenerator), new string[] { "locals" });

		private static readonly GetterHandler localCountGetter = FastAccess.CreateFieldGetter(typeof(ILGenerator), new string[] { "m_localCount" });
	}
}
