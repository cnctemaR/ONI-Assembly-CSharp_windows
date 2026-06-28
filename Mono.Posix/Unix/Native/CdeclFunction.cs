using System;
using System.Collections;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.InteropServices;
using System.Text;

namespace Mono.Unix.Native
{
	public sealed class CdeclFunction
	{
		public CdeclFunction(string library, string method)
			: this(library, method, typeof(void))
		{
		}

		public CdeclFunction(string library, string method, Type returnType)
		{
			this.library = library;
			this.method = method;
			this.returnType = returnType;
			this.overloads = new Hashtable();
			this.assemblyName = new AssemblyName();
			this.assemblyName.Name = "Mono.Posix.Imports." + library;
			this.assemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(this.assemblyName, AssemblyBuilderAccess.Run);
			this.moduleBuilder = this.assemblyBuilder.DefineDynamicModule(this.assemblyName.Name);
		}

		public object Invoke(object[] parameters)
		{
			Type[] parameterTypes = CdeclFunction.GetParameterTypes(parameters);
			MethodInfo methodInfo = this.CreateMethod(parameterTypes);
			return methodInfo.Invoke(null, parameters);
		}

		private MethodInfo CreateMethod(Type[] parameterTypes)
		{
			string typeName = this.GetTypeName(parameterTypes);
			Hashtable hashtable = this.overloads;
			MethodInfo methodInfo2;
			lock (hashtable)
			{
				MethodInfo methodInfo = (MethodInfo)this.overloads[typeName];
				if (methodInfo != null)
				{
					methodInfo2 = methodInfo;
				}
				else
				{
					TypeBuilder typeBuilder = this.CreateType(typeName);
					typeBuilder.DefinePInvokeMethod(this.method, this.library, MethodAttributes.FamANDAssem | MethodAttributes.Family | MethodAttributes.Static | MethodAttributes.PinvokeImpl, CallingConventions.Standard, this.returnType, parameterTypes, CallingConvention.Cdecl, CharSet.Ansi);
					methodInfo = typeBuilder.CreateType().GetMethod(this.method);
					this.overloads.Add(typeName, methodInfo);
					methodInfo2 = methodInfo;
				}
			}
			return methodInfo2;
		}

		private TypeBuilder CreateType(string typeName)
		{
			return this.moduleBuilder.DefineType(typeName, TypeAttributes.Public);
		}

		private static Type GetMarshalType(Type t)
		{
			switch (Type.GetTypeCode(t))
			{
			case TypeCode.Boolean:
			case TypeCode.Char:
			case TypeCode.SByte:
			case TypeCode.Int16:
			case TypeCode.Int32:
				return typeof(int);
			case TypeCode.Byte:
			case TypeCode.UInt16:
			case TypeCode.UInt32:
				return typeof(uint);
			case TypeCode.Int64:
				return typeof(long);
			case TypeCode.UInt64:
				return typeof(ulong);
			case TypeCode.Single:
			case TypeCode.Double:
				return typeof(double);
			default:
				return t;
			}
		}

		private string GetTypeName(Type[] parameterTypes)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("[").Append(this.library).Append("] ")
				.Append(this.method);
			stringBuilder.Append("(");
			if (parameterTypes.Length > 0)
			{
				stringBuilder.Append(parameterTypes[0]);
			}
			for (int i = 1; i < parameterTypes.Length; i++)
			{
				stringBuilder.Append(",").Append(parameterTypes[i]);
			}
			stringBuilder.Append(") : ").Append(this.returnType.FullName);
			return stringBuilder.ToString();
		}

		private static Type[] GetParameterTypes(object[] parameters)
		{
			Type[] array = new Type[parameters.Length];
			for (int i = 0; i < parameters.Length; i++)
			{
				array[i] = CdeclFunction.GetMarshalType(parameters[i].GetType());
			}
			return array;
		}

		private readonly string library;

		private readonly string method;

		private readonly Type returnType;

		private readonly AssemblyName assemblyName;

		private readonly AssemblyBuilder assemblyBuilder;

		private readonly ModuleBuilder moduleBuilder;

		private Hashtable overloads;
	}
}
