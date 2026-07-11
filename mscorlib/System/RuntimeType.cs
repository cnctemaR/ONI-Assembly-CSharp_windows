using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Activation;
using System.Runtime.Serialization;
using System.Security;
using System.Threading;
using Mono;

namespace System
{
	[Serializable]
	[StructLayout(LayoutKind.Sequential)]
	internal class RuntimeType : global::System.Reflection.TypeInfo, ISerializable, ICloneable
	{
		private static void ThrowIfTypeNeverValidGenericArgument(RuntimeType type)
		{
			if (type.IsPointer || type.IsByRef || type == typeof(void))
			{
				throw new ArgumentException(Environment.GetResourceString("The type '{0}' may not be used as a type argument.", new object[] { type.ToString() }));
			}
		}

		internal static void SanityCheckGenericArguments(RuntimeType[] genericArguments, RuntimeType[] genericParamters)
		{
			if (genericArguments == null)
			{
				throw new ArgumentNullException();
			}
			for (int i = 0; i < genericArguments.Length; i++)
			{
				if (genericArguments[i] == null)
				{
					throw new ArgumentNullException();
				}
				RuntimeType.ThrowIfTypeNeverValidGenericArgument(genericArguments[i]);
			}
			if (genericArguments.Length != genericParamters.Length)
			{
				throw new ArgumentException(Environment.GetResourceString("The type or method has {1} generic parameter(s), but {0} generic argument(s) were provided. A generic argument must be provided for each generic parameter.", new object[] { genericArguments.Length, genericParamters.Length }));
			}
		}

		private static void SplitName(string fullname, out string name, out string ns)
		{
			name = null;
			ns = null;
			if (fullname == null)
			{
				return;
			}
			int num = fullname.LastIndexOf(".", StringComparison.Ordinal);
			if (num == -1)
			{
				name = fullname;
				return;
			}
			ns = fullname.Substring(0, num);
			int num2 = fullname.Length - ns.Length - 1;
			if (num2 != 0)
			{
				name = fullname.Substring(num + 1, num2);
				return;
			}
			name = "";
		}

		internal static BindingFlags FilterPreCalculate(bool isPublic, bool isInherited, bool isStatic)
		{
			BindingFlags bindingFlags = (isPublic ? BindingFlags.Public : BindingFlags.NonPublic);
			if (isInherited)
			{
				bindingFlags |= BindingFlags.DeclaredOnly;
				if (isStatic)
				{
					bindingFlags |= BindingFlags.Static | BindingFlags.FlattenHierarchy;
				}
				else
				{
					bindingFlags |= BindingFlags.Instance;
				}
			}
			else if (isStatic)
			{
				bindingFlags |= BindingFlags.Static;
			}
			else
			{
				bindingFlags |= BindingFlags.Instance;
			}
			return bindingFlags;
		}

		private static void FilterHelper(BindingFlags bindingFlags, ref string name, bool allowPrefixLookup, out bool prefixLookup, out bool ignoreCase, out RuntimeType.MemberListType listType)
		{
			prefixLookup = false;
			ignoreCase = false;
			if (name != null)
			{
				if ((bindingFlags & BindingFlags.IgnoreCase) != BindingFlags.Default)
				{
					name = name.ToLower(CultureInfo.InvariantCulture);
					ignoreCase = true;
					listType = RuntimeType.MemberListType.CaseInsensitive;
				}
				else
				{
					listType = RuntimeType.MemberListType.CaseSensitive;
				}
				if (allowPrefixLookup && name.EndsWith("*", StringComparison.Ordinal))
				{
					name = name.Substring(0, name.Length - 1);
					prefixLookup = true;
					listType = RuntimeType.MemberListType.All;
					return;
				}
			}
			else
			{
				listType = RuntimeType.MemberListType.All;
			}
		}

		private static void FilterHelper(BindingFlags bindingFlags, ref string name, out bool ignoreCase, out RuntimeType.MemberListType listType)
		{
			bool flag;
			RuntimeType.FilterHelper(bindingFlags, ref name, false, out flag, out ignoreCase, out listType);
		}

		private static bool FilterApplyPrefixLookup(MemberInfo memberInfo, string name, bool ignoreCase)
		{
			if (ignoreCase)
			{
				if (!memberInfo.Name.StartsWith(name, StringComparison.OrdinalIgnoreCase))
				{
					return false;
				}
			}
			else if (!memberInfo.Name.StartsWith(name, StringComparison.Ordinal))
			{
				return false;
			}
			return true;
		}

		private static bool FilterApplyBase(MemberInfo memberInfo, BindingFlags bindingFlags, bool isPublic, bool isNonProtectedInternal, bool isStatic, string name, bool prefixLookup)
		{
			if (isPublic)
			{
				if ((bindingFlags & BindingFlags.Public) == BindingFlags.Default)
				{
					return false;
				}
			}
			else if ((bindingFlags & BindingFlags.NonPublic) == BindingFlags.Default)
			{
				return false;
			}
			bool flag = memberInfo.DeclaringType != memberInfo.ReflectedType;
			if ((bindingFlags & BindingFlags.DeclaredOnly) > BindingFlags.Default && flag)
			{
				return false;
			}
			if (memberInfo.MemberType != MemberTypes.TypeInfo && memberInfo.MemberType != MemberTypes.NestedType)
			{
				if (isStatic)
				{
					if ((bindingFlags & BindingFlags.FlattenHierarchy) == BindingFlags.Default && flag)
					{
						return false;
					}
					if ((bindingFlags & BindingFlags.Static) == BindingFlags.Default)
					{
						return false;
					}
				}
				else if ((bindingFlags & BindingFlags.Instance) == BindingFlags.Default)
				{
					return false;
				}
			}
			if (prefixLookup && !RuntimeType.FilterApplyPrefixLookup(memberInfo, name, (bindingFlags & BindingFlags.IgnoreCase) > BindingFlags.Default))
			{
				return false;
			}
			if ((bindingFlags & BindingFlags.DeclaredOnly) == BindingFlags.Default && flag && isNonProtectedInternal && (bindingFlags & BindingFlags.NonPublic) != BindingFlags.Default && !isStatic && (bindingFlags & BindingFlags.Instance) != BindingFlags.Default)
			{
				MethodInfo methodInfo = memberInfo as MethodInfo;
				if (methodInfo == null)
				{
					return false;
				}
				if (!methodInfo.IsVirtual && !methodInfo.IsAbstract)
				{
					return false;
				}
			}
			return true;
		}

		private static bool FilterApplyType(Type type, BindingFlags bindingFlags, string name, bool prefixLookup, string ns)
		{
			bool flag = type.IsNestedPublic || type.IsPublic;
			bool flag2 = false;
			return RuntimeType.FilterApplyBase(type, bindingFlags, flag, type.IsNestedAssembly, flag2, name, prefixLookup) && (ns == null || type.Namespace.Equals(ns));
		}

		private static bool FilterApplyMethodInfo(RuntimeMethodInfo method, BindingFlags bindingFlags, CallingConventions callConv, Type[] argumentTypes)
		{
			return RuntimeType.FilterApplyMethodBase(method, method.BindingFlags, bindingFlags, callConv, argumentTypes);
		}

		private static bool FilterApplyConstructorInfo(RuntimeConstructorInfo constructor, BindingFlags bindingFlags, CallingConventions callConv, Type[] argumentTypes)
		{
			return RuntimeType.FilterApplyMethodBase(constructor, constructor.BindingFlags, bindingFlags, callConv, argumentTypes);
		}

		private static bool FilterApplyMethodBase(MethodBase methodBase, BindingFlags methodFlags, BindingFlags bindingFlags, CallingConventions callConv, Type[] argumentTypes)
		{
			bindingFlags ^= BindingFlags.DeclaredOnly;
			if ((callConv & CallingConventions.Any) == (CallingConventions)0)
			{
				if ((callConv & CallingConventions.VarArgs) != (CallingConventions)0 && (methodBase.CallingConvention & CallingConventions.VarArgs) == (CallingConventions)0)
				{
					return false;
				}
				if ((callConv & CallingConventions.Standard) != (CallingConventions)0 && (methodBase.CallingConvention & CallingConventions.Standard) == (CallingConventions)0)
				{
					return false;
				}
			}
			if (argumentTypes != null)
			{
				ParameterInfo[] parametersNoCopy = methodBase.GetParametersNoCopy();
				if (argumentTypes.Length != parametersNoCopy.Length)
				{
					if ((bindingFlags & (BindingFlags.InvokeMethod | BindingFlags.CreateInstance | BindingFlags.GetProperty | BindingFlags.SetProperty)) == BindingFlags.Default)
					{
						return false;
					}
					bool flag = false;
					if (argumentTypes.Length > parametersNoCopy.Length)
					{
						if ((methodBase.CallingConvention & CallingConventions.VarArgs) == (CallingConventions)0)
						{
							flag = true;
						}
					}
					else if ((bindingFlags & BindingFlags.OptionalParamBinding) == BindingFlags.Default)
					{
						flag = true;
					}
					else if (!parametersNoCopy[argumentTypes.Length].IsOptional)
					{
						flag = true;
					}
					if (flag)
					{
						if (parametersNoCopy.Length == 0)
						{
							return false;
						}
						if (argumentTypes.Length < parametersNoCopy.Length - 1)
						{
							return false;
						}
						ParameterInfo parameterInfo = parametersNoCopy[parametersNoCopy.Length - 1];
						if (!parameterInfo.ParameterType.IsArray)
						{
							return false;
						}
						if (!parameterInfo.IsDefined(typeof(ParamArrayAttribute), false))
						{
							return false;
						}
					}
				}
				else if ((bindingFlags & BindingFlags.ExactBinding) != BindingFlags.Default && (bindingFlags & BindingFlags.InvokeMethod) == BindingFlags.Default)
				{
					for (int i = 0; i < parametersNoCopy.Length; i++)
					{
						if (argumentTypes[i] != null && parametersNoCopy[i].ParameterType != argumentTypes[i])
						{
							return false;
						}
					}
				}
			}
			return true;
		}

		internal RuntimeType()
		{
			throw new NotSupportedException();
		}

		internal bool IsSpecialSerializableType()
		{
			RuntimeType runtimeType = this;
			while (!(runtimeType == RuntimeType.DelegateType) && !(runtimeType == RuntimeType.EnumType))
			{
				runtimeType = runtimeType.GetBaseType();
				if (!(runtimeType != null))
				{
					return false;
				}
			}
			return true;
		}

		private RuntimeType.ListBuilder<MethodInfo> GetMethodCandidates(string name, BindingFlags bindingAttr, CallingConventions callConv, Type[] types, bool allowPrefixLookup)
		{
			bool flag;
			bool flag2;
			RuntimeType.MemberListType memberListType;
			RuntimeType.FilterHelper(bindingAttr, ref name, allowPrefixLookup, out flag, out flag2, out memberListType);
			RuntimeMethodInfo[] methodsByName = this.GetMethodsByName(name, bindingAttr, flag2, this);
			RuntimeType.ListBuilder<MethodInfo> listBuilder = new RuntimeType.ListBuilder<MethodInfo>(methodsByName.Length);
			foreach (RuntimeMethodInfo runtimeMethodInfo in methodsByName)
			{
				if (RuntimeType.FilterApplyMethodInfo(runtimeMethodInfo, bindingAttr, callConv, types) && (!flag || RuntimeType.FilterApplyPrefixLookup(runtimeMethodInfo, name, flag2)))
				{
					listBuilder.Add(runtimeMethodInfo);
				}
			}
			return listBuilder;
		}

		private RuntimeType.ListBuilder<ConstructorInfo> GetConstructorCandidates(string name, BindingFlags bindingAttr, CallingConventions callConv, Type[] types, bool allowPrefixLookup)
		{
			bool flag;
			bool flag2;
			RuntimeType.MemberListType memberListType;
			RuntimeType.FilterHelper(bindingAttr, ref name, allowPrefixLookup, out flag, out flag2, out memberListType);
			if (name != null && name != ConstructorInfo.ConstructorName && name != ConstructorInfo.TypeConstructorName)
			{
				return new RuntimeType.ListBuilder<ConstructorInfo>(0);
			}
			RuntimeConstructorInfo[] constructors_internal = this.GetConstructors_internal(bindingAttr, this);
			RuntimeType.ListBuilder<ConstructorInfo> listBuilder = new RuntimeType.ListBuilder<ConstructorInfo>(constructors_internal.Length);
			foreach (RuntimeConstructorInfo runtimeConstructorInfo in constructors_internal)
			{
				if (RuntimeType.FilterApplyConstructorInfo(runtimeConstructorInfo, bindingAttr, callConv, types) && (!flag || RuntimeType.FilterApplyPrefixLookup(runtimeConstructorInfo, name, flag2)))
				{
					listBuilder.Add(runtimeConstructorInfo);
				}
			}
			return listBuilder;
		}

