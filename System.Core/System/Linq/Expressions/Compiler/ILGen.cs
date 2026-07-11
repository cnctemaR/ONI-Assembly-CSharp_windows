using System;
using System.Dynamic.Utils;
using System.Reflection;
using System.Reflection.Emit;

namespace System.Linq.Expressions.Compiler
{
	internal static class ILGen
	{
		internal static void Emit(this ILGenerator il, OpCode opcode, MethodBase methodBase)
		{
			ConstructorInfo constructorInfo = methodBase as ConstructorInfo;
			if (constructorInfo != null)
			{
				il.Emit(opcode, constructorInfo);
				return;
			}
			il.Emit(opcode, (MethodInfo)methodBase);
		}

		internal static void EmitLoadArg(this ILGenerator il, int index)
		{
			switch (index)
			{
			case 0:
				il.Emit(OpCodes.Ldarg_0);
				return;
			case 1:
				il.Emit(OpCodes.Ldarg_1);
				return;
			case 2:
				il.Emit(OpCodes.Ldarg_2);
				return;
			case 3:
				il.Emit(OpCodes.Ldarg_3);
				return;
			default:
				if (index <= 255)
				{
					il.Emit(OpCodes.Ldarg_S, (byte)index);
					return;
				}
				il.Emit(OpCodes.Ldarg, index);
				return;
			}
		}

		internal static void EmitLoadArgAddress(this ILGenerator il, int index)
		{
			if (index <= 255)
			{
				il.Emit(OpCodes.Ldarga_S, (byte)index);
				return;
			}
			il.Emit(OpCodes.Ldarga, index);
		}

		internal static void EmitStoreArg(this ILGenerator il, int index)
		{
			if (index <= 255)
			{
				il.Emit(OpCodes.Starg_S, (byte)index);
				return;
			}
			il.Emit(OpCodes.Starg, index);
		}

		internal static void EmitLoadValueIndirect(this ILGenerator il, Type type)
		{
			switch (type.GetTypeCode())
			{
			case TypeCode.Boolean:
			case TypeCode.SByte:
				il.Emit(OpCodes.Ldind_U1);
				return;
			case TypeCode.Char:
			case TypeCode.UInt16:
				il.Emit(OpCodes.Ldind_U2);
				return;
			case TypeCode.Byte:
				il.Emit(OpCodes.Ldind_I1);
				return;
			case TypeCode.Int16:
				il.Emit(OpCodes.Ldind_I2);
				return;
			case TypeCode.Int32:
				il.Emit(OpCodes.Ldind_I4);
				return;
			case TypeCode.UInt32:
				il.Emit(OpCodes.Ldind_U4);
				return;
			case TypeCode.Int64:
			case TypeCode.UInt64:
				il.Emit(OpCodes.Ldind_I8);
				return;
			case TypeCode.Single:
				il.Emit(OpCodes.Ldind_R4);
				return;
			case TypeCode.Double:
				il.Emit(OpCodes.Ldind_R8);
				return;
			default:
				if (type.IsValueType)
				{
					il.Emit(OpCodes.Ldobj, type);
					return;
				}
				il.Emit(OpCodes.Ldind_Ref);
				return;
			}
		}

		internal static void EmitStoreValueIndirect(this ILGenerator il, Type type)
		{
			switch (type.GetTypeCode())
			{
			case TypeCode.Boolean:
			case TypeCode.SByte:
			case TypeCode.Byte:
				il.Emit(OpCodes.Stind_I1);
				return;
			case TypeCode.Char:
			case TypeCode.Int16:
			case TypeCode.UInt16:
				il.Emit(OpCodes.Stind_I2);
				return;
			case TypeCode.Int32:
			case TypeCode.UInt32:
				il.Emit(OpCodes.Stind_I4);
				return;
			case TypeCode.Int64:
			case TypeCode.UInt64:
				il.Emit(OpCodes.Stind_I8);
				return;
			case TypeCode.Single:
				il.Emit(OpCodes.Stind_R4);
				return;
			case TypeCode.Double:
				il.Emit(OpCodes.Stind_R8);
				return;
			default:
				if (type.IsValueType)
				{
					il.Emit(OpCodes.Stobj, type);
					return;
				}
				il.Emit(OpCodes.Stind_Ref);
				return;
			}
		}

