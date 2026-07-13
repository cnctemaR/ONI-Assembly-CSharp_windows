using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using MonoMod.Logs;

namespace MonoMod.Utils
{
	[NullableContext(1)]
	[Nullable(0)]
	internal static class FastReflectionHelper
	{
		[NullableContext(2)]
		private static object FastInvokerForStructInvokerVT<[Nullable(0)] T>([Nullable(1)] FastReflectionHelper.FastStructInvoker invoker, object target, params object[] args) where T : struct
		{
			object obj = default(T);
			invoker(target, obj, args);
			return obj;
		}

		[NullableContext(2)]
		private static object FastInvokerForStructInvokerNullable<[Nullable(0)] T>([Nullable(1)] FastReflectionHelper.FastStructInvoker invoker, object target, params object[] args) where T : struct
		{
			StrongBox<T?> strongBox;
			if ((strongBox = FastReflectionHelper.TypedCache<T>.NullableStrongBox) == null)
			{
				strongBox = (FastReflectionHelper.TypedCache<T>.NullableStrongBox = new StrongBox<T?>(null));
			}
			StrongBox<T?> strongBox2 = strongBox;
			invoker(target, strongBox2, args);
			return strongBox2.Value;
		}

		[NullableContext(2)]
		private static object FastInvokerForStructInvokerClass([Nullable(1)] FastReflectionHelper.FastStructInvoker invoker, object target, params object[] args)
		{
			WeakBox weakBox;
			if ((weakBox = FastReflectionHelper.CachedWeakBox) == null)
			{
				weakBox = (FastReflectionHelper.CachedWeakBox = new WeakBox());
			}
			WeakBox weakBox2 = weakBox;
			invoker(target, weakBox2, args);
			return weakBox2.Value;
		}

		[NullableContext(2)]
		private static object FastInvokerForStructInvokerVoid([Nullable(1)] FastReflectionHelper.FastStructInvoker invoker, object target, params object[] args)
		{
			invoker(target, null, args);
			return null;
		}

		private static FastReflectionHelper.FastInvoker CreateFastInvoker(FastReflectionHelper.FastStructInvoker fsi, FastReflectionHelper.ReturnTypeClass retTypeClass, Type returnType)
		{
			switch (retTypeClass)
			{
			case FastReflectionHelper.ReturnTypeClass.Void:
				return FastReflectionHelper.S2FVoid.CreateDelegate<FastReflectionHelper.FastInvoker>(fsi);
			case FastReflectionHelper.ReturnTypeClass.ValueType:
				return FastReflectionHelper.S2FValueType.MakeGenericMethod(new Type[] { returnType }).CreateDelegate<FastReflectionHelper.FastInvoker>(fsi);
			case FastReflectionHelper.ReturnTypeClass.Nullable:
				return FastReflectionHelper.S2FNullable.MakeGenericMethod(new Type[] { Nullable.GetUnderlyingType(returnType) }).CreateDelegate<FastReflectionHelper.FastInvoker>(fsi);
			case FastReflectionHelper.ReturnTypeClass.ReferenceType:
				return FastReflectionHelper.S2FClass.CreateDelegate<FastReflectionHelper.FastInvoker>(fsi);
			default:
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(24, 1);
				defaultInterpolatedStringHandler.AppendLiteral("Invalid ReturnTypeClass ");
				defaultInterpolatedStringHandler.AppendFormatted<FastReflectionHelper.ReturnTypeClass>(retTypeClass);
				throw new NotImplementedException(defaultInterpolatedStringHandler.ToStringAndClear());
			}
			}
		}

		private static FastReflectionHelper.FSITuple GetFSITuple(MethodBase method)
		{
			return FastReflectionHelper.fastStructInvokers.GetValue(method, delegate(MemberInfo _)
			{
				FastReflectionHelper.ReturnTypeClass returnTypeClass;
				Type type;
				return new FastReflectionHelper.FSITuple(FastReflectionHelper.CreateMethodInvoker(method, out returnTypeClass, out type), returnTypeClass, type);
			});
		}

		private static FastReflectionHelper.FSITuple GetFSITuple(FieldInfo field)
		{
			return FastReflectionHelper.fastStructInvokers.GetValue(field, delegate(MemberInfo _)
			{
				FastReflectionHelper.ReturnTypeClass returnTypeClass;
				Type type;
				return new FastReflectionHelper.FSITuple(FastReflectionHelper.CreateFieldInvoker(field, out returnTypeClass, out type), returnTypeClass, type);
			});
		}