		private RuntimeType.ListBuilder<PropertyInfo> GetPropertyCandidates(string name, BindingFlags bindingAttr, Type[] types, bool allowPrefixLookup)
		{
			bool flag;
			bool flag2;
			RuntimeType.MemberListType memberListType;
			RuntimeType.FilterHelper(bindingAttr, ref name, allowPrefixLookup, out flag, out flag2, out memberListType);
			RuntimePropertyInfo[] propertiesByName = this.GetPropertiesByName(name, bindingAttr, flag2, this);
			bindingAttr ^= BindingFlags.DeclaredOnly;
			RuntimeType.ListBuilder<PropertyInfo> listBuilder = new RuntimeType.ListBuilder<PropertyInfo>(propertiesByName.Length);
			foreach (RuntimePropertyInfo runtimePropertyInfo in propertiesByName)
			{
				if ((bindingAttr & runtimePropertyInfo.BindingFlags) == runtimePropertyInfo.BindingFlags && (!flag || RuntimeType.FilterApplyPrefixLookup(runtimePropertyInfo, name, flag2)) && (types == null || runtimePropertyInfo.GetIndexParameters().Length == types.Length))
				{
					listBuilder.Add(runtimePropertyInfo);
				}
			}
			return listBuilder;
		}

		private RuntimeType.ListBuilder<EventInfo> GetEventCandidates(string name, BindingFlags bindingAttr, bool allowPrefixLookup)
		{
			bool flag;
			bool flag2;
			RuntimeType.MemberListType memberListType;
			RuntimeType.FilterHelper(bindingAttr, ref name, allowPrefixLookup, out flag, out flag2, out memberListType);
			RuntimeEventInfo[] events_internal = this.GetEvents_internal(name, bindingAttr, this);
			bindingAttr ^= BindingFlags.DeclaredOnly;
			RuntimeType.ListBuilder<EventInfo> listBuilder = new RuntimeType.ListBuilder<EventInfo>(events_internal.Length);
			foreach (RuntimeEventInfo runtimeEventInfo in events_internal)
			{
				if ((bindingAttr & runtimeEventInfo.BindingFlags) == runtimeEventInfo.BindingFlags && (!flag || RuntimeType.FilterApplyPrefixLookup(runtimeEventInfo, name, flag2)))
				{
					listBuilder.Add(runtimeEventInfo);
				}
			}
			return listBuilder;
		}

		private RuntimeType.ListBuilder<FieldInfo> GetFieldCandidates(string name, BindingFlags bindingAttr, bool allowPrefixLookup)
		{
			bool flag;
			bool flag2;
			RuntimeType.MemberListType memberListType;
			RuntimeType.FilterHelper(bindingAttr, ref name, allowPrefixLookup, out flag, out flag2, out memberListType);
			RuntimeFieldInfo[] fields_internal = this.GetFields_internal(name, bindingAttr, this);
			bindingAttr ^= BindingFlags.DeclaredOnly;
			RuntimeType.ListBuilder<FieldInfo> listBuilder = new RuntimeType.ListBuilder<FieldInfo>(fields_internal.Length);
			foreach (RuntimeFieldInfo runtimeFieldInfo in fields_internal)
			{
				if ((bindingAttr & runtimeFieldInfo.BindingFlags) == runtimeFieldInfo.BindingFlags && (!flag || RuntimeType.FilterApplyPrefixLookup(runtimeFieldInfo, name, flag2)))
				{
					listBuilder.Add(runtimeFieldInfo);
				}
			}
			return listBuilder;
		}

		private RuntimeType.ListBuilder<Type> GetNestedTypeCandidates(string fullname, BindingFlags bindingAttr, bool allowPrefixLookup)
		{
			bindingAttr &= ~BindingFlags.Static;
			string text;
			string text2;
			RuntimeType.SplitName(fullname, out text, out text2);
			bool flag;
			bool flag2;
			RuntimeType.MemberListType memberListType;
			RuntimeType.FilterHelper(bindingAttr, ref text, allowPrefixLookup, out flag, out flag2, out memberListType);
			RuntimeType[] nestedTypes_internal = this.GetNestedTypes_internal(text, bindingAttr);
			RuntimeType.ListBuilder<Type> listBuilder = new RuntimeType.ListBuilder<Type>(nestedTypes_internal.Length);
			foreach (RuntimeType runtimeType in nestedTypes_internal)
			{
				if (RuntimeType.FilterApplyType(runtimeType, bindingAttr, text, flag, text2))
				{
					listBuilder.Add(runtimeType);
				}
			}
			return listBuilder;
		}

		public override MethodInfo[] GetMethods(BindingFlags bindingAttr)
		{
			return this.GetMethodCandidates(null, bindingAttr, CallingConventions.Any, null, false).ToArray();
		}

		[ComVisible(true)]
		public override ConstructorInfo[] GetConstructors(BindingFlags bindingAttr)
		{
			return this.GetConstructorCandidates(null, bindingAttr, CallingConventions.Any, null, false).ToArray();
		}

		public override PropertyInfo[] GetProperties(BindingFlags bindingAttr)
		{
			return this.GetPropertyCandidates(null, bindingAttr, null, false).ToArray();
		}

		public override EventInfo[] GetEvents(BindingFlags bindingAttr)
		{
			return this.GetEventCandidates(null, bindingAttr, false).ToArray();
		}

		public override FieldInfo[] GetFields(BindingFlags bindingAttr)
		{
			return this.GetFieldCandidates(null, bindingAttr, false).ToArray();
		}

		public override Type[] GetNestedTypes(BindingFlags bindingAttr)
		{
			return this.GetNestedTypeCandidates(null, bindingAttr, false).ToArray();
		}

		public override MemberInfo[] GetMembers(BindingFlags bindingAttr)
		{
			RuntimeType.ListBuilder<MethodInfo> methodCandidates = this.GetMethodCandidates(null, bindingAttr, CallingConventions.Any, null, false);
			RuntimeType.ListBuilder<ConstructorInfo> constructorCandidates = this.GetConstructorCandidates(null, bindingAttr, CallingConventions.Any, null, false);
			RuntimeType.ListBuilder<PropertyInfo> propertyCandidates = this.GetPropertyCandidates(null, bindingAttr, null, false);
			RuntimeType.ListBuilder<EventInfo> eventCandidates = this.GetEventCandidates(null, bindingAttr, false);
			RuntimeType.ListBuilder<FieldInfo> fieldCandidates = this.GetFieldCandidates(null, bindingAttr, false);
			RuntimeType.ListBuilder<Type> nestedTypeCandidates = this.GetNestedTypeCandidates(null, bindingAttr, false);
			MemberInfo[] array = new MemberInfo[methodCandidates.Count + constructorCandidates.Count + propertyCandidates.Count + eventCandidates.Count + fieldCandidates.Count + nestedTypeCandidates.Count];
			int num = 0;
			methodCandidates.CopyTo(array, num);
			num += methodCandidates.Count;
			constructorCandidates.CopyTo(array, num);
			num += constructorCandidates.Count;
			propertyCandidates.CopyTo(array, num);
			num += propertyCandidates.Count;
			eventCandidates.CopyTo(array, num);
			num += eventCandidates.Count;
			fieldCandidates.CopyTo(array, num);
			num += fieldCandidates.Count;
			nestedTypeCandidates.CopyTo(array, num);
			num += nestedTypeCandidates.Count;
			return array;
		}

		protected override MethodInfo GetMethodImpl(string name, BindingFlags bindingAttr, Binder binder, CallingConventions callConv, Type[] types, ParameterModifier[] modifiers)
		{
			RuntimeType.ListBuilder<MethodInfo> methodCandidates = this.GetMethodCandidates(name, bindingAttr, callConv, types, false);
			if (methodCandidates.Count == 0)
			{
				return null;
			}
			if (types == null || types.Length == 0)
			{
				MethodInfo methodInfo = methodCandidates[0];
				if (methodCandidates.Count == 1)
				{
					return methodInfo;
				}
				if (types == null)
				{
					for (int i = 1; i < methodCandidates.Count; i++)
					{
						if (!global::System.DefaultBinder.CompareMethodSigAndName(methodCandidates[i], methodInfo))
						{
							throw new AmbiguousMatchException(Environment.GetResourceString("Ambiguous match found."));
						}
					}
					return global::System.DefaultBinder.FindMostDerivedNewSlotMeth(methodCandidates.ToArray(), methodCandidates.Count) as MethodInfo;
				}
			}
			if (binder == null)
			{
				binder = Type.DefaultBinder;
			}
			return binder.SelectMethod(bindingAttr, methodCandidates.ToArray(), types, modifiers) as MethodInfo;
		}

		protected override ConstructorInfo GetConstructorImpl(BindingFlags bindingAttr, Binder binder, CallingConventions callConvention, Type[] types, ParameterModifier[] modifiers)
		{
			RuntimeType.ListBuilder<ConstructorInfo> constructorCandidates = this.GetConstructorCandidates(null, bindingAttr, CallingConventions.Any, types, false);
			if (constructorCandidates.Count == 0)
			{
				return null;
			}
			if (types.Length == 0 && constructorCandidates.Count == 1)
			{
				ConstructorInfo constructorInfo = constructorCandidates[0];
				ParameterInfo[] parametersNoCopy = constructorInfo.GetParametersNoCopy();
				if (parametersNoCopy == null || parametersNoCopy.Length == 0)
				{
					return constructorInfo;
				}
			}
			if ((bindingAttr & BindingFlags.ExactBinding) != BindingFlags.Default)
			{
				return global::System.DefaultBinder.ExactBinding(constructorCandidates.ToArray(), types, modifiers) as ConstructorInfo;
			}
			if (binder == null)
			{
				binder = Type.DefaultBinder;
			}
			return binder.SelectMethod(bindingAttr, constructorCandidates.ToArray(), types, modifiers) as ConstructorInfo;
		}

		protected override PropertyInfo GetPropertyImpl(string name, BindingFlags bindingAttr, Binder binder, Type returnType, Type[] types, ParameterModifier[] modifiers)
		{
			if (name == null)
			{
				throw new ArgumentNullException();
			}
			RuntimeType.ListBuilder<PropertyInfo> propertyCandidates = this.GetPropertyCandidates(name, bindingAttr, types, false);
			if (propertyCandidates.Count == 0)
			{
				return null;
			}
			if (types == null || types.Length == 0)
			{
				if (propertyCandidates.Count == 1)
				{
					PropertyInfo propertyInfo = propertyCandidates[0];
					if (returnType != null && !returnType.IsEquivalentTo(propertyInfo.PropertyType))
					{
						return null;
					}
					return propertyInfo;
				}
				else if (returnType == null)
				{
					throw new AmbiguousMatchException(Environment.GetResourceString("Ambiguous match found."));
				}
			}
			if ((bindingAttr & BindingFlags.ExactBinding) != BindingFlags.Default)
			{
				return global::System.DefaultBinder.ExactPropertyBinding(propertyCandidates.ToArray(), returnType, types, modifiers);
			}
			if (binder == null)
			{
				binder = Type.DefaultBinder;
			}
			return binder.SelectProperty(bindingAttr, propertyCandidates.ToArray(), returnType, types, modifiers);
		}

		public override EventInfo GetEvent(string name, BindingFlags bindingAttr)
		{
			if (name == null)
			{
				throw new ArgumentNullException();
			}
			bool flag;
			RuntimeType.MemberListType memberListType;
			RuntimeType.FilterHelper(bindingAttr, ref name, out flag, out memberListType);
			RuntimeEventInfo[] events_internal = this.GetEvents_internal(name, bindingAttr, this);
			EventInfo eventInfo = null;
			bindingAttr ^= BindingFlags.DeclaredOnly;
			foreach (RuntimeEventInfo runtimeEventInfo in events_internal)
			{
				if ((bindingAttr & runtimeEventInfo.BindingFlags) == runtimeEventInfo.BindingFlags)
				{
					if (eventInfo != null)
					{
						throw new AmbiguousMatchException(Environment.GetResourceString("Ambiguous match found."));
					}
					eventInfo = runtimeEventInfo;
				}
			}
			return eventInfo;
		}