		internal static void EmitLoadElement(this ILGenerator il, Type type)
		{
			if (!type.IsValueType)
			{
				il.Emit(OpCodes.Ldelem_Ref);
				return;
			}
			switch (type.GetTypeCode())
			{
			case TypeCode.Boolean:
			case TypeCode.SByte:
				il.Emit(OpCodes.Ldelem_I1);
				return;
			case TypeCode.Char:
			case TypeCode.UInt16:
				il.Emit(OpCodes.Ldelem_U2);
				return;
			case TypeCode.Byte:
				il.Emit(OpCodes.Ldelem_U1);
				return;
			case TypeCode.Int16:
				il.Emit(OpCodes.Ldelem_I2);
				return;
			case TypeCode.Int32:
				il.Emit(OpCodes.Ldelem_I4);
				return;
			case TypeCode.UInt32:
				il.Emit(OpCodes.Ldelem_U4);
				return;
			case TypeCode.Int64:
			case TypeCode.UInt64:
				il.Emit(OpCodes.Ldelem_I8);
				return;
			case TypeCode.Single:
				il.Emit(OpCodes.Ldelem_R4);
				return;
			case TypeCode.Double:
				il.Emit(OpCodes.Ldelem_R8);
				return;
			default:
				il.Emit(OpCodes.Ldelem, type);
				return;
			}
		}

		internal static void EmitStoreElement(this ILGenerator il, Type type)
		{
			switch (type.GetTypeCode())
			{
			case TypeCode.Boolean:
			case TypeCode.SByte:
			case TypeCode.Byte:
				il.Emit(OpCodes.Stelem_I1);
				return;
			case TypeCode.Char:
			case TypeCode.Int16:
			case TypeCode.UInt16:
				il.Emit(OpCodes.Stelem_I2);
				return;
			case TypeCode.Int32:
			case TypeCode.UInt32:
				il.Emit(OpCodes.Stelem_I4);
				return;
			case TypeCode.Int64:
			case TypeCode.UInt64:
				il.Emit(OpCodes.Stelem_I8);
				return;
			case TypeCode.Single:
				il.Emit(OpCodes.Stelem_R4);
				return;
			case TypeCode.Double:
				il.Emit(OpCodes.Stelem_R8);
				return;
			default:
				if (type.IsValueType)
				{
					il.Emit(OpCodes.Stelem, type);
					return;
				}
				il.Emit(OpCodes.Stelem_Ref);
				return;
			}
		}

		internal static void EmitType(this ILGenerator il, Type type)
		{
			il.Emit(OpCodes.Ldtoken, type);
			il.Emit(OpCodes.Call, CachedReflectionInfo.Type_GetTypeFromHandle);
		}

		internal static void EmitFieldAddress(this ILGenerator il, FieldInfo fi)
		{
			il.Emit(fi.IsStatic ? OpCodes.Ldsflda : OpCodes.Ldflda, fi);
		}

		internal static void EmitFieldGet(this ILGenerator il, FieldInfo fi)
		{
			il.Emit(fi.IsStatic ? OpCodes.Ldsfld : OpCodes.Ldfld, fi);
		}

		internal static void EmitFieldSet(this ILGenerator il, FieldInfo fi)
		{
			il.Emit(fi.IsStatic ? OpCodes.Stsfld : OpCodes.Stfld, fi);
		}

		internal static void EmitNew(this ILGenerator il, ConstructorInfo ci)
		{
			il.Emit(OpCodes.Newobj, ci);
		}

		internal static void EmitNull(this ILGenerator il)
		{
			il.Emit(OpCodes.Ldnull);
		}

		internal static void EmitString(this ILGenerator il, string value)
		{
			il.Emit(OpCodes.Ldstr, value);
		}

