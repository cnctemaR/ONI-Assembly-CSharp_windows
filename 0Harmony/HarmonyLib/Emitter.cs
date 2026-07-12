using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.InteropServices;
using Mono.Cecil.Cil;
using MonoMod.Utils.Cil;

namespace HarmonyLib
{
	internal class Emitter
	{
		internal Emitter(ILGenerator il, bool debug)
		{
			this.il = il.GetProxiedShim<CecilILGenerator>();
			this.debug = debug;
		}

		internal Dictionary<int, CodeInstruction> GetInstructions()
		{
			return this.instructions;
		}

		internal void AddInstruction(global::System.Reflection.Emit.OpCode opcode, object operand)
		{
			this.instructions.Add(this.CurrentPos(), new CodeInstruction(opcode, operand));
		}

		internal int CurrentPos()
		{
			return this.il.ILOffset;
		}

		internal static string CodePos(int offset)
		{
			return string.Format("IL_{0:X4}: ", offset);
		}

		internal string CodePos()
		{
			return Emitter.CodePos(this.CurrentPos());
		}

		internal void LogComment(string comment)
		{
			if (this.debug)
			{
				FileLog.LogBuffered(string.Format("{0}// {1}", this.CodePos(), comment));
			}
		}

		internal void LogIL(global::System.Reflection.Emit.OpCode opcode)
		{
			if (this.debug)
			{
				FileLog.LogBuffered(string.Format("{0}{1}", this.CodePos(), opcode));
			}
		}

		internal void LogIL(global::System.Reflection.Emit.OpCode opcode, object arg, string extra = null)
		{
			if (this.debug)
			{
				string text = Emitter.FormatArgument(arg, extra);
				string text2 = ((text.Length > 0) ? " " : "");
				string text3 = opcode.ToString();
				if (opcode.FlowControl == global::System.Reflection.Emit.FlowControl.Branch || opcode.FlowControl == global::System.Reflection.Emit.FlowControl.Cond_Branch)
				{
					text3 += " =>";
				}
				text3 = text3.PadRight(10);
				FileLog.LogBuffered(string.Format("{0}{1}{2}{3}", new object[]
				{
					this.CodePos(),
					text3,
					text2,
					text
				}));
			}
		}

		internal void LogAllLocalVariables()
		{
			if (!this.debug)
			{
				return;
			}
			this.il.IL.Body.Variables.Do<VariableDefinition>(delegate(VariableDefinition v)
			{
				FileLog.LogBuffered(string.Format("{0}Local var {1}: {2}{3}", new object[]
				{
					Emitter.CodePos(0),
					v.Index,
					v.VariableType.FullName,
					v.IsPinned ? "(pinned)" : ""
				}));
			});
		}

		internal static string FormatArgument(object argument, string extra = null)
		{
			if (argument == null)
			{
				return "NULL";
			}
			Type type = argument.GetType();
			MethodBase methodBase = argument as MethodBase;
			if (methodBase != null)
			{
				return methodBase.FullDescription() + ((extra != null) ? (" " + extra) : "");
			}
			FieldInfo fieldInfo = argument as FieldInfo;
			if (fieldInfo != null)
			{
				return string.Concat(new string[]
				{
					fieldInfo.FieldType.FullDescription(),
					" ",
					fieldInfo.DeclaringType.FullDescription(),
					"::",
					fieldInfo.Name
				});
			}
			if (type == typeof(Label))
			{
				return string.Format("Label{0}", ((Label)argument).GetHashCode());
			}
			if (type == typeof(Label[]))
			{
				return "Labels" + string.Join(",", ((Label[])argument).Select<Label, string>((Label l) => l.GetHashCode().ToString()).ToArray<string>());
			}
			if (type == typeof(LocalBuilder))
			{
				return string.Format("{0} ({1})", ((LocalBuilder)argument).LocalIndex, ((LocalBuilder)argument).LocalType);
			}
			if (type == typeof(string))
			{
				return argument.ToString().ToLiteral("\"");
			}
			return argument.ToString().Trim();
		}

		internal void MarkLabel(Label label)
		{
			if (this.debug)
			{
				FileLog.LogBuffered(this.CodePos() + Emitter.FormatArgument(label, null));
			}
			this.il.MarkLabel(label);
		}

