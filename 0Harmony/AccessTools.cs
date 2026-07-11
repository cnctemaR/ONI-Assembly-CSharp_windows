using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.Serialization;

namespace Harmony
{
	public static class AccessTools
	{
		public static Type TypeByName(string name)
		{
			Type type = Type.GetType(name, false);
			bool flag = type == null;
			if (flag)
			{
				type = AppDomain.CurrentDomain.GetAssemblies().SelectMany<Assembly, Type>((Assembly x) => x.GetTypes()).FirstOrDefault<Type>((Type x) => x.FullName == name);
			}
			bool flag2 = type == null;
			if (flag2)
			{
				type = AppDomain.CurrentDomain.GetAssemblies().SelectMany<Assembly, Type>((Assembly x) => x.GetTypes()).FirstOrDefault<Type>((Type x) => x.Name == name);
			}
			return type;
		}

		public static T FindIncludingBaseTypes<T>(Type type, Func<Type, T> action)
		{
			T t;
			for (;;)
			{
				t = action(type);
				bool flag = t != null;
				if (flag)
				{
					break;
				}
				bool flag2 = type == typeof(object);
				if (flag2)
				{
					goto Block_2;
				}
				type = type.BaseType;
			}
			return t;
			Block_2:
			return default(T);
		}

		public static T FindIncludingInnerTypes<T>(Type type, Func<Type, T> action)
		{
			T t = action(type);
			bool flag = t != null;
			T t2;
			if (flag)
			{
				t2 = t;
			}
			else
			{
				foreach (Type type2 in type.GetNestedTypes(AccessTools.all))
				{
					t = AccessTools.FindIncludingInnerTypes<T>(type2, action);
					bool flag2 = t != null;
					if (flag2)
					{
						break;
					}
				}
				t2 = t;
			}
			return t2;
		}

		public static FieldInfo Field(Type type, string name)
		{
			bool flag = type == null || name == null;
			FieldInfo fieldInfo;
			if (flag)
			{
				fieldInfo = null;
			}
			else
			{
				fieldInfo = AccessTools.FindIncludingBaseTypes<FieldInfo>(type, (Type t) => t.GetField(name, AccessTools.all));
			}
			return fieldInfo;
		}

		public static FieldInfo Field(Type type, int idx)
		{
			return AccessTools.GetDeclaredFields(type).ElementAtOrDefault<FieldInfo>(idx);
		}

		public static PropertyInfo DeclaredProperty(Type type, string name)
		{
			bool flag = type == null || name == null;
			PropertyInfo propertyInfo;
			if (flag)
			{
				propertyInfo = null;
			}
			else
			{
				propertyInfo = type.GetProperty(name, AccessTools.all);
			}
			return propertyInfo;
		}

		public static PropertyInfo Property(Type type, string name)
		{
			bool flag = type == null || name == null;
			PropertyInfo propertyInfo;
			if (flag)
			{
				propertyInfo = null;
			}
			else
			{
				propertyInfo = AccessTools.FindIncludingBaseTypes<PropertyInfo>(type, (Type t) => t.GetProperty(name, AccessTools.all));
			}
			return propertyInfo;
		}

		public static MethodInfo DeclaredMethod(Type type, string name, Type[] parameters = null, Type[] generics = null)
		{
			bool flag = type == null || name == null;
			MethodInfo methodInfo;
			if (flag)
			{
				methodInfo = null;
			}
			else
			{
				ParameterModifier[] array = new ParameterModifier[0];
				bool flag2 = parameters == null;
				MethodInfo methodInfo2;
				if (flag2)
				{
					methodInfo2 = type.GetMethod(name, AccessTools.all);
				}
				else
				{
					methodInfo2 = type.GetMethod(name, AccessTools.all, null, parameters, array);
				}
				bool flag3 = methodInfo2 == null;
				if (flag3)
				{
					methodInfo = null;
				}
				else
				{
					bool flag4 = generics != null;
					if (flag4)
					{
						methodInfo2 = methodInfo2.MakeGenericMethod(generics);
					}
					methodInfo = methodInfo2;
				}
			}
			return methodInfo;
		}