		internal static void EmitPrimitive(this ILGenerator il, bool value)
		{
			il.Emit(value ? OpCodes.Ldc_I4_1 : OpCodes.Ldc_I4_0);
		}

		internal static void EmitPrimitive(this ILGenerator il, int value)
		{
			OpCode opCode;
			switch (value)
			{
			case -1:
				opCode = OpCodes.Ldc_I4_M1;
				break;
			case 0:
				opCode = OpCodes.Ldc_I4_0;
				break;
			case 1:
				opCode = OpCodes.Ldc_I4_1;
				break;
			case 2:
				opCode = OpCodes.Ldc_I4_2;
				break;
			case 3:
				opCode = OpCodes.Ldc_I4_3;
				break;
			case 4:
				opCode = OpCodes.Ldc_I4_4;
				break;
			case 5:
				opCode = OpCodes.Ldc_I4_5;
				break;
			case 6:
				opCode = OpCodes.Ldc_I4_6;
				break;
			case 7:
				opCode = OpCodes.Ldc_I4_7;
				break;
			case 8:
				opCode = OpCodes.Ldc_I4_8;
				break;
			default:
				if (value >= -128 && value <= 127)
				{
					il.Emit(OpCodes.Ldc_I4_S, (sbyte)value);
					return;
				}
				il.Emit(OpCodes.Ldc_I4, value);
				return;
			}
			il.Emit(opCode);
		}

		private static void EmitPrimitive(this ILGenerator il, uint value)
		{
			il.EmitPrimitive((int)value);
		}

		private static void EmitPrimitive(this ILGenerator il, long value)
		{
			if ((-2147483648L <= value) & (value <= (long)((ulong)(-1))))
			{
				il.EmitPrimitive((int)value);
				il.Emit((value > 0L) ? OpCodes.Conv_U8 : OpCodes.Conv_I8);
				return;
			}
			il.Emit(OpCodes.Ldc_I8, value);
		}

		private static void EmitPrimitive(this ILGenerator il, ulong value)
		{
			il.EmitPrimitive((long)value);
		}

		private static void EmitPrimitive(this ILGenerator il, double value)
		{
			il.Emit(OpCodes.Ldc_R8, value);
		}

		private static void EmitPrimitive(this ILGenerator il, float value)
		{
			il.Emit(OpCodes.Ldc_R4, value);
		}

		internal static bool CanEmitConstant(object value, Type type)
		{
			if (value == null || ILGen.CanEmitILConstant(type))
			{
				return true;
			}
			Type type2 = value as Type;
			if (type2 != null)
			{
				return ILGen.ShouldLdtoken(type2);
			}
			MethodBase methodBase = value as MethodBase;
			return methodBase != null && ILGen.ShouldLdtoken(methodBase);
		}

		private static bool CanEmitILConstant(Type type)
		{
			TypeCode typeCode = type.GetNonNullableType().GetTypeCode();
			return typeCode - TypeCode.Boolean <= 12 || typeCode == TypeCode.String;
		}

		internal static bool TryEmitConstant(this ILGenerator il, object value, Type type, ILocalCache locals)
		{
			if (value == null)
			{
				il.EmitDefault(type, locals);
				return true;
			}
			if (il.TryEmitILConstant(value, type))
			{
				return true;
			}
			Type type2 = value as Type;
			if (type2 != null)
			{
				if (ILGen.ShouldLdtoken(type2))
				{
					il.EmitType(type2);
					if (type != typeof(Type))
					{
						il.Emit(OpCodes.Castclass, type);
					}
					return true;
				}
				return false;
			}
			else
			{
				MethodBase methodBase = value as MethodBase;
				if (methodBase != null && ILGen.ShouldLdtoken(methodBase))
				{
					il.Emit(OpCodes.Ldtoken, methodBase);
					Type declaringType = methodBase.DeclaringType;
					if (declaringType != null && declaringType.IsGenericType)
					{
						il.Emit(OpCodes.Ldtoken, declaringType);
						il.Emit(OpCodes.Call, CachedReflectionInfo.MethodBase_GetMethodFromHandle_RuntimeMethodHandle_RuntimeTypeHandle);
					}
					else
					{
						il.Emit(OpCodes.Call, CachedReflectionInfo.MethodBase_GetMethodFromHandle_RuntimeMethodHandle);
					}
					if (type != typeof(MethodBase))
					{
						il.Emit(OpCodes.Castclass, type);
					}
					return true;
				}
				return false;
			}
		}