		internal void MarkBlockBefore(ExceptionBlock block, out Label? label)
		{
			label = null;
			switch (block.blockType)
			{
			case ExceptionBlockType.BeginExceptionBlock:
				if (this.debug)
				{
					FileLog.LogBuffered(".try");
					FileLog.LogBuffered("{");
					FileLog.ChangeIndent(1);
				}
				label = new Label?(this.il.BeginExceptionBlock());
				return;
			case ExceptionBlockType.BeginCatchBlock:
				if (this.debug)
				{
					this.LogIL(global::System.Reflection.Emit.OpCodes.Leave, new LeaveTry(), null);
					FileLog.ChangeIndent(-1);
					FileLog.LogBuffered("} // end try");
					FileLog.LogBuffered(string.Format(".catch {0}", block.catchType));
					FileLog.LogBuffered("{");
					FileLog.ChangeIndent(1);
				}
				this.il.BeginCatchBlock(block.catchType);
				return;
			case ExceptionBlockType.BeginExceptFilterBlock:
				if (this.debug)
				{
					this.LogIL(global::System.Reflection.Emit.OpCodes.Leave, new LeaveTry(), null);
					FileLog.ChangeIndent(-1);
					FileLog.LogBuffered("} // end try");
					FileLog.LogBuffered(".filter");
					FileLog.LogBuffered("{");
					FileLog.ChangeIndent(1);
				}
				this.il.BeginExceptFilterBlock();
				return;
			case ExceptionBlockType.BeginFaultBlock:
				if (this.debug)
				{
					this.LogIL(global::System.Reflection.Emit.OpCodes.Leave, new LeaveTry(), null);
					FileLog.ChangeIndent(-1);
					FileLog.LogBuffered("} // end try");
					FileLog.LogBuffered(".fault");
					FileLog.LogBuffered("{");
					FileLog.ChangeIndent(1);
				}
				this.il.BeginFaultBlock();
				return;
			case ExceptionBlockType.BeginFinallyBlock:
				if (this.debug)
				{
					this.LogIL(global::System.Reflection.Emit.OpCodes.Leave, new LeaveTry(), null);
					FileLog.ChangeIndent(-1);
					FileLog.LogBuffered("} // end try");
					FileLog.LogBuffered(".finally");
					FileLog.LogBuffered("{");
					FileLog.ChangeIndent(1);
				}
				this.il.BeginFinallyBlock();
				return;
			default:
				return;
			}
		}

		internal void MarkBlockAfter(ExceptionBlock block)
		{
			if (block.blockType == ExceptionBlockType.EndExceptionBlock)
			{
				if (this.debug)
				{
					this.LogIL(global::System.Reflection.Emit.OpCodes.Leave, new LeaveTry(), null);
					FileLog.ChangeIndent(-1);
					FileLog.LogBuffered("} // end handler");
				}
				this.il.EndExceptionBlock();
			}
		}

		internal void Emit(global::System.Reflection.Emit.OpCode opcode)
		{
			this.instructions.Add(this.CurrentPos(), new CodeInstruction(opcode, null));
			this.LogIL(opcode);
			this.il.Emit(opcode);
		}

		internal void Emit(global::System.Reflection.Emit.OpCode opcode, LocalBuilder local)
		{
			this.instructions.Add(this.CurrentPos(), new CodeInstruction(opcode, local));
			this.LogIL(opcode, local, null);
			this.il.Emit(opcode, local);
		}

		internal void Emit(global::System.Reflection.Emit.OpCode opcode, FieldInfo field)
		{
			this.instructions.Add(this.CurrentPos(), new CodeInstruction(opcode, field));
			this.LogIL(opcode, field, null);
			this.il.Emit(opcode, field);
		}

		internal void Emit(global::System.Reflection.Emit.OpCode opcode, Label[] labels)
		{
			this.instructions.Add(this.CurrentPos(), new CodeInstruction(opcode, labels));
			this.LogIL(opcode, labels, null);
			this.il.Emit(opcode, labels);
		}

		internal void Emit(global::System.Reflection.Emit.OpCode opcode, Label label)
		{
			this.instructions.Add(this.CurrentPos(), new CodeInstruction(opcode, label));
			this.LogIL(opcode, label, null);
			this.il.Emit(opcode, label);
		}

		internal void Emit(global::System.Reflection.Emit.OpCode opcode, string str)
		{
			this.instructions.Add(this.CurrentPos(), new CodeInstruction(opcode, str));
			this.LogIL(opcode, str, null);
			this.il.Emit(opcode, str);
		}

		internal void Emit(global::System.Reflection.Emit.OpCode opcode, float arg)
		{
			this.instructions.Add(this.CurrentPos(), new CodeInstruction(opcode, arg));
			this.LogIL(opcode, arg, null);
			this.il.Emit(opcode, arg);
		}

		internal void Emit(global::System.Reflection.Emit.OpCode opcode, byte arg)
		{
			this.instructions.Add(this.CurrentPos(), new CodeInstruction(opcode, arg));
			this.LogIL(opcode, arg, null);
			this.il.Emit(opcode, arg);
		}