		private static FastReflectionHelper.FSITuple GetFSITuple(MemberInfo member)
		{
			MethodBase methodBase = member as MethodBase;
			FastReflectionHelper.FSITuple fsituple;
			if (methodBase == null)
			{
				FieldInfo fieldInfo = member as FieldInfo;
				if (fieldInfo == null)
				{
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(29, 1);
					defaultInterpolatedStringHandler.AppendLiteral("Member type ");
					defaultInterpolatedStringHandler.AppendFormatted<Type>(member.GetType());
					defaultInterpolatedStringHandler.AppendLiteral(" is not supported");
					throw new NotSupportedException(defaultInterpolatedStringHandler.ToStringAndClear());
				}
				fsituple = FastReflectionHelper.GetFSITuple(fieldInfo);
			}
			else
			{
				fsituple = FastReflectionHelper.GetFSITuple(methodBase);
			}
			return fsituple;
		}

		private static FastReflectionHelper.FastInvoker GetFastInvoker(FastReflectionHelper.FSITuple tuple)
		{
			return FastReflectionHelper.fastInvokers.GetValue(tuple, (FastReflectionHelper.FSITuple t) => FastReflectionHelper.CreateFastInvoker(t.FSI, t.RTC, t.ReturnType));
		}

		public static FastReflectionHelper.FastStructInvoker GetFastStructInvoker(MethodBase method)
		{
			return FastReflectionHelper.GetFSITuple(method).FSI;
		}

		public static FastReflectionHelper.FastStructInvoker GetFastStructInvoker(FieldInfo field)
		{
			return FastReflectionHelper.GetFSITuple(field).FSI;
		}

		public static FastReflectionHelper.FastStructInvoker GetFastStructInvoker(MemberInfo member)
		{
			return FastReflectionHelper.GetFSITuple(member).FSI;
		}

		public static FastReflectionHelper.FastInvoker GetFastInvoker(this MethodBase method)
		{
			return FastReflectionHelper.GetFastInvoker(FastReflectionHelper.GetFSITuple(method));
		}

		public static FastReflectionHelper.FastInvoker GetFastInvoker(this FieldInfo field)
		{
			return FastReflectionHelper.GetFastInvoker(FastReflectionHelper.GetFSITuple(field));
		}

		public static FastReflectionHelper.FastInvoker GetFastInvoker(this MemberInfo member)
		{
			return FastReflectionHelper.GetFastInvoker(FastReflectionHelper.GetFSITuple(member));
		}

		[NullableContext(2)]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static void CheckArgs(bool isStatic, object target, int retTypeClass, object result, int expectLen, object[] args)
		{
			if (!isStatic)
			{
				Helpers.ThrowIfArgumentNull<object>(target, "target");
			}
			if (retTypeClass != 0 && retTypeClass - 1 <= 2)
			{
				Helpers.ThrowIfArgumentNull<object>(result, "result");
			}
			if (expectLen == 0)
			{
				return;
			}
			Helpers.ThrowIfArgumentNull<object[]>(args, "args");
			if (args.Length < expectLen)
			{
				FastReflectionHelper.<CheckArgs>g__ThrowArgumentOutOfRange|28_0();
			}
		}