		private static bool ShouldLdtoken(Type t)
		{
			return t.IsGenericParameter || t.IsVisible;
		}

		internal static bool ShouldLdtoken(MethodBase mb)
		{
			if (mb is DynamicMethod)
			{
				return false;
			}
			Type declaringType = mb.DeclaringType;
			return declaringType == null || ILGen.ShouldLdtoken(declaringType);
		}

		private static bool TryEmitILConstant(this ILGenerator il, object value, Type type)
		{
			if (!type.IsNullableType())
			{
				switch (type.GetTypeCode())
				{
				case TypeCode.Boolean:
					il.EmitPrimitive((bool)value);
					return true;
				case TypeCode.Char:
					il.EmitPrimitive((int)((char)value));
					return true;
				case TypeCode.SByte:
					il.EmitPrimitive((int)((sbyte)value));
					return true;
				case TypeCode.Byte:
					il.EmitPrimitive((int)((byte)value));
					return true;
				case TypeCode.Int16:
					il.EmitPrimitive((int)((short)value));
					return true;
				case TypeCode.UInt16:
					il.EmitPrimitive((int)((ushort)value));
					return true;
				case TypeCode.Int32:
					il.EmitPrimitive((int)value);
					return true;
				case TypeCode.UInt32:
					il.EmitPrimitive((uint)value);
					return true;
				case TypeCode.Int64:
					il.EmitPrimitive((long)value);
					return true;
				case TypeCode.UInt64:
					il.EmitPrimitive((ulong)value);
					return true;
				case TypeCode.Single:
					il.EmitPrimitive((float)value);
					return true;
				case TypeCode.Double:
					il.EmitPrimitive((double)value);
					return true;
				case TypeCode.Decimal:
					il.EmitDecimal((decimal)value);
					return true;
				case TypeCode.String:
					il.EmitString((string)value);
					return true;
				}
				return false;
			}
			Type nonNullableType = type.GetNonNullableType();
			if (il.TryEmitILConstant(value, nonNullableType))
			{
				il.Emit(OpCodes.Newobj, type.GetConstructor(new Type[] { nonNullableType }));
				return true;
			}
			return false;
		}

		internal static void EmitConvertToType(this ILGenerator il, Type typeFrom, Type typeTo, bool isChecked, ILocalCache locals)
		{
			if (TypeUtils.AreEquivalent(typeFrom, typeTo))
			{
				return;
			}
			bool flag = typeFrom.IsNullableType();
			bool flag2 = typeTo.IsNullableType();
			Type nonNullableType = typeFrom.GetNonNullableType();
			Type nonNullableType2 = typeTo.GetNonNullableType();
			if (typeFrom.IsInterface || typeTo.IsInterface || typeFrom == typeof(object) || typeTo == typeof(object) || typeFrom == typeof(Enum) || typeFrom == typeof(ValueType) || TypeUtils.IsLegalExplicitVariantDelegateConversion(typeFrom, typeTo))
			{
				il.EmitCastToType(typeFrom, typeTo);
				return;
			}
			if (flag || flag2)
			{
				il.EmitNullableConversion(typeFrom, typeTo, isChecked, locals);
				return;
			}
			if ((!typeFrom.IsConvertible() || !typeTo.IsConvertible()) && (nonNullableType.IsAssignableFrom(nonNullableType2) || nonNullableType2.IsAssignableFrom(nonNullableType)))
			{
				il.EmitCastToType(typeFrom, typeTo);
				return;
			}
			if (typeFrom.IsArray && typeTo.IsArray)
			{
				il.EmitCastToType(typeFrom, typeTo);
				return;
			}
			il.EmitNumericConversion(typeFrom, typeTo, isChecked);
		}