		public override FieldInfo GetField(string name, BindingFlags bindingAttr)
		{
			if (name == null)
			{
				throw new ArgumentNullException();
			}
			bool flag;
			RuntimeType.MemberListType memberListType;
			RuntimeType.FilterHelper(bindingAttr, ref name, out flag, out memberListType);
			RuntimeFieldInfo[] fields_internal = this.GetFields_internal(name, bindingAttr, this);
			FieldInfo fieldInfo = null;
			bindingAttr ^= BindingFlags.DeclaredOnly;
			bool flag2 = false;
			foreach (RuntimeFieldInfo runtimeFieldInfo in fields_internal)
			{
				if ((bindingAttr & runtimeFieldInfo.BindingFlags) == runtimeFieldInfo.BindingFlags)
				{
					if (fieldInfo != null)
					{
						if (runtimeFieldInfo.DeclaringType == fieldInfo.DeclaringType)
						{
							throw new AmbiguousMatchException(Environment.GetResourceString("Ambiguous match found."));
						}
						if (fieldInfo.DeclaringType.IsInterface && runtimeFieldInfo.DeclaringType.IsInterface)
						{
							flag2 = true;
						}
					}
					if (fieldInfo == null || runtimeFieldInfo.DeclaringType.IsSubclassOf(fieldInfo.DeclaringType) || fieldInfo.DeclaringType.IsInterface)
					{
						fieldInfo = runtimeFieldInfo;
					}
				}
			}
			if (flag2 && fieldInfo.DeclaringType.IsInterface)
			{
				throw new AmbiguousMatchException(Environment.GetResourceString("Ambiguous match found."));
			}
			return fieldInfo;
		}

		public override Type GetInterface(string fullname, bool ignoreCase)
		{
			if (fullname == null)
			{
				throw new ArgumentNullException();
			}
			BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.NonPublic;
			bindingFlags &= ~BindingFlags.Static;
			if (ignoreCase)
			{
				bindingFlags |= BindingFlags.IgnoreCase;
			}
			string text;
			string text2;
			RuntimeType.SplitName(fullname, out text, out text2);
			RuntimeType.MemberListType memberListType;
			RuntimeType.FilterHelper(bindingFlags, ref text, out ignoreCase, out memberListType);
			List<RuntimeType> list = null;
			StringComparison stringComparison = (ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal);
			foreach (RuntimeType runtimeType in this.GetInterfaces())
			{
				if (string.Equals(runtimeType.Name, text, stringComparison))
				{
					if (list == null)
					{
						list = new List<RuntimeType>(2);
					}
					list.Add(runtimeType);
				}
			}
			if (list == null)
			{
				return null;
			}
			RuntimeType[] array = list.ToArray();
			RuntimeType runtimeType2 = null;
			foreach (RuntimeType runtimeType3 in array)
			{
				if (RuntimeType.FilterApplyType(runtimeType3, bindingFlags, text, false, text2))
				{
					if (runtimeType2 != null)
					{
						throw new AmbiguousMatchException(Environment.GetResourceString("Ambiguous match found."));
					}
					runtimeType2 = runtimeType3;
				}
			}
			return runtimeType2;
		}

		public override Type GetNestedType(string fullname, BindingFlags bindingAttr)
		{
			if (fullname == null)
			{
				throw new ArgumentNullException();
			}
			bindingAttr &= ~BindingFlags.Static;
			string text;
			string text2;
			RuntimeType.SplitName(fullname, out text, out text2);
			bool flag;
			RuntimeType.MemberListType memberListType;
			RuntimeType.FilterHelper(bindingAttr, ref text, out flag, out memberListType);
			RuntimeType[] nestedTypes_internal = this.GetNestedTypes_internal(text, bindingAttr);
			RuntimeType runtimeType = null;
			foreach (RuntimeType runtimeType2 in nestedTypes_internal)
			{
				if (RuntimeType.FilterApplyType(runtimeType2, bindingAttr, text, false, text2))
				{
					if (runtimeType != null)
					{
						throw new AmbiguousMatchException(Environment.GetResourceString("Ambiguous match found."));
					}
					runtimeType = runtimeType2;
				}
			}
			return runtimeType;
		}

		public override MemberInfo[] GetMember(string name, MemberTypes type, BindingFlags bindingAttr)
		{
			if (name == null)
			{
				throw new ArgumentNullException();
			}
			RuntimeType.ListBuilder<MethodInfo> listBuilder = default(RuntimeType.ListBuilder<MethodInfo>);
			RuntimeType.ListBuilder<ConstructorInfo> listBuilder2 = default(RuntimeType.ListBuilder<ConstructorInfo>);
			RuntimeType.ListBuilder<PropertyInfo> listBuilder3 = default(RuntimeType.ListBuilder<PropertyInfo>);
			RuntimeType.ListBuilder<EventInfo> listBuilder4 = default(RuntimeType.ListBuilder<EventInfo>);
			RuntimeType.ListBuilder<FieldInfo> listBuilder5 = default(RuntimeType.ListBuilder<FieldInfo>);
			RuntimeType.ListBuilder<Type> listBuilder6 = default(RuntimeType.ListBuilder<Type>);
			int num = 0;
			if ((type & MemberTypes.Method) != (MemberTypes)0)
			{
				listBuilder = this.GetMethodCandidates(name, bindingAttr, CallingConventions.Any, null, true);
				if (type == MemberTypes.Method)
				{
					return listBuilder.ToArray();
				}
				num += listBuilder.Count;
			}
			if ((type & MemberTypes.Constructor) != (MemberTypes)0)
			{
				listBuilder2 = this.GetConstructorCandidates(name, bindingAttr, CallingConventions.Any, null, true);
				if (type == MemberTypes.Constructor)
				{
					return listBuilder2.ToArray();
				}
				num += listBuilder2.Count;
			}
			if ((type & MemberTypes.Property) != (MemberTypes)0)
			{
				listBuilder3 = this.GetPropertyCandidates(name, bindingAttr, null, true);
				if (type == MemberTypes.Property)
				{
					return listBuilder3.ToArray();
				}
				num += listBuilder3.Count;
			}
			if ((type & MemberTypes.Event) != (MemberTypes)0)
			{
				listBuilder4 = this.GetEventCandidates(name, bindingAttr, true);
				if (type == MemberTypes.Event)
				{
					return listBuilder4.ToArray();
				}
				num += listBuilder4.Count;
			}
			if ((type & MemberTypes.Field) != (MemberTypes)0)
			{
				listBuilder5 = this.GetFieldCandidates(name, bindingAttr, true);
				if (type == MemberTypes.Field)
				{
					return listBuilder5.ToArray();
				}
				num += listBuilder5.Count;
			}
			if ((type & (MemberTypes.TypeInfo | MemberTypes.NestedType)) != (MemberTypes)0)
			{
				listBuilder6 = this.GetNestedTypeCandidates(name, bindingAttr, true);
				if (type == MemberTypes.NestedType || type == MemberTypes.TypeInfo)
				{
					return listBuilder6.ToArray();
				}
				num += listBuilder6.Count;
			}
			MemberInfo[] array = ((type == (MemberTypes.Constructor | MemberTypes.Method)) ? new MethodBase[num] : new MemberInfo[num]);
			int num2 = 0;
			listBuilder.CopyTo(array, num2);
			num2 += listBuilder.Count;
			listBuilder2.CopyTo(array, num2);
			num2 += listBuilder2.Count;
			listBuilder3.CopyTo(array, num2);
			num2 += listBuilder3.Count;
			listBuilder4.CopyTo(array, num2);
			num2 += listBuilder4.Count;
			listBuilder5.CopyTo(array, num2);
			num2 += listBuilder5.Count;
			listBuilder6.CopyTo(array, num2);
			num2 += listBuilder6.Count;
			return array;
		}

		public override Module Module
		{
			get
			{
				return this.GetRuntimeModule();
			}
		}

		internal RuntimeModule GetRuntimeModule()
		{
			return RuntimeTypeHandle.GetModule(this);
		}

		public override Assembly Assembly
		{
			get
			{
				return this.GetRuntimeAssembly();
			}
		}

		internal RuntimeAssembly GetRuntimeAssembly()
		{
			return RuntimeTypeHandle.GetAssembly(this);
		}

		public override RuntimeTypeHandle TypeHandle
		{
			get
			{
				return new RuntimeTypeHandle(this);
			}
		}

		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
		internal sealed override RuntimeTypeHandle GetTypeHandleInternal()
		{
			return new RuntimeTypeHandle(this);
		}

		[SecuritySafeCritical]
		public override bool IsInstanceOfType(object o)
		{
			return RuntimeTypeHandle.IsInstanceOfType(this, o);
		}

		[ComVisible(true)]
		public override bool IsSubclassOf(Type type)
		{
			if (type == null)
			{
				throw new ArgumentNullException("type");
			}
			RuntimeType runtimeType = type as RuntimeType;
			if (runtimeType == null)
			{
				return false;
			}
			RuntimeType runtimeType2 = this.GetBaseType();
			while (runtimeType2 != null)
			{
				if (runtimeType2 == runtimeType)
				{
					return true;
				}
				runtimeType2 = runtimeType2.GetBaseType();
			}
			return runtimeType == RuntimeType.ObjectType && runtimeType != this;
		}

		public override bool IsAssignableFrom(global::System.Reflection.TypeInfo typeInfo)
		{
			return !(typeInfo == null) && this.IsAssignableFrom(typeInfo.AsType());
		}

		public override bool IsAssignableFrom(Type c)
		{
			if (c == null)
			{
				return false;
			}
			if (c == this)
			{
				return true;
			}
			RuntimeType runtimeType = c.UnderlyingSystemType as RuntimeType;
			if (runtimeType != null)
			{
				return RuntimeTypeHandle.CanCastTo(runtimeType, this);
			}
			if (c is TypeBuilder)
			{
				if (c.IsSubclassOf(this))
				{
					return true;
				}
				if (base.IsInterface)
				{
					return c.ImplementInterface(this);
				}
				if (this.IsGenericParameter)
				{
					Type[] genericParameterConstraints = this.GetGenericParameterConstraints();
					for (int i = 0; i < genericParameterConstraints.Length; i++)
					{
						if (!genericParameterConstraints[i].IsAssignableFrom(c))
						{
							return false;
						}
					}
					return true;
				}
			}
			return false;
		}

		public override bool IsEquivalentTo(Type other)
		{
			RuntimeType runtimeType = other as RuntimeType;
			return runtimeType != null && (runtimeType == this || RuntimeTypeHandle.IsEquivalentTo(this, runtimeType));
		}

		public override Type BaseType
		{
			get
			{
				return this.GetBaseType();
			}
		}

		private RuntimeType GetBaseType()
		{
			if (base.IsInterface)
			{
				return null;
			}
			if (RuntimeTypeHandle.IsGenericVariable(this))
			{
				Type[] genericParameterConstraints = this.GetGenericParameterConstraints();
				RuntimeType runtimeType = RuntimeType.ObjectType;
				foreach (RuntimeType runtimeType2 in genericParameterConstraints)
				{
					if (!runtimeType2.IsInterface)
					{
						if (runtimeType2.IsGenericParameter)
						{
							GenericParameterAttributes genericParameterAttributes = runtimeType2.GenericParameterAttributes & GenericParameterAttributes.SpecialConstraintMask;
							if ((genericParameterAttributes & GenericParameterAttributes.ReferenceTypeConstraint) == GenericParameterAttributes.None && (genericParameterAttributes & GenericParameterAttributes.NotNullableValueTypeConstraint) == GenericParameterAttributes.None)
							{
								goto IL_0055;
							}
						}
						runtimeType = runtimeType2;
					}
					IL_0055:;
				}
				if (runtimeType == RuntimeType.ObjectType && (this.GenericParameterAttributes & GenericParameterAttributes.SpecialConstraintMask & GenericParameterAttributes.NotNullableValueTypeConstraint) != GenericParameterAttributes.None)
				{
					runtimeType = RuntimeType.ValueType;
				}
				return runtimeType;
			}
			return RuntimeTypeHandle.GetBaseType(this);
		}

		public override Type UnderlyingSystemType
		{
			get
			{
				return this;
			}
		}