		[NullableContext(2)]
		[return: Nullable(1)]
		private static Exception BadArgException(int arg, RuntimeTypeHandle expectType, object target, object result, [Nullable(new byte[] { 1, 2 })] object[] args)
		{
			Type typeFromHandle = Type.GetTypeFromHandle(expectType);
			Type type;
			if (arg != -2)
			{
				if (arg == -1)
				{
					type = ((target != null) ? target.GetType() : null);
				}
				else
				{
					object obj = args[arg];
					type = ((obj != null) ? obj.GetType() : null);
				}
			}
			else
			{
				type = ((result != null) ? result.GetType() : null);
			}
			Type type2 = type;
			string text;
			if (arg != -2)
			{
				if (arg == -1)
				{
					text = "target";
				}
				else
				{
					FormatInterpolatedStringHandler formatInterpolatedStringHandler = new FormatInterpolatedStringHandler(6, 1);
					formatInterpolatedStringHandler.AppendLiteral("args[");
					formatInterpolatedStringHandler.AppendFormatted<int>(arg);
					formatInterpolatedStringHandler.AppendLiteral("]");
					text = DebugFormatter.Format(ref formatInterpolatedStringHandler);
				}
			}
			else
			{
				text = "result";
			}
			string text2 = text;
			if (type2 == null)
			{
				return new ArgumentNullException(text2);
			}
			if (arg != -2)
			{
				if (arg == -1)
				{
					FormatInterpolatedStringHandler formatInterpolatedStringHandler2 = new FormatInterpolatedStringHandler(48, 2);
					formatInterpolatedStringHandler2.AppendLiteral("Target object is the wrong type; expected ");
					formatInterpolatedStringHandler2.AppendFormatted<Type>(typeFromHandle);
					formatInterpolatedStringHandler2.AppendLiteral(", got ");
					formatInterpolatedStringHandler2.AppendFormatted<Type>(type2);
					text = DebugFormatter.Format(ref formatInterpolatedStringHandler2);
				}
				else
				{
					FormatInterpolatedStringHandler formatInterpolatedStringHandler3 = new FormatInterpolatedStringHandler(44, 3);
					formatInterpolatedStringHandler3.AppendLiteral("Argument ");
					formatInterpolatedStringHandler3.AppendFormatted<int>(arg);
					formatInterpolatedStringHandler3.AppendLiteral(" is the wrong type; expected ");
					formatInterpolatedStringHandler3.AppendFormatted<Type>(typeFromHandle);
					formatInterpolatedStringHandler3.AppendLiteral(", got ");
					formatInterpolatedStringHandler3.AppendFormatted<Type>(type2);
					text = DebugFormatter.Format(ref formatInterpolatedStringHandler3);
				}
			}
			else
			{
				FormatInterpolatedStringHandler formatInterpolatedStringHandler4 = new FormatInterpolatedStringHandler(48, 2);
				formatInterpolatedStringHandler4.AppendLiteral("Result object is the wrong type; expected ");
				formatInterpolatedStringHandler4.AppendFormatted<Type>(typeFromHandle);
				formatInterpolatedStringHandler4.AppendLiteral(", got ");
				formatInterpolatedStringHandler4.AppendFormatted<Type>(type2);
				text = DebugFormatter.Format(ref formatInterpolatedStringHandler4);
			}
			return new ArgumentException(text, text2);
		}

		private static FastReflectionHelper.ReturnTypeClass ClassifyType(Type returnType)
		{
			if (returnType == typeof(void))
			{
				return FastReflectionHelper.ReturnTypeClass.Void;
			}
			if (!returnType.IsValueType)
			{
				return FastReflectionHelper.ReturnTypeClass.ReferenceType;
			}
			if (Nullable.GetUnderlyingType(returnType) != null)
			{
				return FastReflectionHelper.ReturnTypeClass.Nullable;
			}
			return FastReflectionHelper.ReturnTypeClass.ValueType;
		}

		private static void EmitCheckArgs(ILCursor il, bool isStatic, FastReflectionHelper.ReturnTypeClass rtc, int expectParams)
		{
			il.Emit(OpCodes.Ldc_I4, (isStatic > false) ? 1 : 0);
			il.Emit(OpCodes.Ldarg_0);
			il.Emit(OpCodes.Ldc_I4, (int)rtc);
			il.Emit(OpCodes.Ldarg_1);
			il.Emit(OpCodes.Ldc_I4, expectParams);
			il.Emit(OpCodes.Ldarg_2);
			il.Emit(OpCodes.Call, FastReflectionHelper.CheckArgsMethod);
		}