		private static void EmitCastToType(this ILGenerator il, Type typeFrom, Type typeTo)
		{
			if (typeFrom.IsValueType)
			{
				il.Emit(OpCodes.Box, typeFrom);
				if (typeTo != typeof(object))
				{
					il.Emit(OpCodes.Castclass, typeTo);
					return;
				}
			}
			else
			{
				il.Emit(typeTo.IsValueType ? OpCodes.Unbox_Any : OpCodes.Castclass, typeTo);
			}
		}

		private static void EmitNumericConversion(this ILGenerator il, Type typeFrom, Type typeTo, bool isChecked)
		{
			TypeCode typeCode = typeTo.GetTypeCode();
			TypeCode typeCode2 = typeFrom.GetTypeCode();
			if (typeCode == typeCode2)
			{
				return;
			}
			bool flag = typeCode2.IsUnsigned();
			OpCode opCode;
			switch (typeCode)
			{
			case TypeCode.Char:
			case TypeCode.UInt16:
				switch (typeCode2)
				{
				case TypeCode.Char:
				case TypeCode.Byte:
				case TypeCode.UInt16:
					return;
				case TypeCode.SByte:
				case TypeCode.Int16:
					if (!isChecked)
					{
						return;
					}
					break;
				}
				opCode = (isChecked ? (flag ? OpCodes.Conv_Ovf_U2_Un : OpCodes.Conv_Ovf_U2) : OpCodes.Conv_U2);
				break;
			case TypeCode.SByte:
				if (isChecked)
				{
					opCode = (flag ? OpCodes.Conv_Ovf_I1_Un : OpCodes.Conv_Ovf_I1);
				}
				else
				{
					if (typeCode2 == TypeCode.Byte)
					{
						return;
					}
					opCode = OpCodes.Conv_I1;
				}
				break;
			case TypeCode.Byte:
				if (isChecked)
				{
					opCode = (flag ? OpCodes.Conv_Ovf_U1_Un : OpCodes.Conv_Ovf_U1);
				}
				else
				{
					if (typeCode2 == TypeCode.SByte)
					{
						return;
					}
					opCode = OpCodes.Conv_U1;
				}
				break;
			case TypeCode.Int16:
				switch (typeCode2)
				{
				case TypeCode.Char:
				case TypeCode.UInt16:
					if (!isChecked)
					{
						return;
					}
					break;
				case TypeCode.SByte:
				case TypeCode.Byte:
					return;
				}
				opCode = (isChecked ? (flag ? OpCodes.Conv_Ovf_I2_Un : OpCodes.Conv_Ovf_I2) : OpCodes.Conv_I2);
				break;
			case TypeCode.Int32:
				if (typeCode2 - TypeCode.SByte <= 3)
				{
					return;
				}
				if (typeCode2 == TypeCode.UInt32)
				{
					if (!isChecked)
					{
						return;
					}
				}
				opCode = (isChecked ? (flag ? OpCodes.Conv_Ovf_I4_Un : OpCodes.Conv_Ovf_I4) : OpCodes.Conv_I4);
				break;
			case TypeCode.UInt32:
				switch (typeCode2)
				{
				case TypeCode.Char:
				case TypeCode.Byte:
				case TypeCode.UInt16:
					return;
				case TypeCode.SByte:
				case TypeCode.Int16:
				case TypeCode.Int32:
					if (!isChecked)
					{
						return;
					}
					break;
				}
				opCode = (isChecked ? (flag ? OpCodes.Conv_Ovf_U4_Un : OpCodes.Conv_Ovf_U4) : OpCodes.Conv_U4);
				break;
			case TypeCode.Int64:
				if (!isChecked && typeCode2 == TypeCode.UInt64)
				{
					return;
				}
				opCode = (isChecked ? (flag ? OpCodes.Conv_Ovf_I8_Un : OpCodes.Conv_Ovf_I8) : (flag ? OpCodes.Conv_U8 : OpCodes.Conv_I8));
				break;
			case TypeCode.UInt64:
				if (!isChecked && typeCode2 == TypeCode.Int64)
				{
					return;
				}
				opCode = (isChecked ? ((flag || typeCode2.IsFloatingPoint()) ? OpCodes.Conv_Ovf_U8_Un : OpCodes.Conv_Ovf_U8) : ((flag || typeCode2.IsFloatingPoint()) ? OpCodes.Conv_U8 : OpCodes.Conv_I8));
				break;
			case TypeCode.Single:
				if (flag)
				{
					il.Emit(OpCodes.Conv_R_Un);
				}
				opCode = OpCodes.Conv_R4;
				break;
			case TypeCode.Double:
				if (flag)
				{
					il.Emit(OpCodes.Conv_R_Un);
				}
				opCode = OpCodes.Conv_R8;
				break;
			case TypeCode.Decimal:
			{
				MethodInfo methodInfo;
				switch (typeCode2)
				{
				case TypeCode.Char:
					methodInfo = CachedReflectionInfo.Decimal_op_Implicit_Char;
					break;
				case TypeCode.SByte:
					methodInfo = CachedReflectionInfo.Decimal_op_Implicit_SByte;
					break;
				case TypeCode.Byte:
					methodInfo = CachedReflectionInfo.Decimal_op_Implicit_Byte;
					break;
				case TypeCode.Int16:
					methodInfo = CachedReflectionInfo.Decimal_op_Implicit_Int16;
					break;
				case TypeCode.UInt16:
					methodInfo = CachedReflectionInfo.Decimal_op_Implicit_UInt16;
					break;
				case TypeCode.Int32:
					methodInfo = CachedReflectionInfo.Decimal_op_Implicit_Int32;
					break;
				case TypeCode.UInt32:
					methodInfo = CachedReflectionInfo.Decimal_op_Implicit_UInt32;
					break;
				case TypeCode.Int64:
					methodInfo = CachedReflectionInfo.Decimal_op_Implicit_Int64;
					break;
				case TypeCode.UInt64:
					methodInfo = CachedReflectionInfo.Decimal_op_Implicit_UInt64;
					break;
				default:
					throw ContractUtils.Unreachable;
				}
				il.Emit(OpCodes.Call, methodInfo);
				return;
			}
			default:
				throw ContractUtils.Unreachable;
			}
			il.Emit(opCode);
		}