		public static MethodInfo Method(Type type, string name, Type[] parameters = null, Type[] generics = null)
		{
			bool flag = type == null || name == null;
			MethodInfo methodInfo;
			if (flag)
			{
				methodInfo = null;
			}
			else
			{
				ParameterModifier[] modifiers = new ParameterModifier[0];
				bool flag2 = parameters == null;
				MethodInfo methodInfo2;
				if (flag2)
				{
					try
					{
						methodInfo2 = AccessTools.FindIncludingBaseTypes<MethodInfo>(type, (Type t) => t.GetMethod(name, AccessTools.all));
					}
					catch (AmbiguousMatchException)
					{
						methodInfo2 = AccessTools.FindIncludingBaseTypes<MethodInfo>(type, (Type t) => t.GetMethod(name, AccessTools.all, null, new Type[0], modifiers));
					}
				}
				else
				{
					methodInfo2 = AccessTools.FindIncludingBaseTypes<MethodInfo>(type, (Type t) => t.GetMethod(name, AccessTools.all, null, parameters, modifiers));
				}
				bool flag3 = methodInfo2 == null;
				if (flag3)
				{
					methodInfo = null;
				}
				else
				{
					bool flag4 = generics != null;
					if (flag4)
					{
						methodInfo2 = methodInfo2.MakeGenericMethod(generics);
					}
					methodInfo = methodInfo2;
				}
			}
			return methodInfo;
		}

		public static MethodInfo Method(string typeColonMethodname, Type[] parameters = null, Type[] generics = null)
		{
			bool flag = typeColonMethodname == null;
			MethodInfo methodInfo;
			if (flag)
			{
				methodInfo = null;
			}
			else
			{
				string[] array = typeColonMethodname.Split(new char[] { ':' });
				bool flag2 = array.Length != 2;
				if (flag2)
				{
					throw new ArgumentException("Method must be specified as 'Namespace.Type1.Type2:MethodName", "typeColonMethodname");
				}
				Type type = AccessTools.TypeByName(array[0]);
				methodInfo = AccessTools.Method(type, array[1], parameters, generics);
			}
			return methodInfo;
		}

		public static List<string> GetMethodNames(Type type)
		{
			bool flag = type == null;
			List<string> list;
			if (flag)
			{
				list = new List<string>();
			}
			else
			{
				list = (from m in type.GetMethods(AccessTools.all)
					select m.Name).ToList<string>();
			}
			return list;
		}

		public static List<string> GetMethodNames(object instance)
		{
			bool flag = instance == null;
			List<string> list;
			if (flag)
			{
				list = new List<string>();
			}
			else
			{
				list = AccessTools.GetMethodNames(instance.GetType());
			}
			return list;
		}

		public static ConstructorInfo DeclaredConstructor(Type type, Type[] parameters = null)
		{
			bool flag = type == null;
			ConstructorInfo constructorInfo;
			if (flag)
			{
				constructorInfo = null;
			}
			else
			{
				bool flag2 = parameters == null;
				if (flag2)
				{
					parameters = new Type[0];
				}
				constructorInfo = type.GetConstructor(AccessTools.all, null, parameters, new ParameterModifier[0]);
			}
			return constructorInfo;
		}

		public static ConstructorInfo Constructor(Type type, Type[] parameters = null)
		{
			bool flag = type == null;
			ConstructorInfo constructorInfo;
			if (flag)
			{
				constructorInfo = null;
			}
			else
			{
				bool flag2 = parameters == null;
				if (flag2)
				{
					parameters = new Type[0];
				}
				constructorInfo = AccessTools.FindIncludingBaseTypes<ConstructorInfo>(type, (Type t) => t.GetConstructor(AccessTools.all, null, parameters, new ParameterModifier[0]));
			}
			return constructorInfo;
		}

