using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Dynamic;
using System.Dynamic.Utils;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;

namespace System.Linq.Expressions.Compiler
{
	internal static class DelegateHelpers
	{
		internal static Type MakeDelegateType(Type[] types)
		{
			DelegateHelpers.TypeInfo delegateCache = DelegateHelpers._DelegateCache;
			Type delegateType;
			lock (delegateCache)
			{
				DelegateHelpers.TypeInfo typeInfo = DelegateHelpers._DelegateCache;
				for (int i = 0; i < types.Length; i++)
				{
					typeInfo = DelegateHelpers.NextTypeInfo(types[i], typeInfo);
				}
				if (typeInfo.DelegateType == null)
				{
					typeInfo.DelegateType = DelegateHelpers.MakeNewDelegate((Type[])types.Clone());
				}
				delegateType = typeInfo.DelegateType;
			}
			return delegateType;
		}

		internal static DelegateHelpers.TypeInfo NextTypeInfo(Type initialArg)
		{
			DelegateHelpers.TypeInfo delegateCache = DelegateHelpers._DelegateCache;
			DelegateHelpers.TypeInfo typeInfo;
			lock (delegateCache)
			{
				typeInfo = DelegateHelpers.NextTypeInfo(initialArg, DelegateHelpers._DelegateCache);
			}
			return typeInfo;
		}

		internal static DelegateHelpers.TypeInfo GetNextTypeInfo(Type initialArg, DelegateHelpers.TypeInfo curTypeInfo)
		{
			DelegateHelpers.TypeInfo delegateCache = DelegateHelpers._DelegateCache;
			DelegateHelpers.TypeInfo typeInfo;
			lock (delegateCache)
			{
				typeInfo = DelegateHelpers.NextTypeInfo(initialArg, curTypeInfo);
			}
			return typeInfo;
		}

		private static DelegateHelpers.TypeInfo NextTypeInfo(Type initialArg, DelegateHelpers.TypeInfo curTypeInfo)
		{
			if (curTypeInfo.TypeChain == null)
			{
				curTypeInfo.TypeChain = new Dictionary<Type, DelegateHelpers.TypeInfo>();
			}
			DelegateHelpers.TypeInfo typeInfo;
			if (!curTypeInfo.TypeChain.TryGetValue(initialArg, out typeInfo))
			{
				typeInfo = new DelegateHelpers.TypeInfo();
				if (!initialArg.IsCollectible)
				{
					curTypeInfo.TypeChain[initialArg] = typeInfo;
				}
			}
			return typeInfo;
		}

		internal static Type MakeNewDelegate(Type[] types)
		{
			bool flag;
			if (types.Length > 17)
			{
				flag = true;
			}
			else
			{
				flag = false;
				foreach (Type type in types)
				{
					if (type.IsByRef || type.IsPointer)
					{
						flag = true;
						break;
					}
				}
			}
			if (flag)
			{
				return DelegateHelpers.MakeNewCustomDelegate(types);
			}
			Type type2;
			if (types[types.Length - 1] == typeof(void))
			{
				type2 = DelegateHelpers.GetActionType(types.RemoveLast<Type>());
			}
			else
			{
				type2 = DelegateHelpers.GetFuncType(types);
			}
			return type2;
		}

		internal static Type GetFuncType(Type[] types)
		{
			switch (types.Length)
			{
			case 1:
				return typeof(Func<>).MakeGenericType(types);
			case 2:
				return typeof(Func<, >).MakeGenericType(types);
			case 3:
				return typeof(Func<, , >).MakeGenericType(types);
			case 4:
				return typeof(Func<, , , >).MakeGenericType(types);
			case 5:
				return typeof(Func<, , , , >).MakeGenericType(types);
			case 6:
				return typeof(Func<, , , , , >).MakeGenericType(types);
			case 7:
				return typeof(Func<, , , , , , >).MakeGenericType(types);
			case 8:
				return typeof(Func<, , , , , , , >).MakeGenericType(types);
			case 9:
				return typeof(Func<, , , , , , , , >).MakeGenericType(types);
			case 10:
				return typeof(Func<, , , , , , , , , >).MakeGenericType(types);
			case 11:
				return typeof(Func<, , , , , , , , , , >).MakeGenericType(types);
			case 12:
				return typeof(Func<, , , , , , , , , , , >).MakeGenericType(types);
			case 13:
				return typeof(Func<, , , , , , , , , , , , >).MakeGenericType(types);
			case 14:
				return typeof(Func<, , , , , , , , , , , , , >).MakeGenericType(types);
			case 15:
				return typeof(Func<, , , , , , , , , , , , , , >).MakeGenericType(types);
			case 16:
				return typeof(Func<, , , , , , , , , , , , , , , >).MakeGenericType(types);
			case 17:
				return typeof(Func<, , , , , , , , , , , , , , , , >).MakeGenericType(types);
			default:
				return null;
			}
		}

