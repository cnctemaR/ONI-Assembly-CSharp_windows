using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace Microsoft.Internal
{
	internal static class GenerationServices
	{
		public static ILGenerator CreateGeneratorForPublicConstructor(this TypeBuilder typeBuilder, Type[] ctrArgumentTypes)
		{
			ILGenerator ilgenerator = typeBuilder.DefineConstructor(MethodAttributes.Public, CallingConventions.Standard, ctrArgumentTypes).GetILGenerator();
			ilgenerator.Emit(OpCodes.Ldarg_0);
			ilgenerator.Emit(OpCodes.Call, GenerationServices.ObjectCtor);
			return ilgenerator;
		}

		public static void LoadValue(this ILGenerator ilGenerator, object value)
		{
			Assumes.NotNull<ILGenerator>(ilGenerator);
			if (value == null)
			{
				ilGenerator.LoadNull();
				return;
			}
			Type type = value.GetType();
			object obj = value;
			if (type.IsEnum)
			{
				obj = Convert.ChangeType(value, Enum.GetUnderlyingType(type), null);
				type = obj.GetType();
			}
			if (type == GenerationServices.StringType)
			{
				ilGenerator.LoadString((string)obj);
				return;
			}
			if (GenerationServices.TypeType.IsAssignableFrom(type))
			{
				ilGenerator.LoadTypeOf((Type)obj);
				return;
			}
			if (GenerationServices.IEnumerableType.IsAssignableFrom(type))
			{
				ilGenerator.LoadEnumerable((IEnumerable)obj);
				return;
			}
			if (type == GenerationServices.CharType || type == GenerationServices.BooleanType || type == GenerationServices.ByteType || type == GenerationServices.SByteType || type == GenerationServices.Int16Type || type == GenerationServices.UInt16Type || type == GenerationServices.Int32Type)
			{
				ilGenerator.LoadInt((int)Convert.ChangeType(obj, typeof(int), CultureInfo.InvariantCulture));
				return;
			}
			if (type == GenerationServices.UInt32Type)
			{
				ilGenerator.LoadInt((int)((uint)obj));
				return;
			}
			if (type == GenerationServices.Int64Type)
			{
				ilGenerator.LoadLong((long)obj);
				return;
			}
			if (type == GenerationServices.UInt64Type)
			{
				ilGenerator.LoadLong((long)((ulong)obj));
				return;
			}
			if (type == GenerationServices.SingleType)
			{
				ilGenerator.LoadFloat((float)obj);
				return;
			}
			if (type == GenerationServices.DoubleType)
			{
				ilGenerator.LoadDouble((double)obj);
				return;
			}
			throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, Strings.InvalidMetadataValue, value.GetType().FullName));
		}

		public static void AddItemToLocalDictionary(this ILGenerator ilGenerator, LocalBuilder dictionary, object key, object value)
		{
			Assumes.NotNull<ILGenerator>(ilGenerator);
			Assumes.NotNull<LocalBuilder>(dictionary);
			Assumes.NotNull<object>(key);
			Assumes.NotNull<object>(value);
			ilGenerator.Emit(OpCodes.Ldloc, dictionary);
			ilGenerator.LoadValue(key);
			ilGenerator.LoadValue(value);
			ilGenerator.Emit(OpCodes.Callvirt, GenerationServices.DictionaryAdd);
		}

		public static void AddLocalToLocalDictionary(this ILGenerator ilGenerator, LocalBuilder dictionary, object key, LocalBuilder value)
		{
			Assumes.NotNull<ILGenerator>(ilGenerator);
			Assumes.NotNull<LocalBuilder>(dictionary);
			Assumes.NotNull<object>(key);
			Assumes.NotNull<LocalBuilder>(value);
			ilGenerator.Emit(OpCodes.Ldloc, dictionary);
			ilGenerator.LoadValue(key);
			ilGenerator.Emit(OpCodes.Ldloc, value);
			ilGenerator.Emit(OpCodes.Callvirt, GenerationServices.DictionaryAdd);
		}

		public static void GetExceptionDataAndStoreInLocal(this ILGenerator ilGenerator, LocalBuilder exception, LocalBuilder dataStore)
		{
			Assumes.NotNull<ILGenerator>(ilGenerator);
			Assumes.NotNull<LocalBuilder>(exception);
			Assumes.NotNull<LocalBuilder>(dataStore);
			ilGenerator.Emit(OpCodes.Ldloc, exception);
			ilGenerator.Emit(OpCodes.Callvirt, GenerationServices.ExceptionGetData);
			ilGenerator.Emit(OpCodes.Stloc, dataStore);
		}

		private static void LoadEnumerable(this ILGenerator ilGenerator, IEnumerable enumerable)
		{
			Assumes.NotNull<ILGenerator>(ilGenerator);
			Assumes.NotNull<IEnumerable>(enumerable);
			Type type = null;
			Type type2;
			if (ReflectionServices.TryGetGenericInterfaceType(enumerable.GetType(), GenerationServices.IEnumerableTypeofT, out type))
			{
				type2 = type.GetGenericArguments()[0];
			}
			else
			{
				type2 = typeof(object);
			}
			Type type3 = type2.MakeArrayType();
			LocalBuilder localBuilder = ilGenerator.DeclareLocal(type3);
			ilGenerator.LoadInt(enumerable.Cast<object>().Count<object>());
			ilGenerator.Emit(OpCodes.Newarr, type2);
			ilGenerator.Emit(OpCodes.Stloc, localBuilder);
			int num = 0;
			foreach (object obj in enumerable)
			{
				ilGenerator.Emit(OpCodes.Ldloc, localBuilder);
				ilGenerator.LoadInt(num);
				ilGenerator.LoadValue(obj);
				if (GenerationServices.IsBoxingRequiredForValue(obj) && !type2.IsValueType)
				{
					ilGenerator.Emit(OpCodes.Box, obj.GetType());
				}
				ilGenerator.Emit(OpCodes.Stelem, type2);
				num++;
			}
			ilGenerator.Emit(OpCodes.Ldloc, localBuilder);
		}

		private static bool IsBoxingRequiredForValue(object value)
		{
			return value != null && value.GetType().IsValueType;
		}

		private static void LoadNull(this ILGenerator ilGenerator)
		{
			ilGenerator.Emit(OpCodes.Ldnull);
		}

		private static void LoadString(this ILGenerator ilGenerator, string s)
		{
			Assumes.NotNull<ILGenerator>(ilGenerator);
			if (s == null)
			{
				ilGenerator.LoadNull();
				return;
			}
			ilGenerator.Emit(OpCodes.Ldstr, s);
		}

		private static void LoadInt(this ILGenerator ilGenerator, int value)
		{
			Assumes.NotNull<ILGenerator>(ilGenerator);
			ilGenerator.Emit(OpCodes.Ldc_I4, value);
		}

		private static void LoadLong(this ILGenerator ilGenerator, long value)
		{
			Assumes.NotNull<ILGenerator>(ilGenerator);
			ilGenerator.Emit(OpCodes.Ldc_I8, value);
		}

		private static void LoadFloat(this ILGenerator ilGenerator, float value)
		{
			Assumes.NotNull<ILGenerator>(ilGenerator);
			ilGenerator.Emit(OpCodes.Ldc_R4, value);
		}

		private static void LoadDouble(this ILGenerator ilGenerator, double value)
		{
			Assumes.NotNull<ILGenerator>(ilGenerator);
			ilGenerator.Emit(OpCodes.Ldc_R8, value);
		}

		private static void LoadTypeOf(this ILGenerator ilGenerator, Type type)
		{
			Assumes.NotNull<ILGenerator>(ilGenerator);
			ilGenerator.Emit(OpCodes.Ldtoken, type);
			ilGenerator.EmitCall(OpCodes.Call, GenerationServices._typeGetTypeFromHandleMethod, null);
		}

		private static readonly MethodInfo _typeGetTypeFromHandleMethod = typeof(Type).GetMethod("GetTypeFromHandle");

		private static readonly Type TypeType = typeof(Type);

		private static readonly Type StringType = typeof(string);

		private static readonly Type CharType = typeof(char);

		private static readonly Type BooleanType = typeof(bool);

		private static readonly Type ByteType = typeof(byte);

		private static readonly Type SByteType = typeof(sbyte);

		private static readonly Type Int16Type = typeof(short);

		private static readonly Type UInt16Type = typeof(ushort);

		private static readonly Type Int32Type = typeof(int);

		private static readonly Type UInt32Type = typeof(uint);

		private static readonly Type Int64Type = typeof(long);

		private static readonly Type UInt64Type = typeof(ulong);

		private static readonly Type DoubleType = typeof(double);

		private static readonly Type SingleType = typeof(float);

		private static readonly Type IEnumerableTypeofT = typeof(IEnumerable<>);

		private static readonly Type IEnumerableType = typeof(IEnumerable);

		private static readonly MethodInfo ExceptionGetData = typeof(Exception).GetProperty("Data").GetGetMethod();

		private static readonly MethodInfo DictionaryAdd = typeof(IDictionary).GetMethod("Add");

		private static readonly ConstructorInfo ObjectCtor = typeof(object).GetConstructor(Type.EmptyTypes);
	}
}