		public static List<ConstructorInfo> GetDeclaredConstructors(Type type)
		{
			return (from method in type.GetConstructors(AccessTools.all)
				where method.DeclaringType == type
				select method).ToList<ConstructorInfo>();
		}

		public static List<MethodInfo> GetDeclaredMethods(Type type)
		{
			return (from method in type.GetMethods(AccessTools.all)
				where method.DeclaringType == type
				select method).ToList<MethodInfo>();
		}

		public static List<PropertyInfo> GetDeclaredProperties(Type type)
		{
			return (from property in type.GetProperties(AccessTools.all)
				where property.DeclaringType == type
				select property).ToList<PropertyInfo>();
		}

		public static List<FieldInfo> GetDeclaredFields(Type type)
		{
			return (from field in type.GetFields(AccessTools.all)
				where field.DeclaringType == type
				select field).ToList<FieldInfo>();
		}

		public static Type GetReturnedType(MethodBase method)
		{
			ConstructorInfo constructorInfo = method as ConstructorInfo;
			bool flag = constructorInfo != null;
			Type type;
			if (flag)
			{
				type = typeof(void);
			}
			else
			{
				type = ((MethodInfo)method).ReturnType;
			}
			return type;
		}

		public static Type Inner(Type type, string name)
		{
			bool flag = type == null || name == null;
			Type type2;
			if (flag)
			{
				type2 = null;
			}
			else
			{
				type2 = AccessTools.FindIncludingBaseTypes<Type>(type, (Type t) => t.GetNestedType(name, AccessTools.all));
			}
			return type2;
		}

		public static Type FirstInner(Type type, Func<Type, bool> predicate)
		{
			bool flag = type == null || predicate == null;
			Type type2;
			if (flag)
			{
				type2 = null;
			}
			else
			{
				type2 = type.GetNestedTypes(AccessTools.all).FirstOrDefault<Type>((Type subType) => predicate(subType));
			}
			return type2;
		}

		public static MethodInfo FirstMethod(Type type, Func<MethodInfo, bool> predicate)
		{
			bool flag = type == null || predicate == null;
			MethodInfo methodInfo;
			if (flag)
			{
				methodInfo = null;
			}
			else
			{
				methodInfo = type.GetMethods(AccessTools.all).FirstOrDefault<MethodInfo>((MethodInfo method) => predicate(method));
			}
			return methodInfo;
		}

		public static ConstructorInfo FirstConstructor(Type type, Func<ConstructorInfo, bool> predicate)
		{
			bool flag = type == null || predicate == null;
			ConstructorInfo constructorInfo;
			if (flag)
			{
				constructorInfo = null;
			}
			else
			{
				constructorInfo = type.GetConstructors(AccessTools.all).FirstOrDefault<ConstructorInfo>((ConstructorInfo constructor) => predicate(constructor));
			}
			return constructorInfo;
		}

		public static PropertyInfo FirstProperty(Type type, Func<PropertyInfo, bool> predicate)
		{
			bool flag = type == null || predicate == null;
			PropertyInfo propertyInfo;
			if (flag)
			{
				propertyInfo = null;
			}
			else
			{
				propertyInfo = type.GetProperties(AccessTools.all).FirstOrDefault<PropertyInfo>((PropertyInfo property) => predicate(property));
			}
			return propertyInfo;
		}

		public static Type[] GetTypes(object[] parameters)
		{
			bool flag = parameters == null;
			Type[] array;
			if (flag)
			{
				array = new Type[0];
			}
			else
			{
				array = parameters.Select<object, Type>((object p) => (p == null) ? typeof(object) : p.GetType()).ToArray<Type>();
			}
			return array;
		}