		internal static Type GetActionType(Type[] types)
		{
			switch (types.Length)
			{
			case 0:
				return typeof(Action);
			case 1:
				return typeof(Action<>).MakeGenericType(types);
			case 2:
				return typeof(Action<, >).MakeGenericType(types);
			case 3:
				return typeof(Action<, , >).MakeGenericType(types);
			case 4:
				return typeof(Action<, , , >).MakeGenericType(types);
			case 5:
				return typeof(Action<, , , , >).MakeGenericType(types);
			case 6:
				return typeof(Action<, , , , , >).MakeGenericType(types);
			case 7:
				return typeof(Action<, , , , , , >).MakeGenericType(types);
			case 8:
				return typeof(Action<, , , , , , , >).MakeGenericType(types);
			case 9:
				return typeof(Action<, , , , , , , , >).MakeGenericType(types);
			case 10:
				return typeof(Action<, , , , , , , , , >).MakeGenericType(types);
			case 11:
				return typeof(Action<, , , , , , , , , , >).MakeGenericType(types);
			case 12:
				return typeof(Action<, , , , , , , , , , , >).MakeGenericType(types);
			case 13:
				return typeof(Action<, , , , , , , , , , , , >).MakeGenericType(types);
			case 14:
				return typeof(Action<, , , , , , , , , , , , , >).MakeGenericType(types);
			case 15:
				return typeof(Action<, , , , , , , , , , , , , , >).MakeGenericType(types);
			case 16:
				return typeof(Action<, , , , , , , , , , , , , , , >).MakeGenericType(types);
			default:
				return null;
			}
		}

		internal static Type MakeCallSiteDelegate(ReadOnlyCollection<Expression> types, Type returnType)
		{
			DelegateHelpers.TypeInfo delegateCache = DelegateHelpers._DelegateCache;
			Type delegateType;
			lock (delegateCache)
			{
				DelegateHelpers.TypeInfo typeInfo = DelegateHelpers._DelegateCache;
				typeInfo = DelegateHelpers.NextTypeInfo(typeof(CallSite), typeInfo);
				for (int i = 0; i < types.Count; i++)
				{
					typeInfo = DelegateHelpers.NextTypeInfo(types[i].Type, typeInfo);
				}
				typeInfo = DelegateHelpers.NextTypeInfo(returnType, typeInfo);
				if (typeInfo.DelegateType == null)
				{
					typeInfo.MakeDelegateType(returnType, types);
				}
				delegateType = typeInfo.DelegateType;
			}
			return delegateType;
		}

		internal static Type MakeDeferredSiteDelegate(DynamicMetaObject[] args, Type returnType)
		{
			DelegateHelpers.TypeInfo delegateCache = DelegateHelpers._DelegateCache;
			Type delegateType;
			lock (delegateCache)
			{
				DelegateHelpers.TypeInfo typeInfo = DelegateHelpers._DelegateCache;
				typeInfo = DelegateHelpers.NextTypeInfo(typeof(CallSite), typeInfo);
				foreach (DynamicMetaObject dynamicMetaObject in args)
				{
					Type type = dynamicMetaObject.Expression.Type;
					if (DelegateHelpers.IsByRef(dynamicMetaObject))
					{
						type = type.MakeByRefType();
					}
					typeInfo = DelegateHelpers.NextTypeInfo(type, typeInfo);
				}
				typeInfo = DelegateHelpers.NextTypeInfo(returnType, typeInfo);
				if (typeInfo.DelegateType == null)
				{
					Type[] array = new Type[args.Length + 2];
					array[0] = typeof(CallSite);
					array[array.Length - 1] = returnType;
					for (int j = 0; j < args.Length; j++)
					{
						DynamicMetaObject dynamicMetaObject2 = args[j];
						Type type2 = dynamicMetaObject2.Expression.Type;
						if (DelegateHelpers.IsByRef(dynamicMetaObject2))
						{
							type2 = type2.MakeByRefType();
						}
						array[j + 1] = type2;
					}
					typeInfo.DelegateType = DelegateHelpers.MakeNewDelegate(array);
				}
				delegateType = typeInfo.DelegateType;
			}
			return delegateType;
		}

		private static bool IsByRef(DynamicMetaObject mo)
		{
			ParameterExpression parameterExpression = mo.Expression as ParameterExpression;
			return parameterExpression != null && parameterExpression.IsByRef;
		}

		private static Type MakeNewCustomDelegate(Type[] types)
		{
			Type type = types[types.Length - 1];
			Type[] array = types.RemoveLast<Type>();
			TypeBuilder typeBuilder = AssemblyGen.DefineDelegateType("Delegate" + types.Length.ToString());
			typeBuilder.DefineConstructor(MethodAttributes.FamANDAssem | MethodAttributes.Family | MethodAttributes.HideBySig | MethodAttributes.RTSpecialName, CallingConventions.Standard, DelegateHelpers.s_delegateCtorSignature).SetImplementationFlags(MethodImplAttributes.CodeTypeMask);
			typeBuilder.DefineMethod("Invoke", MethodAttributes.FamANDAssem | MethodAttributes.Family | MethodAttributes.Virtual | MethodAttributes.HideBySig | MethodAttributes.VtableLayoutMask, type, array).SetImplementationFlags(MethodImplAttributes.CodeTypeMask);
			return typeBuilder.CreateTypeInfo();
		}

		private static DelegateHelpers.TypeInfo _DelegateCache = new DelegateHelpers.TypeInfo();

		private const int MaximumArity = 17;

		private const MethodAttributes CtorAttributes = MethodAttributes.FamANDAssem | MethodAttributes.Family | MethodAttributes.HideBySig | MethodAttributes.RTSpecialName;

		private const MethodImplAttributes ImplAttributes = MethodImplAttributes.CodeTypeMask;

		private const MethodAttributes InvokeAttributes = MethodAttributes.FamANDAssem | MethodAttributes.Family | MethodAttributes.Virtual | MethodAttributes.HideBySig | MethodAttributes.VtableLayoutMask;

		private static readonly Type[] s_delegateCtorSignature = new Type[]
		{
			typeof(object),
			typeof(IntPtr)
		};

		internal class TypeInfo
		{
			public Type DelegateType;

			public Dictionary<Type, DelegateHelpers.TypeInfo> TypeChain;
		}
	}
}
