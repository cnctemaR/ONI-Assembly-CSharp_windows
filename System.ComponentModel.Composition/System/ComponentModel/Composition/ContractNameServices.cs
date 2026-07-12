using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Microsoft.Internal;

namespace System.ComponentModel.Composition
{
	internal static class ContractNameServices
	{
		private static Dictionary<Type, string> TypeIdentityCache
		{
			get
			{
				return ContractNameServices.typeIdentityCache = ContractNameServices.typeIdentityCache ?? new Dictionary<Type, string>();
			}
		}

		internal static string GetTypeIdentity(Type type)
		{
			return ContractNameServices.GetTypeIdentity(type, true);
		}

		internal static string GetTypeIdentity(Type type, bool formatGenericName)
		{
			Assumes.NotNull<Type>(type);
			string text = null;
			if (!ContractNameServices.TypeIdentityCache.TryGetValue(type, out text))
			{
				if (!type.IsAbstract && type.IsSubclassOf(typeof(Delegate)))
				{
					text = ContractNameServices.GetTypeIdentityFromMethod(type.GetMethod("Invoke"));
				}
				else if (type.IsGenericParameter)
				{
					StringBuilder stringBuilder = new StringBuilder();
					ContractNameServices.WriteTypeArgument(stringBuilder, false, type, formatGenericName);
					stringBuilder.Remove(stringBuilder.Length - 1, 1);
					text = stringBuilder.ToString();
				}
				else
				{
					StringBuilder stringBuilder2 = new StringBuilder();
					ContractNameServices.WriteTypeWithNamespace(stringBuilder2, type, formatGenericName);
					text = stringBuilder2.ToString();
				}
				Assumes.IsTrue(!string.IsNullOrEmpty(text));
				ContractNameServices.TypeIdentityCache.Add(type, text);
			}
			return text;
		}

		internal static string GetTypeIdentityFromMethod(MethodInfo method)
		{
			return ContractNameServices.GetTypeIdentityFromMethod(method, true);
		}

		internal static string GetTypeIdentityFromMethod(MethodInfo method, bool formatGenericName)
		{
			StringBuilder stringBuilder = new StringBuilder();
			ContractNameServices.WriteTypeWithNamespace(stringBuilder, method.ReturnType, formatGenericName);
			stringBuilder.Append("(");
			ParameterInfo[] parameters = method.GetParameters();
			for (int i = 0; i < parameters.Length; i++)
			{
				if (i != 0)
				{
					stringBuilder.Append(",");
				}
				ContractNameServices.WriteTypeWithNamespace(stringBuilder, parameters[i].ParameterType, formatGenericName);
			}
			stringBuilder.Append(")");
			return stringBuilder.ToString();
		}

		private static void WriteTypeWithNamespace(StringBuilder typeName, Type type, bool formatGenericName)
		{
			if (!string.IsNullOrEmpty(type.Namespace))
			{
				typeName.Append(type.Namespace);
				typeName.Append('.');
			}
			ContractNameServices.WriteType(typeName, type, formatGenericName);
		}

		private static void WriteType(StringBuilder typeName, Type type, bool formatGenericName)
		{
			if (type.IsGenericType)
			{
				Queue<Type> queue = new Queue<Type>(type.GetGenericArguments());
				ContractNameServices.WriteGenericType(typeName, type, type.IsGenericTypeDefinition, queue, formatGenericName);
				Assumes.IsTrue(queue.Count == 0, "Expecting genericTypeArguments queue to be empty.");
				return;
			}
			ContractNameServices.WriteNonGenericType(typeName, type, formatGenericName);
		}

		private static void WriteNonGenericType(StringBuilder typeName, Type type, bool formatGenericName)
		{
			if (type.DeclaringType != null)
			{
				ContractNameServices.WriteType(typeName, type.DeclaringType, formatGenericName);
				typeName.Append('+');
			}
			if (type.IsArray)
			{
				ContractNameServices.WriteArrayType(typeName, type, formatGenericName);
				return;
			}
			if (type.IsPointer)
			{
				ContractNameServices.WritePointerType(typeName, type, formatGenericName);
				return;
			}
			if (type.IsByRef)
			{
				ContractNameServices.WriteByRefType(typeName, type, formatGenericName);
				return;
			}
			typeName.Append(type.Name);
		}

		private static void WriteArrayType(StringBuilder typeName, Type type, bool formatGenericName)
		{
			Type type2 = ContractNameServices.FindArrayElementType(type);
			ContractNameServices.WriteType(typeName, type2, formatGenericName);
			Type type3 = type;
			do
			{
				ContractNameServices.WriteArrayTypeDimensions(typeName, type3);
			}
			while ((type3 = type3.GetElementType()) != null && type3.IsArray);
		}

		private static void WritePointerType(StringBuilder typeName, Type type, bool formatGenericName)
		{
			ContractNameServices.WriteType(typeName, type.GetElementType(), formatGenericName);
			typeName.Append('*');
		}