		internal void Emit(global::System.Reflection.Emit.OpCode opcode, sbyte arg)
		{
			this.instructions.Add(this.CurrentPos(), new CodeInstruction(opcode, arg));
			this.LogIL(opcode, arg, null);
			this.il.Emit(opcode, arg);
		}

		internal void Emit(global::System.Reflection.Emit.OpCode opcode, double arg)
		{
			this.instructions.Add(this.CurrentPos(), new CodeInstruction(opcode, arg));
			this.LogIL(opcode, arg, null);
			this.il.Emit(opcode, arg);
		}

		internal void Emit(global::System.Reflection.Emit.OpCode opcode, int arg)
		{
			this.instructions.Add(this.CurrentPos(), new CodeInstruction(opcode, arg));
			this.LogIL(opcode, arg, null);
			this.il.Emit(opcode, arg);
		}

		internal void Emit(global::System.Reflection.Emit.OpCode opcode, MethodInfo meth)
		{
			if (opcode.Equals(global::System.Reflection.Emit.OpCodes.Call) || opcode.Equals(global::System.Reflection.Emit.OpCodes.Callvirt) || opcode.Equals(global::System.Reflection.Emit.OpCodes.Newobj))
			{
				this.EmitCall(opcode, meth, null);
				return;
			}
			this.instructions.Add(this.CurrentPos(), new CodeInstruction(opcode, meth));
			this.LogIL(opcode, meth, null);
			this.il.Emit(opcode, meth);
		}

		internal void Emit(global::System.Reflection.Emit.OpCode opcode, short arg)
		{
			this.instructions.Add(this.CurrentPos(), new CodeInstruction(opcode, arg));
			this.LogIL(opcode, arg, null);
			this.il.Emit(opcode, arg);
		}

		internal void Emit(global::System.Reflection.Emit.OpCode opcode, SignatureHelper signature)
		{
			this.instructions.Add(this.CurrentPos(), new CodeInstruction(opcode, signature));
			this.LogIL(opcode, signature, null);
			this.il.Emit(opcode, signature);
		}

		internal void Emit(global::System.Reflection.Emit.OpCode opcode, ConstructorInfo con)
		{
			this.instructions.Add(this.CurrentPos(), new CodeInstruction(opcode, con));
			this.LogIL(opcode, con, null);
			this.il.Emit(opcode, con);
		}

		internal void Emit(global::System.Reflection.Emit.OpCode opcode, Type cls)
		{
			this.instructions.Add(this.CurrentPos(), new CodeInstruction(opcode, cls));
			this.LogIL(opcode, cls, null);
			this.il.Emit(opcode, cls);
		}

		internal void Emit(global::System.Reflection.Emit.OpCode opcode, long arg)
		{
			this.instructions.Add(this.CurrentPos(), new CodeInstruction(opcode, arg));
			this.LogIL(opcode, arg, null);
			this.il.Emit(opcode, arg);
		}

		internal void EmitCall(global::System.Reflection.Emit.OpCode opcode, MethodInfo methodInfo, Type[] optionalParameterTypes)
		{
			this.instructions.Add(this.CurrentPos(), new CodeInstruction(opcode, methodInfo));
			string text = ((optionalParameterTypes != null && optionalParameterTypes.Length != 0) ? optionalParameterTypes.Description() : null);
			this.LogIL(opcode, methodInfo, text);
			this.il.EmitCall(opcode, methodInfo, optionalParameterTypes);
		}

		internal void EmitCalli(global::System.Reflection.Emit.OpCode opcode, CallingConvention unmanagedCallConv, Type returnType, Type[] parameterTypes)
		{
			this.instructions.Add(this.CurrentPos(), new CodeInstruction(opcode, unmanagedCallConv));
			string text = returnType.FullName + " " + parameterTypes.Description();
			this.LogIL(opcode, unmanagedCallConv, text);
			this.il.EmitCalli(opcode, unmanagedCallConv, returnType, parameterTypes);
		}

		internal void EmitCalli(global::System.Reflection.Emit.OpCode opcode, CallingConventions callingConvention, Type returnType, Type[] parameterTypes, Type[] optionalParameterTypes)
		{
			this.instructions.Add(this.CurrentPos(), new CodeInstruction(opcode, callingConvention));
			string text = string.Concat(new string[]
			{
				returnType.FullName,
				" ",
				parameterTypes.Description(),
				" ",
				optionalParameterTypes.Description()
			});
			this.LogIL(opcode, callingConvention, text);
			this.il.EmitCalli(opcode, callingConvention, returnType, parameterTypes, optionalParameterTypes);
		}

		private readonly CecilILGenerator il;

		private readonly Dictionary<int, CodeInstruction> instructions = new Dictionary<int, CodeInstruction>();

		private readonly bool debug;
	}
}