		private static void EmitNullableToNullableConversion(this ILGenerator il, Type typeFrom, Type typeTo, bool isChecked, ILocalCache locals)
		{
			LocalBuilder local = locals.GetLocal(typeFrom);
			il.Emit(OpCodes.Stloc, local);
			il.Emit(OpCodes.Ldloca, local);
			il.EmitHasValue(typeFrom);
			Label label = il.DefineLabel();
			il.Emit(OpCodes.Brfalse_S, label);
			il.Emit(OpCodes.Ldloca, local);
			locals.FreeLocal(local);
			il.EmitGetValueOrDefault(typeFrom);
			Type nonNullableType = typeFrom.GetNonNullableType();
			Type nonNullableType2 = typeTo.GetNonNullableType();
			il.EmitConvertToType(nonNullableType, nonNullableType2, isChecked, locals);
			ConstructorInfo constructor = typeTo.GetConstructor(new Type[] { nonNullableType2 });
			il.Emit(OpCodes.Newobj, constructor);
			Label label2 = il.DefineLabel();
			il.Emit(OpCodes.Br_S, label2);
			il.MarkLabel(label);
			LocalBuilder local2 = locals.GetLocal(typeTo);
			il.Emit(OpCodes.Ldloca, local2);
			il.Emit(OpCodes.Initobj, typeTo);
			il.Emit(OpCodes.Ldloc, local2);
			locals.FreeLocal(local2);
			il.MarkLabel(label2);
		}

		private static void EmitNonNullableToNullableConversion(this ILGenerator il, Type typeFrom, Type typeTo, bool isChecked, ILocalCache locals)
		{
			Type nonNullableType = typeTo.GetNonNullableType();
			il.EmitConvertToType(typeFrom, nonNullableType, isChecked, locals);
			ConstructorInfo constructor = typeTo.GetConstructor(new Type[] { nonNullableType });
			il.Emit(OpCodes.Newobj, constructor);
		}