		public static List<string> GetFieldNames(Type type)
		{
			bool flag = type == null;
			List<string> list;
			if (flag)
			{
				list = new List<string>();
			}
			else
			{
				list = (from f in type.GetFields(AccessTools.all)
					select f.Name).ToList<string>();
			}
			return list;
		}

		public static List<string> GetFieldNames(object instance)
		{
			bool flag = instance == null;
			List<string> list;
			if (flag)
			{
				list = new List<string>();
			}
			else
			{
				list = AccessTools.GetFieldNames(instance.GetType());
			}
			return list;
		}

		public static List<string> GetPropertyNames(Type type)
		{
			bool flag = type == null;
			List<string> list;
			if (flag)
			{
				list = new List<string>();
			}
			else
			{
				list = (from f in type.GetProperties(AccessTools.all)
					select f.Name).ToList<string>();
			}
			return list;
		}

		public static List<string> GetPropertyNames(object instance)
		{
			bool flag = instance == null;
			List<string> list;
			if (flag)
			{
				list = new List<string>();
			}
			else
			{
				list = AccessTools.GetPropertyNames(instance.GetType());
			}
			return list;
		}

		public static AccessTools.FieldRef<T, U> FieldRefAccess<T, U>(string fieldName)
		{
			FieldInfo field = typeof(T).GetField(fieldName, BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.NonPublic);
			bool flag = field == null;
			if (flag)
			{
				throw new MissingFieldException(typeof(T).Name, fieldName);
			}
			string text = "__refget_" + typeof(T).Name + "_fi_" + field.Name;
			DynamicMethod dynamicMethod = new DynamicMethod(text, typeof(U), new Type[] { typeof(T) }, typeof(T), true);
			Traverse traverse = Traverse.Create(dynamicMethod);
			traverse.Field("returnType").SetValue(typeof(U).MakeByRefType());
			traverse.Field("m_returnType").SetValue(typeof(U).MakeByRefType());
			ILGenerator ilgenerator = dynamicMethod.GetILGenerator();
			ilgenerator.Emit(OpCodes.Ldarg_0);
			ilgenerator.Emit(OpCodes.Ldflda, field);
			ilgenerator.Emit(OpCodes.Ret);
			return (AccessTools.FieldRef<T, U>)dynamicMethod.CreateDelegate(typeof(AccessTools.FieldRef<T, U>));
		}

		public static ref U FieldRefAccess<T, U>(T instance, string fieldName)
		{
			return AccessTools.FieldRefAccess<T, U>(fieldName)(instance);
		}

		public static void ThrowMissingMemberException(Type type, params string[] names)
		{
			string text = string.Join(",", AccessTools.GetFieldNames(type).ToArray());
			string text2 = string.Join(",", AccessTools.GetPropertyNames(type).ToArray());
			throw new MissingMemberException(string.Concat(new string[]
			{
				string.Join(",", names),
				"; available fields: ",
				text,
				"; available properties: ",
				text2
			}));
		}

		public static object GetDefaultValue(Type type)
		{
			bool flag = type == null;
			object obj;
			if (flag)
			{
				obj = null;
			}
			else
			{
				bool flag2 = type == typeof(void);
				if (flag2)
				{
					obj = null;
				}
				else
				{
					bool isValueType = type.IsValueType;
					if (isValueType)
					{
						obj = Activator.CreateInstance(type);
					}
					else
					{
						obj = null;
					}
				}
			}
			return obj;
		}

		public static object CreateInstance(Type type)
		{
			bool flag = type == null;
			if (flag)
			{
				throw new NullReferenceException("Cannot create instance for NULL type");
			}
			ConstructorInfo constructor = type.GetConstructor(BindingFlags.Instance | BindingFlags.Public, null, CallingConventions.Any, new Type[0], null);
			bool flag2 = constructor != null;
			object obj;
			if (flag2)
			{
				obj = Activator.CreateInstance(type);
			}
			else
			{
				obj = FormatterServices.GetUninitializedObject(type);
			}
			return obj;
		}