		private static void WriteByRefType(StringBuilder typeName, Type type, bool formatGenericName)
		{
			ContractNameServices.WriteType(typeName, type.GetElementType(), formatGenericName);
			typeName.Append('&');
		}

		private static void WriteArrayTypeDimensions(StringBuilder typeName, Type type)
		{
			typeName.Append('[');
			int arrayRank = type.GetArrayRank();
			for (int i = 1; i < arrayRank; i++)
			{
				typeName.Append(',');
			}
			typeName.Append(']');
		}

		private static void WriteGenericType(StringBuilder typeName, Type type, bool isDefinition, Queue<Type> genericTypeArguments, bool formatGenericName)
		{
			if (type.DeclaringType != null)
			{
				if (type.DeclaringType.IsGenericType)
				{
					ContractNameServices.WriteGenericType(typeName, type.DeclaringType, isDefinition, genericTypeArguments, formatGenericName);
				}
				else
				{
					ContractNameServices.WriteNonGenericType(typeName, type.DeclaringType, formatGenericName);
				}
				typeName.Append('+');
			}
			ContractNameServices.WriteGenericTypeName(typeName, type, isDefinition, genericTypeArguments, formatGenericName);
		}

		private static void WriteGenericTypeName(StringBuilder typeName, Type type, bool isDefinition, Queue<Type> genericTypeArguments, bool formatGenericName)
		{
			Assumes.IsTrue(type.IsGenericType, "Expecting type to be a generic type");
			int genericArity = ContractNameServices.GetGenericArity(type);
			string text = ContractNameServices.FindGenericTypeName(type.GetGenericTypeDefinition().Name);
			typeName.Append(text);
			ContractNameServices.WriteTypeArgumentsString(typeName, genericArity, isDefinition, genericTypeArguments, formatGenericName);
		}

		private static void WriteTypeArgumentsString(StringBuilder typeName, int argumentsCount, bool isDefinition, Queue<Type> genericTypeArguments, bool formatGenericName)
		{
			if (argumentsCount == 0)
			{
				return;
			}
			typeName.Append('(');
			for (int i = 0; i < argumentsCount; i++)
			{
				Assumes.IsTrue(genericTypeArguments.Count > 0, "Expecting genericTypeArguments to contain at least one Type");
				Type type = genericTypeArguments.Dequeue();
				ContractNameServices.WriteTypeArgument(typeName, isDefinition, type, formatGenericName);
			}
			typeName.Remove(typeName.Length - 1, 1);
			typeName.Append(')');
		}

		private static void WriteTypeArgument(StringBuilder typeName, bool isDefinition, Type genericTypeArgument, bool formatGenericName)
		{
			if (!isDefinition && !genericTypeArgument.IsGenericParameter)
			{
				ContractNameServices.WriteTypeWithNamespace(typeName, genericTypeArgument, formatGenericName);
			}
			if (formatGenericName && genericTypeArgument.IsGenericParameter)
			{
				typeName.Append('{');
				typeName.Append(genericTypeArgument.GenericParameterPosition);
				typeName.Append('}');
			}
			typeName.Append(',');
		}

		internal static void WriteCustomModifiers(StringBuilder typeName, string customKeyword, Type[] types, bool formatGenericName)
		{
			typeName.Append(' ');
			typeName.Append(customKeyword);
			Queue<Type> queue = new Queue<Type>(types);
			ContractNameServices.WriteTypeArgumentsString(typeName, types.Length, false, queue, formatGenericName);
			Assumes.IsTrue(queue.Count == 0, "Expecting genericTypeArguments queue to be empty.");
		}

		private static Type FindArrayElementType(Type type)
		{
			Type type2 = type;
			while ((type2 = type2.GetElementType()) != null && type2.IsArray)
			{
			}
			return type2;
		}

		private static string FindGenericTypeName(string genericName)
		{
			int num = genericName.IndexOf('`');
			if (num > -1)
			{
				genericName = genericName.Substring(0, num);
			}
			return genericName;
		}

		private static int GetGenericArity(Type type)
		{
			if (type.DeclaringType == null)
			{
				return type.GetGenericArguments().Length;
			}
			int num = type.DeclaringType.GetGenericArguments().Length;
			int num2 = type.GetGenericArguments().Length;
			Assumes.IsTrue(num2 >= num);
			return num2 - num;
		}

		private const char NamespaceSeparator = '.';

		private const char ArrayOpeningBracket = '[';

		private const char ArrayClosingBracket = ']';

		private const char ArraySeparator = ',';

		private const char PointerSymbol = '*';

		private const char ReferenceSymbol = '&';

		private const char GenericArityBackQuote = '`';

		private const char NestedClassSeparator = '+';

		private const char ContractNameGenericOpeningBracket = '(';

		private const char ContractNameGenericClosingBracket = ')';

		private const char ContractNameGenericArgumentSeparator = ',';

		private const char CustomModifiersSeparator = ' ';

		private const char GenericFormatOpeningBracket = '{';

		private const char GenericFormatClosingBracket = '}';

		[ThreadStatic]
		private static Dictionary<Type, string> typeIdentityCache;
	}
}
