using System;
using System.Reflection;
using System.Reflection.Emit;

namespace Harmony
{
	public class FastAccess
	{
		public static InstantiationHandler CreateInstantiationHandler(Type type)
		{
			ConstructorInfo constructor = type.GetConstructor(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, new Type[0], null);
			bool flag = constructor == null;
			if (flag)
			{
				throw new ApplicationException(string.Format("The type {0} must declare an empty constructor (the constructor may be private, internal, protected, protected internal, or public).", type));
			}
			DynamicMethod dynamicMethod = new DynamicMethod("InstantiateObject_" + type.Name, MethodAttributes.FamANDAssem | MethodAttributes.Family | MethodAttributes.Static, CallingConventions.Standard, typeof(object), null, type, true);
			ILGenerator ilgenerator = dynamicMethod.GetILGenerator();
			ilgenerator.Emit(OpCodes.Newobj, constructor);
			ilgenerator.Emit(OpCodes.Ret);
			return (InstantiationHandler)dynamicMethod.CreateDelegate(typeof(InstantiationHandler));
		}

		public static GetterHandler CreateGetterHandler(PropertyInfo propertyInfo)
		{
			MethodInfo getMethod = propertyInfo.GetGetMethod(true);
			DynamicMethod dynamicMethod = FastAccess.CreateGetDynamicMethod(propertyInfo.DeclaringType);
			ILGenerator ilgenerator = dynamicMethod.GetILGenerator();
			ilgenerator.Emit(OpCodes.Ldarg_0);
			ilgenerator.Emit(OpCodes.Call, getMethod);
			FastAccess.BoxIfNeeded(getMethod.ReturnType, ilgenerator);
			ilgenerator.Emit(OpCodes.Ret);
			return (GetterHandler)dynamicMethod.CreateDelegate(typeof(GetterHandler));
		}

		public static GetterHandler CreateGetterHandler(FieldInfo fieldInfo)
		{
			DynamicMethod dynamicMethod = FastAccess.CreateGetDynamicMethod(fieldInfo.DeclaringType);
			ILGenerator ilgenerator = dynamicMethod.GetILGenerator();
			ilgenerator.Emit(OpCodes.Ldarg_0);
			ilgenerator.Emit(OpCodes.Ldfld, fieldInfo);
			FastAccess.BoxIfNeeded(fieldInfo.FieldType, ilgenerator);
			ilgenerator.Emit(OpCodes.Ret);
			return (GetterHandler)dynamicMethod.CreateDelegate(typeof(GetterHandler));
		}

		public static GetterHandler CreateFieldGetter(Type type, params string[] names)
		{
			int i = 0;
			while (i < names.Length)
			{
				string text = names[i];
				bool flag = AccessTools.Field(typeof(ILGenerator), text) != null;
				GetterHandler getterHandler;
				if (flag)
				{
					getterHandler = FastAccess.CreateGetterHandler(AccessTools.Field(type, text));
				}
				else
				{
					bool flag2 = AccessTools.Property(typeof(ILGenerator), text) != null;
					if (!flag2)
					{
						i++;
						continue;
					}
					getterHandler = FastAccess.CreateGetterHandler(AccessTools.Property(type, text));
				}
				return getterHandler;
			}
			return null;
		}

		public static SetterHandler CreateSetterHandler(PropertyInfo propertyInfo)
		{
			MethodInfo setMethod = propertyInfo.GetSetMethod(true);
			DynamicMethod dynamicMethod = FastAccess.CreateSetDynamicMethod(propertyInfo.DeclaringType);
			ILGenerator ilgenerator = dynamicMethod.GetILGenerator();
			ilgenerator.Emit(OpCodes.Ldarg_0);
			ilgenerator.Emit(OpCodes.Ldarg_1);
			FastAccess.UnboxIfNeeded(setMethod.GetParameters()[0].ParameterType, ilgenerator);
			ilgenerator.Emit(OpCodes.Call, setMethod);
			ilgenerator.Emit(OpCodes.Ret);
			return (SetterHandler)dynamicMethod.CreateDelegate(typeof(SetterHandler));
		}

		public static SetterHandler CreateSetterHandler(FieldInfo fieldInfo)
		{
			DynamicMethod dynamicMethod = FastAccess.CreateSetDynamicMethod(fieldInfo.DeclaringType);
			ILGenerator ilgenerator = dynamicMethod.GetILGenerator();
			ilgenerator.Emit(OpCodes.Ldarg_0);
			ilgenerator.Emit(OpCodes.Ldarg_1);
			FastAccess.UnboxIfNeeded(fieldInfo.FieldType, ilgenerator);
			ilgenerator.Emit(OpCodes.Stfld, fieldInfo);
			ilgenerator.Emit(OpCodes.Ret);
			return (SetterHandler)dynamicMethod.CreateDelegate(typeof(SetterHandler));
		}

		private static DynamicMethod CreateGetDynamicMethod(Type type)
		{
			return new DynamicMethod("DynamicGet_" + type.Name, typeof(object), new Type[] { typeof(object) }, type, true);
		}

		private static DynamicMethod CreateSetDynamicMethod(Type type)
		{
			return new DynamicMethod("DynamicSet_" + type.Name, typeof(void), new Type[]
			{
				typeof(object),
				typeof(object)
			}, type, true);
		}

		private static void BoxIfNeeded(Type type, ILGenerator generator)
		{
			bool isValueType = type.IsValueType;
			if (isValueType)
			{
				generator.Emit(OpCodes.Box, type);
			}
		}

		private static void UnboxIfNeeded(Type type, ILGenerator generator)
		{
			bool isValueType = type.IsValueType;
			if (isValueType)
			{
				generator.Emit(OpCodes.Unbox_Any, type);
			}
		}
	}
}