		private static void EmitCheckType(ILCursor il, int argId, Type expectType, ILLabel badArgLbl)
		{
			ILLabel illabel = il.DefineLabel();
			bool isByRef = expectType.IsByRef;
			VariableDefinition variableDefinition = null;
			if (isByRef)
			{
				expectType = expectType.GetElementType() ?? expectType;
				FastReflectionHelper.ReturnTypeClass returnTypeClass = FastReflectionHelper.ClassifyType(expectType);
				if (!expectType.IsValueType)
				{
					variableDefinition = new VariableDefinition(il.Module.TypeSystem.Object);
					il.Context.Body.Variables.Add(variableDefinition);
					il.Emit(OpCodes.Stloc, variableDefinition);
					il.Emit(OpCodes.Ldloc, variableDefinition);
				}
				FastReflectionHelper.EmitCheckByref(il, returnTypeClass, expectType, badArgLbl, argId);
				if (expectType.IsValueType)
				{
					return;
				}
				if (variableDefinition != null)
				{
					il.Emit(OpCodes.Ldloc, variableDefinition);
				}
				FastReflectionHelper.EmitLoadByref(il, returnTypeClass, expectType);
				il.Emit(OpCodes.Ldind_Ref);
			}
			if (expectType != typeof(object))
			{
				il.Emit(OpCodes.Isinst, expectType);
			}
			il.Emit(OpCodes.Brtrue, illabel);
			il.Emit(OpCodes.Ldc_I4, argId);
			il.Emit(OpCodes.Ldtoken, expectType);
			il.Emit(OpCodes.Br, badArgLbl);
			il.MarkLabel(illabel);
		}

		private static void EmitCheckAllowNull(ILCursor il, int argId, Type expectType, ILLabel badArgLbl)
		{
			ILLabel illabel = il.DefineLabel();
			bool isByRef = expectType.IsByRef;
			VariableDefinition variableDefinition = null;
			if (isByRef)
			{
				expectType = expectType.GetElementType() ?? expectType;
				FastReflectionHelper.ReturnTypeClass returnTypeClass = FastReflectionHelper.ClassifyType(expectType);
				if (!expectType.IsValueType)
				{
					variableDefinition = new VariableDefinition(il.Module.TypeSystem.Object);
					il.Context.Body.Variables.Add(variableDefinition);
					il.Emit(OpCodes.Stloc, variableDefinition);
					il.Emit(OpCodes.Ldloc, variableDefinition);
				}
				FastReflectionHelper.EmitCheckByref(il, returnTypeClass, expectType, badArgLbl, argId);
				if (expectType.IsValueType)
				{
					return;
				}
				if (variableDefinition != null)
				{
					il.Emit(OpCodes.Ldloc, variableDefinition);
				}
				FastReflectionHelper.EmitLoadByref(il, returnTypeClass, expectType);
				il.Emit(OpCodes.Ldind_Ref);
			}
			if (expectType == typeof(object))
			{
				il.Emit(OpCodes.Pop);
				return;
			}
			if (!expectType.IsValueType || Nullable.GetUnderlyingType(expectType) != null)
			{
				ILLabel illabel2 = il.DefineLabel();
				VariableDefinition variableDefinition2 = new VariableDefinition(il.Module.TypeSystem.Object);
				il.Context.Body.Variables.Add(variableDefinition2);
				il.Emit(OpCodes.Stloc, variableDefinition2);
				il.Emit(OpCodes.Ldloc, variableDefinition2);
				il.Emit(OpCodes.Brtrue, illabel2);
				il.Emit(OpCodes.Br, illabel);
				il.MarkLabel(illabel2);
				il.Emit(OpCodes.Ldloc, variableDefinition2);
			}
			if (!expectType.IsValueType || (!isByRef && expectType.IsValueType))
			{
				FastReflectionHelper.EmitCheckType(il, argId, expectType, badArgLbl);
			}
			il.MarkLabel(illabel);
		}

		private static void EmitBadArgCall(ILCursor il, ILLabel badArgLbl)
		{
			il.MarkLabel(badArgLbl);
			il.Emit(OpCodes.Ldarg_0);
			il.Emit(OpCodes.Ldarg_1);
			il.Emit(OpCodes.Ldarg_2);
			il.Emit(OpCodes.Call, FastReflectionHelper.BadArgExceptionMethod);
			il.Emit(OpCodes.Throw);
		}

		private static void EmitCheckByref(ILCursor il, FastReflectionHelper.ReturnTypeClass rtc, Type returnType, ILLabel badArgLbl, int argId = -2)
		{
			Type type;
			switch (rtc)
			{
			case FastReflectionHelper.ReturnTypeClass.Void:
				return;
			case FastReflectionHelper.ReturnTypeClass.ValueType:
				type = returnType;
				break;
			case FastReflectionHelper.ReturnTypeClass.Nullable:
				type = typeof(StrongBox<>).MakeGenericType(new Type[] { returnType });
				break;
			case FastReflectionHelper.ReturnTypeClass.ReferenceType:
				type = typeof(WeakBox);
				break;
			default:
				return;
			}
			FastReflectionHelper.EmitCheckType(il, argId, type, badArgLbl);
		}