		[SecuritySafeCritical]
		protected override TypeAttributes GetAttributeFlagsImpl()
		{
			return RuntimeTypeHandle.GetAttributes(this);
		}

		[SecuritySafeCritical]
		protected override bool IsContextfulImpl()
		{
			return RuntimeTypeHandle.IsContextful(this);
		}

		protected override bool IsByRefImpl()
		{
			return RuntimeTypeHandle.IsByRef(this);
		}

		protected override bool IsPrimitiveImpl()
		{
			return RuntimeTypeHandle.IsPrimitive(this);
		}

		protected override bool IsPointerImpl()
		{
			return RuntimeTypeHandle.IsPointer(this);
		}

		[SecuritySafeCritical]
		protected override bool IsCOMObjectImpl()
		{
			return RuntimeTypeHandle.IsComObject(this, false);
		}

		[SecuritySafeCritical]
		internal override bool IsWindowsRuntimeObjectImpl()
		{
			return RuntimeType.IsWindowsRuntimeObjectType(this);
		}

		[SecuritySafeCritical]
		internal override bool IsExportedToWindowsRuntimeImpl()
		{
			return RuntimeType.IsTypeExportedToWindowsRuntime(this);
		}

		[SecurityCritical]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool IsWindowsRuntimeObjectType(RuntimeType type);

		[SecurityCritical]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool IsTypeExportedToWindowsRuntime(RuntimeType type);

		[SecuritySafeCritical]
		internal override bool HasProxyAttributeImpl()
		{
			return RuntimeTypeHandle.HasProxyAttribute(this);
		}

		internal bool IsDelegate()
		{
			return this.GetBaseType() == typeof(MulticastDelegate);
		}

		protected override bool IsValueTypeImpl()
		{
			return !(this == typeof(ValueType)) && !(this == typeof(Enum)) && this.IsSubclassOf(typeof(ValueType));
		}

		public override bool IsEnum
		{
			get
			{
				return this.GetBaseType() == RuntimeType.EnumType;
			}
		}

		protected override bool HasElementTypeImpl()
		{
			return RuntimeTypeHandle.HasElementType(this);
		}

		public override GenericParameterAttributes GenericParameterAttributes
		{
			[SecuritySafeCritical]
			get
			{
				if (!this.IsGenericParameter)
				{
					throw new InvalidOperationException(Environment.GetResourceString("Method may only be called on a Type for which Type.IsGenericParameter is true."));
				}
				return this.GetGenericParameterAttributes();
			}
		}

		internal override bool IsSzArray
		{
			get
			{
				return RuntimeTypeHandle.IsSzArray(this);
			}
		}

		protected override bool IsArrayImpl()
		{
			return RuntimeTypeHandle.IsArray(this);
		}

		[SecuritySafeCritical]
		public override int GetArrayRank()
		{
			if (!this.IsArrayImpl())
			{
				throw new ArgumentException(Environment.GetResourceString("Must be an array type."));
			}
			return RuntimeTypeHandle.GetArrayRank(this);
		}

		public override Type GetElementType()
		{
			return RuntimeTypeHandle.GetElementType(this);
		}

		public override string[] GetEnumNames()
		{
			if (!this.IsEnum)
			{
				throw new ArgumentException(Environment.GetResourceString("Type provided must be an Enum."), "enumType");
			}
			string[] array = Enum.InternalGetNames(this);
			string[] array2 = new string[array.Length];
			Array.Copy(array, array2, array.Length);
			return array2;
		}

		[SecuritySafeCritical]
		public override Array GetEnumValues()
		{
			if (!this.IsEnum)
			{
				throw new ArgumentException(Environment.GetResourceString("Type provided must be an Enum."), "enumType");
			}
			ulong[] array = Enum.InternalGetValues(this);
			Array array2 = Array.UnsafeCreateInstance(this, new int[] { array.Length });
			for (int i = 0; i < array.Length; i++)
			{
				object obj = Enum.ToObject(this, array[i]);
				array2.SetValue(obj, i);
			}
			return array2;
		}

		public override Type GetEnumUnderlyingType()
		{
			if (!this.IsEnum)
			{
				throw new ArgumentException(Environment.GetResourceString("Type provided must be an Enum."), "enumType");
			}
			return Enum.InternalGetUnderlyingType(this);
		}

		public override bool IsEnumDefined(object value)
		{
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			RuntimeType runtimeType = (RuntimeType)value.GetType();
			if (runtimeType.IsEnum)
			{
				if (!runtimeType.IsEquivalentTo(this))
				{
					throw new ArgumentException(Environment.GetResourceString("Object must be the same type as the enum. The type passed in was '{0}'; the enum type was '{1}'.", new object[]
					{
						runtimeType.ToString(),
						this.ToString()
					}));
				}
				runtimeType = (RuntimeType)runtimeType.GetEnumUnderlyingType();
			}
			if (runtimeType == RuntimeType.StringType)
			{
				return Array.IndexOf<object>(Enum.InternalGetNames(this), value) >= 0;
			}
			if (Type.IsIntegerType(runtimeType))
			{
				RuntimeType runtimeType2 = Enum.InternalGetUnderlyingType(this);
				if (runtimeType2 != runtimeType)
				{
					throw new ArgumentException(Environment.GetResourceString("Enum underlying type and the object must be same type or object must be a String. Type passed in was '{0}'; the enum underlying type was '{1}'.", new object[]
					{
						runtimeType.ToString(),
						runtimeType2.ToString()
					}));
				}
				ulong[] array = Enum.InternalGetValues(this);
				ulong num = Enum.ToUInt64(value);
				return Array.BinarySearch<ulong>(array, num) >= 0;
			}
			else
			{
				if (CompatibilitySwitches.IsAppEarlierThanWindowsPhone8)
				{
					throw new ArgumentException(Environment.GetResourceString("Enum underlying type and the object must be same type or object must be a String. Type passed in was '{0}'; the enum underlying type was '{1}'.", new object[]
					{
						runtimeType.ToString(),
						this.GetEnumUnderlyingType()
					}));
				}
				throw new InvalidOperationException(Environment.GetResourceString("Unknown enum type."));
			}
		}

		public override string GetEnumName(object value)
		{
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			Type type = value.GetType();
			if (!type.IsEnum && !Type.IsIntegerType(type))
			{
				throw new ArgumentException(Environment.GetResourceString("The value passed in must be an enum base or an underlying type for an enum, such as an Int32."), "value");
			}
			ulong[] array = Enum.InternalGetValues(this);
			ulong num = Enum.ToUInt64(value);
			int num2 = Array.BinarySearch<ulong>(array, num);
			if (num2 >= 0)
			{
				return Enum.InternalGetNames(this)[num2];
			}
			return null;
		}

		internal RuntimeType[] GetGenericArgumentsInternal()
		{
			return (RuntimeType[])this.GetGenericArgumentsInternal(true);
		}

		public override Type[] GetGenericArguments()
		{
			Type[] array = this.GetGenericArgumentsInternal(false);
			if (array == null)
			{
				array = EmptyArray<Type>.Value;
			}
			return array;
		}

		[SecuritySafeCritical]
		public override Type MakeGenericType(params Type[] instantiation)
		{
			if (instantiation == null)
			{
				throw new ArgumentNullException("instantiation");
			}
			RuntimeType[] array = new RuntimeType[instantiation.Length];
			if (!this.IsGenericTypeDefinition)
			{
				throw new InvalidOperationException(Environment.GetResourceString("{0} is not a GenericTypeDefinition. MakeGenericType may only be called on a type for which Type.IsGenericTypeDefinition is true.", new object[] { this }));
			}
			if (this.GetGenericArguments().Length != instantiation.Length)
			{
				throw new ArgumentException(Environment.GetResourceString("The number of generic arguments provided doesn't equal the arity of the generic type definition."), "instantiation");
			}
			for (int i = 0; i < instantiation.Length; i++)
			{
				Type type = instantiation[i];
				if (type == null)
				{
					throw new ArgumentNullException();
				}
				RuntimeType runtimeType = type as RuntimeType;
				if (runtimeType == null)
				{
					Type[] array2 = new Type[instantiation.Length];
					for (int j = 0; j < instantiation.Length; j++)
					{
						array2[j] = instantiation[j];
					}
					instantiation = array2;
					return TypeBuilderInstantiation.MakeGenericType(this, instantiation);
				}
				array[i] = runtimeType;
			}
			RuntimeType[] genericArgumentsInternal = this.GetGenericArgumentsInternal();
			RuntimeType.SanityCheckGenericArguments(array, genericArgumentsInternal);
			Type type2 = RuntimeType.MakeGenericType(this, array);
			if (type2 == null)
			{
				throw new TypeLoadException();
			}
			return type2;
		}

		public override bool IsGenericTypeDefinition
		{
			get
			{
				return RuntimeTypeHandle.IsGenericTypeDefinition(this);
			}
		}

		public override bool IsGenericParameter
		{
			get
			{
				return RuntimeTypeHandle.IsGenericVariable(this);
			}
		}

		public override int GenericParameterPosition
		{
			get
			{
				if (!this.IsGenericParameter)
				{
					throw new InvalidOperationException(Environment.GetResourceString("Method may only be called on a Type for which Type.IsGenericParameter is true."));
				}
				return this.GetGenericParameterPosition();
			}
		}

		public override Type GetGenericTypeDefinition()
		{
			if (!this.IsGenericType)
			{
				throw new InvalidOperationException(Environment.GetResourceString("This operation is only valid on generic types."));
			}
			return RuntimeTypeHandle.GetGenericTypeDefinition(this);
		}

		public override bool IsGenericType
		{
			get
			{
				return RuntimeTypeHandle.HasInstantiation(this);
			}
		}

		public override bool IsConstructedGenericType
		{
			get
			{
				return this.IsGenericType && !this.IsGenericTypeDefinition;
			}
		}

		public override MemberInfo[] GetDefaultMembers()
		{
			MemberInfo[] array = null;
			string defaultMemberName = this.GetDefaultMemberName();
			if (defaultMemberName != null)
			{
				array = base.GetMember(defaultMemberName);
			}
			if (array == null)
			{
				array = EmptyArray<MemberInfo>.Value;
			}
			return array;
		}

