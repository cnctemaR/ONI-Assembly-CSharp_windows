using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using MonoMod.Utils;

namespace HarmonyLib
{
	public class CodeInstruction
	{
		internal CodeInstruction()
		{
		}

		internal static CodeInstruction Annotation(string annotation)
		{
			return new CodeInstruction(OpCodes.Nop, annotation);
		}

		internal string IsAnnotation()
		{
			if (!(this.opcode == OpCodes.Nop))
			{
				return null;
			}
			return this.operand as string;
		}

		public CodeInstruction(OpCode opcode, object operand = null)
		{
			this.opcode = opcode;
			this.operand = operand;
		}

		public CodeInstruction(CodeInstruction instruction)
		{
			this.opcode = instruction.opcode;
			this.operand = instruction.operand;
			this.labels = instruction.labels.ToList<Label>();
			this.blocks = instruction.blocks.ToList<ExceptionBlock>();
		}

		public CodeInstruction Clone()
		{
			return new CodeInstruction(this)
			{
				labels = new List<Label>(),
				blocks = new List<ExceptionBlock>()
			};
		}

		public CodeInstruction Clone(OpCode opcode)
		{
			CodeInstruction codeInstruction = this.Clone();
			codeInstruction.opcode = opcode;
			return codeInstruction;
		}

		public CodeInstruction Clone(object operand)
		{
			CodeInstruction codeInstruction = this.Clone();
			codeInstruction.operand = operand;
			return codeInstruction;
		}

		public static CodeInstruction Call(Type type, string name, Type[] parameters = null, Type[] generics = null)
		{
			MethodInfo methodInfo = AccessTools.Method(type, name, parameters, generics);
			if (methodInfo == null)
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(56, 4);
				defaultInterpolatedStringHandler.AppendLiteral("No method found for type=");
				defaultInterpolatedStringHandler.AppendFormatted<Type>(type);
				defaultInterpolatedStringHandler.AppendLiteral(", name=");
				defaultInterpolatedStringHandler.AppendFormatted(name);
				defaultInterpolatedStringHandler.AppendLiteral(", parameters=");
				defaultInterpolatedStringHandler.AppendFormatted(parameters.Description());
				defaultInterpolatedStringHandler.AppendLiteral(", generics=");
				defaultInterpolatedStringHandler.AppendFormatted(generics.Description());
				throw new ArgumentException(defaultInterpolatedStringHandler.ToStringAndClear());
			}
			return new CodeInstruction(OpCodes.Call, methodInfo);
		}