		private static void EmitLoadByref(ILCursor il, FastReflectionHelper.ReturnTypeClass rtc, Type returnType)
		{
			switch (rtc)
			{
			case FastReflectionHelper.ReturnTypeClass.Void:
				break;
			case FastReflectionHelper.ReturnTypeClass.ValueType:
				il.Emit(OpCodes.Unbox, returnType);
				return;
			case FastReflectionHelper.ReturnTypeClass.Nullable:
			{
				FieldInfo field = typeof(StrongBox<>).MakeGenericType(new Type[] { returnType }).GetField("Value");
				il.Emit(OpCodes.Ldflda, field);
				return;
			}
			case FastReflectionHelper.ReturnTypeClass.ReferenceType:
				il.Emit(OpCodes.Ldflda, FastReflectionHelper.WeakBoxValueField);
				break;
			default:
				return;
			}
		}

		private static void EmitLoadArgO(ILCursor il, int arg)
		{
			il.Emit(OpCodes.Ldarg_2);
			il.Emit(OpCodes.Ldc_I4, arg);
			il.Emit(OpCodes.Ldelem_Ref);
		}

		private static void EmitStoreByref(ILCursor il, FastReflectionHelper.ReturnTypeClass rtc, Type returnType)
		{
			if (rtc != FastReflectionHelper.ReturnTypeClass.Void)
			{
				if (returnType.IsValueType)
				{
					il.Emit(OpCodes.Stobj, returnType);
					return;
				}
				il.Emit(OpCodes.Stind_Ref);
			}
		}