		[SecuritySafeCritical]
		[DebuggerStepThrough]
		[DebuggerHidden]
		public override object InvokeMember(string name, BindingFlags bindingFlags, Binder binder, object target, object[] providedArgs, ParameterModifier[] modifiers, CultureInfo culture, string[] namedParams)
		{
			if (this.IsGenericParameter)
			{
				throw new InvalidOperationException(Environment.GetResourceString("Method must be called on a Type for which Type.IsGenericParameter is false."));
			}
			if ((bindingFlags & (BindingFlags.InvokeMethod | BindingFlags.CreateInstance | BindingFlags.GetField | BindingFlags.SetField | BindingFlags.GetProperty | BindingFlags.SetProperty | BindingFlags.PutDispProperty | BindingFlags.PutRefDispProperty)) == BindingFlags.Default)
			{
				throw new ArgumentException(Environment.GetResourceString("Must specify binding flags describing the invoke operation required (BindingFlags.InvokeMethod CreateInstance GetField SetField GetProperty SetProperty)."), "bindingFlags");
			}
			if ((bindingFlags & (BindingFlags)255) == BindingFlags.Default)
			{
				bindingFlags |= BindingFlags.Instance | BindingFlags.Public;
				if ((bindingFlags & BindingFlags.CreateInstance) == BindingFlags.Default)
				{
					bindingFlags |= BindingFlags.Static;
				}
			}
			if (namedParams != null)
			{
				if (providedArgs != null)
				{
					if (namedParams.Length > providedArgs.Length)
					{
						throw new ArgumentException(Environment.GetResourceString("Named parameter array cannot be bigger than argument array."), "namedParams");
					}
				}
				else if (namedParams.Length != 0)
				{
					throw new ArgumentException(Environment.GetResourceString("Named parameter array cannot be bigger than argument array."), "namedParams");
				}
			}
			if (target != null && target.GetType().IsCOMObject)
			{
				if ((bindingFlags & (BindingFlags.InvokeMethod | BindingFlags.GetProperty | BindingFlags.SetProperty | BindingFlags.PutDispProperty | BindingFlags.PutRefDispProperty)) == BindingFlags.Default)
				{
					throw new ArgumentException(Environment.GetResourceString("Must specify property Set or Get or method call for a COM Object."), "bindingFlags");
				}
				if ((bindingFlags & BindingFlags.GetProperty) != BindingFlags.Default && (bindingFlags & (BindingFlags.InvokeMethod | BindingFlags.GetProperty | BindingFlags.SetProperty | BindingFlags.PutDispProperty | BindingFlags.PutRefDispProperty) & ~(BindingFlags.InvokeMethod | BindingFlags.GetProperty)) != BindingFlags.Default)
				{
					throw new ArgumentException(Environment.GetResourceString("Cannot specify both Get and Set on a property."), "bindingFlags");
				}
				if ((bindingFlags & BindingFlags.InvokeMethod) != BindingFlags.Default && (bindingFlags & (BindingFlags.InvokeMethod | BindingFlags.GetProperty | BindingFlags.SetProperty | BindingFlags.PutDispProperty | BindingFlags.PutRefDispProperty) & ~(BindingFlags.InvokeMethod | BindingFlags.GetProperty)) != BindingFlags.Default)
				{
					throw new ArgumentException(Environment.GetResourceString("Cannot specify Set on a property and Invoke on a method."), "bindingFlags");
				}
				if ((bindingFlags & BindingFlags.SetProperty) != BindingFlags.Default && (bindingFlags & (BindingFlags.InvokeMethod | BindingFlags.GetProperty | BindingFlags.SetProperty | BindingFlags.PutDispProperty | BindingFlags.PutRefDispProperty) & ~BindingFlags.SetProperty) != BindingFlags.Default)
				{
					throw new ArgumentException(Environment.GetResourceString("Only one of the following binding flags can be set: BindingFlags.SetProperty, BindingFlags.PutDispProperty,  BindingFlags.PutRefDispProperty."), "bindingFlags");
				}
				if ((bindingFlags & BindingFlags.PutDispProperty) != BindingFlags.Default && (bindingFlags & (BindingFlags.InvokeMethod | BindingFlags.GetProperty | BindingFlags.SetProperty | BindingFlags.PutDispProperty | BindingFlags.PutRefDispProperty) & ~BindingFlags.PutDispProperty) != BindingFlags.Default)
				{
					throw new ArgumentException(Environment.GetResourceString("Only one of the following binding flags can be set: BindingFlags.SetProperty, BindingFlags.PutDispProperty,  BindingFlags.PutRefDispProperty."), "bindingFlags");
				}
				if ((bindingFlags & BindingFlags.PutRefDispProperty) != BindingFlags.Default && (bindingFlags & (BindingFlags.InvokeMethod | BindingFlags.GetProperty | BindingFlags.SetProperty | BindingFlags.PutDispProperty | BindingFlags.PutRefDispProperty) & ~BindingFlags.PutRefDispProperty) != BindingFlags.Default)
				{
					throw new ArgumentException(Environment.GetResourceString("Only one of the following binding flags can be set: BindingFlags.SetProperty, BindingFlags.PutDispProperty,  BindingFlags.PutRefDispProperty."), "bindingFlags");
				}
				if (RemotingServices.IsTransparentProxy(target))
				{
					throw new NotImplementedException();
				}
				if (name == null)
				{
					throw new ArgumentNullException("name");
				}
				throw new NotImplementedException();
			}
			else
			{
				if (namedParams != null && Array.IndexOf<string>(namedParams, null) != -1)
				{
					throw new ArgumentException(Environment.GetResourceString("Named parameter value must not be null."), "namedParams");
				}
				int num = ((providedArgs != null) ? providedArgs.Length : 0);
				if (binder == null)
				{
					binder = Type.DefaultBinder;
				}
				if ((bindingFlags & BindingFlags.CreateInstance) != BindingFlags.Default)
				{
					if ((bindingFlags & BindingFlags.CreateInstance) != BindingFlags.Default && (bindingFlags & (BindingFlags.InvokeMethod | BindingFlags.GetField | BindingFlags.SetField | BindingFlags.GetProperty | BindingFlags.SetProperty)) != BindingFlags.Default)
					{
						throw new ArgumentException(Environment.GetResourceString("Cannot specify both CreateInstance and another access type."), "bindingFlags");
					}
					return Activator.CreateInstance(this, bindingFlags, binder, providedArgs, culture);
				}
				else
				{
					if ((bindingFlags & (BindingFlags.PutDispProperty | BindingFlags.PutRefDispProperty)) != BindingFlags.Default)
					{
						bindingFlags |= BindingFlags.SetProperty;
					}
					if (name == null)
					{
						throw new ArgumentNullException("name");
					}
					if (name.Length == 0 || name.Equals("[DISPID=0]"))
					{
						name = this.GetDefaultMemberName();
						if (name == null)
						{
							name = "ToString";
						}
					}
					bool flag = (bindingFlags & BindingFlags.GetField) > BindingFlags.Default;
					bool flag2 = (bindingFlags & BindingFlags.SetField) > BindingFlags.Default;
					if (flag || flag2)
					{
						if (flag)
						{
							if (flag2)
							{
								throw new ArgumentException(Environment.GetResourceString("Cannot specify both Get and Set on a field."), "bindingFlags");
							}
							if ((bindingFlags & BindingFlags.SetProperty) != BindingFlags.Default)
							{
								throw new ArgumentException(Environment.GetResourceString("Cannot specify both GetField and SetProperty."), "bindingFlags");
							}
						}
						else
						{
							if (providedArgs == null)
							{
								throw new ArgumentNullException("providedArgs");
							}
							if ((bindingFlags & BindingFlags.GetProperty) != BindingFlags.Default)
							{
								throw new ArgumentException(Environment.GetResourceString("Cannot specify both SetField and GetProperty."), "bindingFlags");
							}
							if ((bindingFlags & BindingFlags.InvokeMethod) != BindingFlags.Default)
							{
								throw new ArgumentException(Environment.GetResourceString("Cannot specify Set on a Field and Invoke on a method."), "bindingFlags");
							}
						}
						FieldInfo fieldInfo = null;
						FieldInfo[] array = this.GetMember(name, MemberTypes.Field, bindingFlags) as FieldInfo[];
						if (array.Length == 1)
						{
							fieldInfo = array[0];
						}
						else if (array.Length != 0)
						{
							fieldInfo = binder.BindToField(bindingFlags, array, flag ? Empty.Value : providedArgs[0], culture);
						}
						if (fieldInfo != null)
						{
							if (fieldInfo.FieldType.IsArray || fieldInfo.FieldType == typeof(Array))
							{
								int num2;
								if ((bindingFlags & BindingFlags.GetField) != BindingFlags.Default)
								{
									num2 = num;
								}
								else
								{
									num2 = num - 1;
								}
								if (num2 > 0)
								{
									int[] array2 = new int[num2];
									for (int i = 0; i < num2; i++)
									{
										try
										{
											array2[i] = ((IConvertible)providedArgs[i]).ToInt32(null);
										}
										catch (InvalidCastException)
										{
											throw new ArgumentException(Environment.GetResourceString("All indexes must be of type Int32."));
										}
									}
									Array array3 = (Array)fieldInfo.GetValue(target);
									if ((bindingFlags & BindingFlags.GetField) != BindingFlags.Default)
									{
										return array3.GetValue(array2);
									}
									array3.SetValue(providedArgs[num2], array2);
									return null;
								}
							}
							if (flag)
							{
								if (num != 0)
								{
									throw new ArgumentException(Environment.GetResourceString("No arguments can be provided to Get a field value."), "bindingFlags");
								}
								return fieldInfo.GetValue(target);
							}
							else
							{
								if (num != 1)
								{
									throw new ArgumentException(Environment.GetResourceString("Only the field value can be specified to set a field value."), "bindingFlags");
								}
								fieldInfo.SetValue(target, providedArgs[0], bindingFlags, binder, culture);
								return null;
							}
						}
						else if ((bindingFlags & (BindingFlags)16773888) == BindingFlags.Default)
						{
							throw new MissingFieldException(this.FullName, name);
						}
					}
					bool flag3 = (bindingFlags & BindingFlags.GetProperty) > BindingFlags.Default;
					bool flag4 = (bindingFlags & BindingFlags.SetProperty) > BindingFlags.Default;
					if (flag3 || flag4)
					{
						if (flag3)
						{
							if (flag4)
							{
								throw new ArgumentException(Environment.GetResourceString("Cannot specify both Get and Set on a property."), "bindingFlags");
							}
						}
						else if ((bindingFlags & BindingFlags.InvokeMethod) != BindingFlags.Default)
						{
							throw new ArgumentException(Environment.GetResourceString("Cannot specify Set on a property and Invoke on a method."), "bindingFlags");
						}
					}
					MethodInfo[] array4 = null;
					MethodInfo methodInfo = null;
					if ((bindingFlags & BindingFlags.InvokeMethod) != BindingFlags.Default)
					{
						MethodInfo[] array5 = this.GetMember(name, MemberTypes.Method, bindingFlags) as MethodInfo[];
						List<MethodInfo> list = null;
						foreach (MethodInfo methodInfo2 in array5)
						{
							if (RuntimeType.FilterApplyMethodInfo((RuntimeMethodInfo)methodInfo2, bindingFlags, CallingConventions.Any, new Type[num]))
							{
								if (methodInfo == null)
								{
									methodInfo = methodInfo2;
								}
								else
								{
									if (list == null)
									{
										list = new List<MethodInfo>(array5.Length);
										list.Add(methodInfo);
									}
									list.Add(methodInfo2);
								}
							}
						}
						if (list != null)
						{
							array4 = new MethodInfo[list.Count];
							list.CopyTo(array4);
						}
					}
					if ((methodInfo == null && flag3) || flag4)
					{
						PropertyInfo[] array6 = this.GetMember(name, MemberTypes.Property, bindingFlags) as PropertyInfo[];
						List<MethodInfo> list2 = null;
						for (int k = 0; k < array6.Length; k++)
						{
							MethodInfo methodInfo3;
							if (flag4)
							{
								methodInfo3 = array6[k].GetSetMethod(true);
							}
							else
							{
								methodInfo3 = array6[k].GetGetMethod(true);
							}
							if (!(methodInfo3 == null) && RuntimeType.FilterApplyMethodInfo((RuntimeMethodInfo)methodInfo3, bindingFlags, CallingConventions.Any, new Type[num]))
							{
								if (methodInfo == null)
								{
									methodInfo = methodInfo3;
								}
								else
								{
									if (list2 == null)
									{
										list2 = new List<MethodInfo>(array6.Length);
										list2.Add(methodInfo);
									}
									list2.Add(methodInfo3);
								}
							}
						}
						if (list2 != null)
						{
							array4 = new MethodInfo[list2.Count];
							list2.CopyTo(array4);
						}
					}
					if (!(methodInfo != null))
					{
						throw new MissingMethodException(this.FullName, name);
					}
					if (array4 == null && num == 0 && methodInfo.GetParametersNoCopy().Length == 0 && (bindingFlags & BindingFlags.OptionalParamBinding) == BindingFlags.Default)
					{
						return methodInfo.Invoke(target, bindingFlags, binder, providedArgs, culture);
					}
					if (array4 == null)
					{
						array4 = new MethodInfo[] { methodInfo };
					}
					if (providedArgs == null)
					{
						providedArgs = EmptyArray<object>.Value;
					}
					object obj = null;
					MethodBase methodBase = null;
					try
					{
						methodBase = binder.BindToMethod(bindingFlags, array4, ref providedArgs, modifiers, culture, namedParams, out obj);
					}
					catch (MissingMethodException)
					{
					}
					if (methodBase == null)
					{
						throw new MissingMethodException(this.FullName, name);
					}
					object obj2 = ((MethodInfo)methodBase).Invoke(target, bindingFlags, binder, providedArgs, culture);
					if (obj != null)
					{
						binder.ReorderArgumentArray(ref providedArgs, obj);
					}
					return obj2;
				}
			}
		}

		public override bool Equals(object obj)
		{
			return obj == this;
		}

		public static bool operator ==(RuntimeType left, RuntimeType right)
		{
			return left == right;
		}

		public static bool operator !=(RuntimeType left, RuntimeType right)
		{
			return left != right;
		}

		public object Clone()
		{
			return this;
		}

		[SecurityCritical]
		public void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			if (info == null)
			{
				throw new ArgumentNullException("info");
			}
			UnitySerializationHolder.GetUnitySerializationInfo(info, this);
		}

