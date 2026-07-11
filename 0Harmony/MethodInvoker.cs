using System;
using System.Reflection;
using System.Reflection.Emit;

namespace Harmony
{
	public class MethodInvoker
	{
		public static FastInvokeHandler GetHandler(DynamicMethod methodInfo, Module module)
		{
			return MethodInvoker.Handler(methodInfo, module, false);
		}

		public static FastInvokeHandler GetHandler(MethodInfo methodInfo)
		{
			return MethodInvoker.Handler(methodInfo, methodInfo.DeclaringType.Module, false);
		}

		private unsafe static FastInvokeHandler Handler(MethodInfo methodInfo, Module module, bool directBoxValueAccess = false)
		{
			DynamicMethod dynamicMethod = new DynamicMethod("FastInvoke_" + methodInfo.Name + "_" + (directBoxValueAccess ? "direct" : "indirect"), typeof(object), new Type[]
			{
				typeof(object),
				typeof(object[])
			}, module, true);
			ILGenerator ilgenerator = dynamicMethod.GetILGenerator();
			bool flag = !methodInfo.IsStatic;
			if (flag)
			{
				ilgenerator.Emit(OpCodes.Ldarg_0);
				MethodInvoker.EmitUnboxIfNeeded(ilgenerator, methodInfo.DeclaringType);
			}
			bool flag2 = true;
			ParameterInfo[] parameters = methodInfo.GetParameters();
			for (int i = 0; i < parameters.Length; i++)
			{
				Type type = parameters[i].ParameterType;
				bool isByRef = type.IsByRef;
				bool flag3 = isByRef;
				if (flag3)
				{
					type = type.GetElementType();
				}
				bool isValueType = type.IsValueType;
				bool flag4 = isByRef && isValueType && !directBoxValueAccess;
				if (flag4)
				{
					ilgenerator.Emit(OpCodes.Ldarg_1);
					MethodInvoker.EmitFastInt(ilgenerator, i);
				}
				ilgenerator.Emit(OpCodes.Ldarg_1);
				MethodInvoker.EmitFastInt(ilgenerator, i);
				bool flag5 = isByRef && !isValueType;
				if (flag5)
				{
					ilgenerator.Emit(OpCodes.Ldelema, typeof(object));
				}
				else
				{
					ilgenerator.Emit(OpCodes.Ldelem_Ref);
					bool flag6 = isValueType;
					if (flag6)
					{
						bool flag7 = !isByRef || !directBoxValueAccess;
						if (flag7)
						{
							ilgenerator.Emit(OpCodes.Unbox_Any, type);
							bool flag8 = isByRef;
							if (flag8)
							{
								ilgenerator.Emit(OpCodes.Box, type);
								ilgenerator.Emit(OpCodes.Dup);
								ilgenerator.Emit(OpCodes.Unbox, type);
								bool flag9 = flag2;
								if (flag9)
								{
									flag2 = false;
									ilgenerator.DeclareLocal(typeof(void*), true);
								}
								ilgenerator.Emit(OpCodes.Stloc_0);
								ilgenerator.Emit(OpCodes.Stelem_Ref);
								ilgenerator.Emit(OpCodes.Ldloc_0);
							}
						}
						else
						{
							ilgenerator.Emit(OpCodes.Unbox, type);
						}
					}
				}
			}
			bool isStatic = methodInfo.IsStatic;
			if (isStatic)
			{
				ilgenerator.EmitCall(OpCodes.Call, methodInfo, null);
			}
			else
			{
				ilgenerator.EmitCall(OpCodes.Callvirt, methodInfo, null);
			}
			bool flag10 = methodInfo.ReturnType == typeof(void);
			if (flag10)
			{
				ilgenerator.Emit(OpCodes.Ldnull);
			}
			else
			{
				MethodInvoker.EmitBoxIfNeeded(ilgenerator, methodInfo.ReturnType);
			}
			ilgenerator.Emit(OpCodes.Ret);
			return (FastInvokeHandler)dynamicMethod.CreateDelegate(typeof(FastInvokeHandler));
		}

		private static void EmitCastToReference(ILGenerator il, Type type)
		{
			bool isValueType = type.IsValueType;
			if (isValueType)
			{
				il.Emit(OpCodes.Unbox_Any, type);
			}
			else
			{
				il.Emit(OpCodes.Castclass, type);
			}
		}

		private static void EmitUnboxIfNeeded(ILGenerator il, Type type)
		{
			bool isValueType = type.IsValueType;
			if (isValueType)
			{
				il.Emit(OpCodes.Unbox_Any, type);
			}
		}

		private static void EmitBoxIfNeeded(ILGenerator il, Type type)
		{
			bool isValueType = type.IsValueType;
			if (isValueType)
			{
				il.Emit(OpCodes.Box, type);
			}
		}

		private static void EmitFastInt(ILGenerator il, int value)
		{
			switch (value)
			{
			case -1:
				il.Emit(OpCodes.Ldc_I4_M1);
				break;
			case 0:
				il.Emit(OpCodes.Ldc_I4_0);
				break;
			case 1:
				il.Emit(OpCodes.Ldc_I4_1);
				break;
			case 2:
				il.Emit(OpCodes.Ldc_I4_2);
				break;
			case 3:
				il.Emit(OpCodes.Ldc_I4_3);
				break;
			case 4:
				il.Emit(OpCodes.Ldc_I4_4);
				break;
			case 5:
				il.Emit(OpCodes.Ldc_I4_5);
				break;
			case 6:
				il.Emit(OpCodes.Ldc_I4_6);
				break;
			case 7:
				il.Emit(OpCodes.Ldc_I4_7);
				break;
			case 8:
				il.Emit(OpCodes.Ldc_I4_8);
				break;
			default:
			{
				bool flag = value > -129 && value < 128;
				if (flag)
				{
					il.Emit(OpCodes.Ldc_I4_S, (sbyte)value);
				}
				else
				{
					il.Emit(OpCodes.Ldc_I4, value);
				}
				break;
			}
			}
		}
	}
}
