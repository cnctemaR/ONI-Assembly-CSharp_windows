using System;
using System.ComponentModel;
using System.Reflection;
using System.Reflection.Emit;
using MonoMod.Utils;

namespace HarmonyLib
{
	public static class FastAccess
	{
		public static InstantiationHandler<T> CreateInstantiationHandler<T>()
		{
			ConstructorInfo constructor = typeof(T).GetConstructor(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, Array.Empty<Type>(), null);
			if (constructor == null)
			{
				throw new ApplicationException(string.Format("The type {0} must declare an empty constructor (the constructor may be private, internal, protected, protected internal, or public).", typeof(T)));
			}
			DynamicMethodDefinition dynamicMethodDefinition = new DynamicMethodDefinition("InstantiateObject_" + typeof(T).Name, typeof(T), null);
			ILGenerator ilgenerator = dynamicMethodDefinition.GetILGenerator();
			ilgenerator.Emit(OpCodes.Newobj, constructor);
			ilgenerator.Emit(OpCodes.Ret);
			return dynamicMethodDefinition.Generate().CreateDelegate<InstantiationHandler<T>>();
		}

		[Obsolete("Use AccessTools.MethodDelegate<Func<T, S>>(PropertyInfo.GetGetMethod(true))")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public static GetterHandler<T, S> CreateGetterHandler<T, S>(PropertyInfo propertyInfo)
		{
			MethodInfo getMethod = propertyInfo.GetGetMethod(true);
			DynamicMethodDefinition dynamicMethodDefinition = FastAccess.CreateGetDynamicMethod<T, S>(propertyInfo.DeclaringType);
			ILGenerator ilgenerator = dynamicMethodDefinition.GetILGenerator();
			ilgenerator.Emit(OpCodes.Ldarg_0);
			ilgenerator.Emit(OpCodes.Call, getMethod);
			ilgenerator.Emit(OpCodes.Ret);
			return dynamicMethodDefinition.Generate().CreateDelegate<GetterHandler<T, S>>();
		}

		[Obsolete("Use AccessTools.FieldRefAccess<T, S>(fieldInfo)")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public static GetterHandler<T, S> CreateGetterHandler<T, S>(FieldInfo fieldInfo)
		{
			DynamicMethodDefinition dynamicMethodDefinition = FastAccess.CreateGetDynamicMethod<T, S>(fieldInfo.DeclaringType);
			ILGenerator ilgenerator = dynamicMethodDefinition.GetILGenerator();
			ilgenerator.Emit(OpCodes.Ldarg_0);
			ilgenerator.Emit(OpCodes.Ldfld, fieldInfo);
			ilgenerator.Emit(OpCodes.Ret);
			return dynamicMethodDefinition.Generate().CreateDelegate<GetterHandler<T, S>>();
		}

		[Obsolete("Use AccessTools.FieldRefAccess<T, S>(name) for fields and AccessTools.MethodDelegate<Func<T, S>>(AccessTools.PropertyGetter(typeof(T), name)) for properties")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public static GetterHandler<T, S> CreateFieldGetter<T, S>(params string[] names)
		{
			foreach (string text in names)
			{
				FieldInfo field = typeof(T).GetField(text, AccessTools.all);
				if (field != null)
				{
					return FastAccess.CreateGetterHandler<T, S>(field);
				}
				PropertyInfo property = typeof(T).GetProperty(text, AccessTools.all);
				if (property != null)
				{
					return FastAccess.CreateGetterHandler<T, S>(property);
				}
			}
			return null;
		}

		[Obsolete("Use AccessTools.MethodDelegate<Action<T, S>>(PropertyInfo.GetSetMethod(true))")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public static SetterHandler<T, S> CreateSetterHandler<T, S>(PropertyInfo propertyInfo)
		{
			MethodInfo setMethod = propertyInfo.GetSetMethod(true);
			DynamicMethodDefinition dynamicMethodDefinition = FastAccess.CreateSetDynamicMethod<T, S>(propertyInfo.DeclaringType);
			ILGenerator ilgenerator = dynamicMethodDefinition.GetILGenerator();
			ilgenerator.Emit(OpCodes.Ldarg_0);
			ilgenerator.Emit(OpCodes.Ldarg_1);
			ilgenerator.Emit(OpCodes.Call, setMethod);
			ilgenerator.Emit(OpCodes.Ret);
			return dynamicMethodDefinition.Generate().CreateDelegate<SetterHandler<T, S>>();
		}

		[Obsolete("Use AccessTools.FieldRefAccess<T, S>(fieldInfo)")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public static SetterHandler<T, S> CreateSetterHandler<T, S>(FieldInfo fieldInfo)
		{
			DynamicMethodDefinition dynamicMethodDefinition = FastAccess.CreateSetDynamicMethod<T, S>(fieldInfo.DeclaringType);
			ILGenerator ilgenerator = dynamicMethodDefinition.GetILGenerator();
			ilgenerator.Emit(OpCodes.Ldarg_0);
			ilgenerator.Emit(OpCodes.Ldarg_1);
			ilgenerator.Emit(OpCodes.Stfld, fieldInfo);
			ilgenerator.Emit(OpCodes.Ret);
			return dynamicMethodDefinition.Generate().CreateDelegate<SetterHandler<T, S>>();
		}

		private static DynamicMethodDefinition CreateGetDynamicMethod<T, S>(Type type)
		{
			return new DynamicMethodDefinition("DynamicGet_" + type.Name, typeof(S), new Type[] { typeof(T) });
		}

		private static DynamicMethodDefinition CreateSetDynamicMethod<T, S>(Type type)
		{
			return new DynamicMethodDefinition("DynamicSet_" + type.Name, typeof(void), new Type[]
			{
				typeof(T),
				typeof(S)
			});
		}
	}
}
