using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using Mono.Cecil.Cil;
using MonoMod.Utils;
using MonoMod.Utils.Cil;

namespace HarmonyLib
{
	internal class Emitter
	{
		internal Emitter(ILGenerator il)
		{
			this.iLGenerator = il;
			this.il = il.GetProxiedShim<CecilILGenerator>();
		}

		internal Dictionary<int, CodeInstruction> GetInstructions()
		{
			return this.instructions;
		}

		internal void AddInstruction(global::System.Reflection.Emit.OpCode opcode, object operand = null)
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

		internal IEnumerable<VariableDefinition> Variables()
		{
			return this.il.IL.Body.Variables;
		}

		internal static string FormatOperand(object argument)
		{
			if (argument == null)
			{
				return "NULL";
			}
			Type type = argument.GetType();
			MethodBase methodBase = argument as MethodBase;
			if (methodBase != null)
			{
				return methodBase.FullDescription();
			}
			FieldInfo fieldInfo = argument as FieldInfo;
			if (fieldInfo != null)
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(3, 3);
				defaultInterpolatedStringHandler.AppendFormatted(fieldInfo.FieldType.FullDescription());
				defaultInterpolatedStringHandler.AppendLiteral(" ");
				defaultInterpolatedStringHandler.AppendFormatted(fieldInfo.DeclaringType.FullDescription());
				defaultInterpolatedStringHandler.AppendLiteral("::");
				defaultInterpolatedStringHandler.AppendFormatted(fieldInfo.Name);
				return defaultInterpolatedStringHandler.ToStringAndClear();
			}
			if (type == typeof(Label))
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(5, 1);
				defaultInterpolatedStringHandler.AppendLiteral("Label");
				defaultInterpolatedStringHandler.AppendFormatted<int>(((Label)argument).GetHashCode());
				return defaultInterpolatedStringHandler.ToStringAndClear();
			}
			if (type == typeof(Label[]))
			{
				return "Labels" + string.Join(",", ((Label[])argument).Select<Label, string>((Label l) => l.GetHashCode().ToString()).ToArray<string>());
			}
			if (type == typeof(LocalBuilder))
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(3, 2);
				defaultInterpolatedStringHandler.AppendFormatted<int>(((LocalBuilder)argument).LocalIndex);
				defaultInterpolatedStringHandler.AppendLiteral(" (");
				defaultInterpolatedStringHandler.AppendFormatted<Type>(((LocalBuilder)argument).LocalType);
				defaultInterpolatedStringHandler.AppendLiteral(")");
				return defaultInterpolatedStringHandler.ToStringAndClear();
			}
			if (type == typeof(string))
			{
				return argument.ToString().ToLiteral("\"");
			}
			return argument.ToString().Trim();
		}

		internal LocalBuilder DeclareLocalVariable(Type type, bool isReturnValue = false)
		{
			if (type.IsByRef)
			{
				if (isReturnValue)
				{
					LocalBuilder localBuilder = this.il.DeclareLocal(type);
					this.Emit(global::System.Reflection.Emit.OpCodes.Ldc_I4_1);
					this.Emit(global::System.Reflection.Emit.OpCodes.Newarr, type.GetElementType());
					this.Emit(global::System.Reflection.Emit.OpCodes.Ldc_I4_0);
					this.Emit(global::System.Reflection.Emit.OpCodes.Ldelema, type.GetElementType());
					this.Emit(global::System.Reflection.Emit.OpCodes.Stloc, localBuilder);
					return localBuilder;
				}
				type = type.GetElementType();
			}
			if (type.IsEnum)
			{
				type = Enum.GetUnderlyingType(type);
			}
			if (AccessTools.IsClass(type))
			{
				LocalBuilder localBuilder2 = this.il.DeclareLocal(type);
				this.Emit(global::System.Reflection.Emit.OpCodes.Ldnull);
				this.Emit(global::System.Reflection.Emit.OpCodes.Stloc, localBuilder2);
				return localBuilder2;
			}
			if (AccessTools.IsStruct(type))
			{
				LocalBuilder localBuilder3 = this.il.DeclareLocal(type);
				this.Emit(global::System.Reflection.Emit.OpCodes.Ldloca, localBuilder3);
				this.Emit(global::System.Reflection.Emit.OpCodes.Initobj, type);
				return localBuilder3;
			}
			if (AccessTools.IsValue(type))
			{
				LocalBuilder localBuilder4 = this.il.DeclareLocal(type);
				if (type == typeof(float))
				{
					this.Emit(global::System.Reflection.Emit.OpCodes.Ldc_R4, 0f);
				}
				else if (type == typeof(double))
				{
					this.Emit(global::System.Reflection.Emit.OpCodes.Ldc_R8, 0.0);
				}
				else if (type == typeof(long) || type == typeof(ulong))
				{
					this.Emit(global::System.Reflection.Emit.OpCodes.Ldc_I8, 0L);
				}
				else
				{
					this.Emit(global::System.Reflection.Emit.OpCodes.Ldc_I4, 0);
				}
				this.Emit(global::System.Reflection.Emit.OpCodes.Stloc, localBuilder4);
				return localBuilder4;
			}
			return null;
		}

		internal void InitializeOutParameter(int argIndex, Type type)
		{
			if (type.IsByRef)
			{
				type = type.GetElementType();
			}
			this.Emit(global::System.Reflection.Emit.OpCodes.Ldarg, argIndex);
			if (AccessTools.IsStruct(type))
			{
				this.Emit(global::System.Reflection.Emit.OpCodes.Initobj, type);
				return;
			}
			if (!AccessTools.IsValue(type))
			{
				this.Emit(global::System.Reflection.Emit.OpCodes.Ldnull);
				this.Emit(global::System.Reflection.Emit.OpCodes.Stind_Ref);
				return;
			}
			if (type == typeof(float))
			{
				this.Emit(global::System.Reflection.Emit.OpCodes.Ldc_R4, 0f);
				this.Emit(global::System.Reflection.Emit.OpCodes.Stind_R4);
				return;
			}
			if (type == typeof(double))
			{
				this.Emit(global::System.Reflection.Emit.OpCodes.Ldc_R8, 0.0);
				this.Emit(global::System.Reflection.Emit.OpCodes.Stind_R8);
				return;
			}
			if (type == typeof(long))
			{
				this.Emit(global::System.Reflection.Emit.OpCodes.Ldc_I8, 0L);
				this.Emit(global::System.Reflection.Emit.OpCodes.Stind_I8);
				return;
			}
			this.Emit(global::System.Reflection.Emit.OpCodes.Ldc_I4, 0);
			this.Emit(global::System.Reflection.Emit.OpCodes.Stind_I4);
		}

		internal void PrepareArgumentArray(MethodBase original)
		{
			ParameterInfo[] parameters = original.GetParameters();
			int num = 0;
			foreach (ParameterInfo parameterInfo in parameters)
			{
				int num2 = num++ + ((!original.IsStatic) ? 1 : 0);
				if (parameterInfo.IsOut || parameterInfo.IsRetval)
				{
					this.InitializeOutParameter(num2, parameterInfo.ParameterType);
				}
			}
			this.Emit(global::System.Reflection.Emit.OpCodes.Ldc_I4, parameters.Length);
			this.Emit(global::System.Reflection.Emit.OpCodes.Newarr, typeof(object));
			num = 0;
			int num3 = 0;
			foreach (ParameterInfo parameterInfo2 in parameters)
			{
				int num4 = num++ + ((!original.IsStatic) ? 1 : 0);
				Type type = parameterInfo2.ParameterType;
				bool isByRef = type.IsByRef;
				if (isByRef)
				{
					type = type.GetElementType();
				}
				this.Emit(global::System.Reflection.Emit.OpCodes.Dup);
				this.Emit(global::System.Reflection.Emit.OpCodes.Ldc_I4, num3++);
				this.Emit(global::System.Reflection.Emit.OpCodes.Ldarg, num4);
				if (isByRef)
				{
					if (AccessTools.IsStruct(type))
					{
						this.Emit(global::System.Reflection.Emit.OpCodes.Ldobj, type);
					}
					else
					{
						this.Emit(MethodPatcherTools.LoadIndOpCodeFor(type));
					}
				}
				if (type.IsValueType)
				{
					this.Emit(global::System.Reflection.Emit.OpCodes.Box, type);
				}
				this.Emit(global::System.Reflection.Emit.OpCodes.Stelem_Ref);
			}
		}

		internal void RestoreArgumentArray(MethodBase original, LocalBuilderState localState)
		{
			ParameterInfo[] parameters = original.GetParameters();
			int num = 0;
			int num2 = 0;
			foreach (ParameterInfo parameterInfo in parameters)
			{
				int num3 = num++ + ((!original.IsStatic) ? 1 : 0);
				Type type = parameterInfo.ParameterType;
				if (type.IsByRef)
				{
					type = type.GetElementType();
					this.Emit(global::System.Reflection.Emit.OpCodes.Ldarg, num3);
					this.Emit(global::System.Reflection.Emit.OpCodes.Ldloc, localState["__args"]);
					this.Emit(global::System.Reflection.Emit.OpCodes.Ldc_I4, num2);
					this.Emit(global::System.Reflection.Emit.OpCodes.Ldelem_Ref);
					if (type.IsValueType)
					{
						this.Emit(global::System.Reflection.Emit.OpCodes.Unbox_Any, type);
						if (AccessTools.IsStruct(type))
						{
							this.Emit(global::System.Reflection.Emit.OpCodes.Stobj, type);
						}
						else
						{
							this.Emit(MethodPatcherTools.StoreIndOpCodeFor(type));
						}
					}
					else
					{
						this.Emit(global::System.Reflection.Emit.OpCodes.Castclass, type);
						this.Emit(global::System.Reflection.Emit.OpCodes.Stind_Ref);
					}
				}
				else
				{
					this.Emit(global::System.Reflection.Emit.OpCodes.Ldloc, localState["__args"]);
					this.Emit(global::System.Reflection.Emit.OpCodes.Ldc_I4, num2);
					this.Emit(global::System.Reflection.Emit.OpCodes.Ldelem_Ref);
					if (type.IsValueType)
					{
						this.Emit(global::System.Reflection.Emit.OpCodes.Unbox_Any, type);
					}
					else
					{
						this.Emit(global::System.Reflection.Emit.OpCodes.Castclass, type);
					}
					this.Emit(global::System.Reflection.Emit.OpCodes.Starg, num3);
				}
				num2++;
			}
		}

		internal void MarkLabel(Label label)
		{
			this.il.MarkLabel(label);
		}

		internal void MarkBlockBefore(ExceptionBlock block, out Label? label)
		{
			label = null;
			switch (block.blockType)
			{
			case ExceptionBlockType.BeginExceptionBlock:
				label = new Label?(this.il.BeginExceptionBlock());
				return;
			case ExceptionBlockType.BeginCatchBlock:
				this.il.BeginCatchBlock(block.catchType);
				return;
			case ExceptionBlockType.BeginExceptFilterBlock:
				this.il.BeginExceptFilterBlock();
				return;
			case ExceptionBlockType.BeginFaultBlock:
				this.il.BeginFaultBlock();
				return;
			case ExceptionBlockType.BeginFinallyBlock:
				this.il.BeginFinallyBlock();
				return;
			default:
				return;
			}
		}

		internal void MarkBlockAfter(ExceptionBlock block)
		{
			ExceptionBlockType blockType = block.blockType;
			if (blockType == ExceptionBlockType.EndExceptionBlock)
			{
				this.il.EndExceptionBlock();
			}
		}

		internal void Emit(global::System.Reflection.Emit.OpCode opcode)
		{
			this.instructions.Add(this.CurrentPos(), new CodeInstruction(opcode, null));
			this.il.Emit(opcode);
		}

		internal void Emit(global::System.Reflection.Emit.OpCode opcode, LocalBuilder local)
		{
			this.instructions.Add(this.CurrentPos(), new CodeInstruction(opcode, local));
			this.il.Emit(opcode, local);
		}

		internal void Emit(global::System.Reflection.Emit.OpCode opcode, FieldInfo field)
		{
			this.instructions.Add(this.CurrentPos(), new CodeInstruction(opcode, field));
			this.il.Emit(opcode, field);
		}

		internal void Emit(global::System.Reflection.Emit.OpCode opcode, Label[] labels)
		{
			this.instructions.Add(this.CurrentPos(), new CodeInstruction(opcode, labels));
			this.il.Emit(opcode, labels);
		}

		internal void Emit(global::System.Reflection.Emit.OpCode opcode, Label label)
		{
			this.instructions.Add(this.CurrentPos(), new CodeInstruction(opcode, label));
			this.il.Emit(opcode, label);
		}

		internal void Emit(global::System.Reflection.Emit.OpCode opcode, string str)
		{
			this.instructions.Add(this.CurrentPos(), new CodeInstruction(opcode, str));
			this.il.Emit(opcode, str);
		}

		internal void Emit(global::System.Reflection.Emit.OpCode opcode, float arg)
		{
			this.instructions.Add(this.CurrentPos(), new CodeInstruction(opcode, arg));
			this.il.Emit(opcode, arg);
		}

		internal void Emit(global::System.Reflection.Emit.OpCode opcode, byte arg)
		{
			this.instructions.Add(this.CurrentPos(), new CodeInstruction(opcode, arg));
			this.il.Emit(opcode, arg);
		}

		internal void Emit(global::System.Reflection.Emit.OpCode opcode, sbyte arg)
		{
			this.instructions.Add(this.CurrentPos(), new CodeInstruction(opcode, arg));
			this.il.Emit(opcode, arg);
		}

		internal void Emit(global::System.Reflection.Emit.OpCode opcode, double arg)
		{
			this.instructions.Add(this.CurrentPos(), new CodeInstruction(opcode, arg));
			this.il.Emit(opcode, arg);
		}

		internal void Emit(global::System.Reflection.Emit.OpCode opcode, int arg)
		{
			this.instructions.Add(this.CurrentPos(), new CodeInstruction(opcode, arg));
			this.il.Emit(opcode, arg);
		}

		internal void Emit(global::System.Reflection.Emit.OpCode opcode, MethodInfo meth)
		{
			this.instructions.Add(this.CurrentPos(), new CodeInstruction(opcode, meth));
			this.il.Emit(opcode, meth);
		}

		internal void Emit(global::System.Reflection.Emit.OpCode opcode, short arg)
		{
			this.instructions.Add(this.CurrentPos(), new CodeInstruction(opcode, arg));
			this.il.Emit(opcode, arg);
		}

		internal void Emit(global::System.Reflection.Emit.OpCode opcode, SignatureHelper signature)
		{
			this.instructions.Add(this.CurrentPos(), new CodeInstruction(opcode, signature));
			this.il.Emit(opcode, signature);
		}

		internal void Emit(global::System.Reflection.Emit.OpCode opcode, ConstructorInfo con)
		{
			this.instructions.Add(this.CurrentPos(), new CodeInstruction(opcode, con));
			this.il.Emit(opcode, con);
		}

		internal void Emit(global::System.Reflection.Emit.OpCode opcode, Type cls)
		{
			this.instructions.Add(this.CurrentPos(), new CodeInstruction(opcode, cls));
			this.il.Emit(opcode, cls);
		}

		internal void Emit(global::System.Reflection.Emit.OpCode opcode, long arg)
		{
			this.instructions.Add(this.CurrentPos(), new CodeInstruction(opcode, arg));
			this.il.Emit(opcode, arg);
		}

		internal void Emit(global::System.Reflection.Emit.OpCode opcode, ICallSiteGenerator operand)
		{
			this.il.Emit(opcode, operand);
		}

		internal void EmitCall(global::System.Reflection.Emit.OpCode opcode, MethodInfo methodInfo)
		{
			this.instructions.Add(this.CurrentPos(), new CodeInstruction(opcode, methodInfo));
			this.il.EmitCall(opcode, methodInfo, null);
		}

		internal void DynEmit(global::System.Reflection.Emit.OpCode opcode, object operand)
		{
			this.iLGenerator.DynEmit(opcode, operand);
		}

		private readonly ILGenerator iLGenerator;

		private readonly CecilILGenerator il;

		private readonly Dictionary<int, CodeInstruction> instructions = new Dictionary<int, CodeInstruction>();
	}
}