		private static void EmitNullableToNonNullableConversion(this ILGenerator il, Type typeFrom, Type typeTo, bool isChecked, ILocalCache locals)
		{
			if (typeTo.IsValueType)
			{
				il.EmitNullableToNonNullableStructConversion(typeFrom, typeTo, isChecked, locals);
				return;
			}
			il.EmitNullableToReferenceConversion(typeFrom);
		}

		private static void EmitNullableToNonNullableStructConversion(this ILGenerator il, Type typeFrom, Type typeTo, bool isChecked, ILocalCache locals)
		{
			LocalBuilder local = locals.GetLocal(typeFrom);
			il.Emit(OpCodes.Stloc, local);
			il.Emit(OpCodes.Ldloca, local);
			locals.FreeLocal(local);
			il.EmitGetValue(typeFrom);
			Type nonNullableType = typeFrom.GetNonNullableType();
			il.EmitConvertToType(nonNullableType, typeTo, isChecked, locals);
		}

		private static void EmitNullableToReferenceConversion(this ILGenerator il, Type typeFrom)
		{
			il.Emit(OpCodes.Box, typeFrom);
		}

		private static void EmitNullableConversion(this ILGenerator il, Type typeFrom, Type typeTo, bool isChecked, ILocalCache locals)
		{
			bool flag = typeFrom.IsNullableType();
			bool flag2 = typeTo.IsNullableType();
			if (flag && flag2)
			{
				il.EmitNullableToNullableConversion(typeFrom, typeTo, isChecked, locals);
				return;
			}
			if (flag)
			{
				il.EmitNullableToNonNullableConversion(typeFrom, typeTo, isChecked, locals);
				return;
			}
			il.EmitNonNullableToNullableConversion(typeFrom, typeTo, isChecked, locals);
		}

		internal static void EmitHasValue(this ILGenerator il, Type nullableType)
		{
			MethodInfo method = nullableType.GetMethod("get_HasValue", BindingFlags.Instance | BindingFlags.Public);
			il.Emit(OpCodes.Call, method);
		}

		internal static void EmitGetValue(this ILGenerator il, Type nullableType)
		{
			MethodInfo method = nullableType.GetMethod("get_Value", BindingFlags.Instance | BindingFlags.Public);
			il.Emit(OpCodes.Call, method);
		}

		internal static void EmitGetValueOrDefault(this ILGenerator il, Type nullableType)
		{
			MethodInfo method = nullableType.GetMethod("GetValueOrDefault", Type.EmptyTypes);
			il.Emit(OpCodes.Call, method);
		}

		internal static void EmitArray<T>(this ILGenerator il, T[] items, ILocalCache locals)
		{
			il.EmitPrimitive(items.Length);
			il.Emit(OpCodes.Newarr, typeof(T));
			for (int i = 0; i < items.Length; i++)
			{
				il.Emit(OpCodes.Dup);
				il.EmitPrimitive(i);
				il.TryEmitConstant(items[i], typeof(T), locals);
				il.EmitStoreElement(typeof(T));
			}
		}

		internal static void EmitArray(this ILGenerator il, Type elementType, int count)
		{
			il.EmitPrimitive(count);
			il.Emit(OpCodes.Newarr, elementType);
		}

		internal static void EmitArray(this ILGenerator il, Type arrayType)
		{
			if (arrayType.IsSZArray)
			{
				il.Emit(OpCodes.Newarr, arrayType.GetElementType());
				return;
			}
			Type[] array = new Type[arrayType.GetArrayRank()];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = typeof(int);
			}
			ConstructorInfo constructor = arrayType.GetConstructor(array);
			il.EmitNew(constructor);
		}

