using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace FileHelpers
{
	internal static class ReflectionHelper
	{
		public static ObjectToValuesDelegate ObjectToValuesMethod(Type recordType, FieldInfo[] fields)
		{
			DynamicMethod dynamicMethod = new DynamicMethod("FileHelpersDynamic_GetAllValues", MethodAttributes.FamANDAssem | MethodAttributes.Family | MethodAttributes.Static, CallingConventions.Standard, typeof(object[]), new Type[] { typeof(object) }, recordType, true);
			ILGenerator ilgenerator = dynamicMethod.GetILGenerator();
			ilgenerator.DeclareLocal(typeof(object[]));
			ilgenerator.DeclareLocal(recordType);
			ilgenerator.Emit(OpCodes.Ldc_I4, fields.Length);
			ilgenerator.Emit(OpCodes.Newarr, typeof(object));
			ilgenerator.Emit(OpCodes.Stloc_0);
			ilgenerator.Emit(OpCodes.Ldarg_0);
			ilgenerator.Emit(OpCodes.Castclass, recordType);
			ilgenerator.Emit(OpCodes.Stloc_1);
			for (int i = 0; i < fields.Length; i++)
			{
				FieldInfo fieldInfo = fields[i];
				ilgenerator.Emit(OpCodes.Ldloc_0);
				ilgenerator.Emit(OpCodes.Ldc_I4, i);
				ilgenerator.Emit(OpCodes.Ldloc_1);
				ilgenerator.Emit(OpCodes.Ldfld, fieldInfo);
				if (fieldInfo.FieldType.IsValueType)
				{
					ilgenerator.Emit(OpCodes.Box, fieldInfo.FieldType);
				}
				ilgenerator.Emit(OpCodes.Stelem_Ref);
			}
			ilgenerator.Emit(OpCodes.Ldloc_0);
			ilgenerator.Emit(OpCodes.Ret);
			return (ObjectToValuesDelegate)dynamicMethod.CreateDelegate(typeof(ObjectToValuesDelegate));
		}

		public static ConstructorInfo GetDefaultConstructor(Type recordType)
		{
			return recordType.GetConstructor(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, Type.EmptyTypes, new ParameterModifier[0]);
		}

		public static CreateAndAssignDelegate CreateAndAssignValuesMethod(Type recordType, FieldInfo[] fields)
		{
			DynamicMethod dynamicMethod = new DynamicMethod("FileHelpersDynamicCreateAndAssign", MethodAttributes.FamANDAssem | MethodAttributes.Family | MethodAttributes.Static, CallingConventions.Standard, typeof(object), new Type[] { typeof(object[]) }, recordType, true);
			ILGenerator ilgenerator = dynamicMethod.GetILGenerator();
			ilgenerator.DeclareLocal(recordType);
			ilgenerator.Emit(OpCodes.Newobj, ReflectionHelper.GetDefaultConstructor(recordType));
			ilgenerator.Emit(OpCodes.Stloc_0);
			for (int i = 0; i < fields.Length; i++)
			{
				FieldInfo fieldInfo = fields[i];
				ilgenerator.Emit(OpCodes.Ldloc_0);
				ilgenerator.Emit(OpCodes.Ldarg_0);
				ilgenerator.Emit(OpCodes.Ldc_I4, i);
				ilgenerator.Emit(OpCodes.Ldelem_Ref);
				if (fieldInfo.FieldType.IsValueType)
				{
					ilgenerator.Emit(OpCodes.Unbox_Any, fieldInfo.FieldType);
				}
				else
				{
					ilgenerator.Emit(OpCodes.Castclass, fieldInfo.FieldType);
				}
				ilgenerator.Emit(OpCodes.Stfld, fieldInfo);
			}
			ilgenerator.Emit(OpCodes.Ldloc_0);
			ilgenerator.Emit(OpCodes.Ret);
			return (CreateAndAssignDelegate)dynamicMethod.CreateDelegate(typeof(CreateAndAssignDelegate));
		}

		public static AssignDelegate AssignValuesMethod(Type recordType, FieldInfo[] fields)
		{
			DynamicMethod dynamicMethod = new DynamicMethod("FileHelpersDynamic_Assign", MethodAttributes.FamANDAssem | MethodAttributes.Family | MethodAttributes.Static, CallingConventions.Standard, null, new Type[]
			{
				typeof(object),
				typeof(object[])
			}, recordType, true);
			ILGenerator ilgenerator = dynamicMethod.GetILGenerator();
			ilgenerator.DeclareLocal(recordType);
			ilgenerator.Emit(OpCodes.Ldarg_0);
			ilgenerator.Emit(OpCodes.Castclass, recordType);
			ilgenerator.Emit(OpCodes.Stloc_0);
			for (int i = 0; i < fields.Length; i++)
			{
				FieldInfo fieldInfo = fields[i];
				ilgenerator.Emit(OpCodes.Ldloc_0);
				ilgenerator.Emit(OpCodes.Ldarg_1);
				ilgenerator.Emit(OpCodes.Ldc_I4, i);
				ilgenerator.Emit(OpCodes.Ldelem_Ref);
				if (fieldInfo.FieldType.IsValueType)
				{
					ilgenerator.Emit(OpCodes.Unbox_Any, fieldInfo.FieldType);
				}
				else
				{
					ilgenerator.Emit(OpCodes.Castclass, fieldInfo.FieldType);
				}
				ilgenerator.Emit(OpCodes.Stfld, fieldInfo);
			}
			ilgenerator.Emit(OpCodes.Ret);
			return (AssignDelegate)dynamicMethod.CreateDelegate(typeof(AssignDelegate));
		}

		public static CreateObjectDelegate CreateFastConstructor(Type recordType)
		{
			DynamicMethod dynamicMethod = new DynamicMethod("FileHelpersDynamicCreateRecordFast", MethodAttributes.FamANDAssem | MethodAttributes.Family | MethodAttributes.Static, CallingConventions.Standard, typeof(object), new Type[] { typeof(object[]) }, recordType, true);
			ILGenerator ilgenerator = dynamicMethod.GetILGenerator();
			ilgenerator.Emit(OpCodes.Newobj, ReflectionHelper.GetDefaultConstructor(recordType));
			ilgenerator.Emit(OpCodes.Ret);
			return (CreateObjectDelegate)dynamicMethod.CreateDelegate(typeof(CreateObjectDelegate));
		}

		public static GetFieldValueCallback CreateGetFieldMethod(FieldInfo fi)
		{
			DynamicMethod dynamicMethod = new DynamicMethod("FileHelpersDynamic_GetValue" + fi.Name, MethodAttributes.FamANDAssem | MethodAttributes.Family | MethodAttributes.Static, CallingConventions.Standard, typeof(object), new Type[] { typeof(object) }, fi.DeclaringType, true);
			ILGenerator ilgenerator = dynamicMethod.GetILGenerator();
			ilgenerator.Emit(OpCodes.Ldarg_0);
			ilgenerator.Emit(OpCodes.Castclass, fi.DeclaringType);
			ilgenerator.Emit(OpCodes.Ldfld, fi);
			ilgenerator.Emit(OpCodes.Ret);
			return (GetFieldValueCallback)dynamicMethod.CreateDelegate(typeof(GetFieldValueCallback));
		}

		public static IEnumerable<FieldInfo> RecursiveGetFields(Type currentType)
		{
			if (currentType.BaseType != null && !currentType.IsDefined(typeof(IgnoreInheritedClassAttribute), false))
			{
				foreach (FieldInfo item in ReflectionHelper.RecursiveGetFields(currentType.BaseType))
				{
					yield return item;
				}
			}
			if (currentType != typeof(object))
			{
				FieldInfoCacheManipulator.ResetFieldInfoCache(currentType);
				foreach (FieldInfo fi in currentType.GetFields(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
				{
					if (!typeof(Delegate).IsAssignableFrom(fi.FieldType))
					{
						yield return fi;
					}
				}
			}
			yield break;
		}
	}
}