		public static object MakeDeepCopy(object source, Type resultType, Func<string, Traverse, Traverse, object> processor = null, string pathRoot = "")
		{
			bool flag = source == null;
			object obj;
			if (flag)
			{
				obj = null;
			}
			else
			{
				Type type = source.GetType();
				bool isPrimitive = type.IsPrimitive;
				if (isPrimitive)
				{
					obj = source;
				}
				else
				{
					bool isEnum = type.IsEnum;
					if (isEnum)
					{
						obj = Enum.ToObject(resultType, (int)source);
					}
					else
					{
						bool flag2 = type.IsGenericType && resultType.IsGenericType;
						if (flag2)
						{
							MethodInfo methodInfo = AccessTools.FirstMethod(resultType, (MethodInfo m) => m.Name == "Add" && m.GetParameters().Count<ParameterInfo>() == 1);
							bool flag3 = methodInfo != null;
							if (flag3)
							{
								object obj2 = Activator.CreateInstance(resultType);
								FastInvokeHandler handler = MethodInvoker.GetHandler(methodInfo);
								Type type2 = resultType.GetGenericArguments()[0];
								int num = 0;
								foreach (object obj3 in (source as IEnumerable))
								{
									string text = num++.ToString();
									string text2 = ((pathRoot.Length > 0) ? (pathRoot + "." + text) : text);
									object obj4 = AccessTools.MakeDeepCopy(obj3, type2, processor, text2);
									handler(obj2, new object[] { obj4 });
								}
								return obj2;
							}
						}
						bool flag4 = type.IsArray && resultType.IsArray;
						if (flag4)
						{
							Type elementType = resultType.GetElementType();
							int length = ((Array)source).Length;
							object[] array = Activator.CreateInstance(resultType, new object[] { length }) as object[];
							object[] array2 = source as object[];
							for (int i = 0; i < length; i++)
							{
								string text3 = i.ToString();
								string text4 = ((pathRoot.Length > 0) ? (pathRoot + "." + text3) : text3);
								array[i] = AccessTools.MakeDeepCopy(array2[i], elementType, processor, text4);
							}
							obj = array;
						}
						else
						{
							string @namespace = type.Namespace;
							bool flag5 = @namespace == "System" || (@namespace != null && @namespace.StartsWith("System."));
							if (flag5)
							{
								obj = source;
							}
							else
							{
								object obj5 = AccessTools.CreateInstance(resultType);
								Traverse.IterateFields(source, obj5, delegate(string name, Traverse src, Traverse dst)
								{
									string text5 = ((pathRoot.Length > 0) ? (pathRoot + "." + name) : name);
									object obj6 = ((processor != null) ? processor(text5, src, dst) : src.GetValue());
									dst.SetValue(AccessTools.MakeDeepCopy(obj6, dst.GetValueType(), processor, text5));
								});
								obj = obj5;
							}
						}
					}
				}
			}
			return obj;
		}

		public static void MakeDeepCopy<T>(object source, out T result, Func<string, Traverse, Traverse, object> processor = null, string pathRoot = "")
		{
			result = (T)((object)AccessTools.MakeDeepCopy(source, typeof(T), processor, pathRoot));
		}

		public static bool IsStruct(Type type)
		{
			return type.IsValueType && !AccessTools.IsValue(type) && !AccessTools.IsVoid(type);
		}

		public static bool IsClass(Type type)
		{
			return !type.IsValueType;
		}

		public static bool IsValue(Type type)
		{
			return type.IsPrimitive || type.IsEnum;
		}

		public static bool IsVoid(Type type)
		{
			return type == typeof(void);
		}

		public static BindingFlags all = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.GetField | BindingFlags.SetField | BindingFlags.GetProperty | BindingFlags.SetProperty;

		public delegate ref U FieldRef<T, U>(T obj);
	}
}