		private static FastReflectionHelper.FastStructInvoker CreateMethodInvoker(MethodBase method, out FastReflectionHelper.ReturnTypeClass retTypeClass, out Type retType)
		{
			FastReflectionHelper.<>c__DisplayClass44_0 CS$<>8__locals1 = new FastReflectionHelper.<>c__DisplayClass44_0();
			CS$<>8__locals1.method = method;
			if (!CS$<>8__locals1.method.IsStatic)
			{
				Type declaringType = CS$<>8__locals1.method.DeclaringType;
				if (declaringType != null && declaringType.IsByRefLike())
				{
					throw new ArgumentException("Cannot create reflection invoker for instance method on byref-like type", "method");
				}
			}
			FastReflectionHelper.<>c__DisplayClass44_0 CS$<>8__locals2 = CS$<>8__locals1;
			MethodInfo methodInfo = CS$<>8__locals1.method as MethodInfo;
			CS$<>8__locals2.returnType = ((methodInfo != null) ? methodInfo.ReturnType : CS$<>8__locals1.method.DeclaringType);
			retType = CS$<>8__locals1.returnType;
			if (CS$<>8__locals1.returnType.IsByRef || CS$<>8__locals1.returnType.IsByRefLike())
			{
				throw new ArgumentException("Cannot create reflection invoker for method with byref or byref-like return type", "method");
			}
			retTypeClass = FastReflectionHelper.ClassifyType(CS$<>8__locals1.returnType);
			CS$<>8__locals1.typeClass = retTypeClass;
			CS$<>8__locals1.methParams = CS$<>8__locals1.method.GetParameters();
			FormatInterpolatedStringHandler formatInterpolatedStringHandler = new FormatInterpolatedStringHandler(22, 1);
			formatInterpolatedStringHandler.AppendLiteral("MM:FastStructInvoker<");
			formatInterpolatedStringHandler.AppendFormatted<MethodBase>(CS$<>8__locals1.method);
			formatInterpolatedStringHandler.AppendLiteral(">");
			FastReflectionHelper.FastStructInvoker fastStructInvoker;
			using (DynamicMethodDefinition dynamicMethodDefinition = new DynamicMethodDefinition(DebugFormatter.Format(ref formatInterpolatedStringHandler), null, FastReflectionHelper.FastStructInvokerArgs))
			{
				using (ILContext ilcontext = new ILContext(dynamicMethodDefinition.Definition))
				{
					ilcontext.Invoke(delegate(ILContext ilc)
					{
						ILCursor ilcursor = new ILCursor(ilc);
						FastReflectionHelper.EmitCheckArgs(ilcursor, CS$<>8__locals1.method.IsStatic || CS$<>8__locals1.method is ConstructorInfo, CS$<>8__locals1.typeClass, CS$<>8__locals1.methParams.Length);
						ILLabel illabel = ilcursor.DefineLabel();
						if (!CS$<>8__locals1.method.IsStatic && !(CS$<>8__locals1.method is ConstructorInfo))
						{
							Type declaringType2 = CS$<>8__locals1.method.DeclaringType;
							Helpers.Assert(declaringType2 != null, null, "expectType is not null");
							ilcursor.Emit(OpCodes.Ldarg_0);
							FastReflectionHelper.EmitCheckType(ilcursor, -1, declaringType2, illabel);
						}
						if (CS$<>8__locals1.typeClass != FastReflectionHelper.ReturnTypeClass.Void)
						{
							ilcursor.Emit(OpCodes.Ldarg_1);
							FastReflectionHelper.EmitCheckByref(ilcursor, CS$<>8__locals1.typeClass, CS$<>8__locals1.returnType, illabel, -2);
						}
						for (int i = 0; i < CS$<>8__locals1.methParams.Length; i++)
						{
							Type parameterType = CS$<>8__locals1.methParams[i].ParameterType;
							if (parameterType.IsByRefLike())
							{
								throw new ArgumentException("Cannot create reflection invoker for method with byref-like argument types", "method");
							}
							FastReflectionHelper.EmitLoadArgO(ilcursor, i);
							FastReflectionHelper.EmitCheckAllowNull(ilcursor, i, parameterType, illabel);
						}
						if (CS$<>8__locals1.typeClass != FastReflectionHelper.ReturnTypeClass.Void)
						{
							ilcursor.Emit(OpCodes.Ldarg_1);
							FastReflectionHelper.EmitLoadByref(ilcursor, CS$<>8__locals1.typeClass, CS$<>8__locals1.returnType);
						}
						if (!CS$<>8__locals1.method.IsStatic && !(CS$<>8__locals1.method is ConstructorInfo))
						{
							Type declaringType3 = CS$<>8__locals1.method.DeclaringType;
							Helpers.Assert(declaringType3 != null, null, "declType is not null");
							ilcursor.Emit(OpCodes.Ldarg_0);
							if (declaringType3.IsValueType)
							{
								ilcursor.Emit(OpCodes.Unbox, declaringType3);
							}
						}
						for (int j = 0; j < CS$<>8__locals1.methParams.Length; j++)
						{
							ilcursor.DefineLabel();
							Type parameterType2 = CS$<>8__locals1.methParams[j].ParameterType;
							Type type = (parameterType2.IsByRef ? (parameterType2.GetElementType() ?? parameterType2) : parameterType2);
							FastReflectionHelper.EmitLoadArgO(ilcursor, j);
							if (parameterType2.IsByRef)
							{
								FastReflectionHelper.EmitLoadByref(ilcursor, FastReflectionHelper.ClassifyType(type), type);
							}
							else if (parameterType2.IsValueType)
							{
								ilcursor.Emit(OpCodes.Unbox_Any, type);
							}
						}
						ConstructorInfo constructorInfo = CS$<>8__locals1.method as ConstructorInfo;
						if (constructorInfo != null)
						{
							ilcursor.Emit(OpCodes.Newobj, constructorInfo);
						}
						else if (CS$<>8__locals1.method.IsVirtual)
						{
							ilcursor.Emit(OpCodes.Callvirt, CS$<>8__locals1.method);
						}
						else
						{
							ilcursor.Emit(OpCodes.Call, CS$<>8__locals1.method);
						}
						FastReflectionHelper.EmitStoreByref(ilcursor, CS$<>8__locals1.typeClass, CS$<>8__locals1.returnType);
						ilcursor.Emit(OpCodes.Ret);
						FastReflectionHelper.EmitBadArgCall(ilcursor, illabel);
					});
					fastStructInvoker = dynamicMethodDefinition.Generate().CreateDelegate<FastReflectionHelper.FastStructInvoker>();
				}
			}
			return fastStructInvoker;
		}