		private static void EmitDecimal(this ILGenerator il, decimal value)
		{
			int[] bits = decimal.GetBits(value);
			int num = (bits[3] & int.MaxValue) >> 16;
			if (num == 0)
			{
				if (-2147483648m <= value)
				{
					if (value <= 2147483647m)
					{
						int num2 = decimal.ToInt32(value);
						switch (num2)
						{
						case -1:
							il.Emit(OpCodes.Ldsfld, CachedReflectionInfo.Decimal_MinusOne);
							return;
						case 0:
							il.EmitDefault(typeof(decimal), null);
							return;
						case 1:
							il.Emit(OpCodes.Ldsfld, CachedReflectionInfo.Decimal_One);
							return;
						default:
							il.EmitPrimitive(num2);
							il.EmitNew(CachedReflectionInfo.Decimal_Ctor_Int32);
							return;
						}
					}
					else if (value <= 4294967295m)
					{
						il.EmitPrimitive(decimal.ToUInt32(value));
						il.EmitNew(CachedReflectionInfo.Decimal_Ctor_UInt32);
						return;
					}
				}
				if (-9223372036854775808m <= value)
				{
					if (value <= 9223372036854775807m)
					{
						il.EmitPrimitive(decimal.ToInt64(value));
						il.EmitNew(CachedReflectionInfo.Decimal_Ctor_Int64);
						return;
					}
					if (value <= 18446744073709551615m)
					{
						il.EmitPrimitive(decimal.ToUInt64(value));
						il.EmitNew(CachedReflectionInfo.Decimal_Ctor_UInt64);
						return;
					}
					if (value == 79228162514264337593543950335m)
					{
						il.Emit(OpCodes.Ldsfld, CachedReflectionInfo.Decimal_MaxValue);
						return;
					}
				}
				else if (value == -79228162514264337593543950335m)
				{
					il.Emit(OpCodes.Ldsfld, CachedReflectionInfo.Decimal_MinValue);
					return;
				}
			}
			il.EmitPrimitive(bits[0]);
			il.EmitPrimitive(bits[1]);
			il.EmitPrimitive(bits[2]);
			il.EmitPrimitive(((long)bits[3] & (long)((ulong)int.MinValue)) != 0L);
			il.EmitPrimitive((int)((byte)num));
			il.EmitNew(CachedReflectionInfo.Decimal_Ctor_Int32_Int32_Int32_Bool_Byte);
		}

		internal static void EmitDefault(this ILGenerator il, Type type, ILocalCache locals)
		{
			switch (type.GetTypeCode())
			{
			case TypeCode.Empty:
			case TypeCode.DBNull:
			case TypeCode.String:
				break;
			case TypeCode.Object:
				if (type.IsValueType)
				{
					LocalBuilder local = locals.GetLocal(type);
					il.Emit(OpCodes.Ldloca, local);
					il.Emit(OpCodes.Initobj, type);
					il.Emit(OpCodes.Ldloc, local);
					locals.FreeLocal(local);
					return;
				}
				break;
			case TypeCode.Boolean:
			case TypeCode.Char:
			case TypeCode.SByte:
			case TypeCode.Byte:
			case TypeCode.Int16:
			case TypeCode.UInt16:
			case TypeCode.Int32:
			case TypeCode.UInt32:
				il.Emit(OpCodes.Ldc_I4_0);
				return;
			case TypeCode.Int64:
			case TypeCode.UInt64:
				il.Emit(OpCodes.Ldc_I4_0);
				il.Emit(OpCodes.Conv_I8);
				return;
			case TypeCode.Single:
				il.Emit(OpCodes.Ldc_R4, 0f);
				return;
			case TypeCode.Double:
				il.Emit(OpCodes.Ldc_R8, 0.0);
				return;
			case TypeCode.Decimal:
				il.Emit(OpCodes.Ldsfld, CachedReflectionInfo.Decimal_Zero);
				return;
			case TypeCode.DateTime:
				il.Emit(OpCodes.Ldsfld, CachedReflectionInfo.DateTime_MinValue);
				return;
			case (TypeCode)17:
				goto IL_0111;
			default:
				goto IL_0111;
			}
			il.Emit(OpCodes.Ldnull);
			return;
			IL_0111:
			throw ContractUtils.Unreachable;
		}
	}
}