		public static CodeInstruction Call(string typeColonMethodname, Type[] parameters = null, Type[] generics = null)
		{
			MethodInfo methodInfo = AccessTools.Method(typeColonMethodname, parameters, generics);
			if (methodInfo == null)
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(44, 3);
				defaultInterpolatedStringHandler.AppendLiteral("No method found for ");
				defaultInterpolatedStringHandler.AppendFormatted(typeColonMethodname);
				defaultInterpolatedStringHandler.AppendLiteral(", parameters=");
				defaultInterpolatedStringHandler.AppendFormatted(parameters.Description());
				defaultInterpolatedStringHandler.AppendLiteral(", generics=");
				defaultInterpolatedStringHandler.AppendFormatted(generics.Description());
				throw new ArgumentException(defaultInterpolatedStringHandler.ToStringAndClear());
			}
			return new CodeInstruction(OpCodes.Call, methodInfo);
		}

		public static CodeInstruction Call(Expression<Action> expression)
		{
			return new CodeInstruction(OpCodes.Call, SymbolExtensions.GetMethodInfo(expression));
		}

		public static CodeInstruction Call<T>(Expression<Action<T>> expression)
		{
			return new CodeInstruction(OpCodes.Call, SymbolExtensions.GetMethodInfo<T>(expression));
		}

		public static CodeInstruction Call<T, TResult>(Expression<Func<T, TResult>> expression)
		{
			return new CodeInstruction(OpCodes.Call, SymbolExtensions.GetMethodInfo<T, TResult>(expression));
		}

		public static CodeInstruction Call(LambdaExpression expression)
		{
			return new CodeInstruction(OpCodes.Call, SymbolExtensions.GetMethodInfo(expression));
		}

		public static CodeInstruction CallClosure<T>(T closure) where T : Delegate
		{
			if (closure.Method.IsStatic && closure.Target == null)
			{
				return new CodeInstruction(OpCodes.Call, closure.Method);
			}
			Type[] array = (from x in closure.Method.GetParameters()
				select x.ParameterType).ToArray<Type>();
			DynamicMethodDefinition dynamicMethodDefinition = new DynamicMethodDefinition(closure.Method.Name, closure.Method.ReturnType, array);
			ILGenerator ilgenerator = dynamicMethodDefinition.GetILGenerator();
			Type type = closure.Target.GetType();
			bool flag;
			if (closure.Target != null)
			{
				flag = type.GetFields().Any<FieldInfo>((FieldInfo x) => !x.IsStatic);
			}
			else
			{
				flag = false;
			}
			bool flag2 = flag;
			if (flag2)
			{
				CodeInstruction.State.closureCache.Add(closure);
				ilgenerator.Emit(OpCodes.Ldsfld, AccessTools.Field(typeof(CodeInstruction.State), "closureCache"));
				ilgenerator.Emit(OpCodes.Ldc_I4, CodeInstruction.State.closureCache.Count - 1);
				ilgenerator.Emit(OpCodes.Callvirt, AccessTools.PropertyGetter(typeof(List<Delegate>), "Item"));
			}
			else
			{
				if (closure.Target == null)
				{
					ilgenerator.Emit(OpCodes.Ldnull);
				}
				else
				{
					ilgenerator.Emit(OpCodes.Newobj, AccessTools.FirstConstructor(type, (ConstructorInfo x) => !x.IsStatic && x.GetParameters().Length == 0));
				}
				ilgenerator.Emit(OpCodes.Ldftn, closure.Method);
				ilgenerator.Emit(OpCodes.Newobj, AccessTools.Constructor(typeof(T), new Type[]
				{
					typeof(object),
					typeof(IntPtr)
				}, false));
			}
			for (int i = 0; i < array.Length; i++)
			{
				ilgenerator.Emit(OpCodes.Ldarg, i);
			}
			ilgenerator.Emit(OpCodes.Callvirt, AccessTools.Method(typeof(T), "Invoke", null, null));
			ilgenerator.Emit(OpCodes.Ret);
			return new CodeInstruction(OpCodes.Call, dynamicMethodDefinition.Generate());
		}

		public static CodeInstruction LoadField(Type type, string name, bool useAddress = false)
		{
			FieldInfo fieldInfo = AccessTools.Field(type, name);
			if (fieldInfo == null)
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(24, 2);
				defaultInterpolatedStringHandler.AppendLiteral("No field found for ");
				defaultInterpolatedStringHandler.AppendFormatted<Type>(type);
				defaultInterpolatedStringHandler.AppendLiteral(" and ");
				defaultInterpolatedStringHandler.AppendFormatted(name);
				throw new ArgumentException(defaultInterpolatedStringHandler.ToStringAndClear());
			}
			return new CodeInstruction(useAddress ? (fieldInfo.IsStatic ? OpCodes.Ldsflda : OpCodes.Ldflda) : (fieldInfo.IsStatic ? OpCodes.Ldsfld : OpCodes.Ldfld), fieldInfo);
		}

		public static CodeInstruction StoreField(Type type, string name)
		{
			FieldInfo fieldInfo = AccessTools.Field(type, name);
			if (fieldInfo == null)
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(24, 2);
				defaultInterpolatedStringHandler.AppendLiteral("No field found for ");
				defaultInterpolatedStringHandler.AppendFormatted<Type>(type);
				defaultInterpolatedStringHandler.AppendLiteral(" and ");
				defaultInterpolatedStringHandler.AppendFormatted(name);
				throw new ArgumentException(defaultInterpolatedStringHandler.ToStringAndClear());
			}
			return new CodeInstruction(fieldInfo.IsStatic ? OpCodes.Stsfld : OpCodes.Stfld, fieldInfo);
		}

		public static CodeInstruction LoadLocal(int index, bool useAddress = false)
		{
			if (useAddress)
			{
				if (index < 256)
				{
					return new CodeInstruction(OpCodes.Ldloca_S, Convert.ToByte(index));
				}
				return new CodeInstruction(OpCodes.Ldloca, index);
			}
			else
			{
				if (index == 0)
				{
					return new CodeInstruction(OpCodes.Ldloc_0, null);
				}
				if (index == 1)
				{
					return new CodeInstruction(OpCodes.Ldloc_1, null);
				}
				if (index == 2)
				{
					return new CodeInstruction(OpCodes.Ldloc_2, null);
				}
				if (index == 3)
				{
					return new CodeInstruction(OpCodes.Ldloc_3, null);
				}
				if (index < 256)
				{
					return new CodeInstruction(OpCodes.Ldloc_S, Convert.ToByte(index));
				}
				return new CodeInstruction(OpCodes.Ldloc, index);
			}
		}

		public static CodeInstruction StoreLocal(int index)
		{
			if (index == 0)
			{
				return new CodeInstruction(OpCodes.Stloc_0, null);
			}
			if (index == 1)
			{
				return new CodeInstruction(OpCodes.Stloc_1, null);
			}
			if (index == 2)
			{
				return new CodeInstruction(OpCodes.Stloc_2, null);
			}
			if (index == 3)
			{
				return new CodeInstruction(OpCodes.Stloc_3, null);
			}
			if (index < 256)
			{
				return new CodeInstruction(OpCodes.Stloc_S, Convert.ToByte(index));
			}
			return new CodeInstruction(OpCodes.Stloc, index);
		}

		public static CodeInstruction LoadArgument(int index, bool useAddress = false)
		{
			if (useAddress)
			{
				if (index < 256)
				{
					return new CodeInstruction(OpCodes.Ldarga_S, Convert.ToByte(index));
				}
				return new CodeInstruction(OpCodes.Ldarga, index);
			}
			else
			{
				if (index == 0)
				{
					return new CodeInstruction(OpCodes.Ldarg_0, null);
				}
				if (index == 1)
				{
					return new CodeInstruction(OpCodes.Ldarg_1, null);
				}
				if (index == 2)
				{
					return new CodeInstruction(OpCodes.Ldarg_2, null);
				}
				if (index == 3)
				{
					return new CodeInstruction(OpCodes.Ldarg_3, null);
				}
				if (index < 256)
				{
					return new CodeInstruction(OpCodes.Ldarg_S, Convert.ToByte(index));
				}
				return new CodeInstruction(OpCodes.Ldarg, index);
			}
		}

		public static CodeInstruction StoreArgument(int index)
		{
			if (index < 256)
			{
				return new CodeInstruction(OpCodes.Starg_S, Convert.ToByte(index));
			}
			return new CodeInstruction(OpCodes.Starg, index);
		}

		public bool HasBlock(ExceptionBlockType type)
		{
			List<ExceptionBlock> list = this.blocks;
			return list != null && list.Any<ExceptionBlock>((ExceptionBlock block) => block.blockType == type);
		}

		public override string ToString()
		{
			List<string> list = new List<string>();
			foreach (Label label in this.labels)
			{
				List<string> list2 = list;
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(5, 1);
				defaultInterpolatedStringHandler.AppendLiteral("Label");
				defaultInterpolatedStringHandler.AppendFormatted<int>(label.GetHashCode());
				list2.Add(defaultInterpolatedStringHandler.ToStringAndClear());
			}
			foreach (ExceptionBlock exceptionBlock in this.blocks)
			{
				list.Add("EX_" + exceptionBlock.blockType.ToString().Replace("Block", ""));
			}
			string text = ((list.Count > 0) ? (" [" + string.Join(", ", list.ToArray()) + "]") : "");
			string text2 = Emitter.FormatOperand(this.operand);
			if (text2.Length > 0)
			{
				text2 = " " + text2;
			}
			OpCode opCode = this.opcode;
			return opCode.ToString() + text2 + text;
		}

		public OpCode opcode;

		public object operand;

		public List<Label> labels = new List<Label>();

		public List<ExceptionBlock> blocks = new List<ExceptionBlock>();

		internal static class State
		{
			internal static readonly List<Delegate> closureCache = new List<Delegate>();
		}
	}
}