		private static FastReflectionHelper.FastStructInvoker CreateFieldInvoker(FieldInfo field, out FastReflectionHelper.ReturnTypeClass retTypeClass, out Type retType)
		{
			if (!field.IsStatic)
			{
				Type declaringType = field.DeclaringType;
				if (declaringType != null && declaringType.IsByRefLike())
				{
					throw new ArgumentException("Cannot create reflection invoker for instance field on byref-like type", "field");
				}
			}
			Type returnType = field.FieldType;
			retType = returnType;
			retTypeClass = FastReflectionHelper.ClassifyType(returnType);
			FastReflectionHelper.ReturnTypeClass typeClass = retTypeClass;
			FormatInterpolatedStringHandler formatInterpolatedStringHandler = new FormatInterpolatedStringHandler(22, 1);
			formatInterpolatedStringHandler.AppendLiteral("MM:FastStructInvoker<");
			formatInterpolatedStringHandler.AppendFormatted<FieldInfo>(field);
			formatInterpolatedStringHandler.AppendLiteral(">");
			FastReflectionHelper.FastStructInvoker fastStructInvoker;
			using (DynamicMethodDefinition dynamicMethodDefinition = new DynamicMethodDefinition(DebugFormatter.Format(ref formatInterpolatedStringHandler), null, FastReflectionHelper.FastStructInvokerArgs))
			{
				using (ILContext ilcontext = new ILContext(dynamicMethodDefinition.Definition))
				{
					ilcontext.Invoke(delegate(ILContext ilc)
					{
						ILCursor ilcursor = new ILCursor(ilc);
						FastReflectionHelper.EmitCheckArgs(ilcursor, field.IsStatic, typeClass, 0);
						ILLabel illabel = ilcursor.DefineLabel();
						if (!field.IsStatic)
						{
							Type declaringType2 = field.DeclaringType;
							ilcursor.Emit(OpCodes.Ldarg_0);
							FastReflectionHelper.EmitCheckType(ilcursor, -1, declaringType2, illabel);
						}
						ilcursor.Emit(OpCodes.Ldarg_1);
						FastReflectionHelper.EmitCheckByref(ilcursor, typeClass, returnType, illabel, -2);
						ILLabel illabel2 = ilcursor.DefineLabel();
						ilcursor.Emit(OpCodes.Ldarg_2);
						ilcursor.Emit(OpCodes.Brfalse, illabel2);
						ilcursor.Emit(OpCodes.Ldarg_2);
						ilcursor.Emit(OpCodes.Ldlen);
						ilcursor.Emit(OpCodes.Ldc_I4_1);
						ilcursor.Emit(OpCodes.Blt, illabel2);
						FastReflectionHelper.EmitLoadArgO(ilcursor, 0);
						FastReflectionHelper.EmitCheckAllowNull(ilcursor, 0, field.FieldType, illabel);
						ilcursor.Emit(OpCodes.Ldarg_1);
						FastReflectionHelper.EmitLoadByref(ilcursor, typeClass, returnType);
						if (!field.IsStatic)
						{
							Type declaringType3 = field.DeclaringType;
							Helpers.Assert(declaringType3 != null, null, "declType is not null");
							ilcursor.Emit(OpCodes.Ldarg_0);
							if (declaringType3.IsValueType)
							{
								ilcursor.Emit(OpCodes.Unbox, declaringType3);
							}
						}
						FastReflectionHelper.EmitLoadArgO(ilcursor, 0);
						ilcursor.Emit(OpCodes.Unbox_Any, field.FieldType);
						if (field.IsStatic)
						{
							ilcursor.Emit(OpCodes.Stsfld, field);
						}
						else
						{
							ilcursor.Emit(OpCodes.Stfld, field);
						}
						FastReflectionHelper.EmitLoadArgO(ilcursor, 0);
						ilcursor.Emit(OpCodes.Unbox_Any, field.FieldType);
						FastReflectionHelper.EmitStoreByref(ilcursor, typeClass, returnType);
						ilcursor.Emit(OpCodes.Ret);
						ilcursor.MarkLabel(illabel2);
						ilcursor.Emit(OpCodes.Ldarg_1);
						FastReflectionHelper.EmitLoadByref(ilcursor, typeClass, returnType);
						if (!field.IsStatic)
						{
							Type declaringType4 = field.DeclaringType;
							Helpers.Assert(declaringType4 != null, null, "declType is not null");
							ilcursor.Emit(OpCodes.Ldarg_0);
							if (declaringType4.IsValueType)
							{
								ilcursor.Emit(OpCodes.Unbox, declaringType4);
							}
						}
						if (field.IsStatic)
						{
							ilcursor.Emit(OpCodes.Ldsfld, field);
						}
						else
						{
							ilcursor.Emit(OpCodes.Ldfld, field);
						}
						FastReflectionHelper.EmitStoreByref(ilcursor, typeClass, returnType);
						ilcursor.Emit(OpCodes.Ret);
						FastReflectionHelper.EmitBadArgCall(ilcursor, illabel);
					});
					fastStructInvoker = dynamicMethodDefinition.Generate().CreateDelegate<FastReflectionHelper.FastStructInvoker>();
				}
			}
			return fastStructInvoker;
		}