		[SecuritySafeCritical]
		public override object[] GetCustomAttributes(bool inherit)
		{
			return MonoCustomAttrs.GetCustomAttributes(this, RuntimeType.ObjectType, inherit);
		}

		[SecuritySafeCritical]
		public override object[] GetCustomAttributes(Type attributeType, bool inherit)
		{
			if (attributeType == null)
			{
				throw new ArgumentNullException("attributeType");
			}
			RuntimeType runtimeType = attributeType.UnderlyingSystemType as RuntimeType;
			if (runtimeType == null)
			{
				throw new ArgumentException(Environment.GetResourceString("Type must be a type provided by the runtime."), "attributeType");
			}
			return MonoCustomAttrs.GetCustomAttributes(this, runtimeType, inherit);
		}

		[SecuritySafeCritical]
		public override bool IsDefined(Type attributeType, bool inherit)
		{
			if (attributeType == null)
			{
				throw new ArgumentNullException("attributeType");
			}
			RuntimeType runtimeType = attributeType.UnderlyingSystemType as RuntimeType;
			if (runtimeType == null)
			{
				throw new ArgumentException(Environment.GetResourceString("Type must be a type provided by the runtime."), "attributeType");
			}
			return MonoCustomAttrs.IsDefined(this, runtimeType, inherit);
		}

		public override IList<CustomAttributeData> GetCustomAttributesData()
		{
			return CustomAttributeData.GetCustomAttributesInternal(this);
		}

		internal override string FormatTypeName(bool serialization)
		{
			if (serialization)
			{
				return this.GetCachedName(TypeNameKind.SerializationName);
			}
			Type rootElementType = base.GetRootElementType();
			if (rootElementType.IsNested)
			{
				return this.Name;
			}
			string text = this.ToString();
			if (rootElementType.IsPrimitive || rootElementType == typeof(void) || rootElementType == typeof(TypedReference))
			{
				text = text.Substring("System.".Length);
			}
			return text;
		}

		public override MemberTypes MemberType
		{
			get
			{
				if (base.IsPublic || base.IsNotPublic)
				{
					return MemberTypes.TypeInfo;
				}
				return MemberTypes.NestedType;
			}
		}

		public override Type ReflectedType
		{
			get
			{
				return this.DeclaringType;
			}
		}

		public override int MetadataToken
		{
			[SecuritySafeCritical]
			get
			{
				return RuntimeTypeHandle.GetToken(this);
			}
		}

		private void CreateInstanceCheckThis()
		{
			if (this is ReflectionOnlyType)
			{
				throw new ArgumentException(Environment.GetResourceString("It is illegal to invoke a method on a Type loaded via ReflectionOnlyGetType."));
			}
			if (this.ContainsGenericParameters)
			{
				throw new ArgumentException(Environment.GetResourceString("Cannot create an instance of {0} because Type.ContainsGenericParameters is true.", new object[] { this }));
			}
			Type rootElementType = base.GetRootElementType();
			if (rootElementType == typeof(ArgIterator))
			{
				throw new NotSupportedException(Environment.GetResourceString("Cannot dynamically create an instance of ArgIterator."));
			}
			if (rootElementType == typeof(void))
			{
				throw new NotSupportedException(Environment.GetResourceString("Cannot dynamically create an instance of System.Void."));
			}
		}

		[SecurityCritical]
		internal object CreateInstanceImpl(BindingFlags bindingAttr, Binder binder, object[] args, CultureInfo culture, object[] activationAttributes, ref StackCrawlMark stackMark)
		{
			this.CreateInstanceCheckThis();
			object obj = null;
			try
			{
				try
				{
					if (activationAttributes != null)
					{
						ActivationServices.PushActivationAttributes(this, activationAttributes);
					}
					if (args == null)
					{
						args = EmptyArray<object>.Value;
					}
					int num = args.Length;
					if (binder == null)
					{
						binder = Type.DefaultBinder;
					}
					if (num == 0 && (bindingAttr & BindingFlags.Public) != BindingFlags.Default && (bindingAttr & BindingFlags.Instance) != BindingFlags.Default && (this.IsGenericCOMObjectImpl() || base.IsValueType))
					{
						obj = this.CreateInstanceDefaultCtor((bindingAttr & BindingFlags.NonPublic) == BindingFlags.Default, false, true, ref stackMark);
					}
					else
					{
						ConstructorInfo[] constructors = this.GetConstructors(bindingAttr);
						List<MethodBase> list = new List<MethodBase>(constructors.Length);
						Type[] array = new Type[num];
						for (int i = 0; i < num; i++)
						{
							if (args[i] != null)
							{
								array[i] = args[i].GetType();
							}
						}
						for (int j = 0; j < constructors.Length; j++)
						{
							if (RuntimeType.FilterApplyConstructorInfo((RuntimeConstructorInfo)constructors[j], bindingAttr, CallingConventions.Any, array))
							{
								list.Add(constructors[j]);
							}
						}
						MethodBase[] array2 = new MethodBase[list.Count];
						list.CopyTo(array2);
						if (array2 != null && array2.Length == 0)
						{
							array2 = null;
						}
						if (array2 == null)
						{
							if (activationAttributes != null)
							{
								ActivationServices.PopActivationAttributes(this);
								activationAttributes = null;
							}
							throw new MissingMethodException(Environment.GetResourceString("Constructor on type '{0}' not found.", new object[] { this.FullName }));
						}
						object obj2 = null;
						MethodBase methodBase;
						try
						{
							methodBase = binder.BindToMethod(bindingAttr, array2, ref args, null, culture, null, out obj2);
						}
						catch (MissingMethodException)
						{
							methodBase = null;
						}
						if (methodBase == null)
						{
							if (activationAttributes != null)
							{
								ActivationServices.PopActivationAttributes(this);
								activationAttributes = null;
							}
							throw new MissingMethodException(Environment.GetResourceString("Constructor on type '{0}' not found.", new object[] { this.FullName }));
						}
						if (methodBase.GetParametersNoCopy().Length == 0)
						{
							if (args.Length != 0)
							{
								throw new NotSupportedException(string.Format(CultureInfo.CurrentCulture, Environment.GetResourceString("Vararg calling convention not supported."), Array.Empty<object>()));
							}
							if (activationAttributes != null && activationAttributes.Length != 0)
							{
								obj = this.ActivationCreateInstance(methodBase, bindingAttr, binder, args, culture, activationAttributes);
							}
							else
							{
								obj = Activator.CreateInstance(this, true);
							}
						}
						else
						{
							if (activationAttributes != null && activationAttributes.Length != 0)
							{
								obj = this.ActivationCreateInstance(methodBase, bindingAttr, binder, args, culture, activationAttributes);
							}
							else
							{
								obj = ((ConstructorInfo)methodBase).Invoke(bindingAttr, binder, args, culture);
							}
							if (obj2 != null)
							{
								binder.ReorderArgumentArray(ref args, obj2);
							}
						}
					}
				}
				finally
				{
					if (activationAttributes != null)
					{
						ActivationServices.PopActivationAttributes(this);
						activationAttributes = null;
					}
				}
			}
			catch (Exception)
			{
				throw;
			}
			return obj;
		}

		private object ActivationCreateInstance(MethodBase invokeMethod, BindingFlags bindingAttr, Binder binder, object[] args, CultureInfo culture, object[] activationAttributes)
		{
			object obj = ActivationServices.CreateProxyFromAttributes(this, activationAttributes);
			if (obj != null)
			{
				invokeMethod.Invoke(obj, bindingAttr, binder, args, culture);
			}
			return obj;
		}

		[DebuggerHidden]
		[DebuggerStepThrough]
		[SecuritySafeCritical]
		internal object CreateInstanceDefaultCtor(bool publicOnly, bool skipCheckThis, bool fillCache, ref StackCrawlMark stackMark)
		{
			if (base.GetType() == typeof(ReflectionOnlyType))
			{
				throw new InvalidOperationException(Environment.GetResourceString("The requested operation is invalid in the ReflectionOnly context."));
			}
			return this.CreateInstanceSlow(publicOnly, skipCheckThis, fillCache, ref stackMark);
		}

		internal RuntimeType(object obj)
		{
			throw new NotImplementedException();
		}

