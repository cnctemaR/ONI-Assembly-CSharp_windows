using System;
using System.Reflection;

namespace Microsoft.Internal
{
	internal static class ReflectionInvoke
	{
		public static object SafeCreateInstance(this Type type, params object[] arguments)
		{
			ReflectionInvoke.DemandMemberAccessIfNeeded(type);
			return Activator.CreateInstance(type, arguments);
		}

		public static object SafeInvoke(this ConstructorInfo constructor, params object[] arguments)
		{
			ReflectionInvoke.DemandMemberAccessIfNeeded(constructor);
			return constructor.Invoke(arguments);
		}

		public static object SafeInvoke(this MethodInfo method, object instance, params object[] arguments)
		{
			ReflectionInvoke.DemandMemberAccessIfNeeded(method);
			return method.Invoke(instance, arguments);
		}

		public static object SafeGetValue(this FieldInfo field, object instance)
		{
			ReflectionInvoke.DemandMemberAccessIfNeeded(field);
			return field.GetValue(instance);
		}

		public static void SafeSetValue(this FieldInfo field, object instance, object value)
		{
			ReflectionInvoke.DemandMemberAccessIfNeeded(field);
			field.SetValue(instance, value);
		}

		public static void DemandMemberAccessIfNeeded(MethodInfo method)
		{
		}

		private static void DemandMemberAccessIfNeeded(ConstructorInfo constructor)
		{
		}

		private static void DemandMemberAccessIfNeeded(FieldInfo field)
		{
		}

		public static void DemandMemberAccessIfNeeded(Type type)
		{
		}
	}
}