		[CompilerGenerated]
		[MethodImpl(MethodImplOptions.NoInlining)]
		internal static void <CheckArgs>g__ThrowArgumentOutOfRange|28_0()
		{
			throw new ArgumentOutOfRangeException("args", "Argument array has too few arguments!");
		}

		private static readonly Type[] FastStructInvokerArgs = new Type[]
		{
			typeof(object),
			typeof(object),
			typeof(object[])
		};

		private static readonly MethodInfo S2FValueType = typeof(FastReflectionHelper).GetMethod("FastInvokerForStructInvokerVT", BindingFlags.Static | BindingFlags.NonPublic);

		private static readonly MethodInfo S2FNullable = typeof(FastReflectionHelper).GetMethod("FastInvokerForStructInvokerNullable", BindingFlags.Static | BindingFlags.NonPublic);

		[Nullable(2)]
		[ThreadStatic]
		private static WeakBox CachedWeakBox;

		private static readonly MethodInfo S2FClass = typeof(FastReflectionHelper).GetMethod("FastInvokerForStructInvokerClass", BindingFlags.Static | BindingFlags.NonPublic);

		private static readonly MethodInfo S2FVoid = typeof(FastReflectionHelper).GetMethod("FastInvokerForStructInvokerVoid", BindingFlags.Static | BindingFlags.NonPublic);

		private static ConditionalWeakTable<MemberInfo, FastReflectionHelper.FSITuple> fastStructInvokers = new ConditionalWeakTable<MemberInfo, FastReflectionHelper.FSITuple>();

		private static ConditionalWeakTable<FastReflectionHelper.FSITuple, FastReflectionHelper.FastInvoker> fastInvokers = new ConditionalWeakTable<FastReflectionHelper.FSITuple, FastReflectionHelper.FastInvoker>();

		private static readonly MethodInfo CheckArgsMethod = typeof(FastReflectionHelper).GetMethod("CheckArgs", BindingFlags.Static | BindingFlags.NonPublic);

		private const int TargetArgId = -1;

		private const int ResultArgId = -2;

		private static readonly MethodInfo BadArgExceptionMethod = typeof(FastReflectionHelper).GetMethod("BadArgException", BindingFlags.Static | BindingFlags.NonPublic);

		private static readonly FieldInfo WeakBoxValueField = typeof(WeakBox).GetField("Value");

		[NullableContext(0)]
		public delegate object FastInvoker(object target, params object[] args);

		[NullableContext(0)]
		public delegate void FastStructInvoker(object target, object result, params object[] args);

		[NullableContext(0)]
		private static class TypedCache<T> where T : struct
		{
			[Nullable(new byte[] { 2, 0 })]
			[ThreadStatic]
			public static StrongBox<T?> NullableStrongBox;
		}

		[NullableContext(0)]
		private enum ReturnTypeClass
		{
			Void,
			ValueType,
			Nullable,
			ReferenceType
		}

		[Nullable(0)]
		private sealed class FSITuple
		{
			public FSITuple(FastReflectionHelper.FastStructInvoker fsi, FastReflectionHelper.ReturnTypeClass rtc, Type rt)
			{
				this.FSI = fsi;
				this.RTC = rtc;
				this.ReturnType = rt;
			}

			public readonly FastReflectionHelper.FastStructInvoker FSI;

			public readonly FastReflectionHelper.ReturnTypeClass RTC;

			public readonly Type ReturnType;
		}
	}
}