		internal MonoCMethod GetDefaultConstructor()
		{
			MonoCMethod monoCMethod = null;
			if (this.type_info == null)
			{
				this.type_info = new MonoTypeInfo();
			}
			else
			{
				monoCMethod = this.type_info.default_ctor;
			}
			if (monoCMethod == null)
			{
				ConstructorInfo[] constructors = this.GetConstructors(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
				for (int i = 0; i < constructors.Length; i++)
				{
					if (constructors[i].GetParametersCount() == 0)
					{
						monoCMethod = (this.type_info.default_ctor = (MonoCMethod)constructors[i]);
						break;
					}
				}
			}
			return monoCMethod;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern MethodInfo GetCorrespondingInflatedMethod(MethodInfo generic);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern ConstructorInfo GetCorrespondingInflatedConstructor(ConstructorInfo generic);

		internal override MethodInfo GetMethod(MethodInfo fromNoninstanciated)
		{
			if (fromNoninstanciated == null)
			{
				throw new ArgumentNullException("fromNoninstanciated");
			}
			return this.GetCorrespondingInflatedMethod(fromNoninstanciated);
		}

		internal override ConstructorInfo GetConstructor(ConstructorInfo fromNoninstanciated)
		{
			if (fromNoninstanciated == null)
			{
				throw new ArgumentNullException("fromNoninstanciated");
			}
			return this.GetCorrespondingInflatedConstructor(fromNoninstanciated);
		}

		internal override FieldInfo GetField(FieldInfo fromNoninstanciated)
		{
			BindingFlags bindingFlags = (fromNoninstanciated.IsStatic ? BindingFlags.Static : BindingFlags.Instance);
			bindingFlags |= (fromNoninstanciated.IsPublic ? BindingFlags.Public : BindingFlags.NonPublic);
			return this.GetField(fromNoninstanciated.Name, bindingFlags);
		}

		private string GetDefaultMemberName()
		{
			object[] customAttributes = this.GetCustomAttributes(typeof(DefaultMemberAttribute), true);
			if (customAttributes.Length == 0)
			{
				return null;
			}
			return ((DefaultMemberAttribute)customAttributes[0]).MemberName;
		}

		internal RuntimeConstructorInfo GetSerializationCtor()
		{
			if (this.m_serializationCtor == null)
			{
				Type[] array = new Type[]
				{
					typeof(SerializationInfo),
					typeof(StreamingContext)
				};
				this.m_serializationCtor = base.GetConstructor(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, CallingConventions.Any, array, null) as RuntimeConstructorInfo;
			}
			return this.m_serializationCtor;
		}

		internal object CreateInstanceSlow(bool publicOnly, bool skipCheckThis, bool fillCache, ref StackCrawlMark stackMark)
		{
			if (!skipCheckThis)
			{
				this.CreateInstanceCheckThis();
			}
			return this.CreateInstanceMono(!publicOnly);
		}

		private object CreateInstanceMono(bool nonPublic)
		{
			MonoCMethod monoCMethod = this.GetDefaultConstructor();
			if (!nonPublic && monoCMethod != null && !monoCMethod.IsPublic)
			{
				monoCMethod = null;
			}
			if (monoCMethod == null)
			{
				Type rootElementType = base.GetRootElementType();
				if (rootElementType == typeof(TypedReference) || rootElementType == typeof(RuntimeArgumentHandle))
				{
					throw new NotSupportedException(Environment.GetResourceString("Cannot create boxed TypedReference, ArgIterator, or RuntimeArgumentHandle Objects."));
				}
				if (base.IsValueType)
				{
					return RuntimeType.CreateInstanceInternal(this);
				}
				throw new MissingMethodException(Locale.GetText("Default constructor not found for type " + this.FullName));
			}
			else
			{
				if (base.IsAbstract)
				{
					throw new MissingMethodException(Locale.GetText("Cannot create an abstract class '{0}'.", new object[] { this.FullName }));
				}
				return monoCMethod.InternalInvoke(null, null);
			}
		}

		internal object CheckValue(object value, Binder binder, CultureInfo culture, BindingFlags invokeAttr)
		{
			bool flag = false;
			object obj = this.TryConvertToType(value, ref flag);
			if (!flag)
			{
				return obj;
			}
			if ((invokeAttr & BindingFlags.ExactBinding) == BindingFlags.ExactBinding)
			{
				throw new ArgumentException(string.Format(CultureInfo.CurrentUICulture, Environment.GetResourceString("Object of type '{0}' cannot be converted to type '{1}'."), value.GetType(), this));
			}
			if (binder != null && binder != Type.DefaultBinder)
			{
				return binder.ChangeType(value, this, culture);
			}
			throw new ArgumentException(string.Format(CultureInfo.CurrentUICulture, Environment.GetResourceString("Object of type '{0}' cannot be converted to type '{1}'."), value.GetType(), this));
		}

		private object TryConvertToType(object value, ref bool failed)
		{
			if (this.IsInstanceOfType(value))
			{
				return value;
			}
			if (base.IsByRef)
			{
				Type elementType = this.GetElementType();
				if (value == null || elementType.IsInstanceOfType(value))
				{
					return value;
				}
			}
			if (value == null)
			{
				return value;
			}
			if (this.IsEnum)
			{
				if (Enum.GetUnderlyingType(this) == value.GetType())
				{
					return value;
				}
				object obj = RuntimeType.IsConvertibleToPrimitiveType(value, this);
				if (obj != null)
				{
					return obj;
				}
			}
			else if (base.IsPrimitive)
			{
				object obj2 = RuntimeType.IsConvertibleToPrimitiveType(value, this);
				if (obj2 != null)
				{
					return obj2;
				}
			}
			else if (base.IsPointer)
			{
				Type type = value.GetType();
				if (type == typeof(IntPtr) || type == typeof(UIntPtr))
				{
					return value;
				}
			}
			failed = true;
			return null;
		}

		private static object IsConvertibleToPrimitiveType(object value, Type targetType)
		{
			Type type = value.GetType();
			if (type.IsEnum)
			{
				type = Enum.GetUnderlyingType(type);
				if (type == targetType)
				{
					return value;
				}
			}
			TypeCode typeCode = Type.GetTypeCode(type);
			switch (Type.GetTypeCode(targetType))
			{
			case TypeCode.Char:
				if (typeCode == TypeCode.Byte)
				{
					return (char)((byte)value);
				}
				if (typeCode == TypeCode.UInt16)
				{
					return value;
				}
				break;
			case TypeCode.Int16:
				if (typeCode == TypeCode.SByte)
				{
					return (short)((sbyte)value);
				}
				if (typeCode == TypeCode.Byte)
				{
					return (short)((byte)value);
				}
				break;
			case TypeCode.UInt16:
				if (typeCode == TypeCode.Char)
				{
					return value;
				}
				if (typeCode == TypeCode.Byte)
				{
					return (ushort)((byte)value);
				}
				break;
			case TypeCode.Int32:
				switch (typeCode)
				{
				case TypeCode.Char:
					return (int)((char)value);
				case TypeCode.SByte:
					return (int)((sbyte)value);
				case TypeCode.Byte:
					return (int)((byte)value);
				case TypeCode.Int16:
					return (int)((short)value);
				case TypeCode.UInt16:
					return (int)((ushort)value);
				}
				break;
			case TypeCode.UInt32:
				switch (typeCode)
				{
				case TypeCode.Char:
					return (uint)((char)value);
				case TypeCode.Byte:
					return (uint)((byte)value);
				case TypeCode.UInt16:
					return (uint)((ushort)value);
				}
				break;
			case TypeCode.Int64:
				switch (typeCode)
				{
				case TypeCode.Char:
					return (long)((ulong)((char)value));
				case TypeCode.SByte:
					return (long)((sbyte)value);
				case TypeCode.Byte:
					return (long)((ulong)((byte)value));
				case TypeCode.Int16:
					return (long)((short)value);
				case TypeCode.UInt16:
					return (long)((ulong)((ushort)value));
				case TypeCode.Int32:
					return (long)((int)value);
				case TypeCode.UInt32:
					return (long)((ulong)((uint)value));
				}
				break;
			case TypeCode.UInt64:
				switch (typeCode)
				{
				case TypeCode.Char:
					return (ulong)((char)value);
				case TypeCode.Byte:
					return (ulong)((byte)value);
				case TypeCode.UInt16:
					return (ulong)((ushort)value);
				case TypeCode.UInt32:
					return (ulong)((uint)value);
				}
				break;
			case TypeCode.Single:
				switch (typeCode)
				{
				case TypeCode.Char:
					return (float)((char)value);
				case TypeCode.SByte:
					return (float)((sbyte)value);
				case TypeCode.Byte:
					return (float)((byte)value);
				case TypeCode.Int16:
					return (float)((short)value);
				case TypeCode.UInt16:
					return (float)((ushort)value);
				case TypeCode.Int32:
					return (float)((int)value);
				case TypeCode.UInt32:
					return (uint)value;
				case TypeCode.Int64:
					return (float)((long)value);
				case TypeCode.UInt64:
					return (ulong)value;
				}
				break;
			case TypeCode.Double:
				switch (typeCode)
				{
				case TypeCode.Char:
					return (double)((char)value);
				case TypeCode.SByte:
					return (double)((sbyte)value);
				case TypeCode.Byte:
					return (double)((byte)value);
				case TypeCode.Int16:
					return (double)((short)value);
				case TypeCode.UInt16:
					return (double)((ushort)value);
				case TypeCode.Int32:
					return (double)((int)value);
				case TypeCode.UInt32:
					return (uint)value;
				case TypeCode.Int64:
					return (double)((long)value);
				case TypeCode.UInt64:
					return (ulong)value;
				case TypeCode.Single:
					return (double)((float)value);
				}
				break;
			}
			return null;
		}

		private string GetCachedName(TypeNameKind kind)
		{
			if (kind == TypeNameKind.SerializationName)
			{
				return this.ToString();
			}
			throw new NotImplementedException();
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern Type make_array_type(int rank);

		public override Type MakeArrayType()
		{
			return this.make_array_type(0);
		}

		public override Type MakeArrayType(int rank)
		{
			if (rank < 1 || rank > 255)
			{
				throw new IndexOutOfRangeException();
			}
			return this.make_array_type(rank);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern Type make_byref_type();

		public override Type MakeByRefType()
		{
			if (base.IsByRef)
			{
				throw new TypeLoadException("Can not call MakeByRefType on a ByRef type");
			}
			return this.make_byref_type();
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern Type MakePointerType(Type type);

		public override Type MakePointerType()
		{
			return RuntimeType.MakePointerType(this);
		}

		public override StructLayoutAttribute StructLayoutAttribute
		{
			get
			{
				return StructLayoutAttribute.GetCustomAttribute(this);
			}
		}

		public override bool ContainsGenericParameters
		{
			get
			{
				if (this.IsGenericParameter)
				{
					return true;
				}
				if (this.IsGenericType)
				{
					Type[] genericArguments = this.GetGenericArguments();
					for (int i = 0; i < genericArguments.Length; i++)
					{
						if (genericArguments[i].ContainsGenericParameters)
						{
							return true;
						}
					}
				}
				return base.HasElementType && this.GetElementType().ContainsGenericParameters;
			}
		}

		public override Type[] GetGenericParameterConstraints()
		{
			if (!this.IsGenericParameter)
			{
				throw new InvalidOperationException(Environment.GetResourceString("Method may only be called on a Type for which Type.IsGenericParameter is true."));
			}
			RuntimeGenericParamInfoHandle runtimeGenericParamInfoHandle = new RuntimeGenericParamInfoHandle(RuntimeTypeHandle.GetGenericParameterInfo(this));
			Type[] array = runtimeGenericParamInfoHandle.Constraints;
			if (array == null)
			{
				array = EmptyArray<Type>.Value;
			}
			return array;
		}

		internal static object CreateInstanceForAnotherGenericParameter(Type genericType, RuntimeType genericArgument)
		{
			return ((RuntimeType)RuntimeType.MakeGenericType(genericType, new Type[] { genericArgument })).GetDefaultConstructor().InternalInvoke(null, null);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern Type MakeGenericType(Type gt, Type[] types);

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern IntPtr GetMethodsByName_native(IntPtr namePtr, BindingFlags bindingAttr, bool ignoreCase);

		internal RuntimeMethodInfo[] GetMethodsByName(string name, BindingFlags bindingAttr, bool ignoreCase, RuntimeType reflectedType)
		{
			RuntimeTypeHandle runtimeTypeHandle = new RuntimeTypeHandle(reflectedType);
			RuntimeMethodInfo[] array2;
			using (SafeStringMarshal safeStringMarshal = new SafeStringMarshal(name))
			{
				using (SafeGPtrArrayHandle safeGPtrArrayHandle = new SafeGPtrArrayHandle(this.GetMethodsByName_native(safeStringMarshal.Value, bindingAttr, ignoreCase)))
				{
					int length = safeGPtrArrayHandle.Length;
					RuntimeMethodInfo[] array = new RuntimeMethodInfo[length];
					for (int i = 0; i < length; i++)
					{
						RuntimeMethodHandle runtimeMethodHandle = new RuntimeMethodHandle(safeGPtrArrayHandle[i]);
						array[i] = (RuntimeMethodInfo)MethodBase.GetMethodFromHandleNoGenericCheck(runtimeMethodHandle, runtimeTypeHandle);
					}
					array2 = array;
				}
			}
			return array2;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern IntPtr GetPropertiesByName_native(IntPtr name, BindingFlags bindingAttr, bool icase);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern IntPtr GetConstructors_native(BindingFlags bindingAttr);

		private RuntimeConstructorInfo[] GetConstructors_internal(BindingFlags bindingAttr, RuntimeType reflectedType)
		{
			RuntimeTypeHandle runtimeTypeHandle = new RuntimeTypeHandle(reflectedType);
			RuntimeConstructorInfo[] array2;
			using (SafeGPtrArrayHandle safeGPtrArrayHandle = new SafeGPtrArrayHandle(this.GetConstructors_native(bindingAttr)))
			{
				int length = safeGPtrArrayHandle.Length;
				RuntimeConstructorInfo[] array = new RuntimeConstructorInfo[length];
				for (int i = 0; i < length; i++)
				{
					RuntimeMethodHandle runtimeMethodHandle = new RuntimeMethodHandle(safeGPtrArrayHandle[i]);
					array[i] = (RuntimeConstructorInfo)MethodBase.GetMethodFromHandleNoGenericCheck(runtimeMethodHandle, runtimeTypeHandle);
				}
				array2 = array;
			}
			return array2;
		}

		private RuntimePropertyInfo[] GetPropertiesByName(string name, BindingFlags bindingAttr, bool icase, RuntimeType reflectedType)
		{
			RuntimeTypeHandle runtimeTypeHandle = new RuntimeTypeHandle(reflectedType);
			RuntimePropertyInfo[] array2;
			using (SafeStringMarshal safeStringMarshal = new SafeStringMarshal(name))
			{
				using (SafeGPtrArrayHandle safeGPtrArrayHandle = new SafeGPtrArrayHandle(this.GetPropertiesByName_native(safeStringMarshal.Value, bindingAttr, icase)))
				{
					int length = safeGPtrArrayHandle.Length;
					RuntimePropertyInfo[] array = new RuntimePropertyInfo[length];
					for (int i = 0; i < length; i++)
					{
						RuntimePropertyHandle runtimePropertyHandle = new RuntimePropertyHandle(safeGPtrArrayHandle[i]);
						array[i] = (RuntimePropertyInfo)PropertyInfo.GetPropertyFromHandle(runtimePropertyHandle, runtimeTypeHandle);
					}
					array2 = array;
				}
			}
			return array2;
		}

		public override InterfaceMapping GetInterfaceMap(Type ifaceType)
		{
			if (this.IsGenericParameter)
			{
				throw new InvalidOperationException(Environment.GetResourceString("Method must be called on a Type for which Type.IsGenericParameter is false."));
			}
			if (ifaceType == null)
			{
				throw new ArgumentNullException("ifaceType");
			}
			if (ifaceType as RuntimeType == null)
			{
				throw new ArgumentException(Environment.GetResourceString("Type must be a runtime Type object."), "ifaceType");
			}
			if (!ifaceType.IsInterface)
			{
				throw new ArgumentException(Locale.GetText("Argument must be an interface."), "ifaceType");
			}
			if (base.IsInterface)
			{
				throw new ArgumentException("'this' type cannot be an interface itself");
			}
			InterfaceMapping interfaceMapping;
			interfaceMapping.TargetType = this;
			interfaceMapping.InterfaceType = ifaceType;
			RuntimeType.GetInterfaceMapData(this, ifaceType, out interfaceMapping.TargetMethods, out interfaceMapping.InterfaceMethods);
			if (interfaceMapping.TargetMethods == null)
			{
				throw new ArgumentException(Locale.GetText("Interface not found"), "ifaceType");
			}
			return interfaceMapping;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetInterfaceMapData(Type t, Type iface, out MethodInfo[] targets, out MethodInfo[] methods);

		public override Guid GUID
		{
			get
			{
				object[] customAttributes = this.GetCustomAttributes(typeof(GuidAttribute), true);
				if (customAttributes.Length == 0)
				{
					return Guid.Empty;
				}
				return new Guid(((GuidAttribute)customAttributes[0]).Value);
			}
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern void GetPacking(out int packing, out int size);

		internal static Type GetTypeFromCLSIDImpl(Guid clsid, string server, bool throwOnError)
		{
			if (RuntimeType.clsid_types == null)
			{
				Dictionary<Guid, Type> dictionary = new Dictionary<Guid, Type>();
				Interlocked.CompareExchange<Dictionary<Guid, Type>>(ref RuntimeType.clsid_types, dictionary, null);
			}
			Dictionary<Guid, Type> dictionary2 = RuntimeType.clsid_types;
			Type type2;
			lock (dictionary2)
			{
				Type type;
				if (RuntimeType.clsid_types.TryGetValue(clsid, out type))
				{
					type2 = type;
				}
				else
				{
					if (RuntimeType.clsid_assemblybuilder == null)
					{
						AssemblyName assemblyName = new AssemblyName();
						assemblyName.Name = "GetTypeFromCLSIDDummyAssembly";
						RuntimeType.clsid_assemblybuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run);
					}
					TypeBuilder typeBuilder = RuntimeType.clsid_assemblybuilder.DefineDynamicModule(clsid.ToString()).DefineType("System.__ComObject", TypeAttributes.Public, typeof(__ComObject));
					Type[] array = new Type[] { typeof(string) };
					CustomAttributeBuilder customAttributeBuilder = new CustomAttributeBuilder(typeof(GuidAttribute).GetConstructor(array), new object[] { clsid.ToString() });
					typeBuilder.SetCustomAttribute(customAttributeBuilder);
					customAttributeBuilder = new CustomAttributeBuilder(typeof(ComImportAttribute).GetConstructor(Type.EmptyTypes), new object[0]);
					typeBuilder.SetCustomAttribute(customAttributeBuilder);
					type = typeBuilder.CreateType();
					RuntimeType.clsid_types.Add(clsid, type);
					type2 = type;
				}
			}
			return type2;
		}

		protected override TypeCode GetTypeCodeImpl()
		{
			return RuntimeType.GetTypeCodeImplInternal(this);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern TypeCode GetTypeCodeImplInternal(Type type);

		internal static Type GetTypeFromProgIDImpl(string progID, string server, bool throwOnError)
		{
			throw new NotImplementedException("Unmanaged activation is not supported");
		}

		public override string ToString()
		{
			return this.getFullName(false, false);
		}

		private bool IsGenericCOMObjectImpl()
		{
			return false;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern object CreateInstanceInternal(Type type);

		public override extern MethodBase DeclaringMethod
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern string getFullName(bool full_name, bool assembly_qualified);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern Type[] GetGenericArgumentsInternal(bool runtimeArray);

		private GenericParameterAttributes GetGenericParameterAttributes()
		{
			return new RuntimeGenericParamInfoHandle(RuntimeTypeHandle.GetGenericParameterInfo(this)).Attributes;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern int GetGenericParameterPosition();

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern IntPtr GetEvents_native(IntPtr name, BindingFlags bindingAttr);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern IntPtr GetFields_native(IntPtr name, BindingFlags bindingAttr);

		private RuntimeFieldInfo[] GetFields_internal(string name, BindingFlags bindingAttr, RuntimeType reflectedType)
		{
			RuntimeTypeHandle runtimeTypeHandle = new RuntimeTypeHandle(reflectedType);
			RuntimeFieldInfo[] array2;
			using (SafeStringMarshal safeStringMarshal = new SafeStringMarshal(name))
			{
				using (SafeGPtrArrayHandle safeGPtrArrayHandle = new SafeGPtrArrayHandle(this.GetFields_native(safeStringMarshal.Value, bindingAttr)))
				{
					int length = safeGPtrArrayHandle.Length;
					RuntimeFieldInfo[] array = new RuntimeFieldInfo[length];
					for (int i = 0; i < length; i++)
					{
						RuntimeFieldHandle runtimeFieldHandle = new RuntimeFieldHandle(safeGPtrArrayHandle[i]);
						array[i] = (RuntimeFieldInfo)FieldInfo.GetFieldFromHandle(runtimeFieldHandle, runtimeTypeHandle);
					}
					array2 = array;
				}
			}
			return array2;
		}

		private RuntimeEventInfo[] GetEvents_internal(string name, BindingFlags bindingAttr, RuntimeType reflectedType)
		{
			RuntimeTypeHandle runtimeTypeHandle = new RuntimeTypeHandle(reflectedType);
			RuntimeEventInfo[] array2;
			using (SafeStringMarshal safeStringMarshal = new SafeStringMarshal(name))
			{
				using (SafeGPtrArrayHandle safeGPtrArrayHandle = new SafeGPtrArrayHandle(this.GetEvents_native(safeStringMarshal.Value, bindingAttr)))
				{
					int length = safeGPtrArrayHandle.Length;
					RuntimeEventInfo[] array = new RuntimeEventInfo[length];
					for (int i = 0; i < length; i++)
					{
						RuntimeEventHandle runtimeEventHandle = new RuntimeEventHandle(safeGPtrArrayHandle[i]);
						array[i] = (RuntimeEventInfo)EventInfo.GetEventFromHandle(runtimeEventHandle, runtimeTypeHandle);
					}
					array2 = array;
				}
			}
			return array2;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		public override extern Type[] GetInterfaces();

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern IntPtr GetNestedTypes_native(IntPtr name, BindingFlags bindingAttr);

		private RuntimeType[] GetNestedTypes_internal(string displayName, BindingFlags bindingAttr)
		{
			string text = null;
			if (displayName != null)
			{
				text = TypeIdentifiers.FromDisplay(displayName).InternalName;
			}
			RuntimeType[] array2;
			using (SafeStringMarshal safeStringMarshal = new SafeStringMarshal(text))
			{
				using (SafeGPtrArrayHandle safeGPtrArrayHandle = new SafeGPtrArrayHandle(this.GetNestedTypes_native(safeStringMarshal.Value, bindingAttr)))
				{
					int length = safeGPtrArrayHandle.Length;
					RuntimeType[] array = new RuntimeType[length];
					for (int i = 0; i < length; i++)
					{
						RuntimeTypeHandle runtimeTypeHandle = new RuntimeTypeHandle(safeGPtrArrayHandle[i]);
						array[i] = (RuntimeType)Type.GetTypeFromHandle(runtimeTypeHandle);
					}
					array2 = array;
				}
			}
			return array2;
		}

		public override string AssemblyQualifiedName
		{
			get
			{
				return this.getFullName(true, true);
			}
		}

		public override extern Type DeclaringType
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public override extern string Name
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public override extern string Namespace
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern int get_core_clr_security_level();

		public override bool IsSecurityTransparent
		{
			get
			{
				return this.get_core_clr_security_level() == 0;
			}
		}

		public override bool IsSecurityCritical
		{
			get
			{
				return this.get_core_clr_security_level() > 0;
			}
		}

		public override bool IsSecuritySafeCritical
		{
			get
			{
				return this.get_core_clr_security_level() == 1;
			}
		}

		public override int GetHashCode()
		{
			Type underlyingSystemType = this.UnderlyingSystemType;
			if (underlyingSystemType != null && underlyingSystemType != this)
			{
				return underlyingSystemType.GetHashCode();
			}
			return (int)this._impl.Value;
		}

		public override string FullName
		{
			get
			{
				if (this.IsGenericType && this.ContainsGenericParameters && !this.IsGenericTypeDefinition)
				{
					return null;
				}
				if (this.type_info == null)
				{
					this.type_info = new MonoTypeInfo();
				}
				string text;
				if ((text = this.type_info.full_name) == null)
				{
					text = (this.type_info.full_name = this.getFullName(true, false));
				}
				return text;
			}
		}

		public override bool IsSZArray
		{
			get
			{
				return base.IsArray && this == this.GetElementType().MakeArrayType();
			}
		}

		internal override bool IsUserType
		{
			get
			{
				return false;
			}
		}

		internal static readonly RuntimeType ValueType = (RuntimeType)typeof(ValueType);

		internal static readonly RuntimeType EnumType = (RuntimeType)typeof(Enum);

		private static readonly RuntimeType ObjectType = (RuntimeType)typeof(object);

		private static readonly RuntimeType StringType = (RuntimeType)typeof(string);

		private static readonly RuntimeType DelegateType = (RuntimeType)typeof(Delegate);

		private static Type[] s_SICtorParamTypes;

		private const BindingFlags MemberBindingMask = (BindingFlags)255;

		private const BindingFlags InvocationMask = BindingFlags.InvokeMethod | BindingFlags.CreateInstance | BindingFlags.GetField | BindingFlags.SetField | BindingFlags.GetProperty | BindingFlags.SetProperty | BindingFlags.PutDispProperty | BindingFlags.PutRefDispProperty;

		private const BindingFlags BinderNonCreateInstance = BindingFlags.InvokeMethod | BindingFlags.GetField | BindingFlags.SetField | BindingFlags.GetProperty | BindingFlags.SetProperty;

		private const BindingFlags BinderGetSetProperty = BindingFlags.GetProperty | BindingFlags.SetProperty;

		private const BindingFlags BinderSetInvokeProperty = BindingFlags.InvokeMethod | BindingFlags.SetProperty;

		private const BindingFlags BinderGetSetField = BindingFlags.GetField | BindingFlags.SetField;

		private const BindingFlags BinderSetInvokeField = BindingFlags.InvokeMethod | BindingFlags.SetField;

		private const BindingFlags BinderNonFieldGetSet = (BindingFlags)16773888;

		private const BindingFlags ClassicBindingMask = BindingFlags.InvokeMethod | BindingFlags.GetProperty | BindingFlags.SetProperty | BindingFlags.PutDispProperty | BindingFlags.PutRefDispProperty;

		private static RuntimeType s_typedRef = (RuntimeType)typeof(TypedReference);

		[NonSerialized]
		private MonoTypeInfo type_info;

		internal object GenericCache;

		private RuntimeConstructorInfo m_serializationCtor;

		private static Dictionary<Guid, Type> clsid_types;

		private static AssemblyBuilder clsid_assemblybuilder;

		internal enum MemberListType
		{
			All,
			CaseSensitive,
			CaseInsensitive,
			HandleToInfo
		}

		private struct ListBuilder<T> where T : class
		{
			public ListBuilder(int capacity)
			{
				this._items = null;
				this._item = default(T);
				this._count = 0;
				this._capacity = capacity;
			}

			public T this[int index]
			{
				get
				{
					if (this._items == null)
					{
						return this._item;
					}
					return this._items[index];
				}
			}

			public T[] ToArray()
			{
				if (this._count == 0)
				{
					return EmptyArray<T>.Value;
				}
				if (this._count == 1)
				{
					return new T[] { this._item };
				}
				Array.Resize<T>(ref this._items, this._count);
				this._capacity = this._count;
				return this._items;
			}

			public void CopyTo(object[] array, int index)
			{
				if (this._count == 0)
				{
					return;
				}
				if (this._count == 1)
				{
					array[index] = this._item;
					return;
				}
				Array.Copy(this._items, 0, array, index, this._count);
			}

			public int Count
			{
				get
				{
					return this._count;
				}
			}

			public void Add(T item)
			{
				if (this._count == 0)
				{
					this._item = item;
				}
				else
				{
					if (this._count == 1)
					{
						if (this._capacity < 2)
						{
							this._capacity = 4;
						}
						this._items = new T[this._capacity];
						this._items[0] = this._item;
					}
					else if (this._capacity == this._count)
					{
						int num = 2 * this._capacity;
						Array.Resize<T>(ref this._items, num);
						this._capacity = num;
					}
					this._items[this._count] = item;
				}
				this._count++;
			}

			private T[] _items;

			private T _item;

			private int _count;

			private int _capacity;
		}
	}
}
