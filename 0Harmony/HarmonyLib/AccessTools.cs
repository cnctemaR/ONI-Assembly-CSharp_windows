using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;
using System.Threading;
using MonoMod.Core.Platforms;
using MonoMod.Utils;

namespace HarmonyLib
{
	public static class AccessTools
	{
		public static IEnumerable<Assembly> AllAssemblies()
		{
			return from a in AppDomain.CurrentDomain.GetAssemblies()
				where !a.FullName.StartsWith("Microsoft.VisualStudio")
				select a;
		}

		public static Type TypeByName(string name)
		{
			Type type = Type.GetType(name, false);
			if (type != null)
			{
				return type;
			}
			foreach (Assembly assembly in AccessTools.AllAssemblies())
			{
				Type type2 = assembly.GetType(name, false);
				if (type2 != null)
				{
					return type2;
				}
			}
			Type[] array = AccessTools.AllTypes().ToArray<Type>();
			Type type3 = array.FirstOrDefault<Type>((Type t) => t.FullName == name);
			if (type3 != null)
			{
				return type3;
			}
			Type type4 = array.FirstOrDefault<Type>((Type t) => t.Name == name);
			if (type4 != null)
			{
				return type4;
			}
			FileLog.Debug("AccessTools.TypeByName: Could not find type named " + name);
			return null;
		}

		public static Type TypeSearch(Regex search, bool invalidateCache = false)
		{
			if (AccessTools.allTypesCached == null || invalidateCache)
			{
				AccessTools.allTypesCached = AccessTools.AllTypes().ToArray<Type>();
			}
			Type type = AccessTools.allTypesCached.FirstOrDefault<Type>((Type t) => search.IsMatch(t.FullName));
			if (type != null)
			{
				return type;
			}
			Type type2 = AccessTools.allTypesCached.FirstOrDefault<Type>((Type t) => search.IsMatch(t.Name));
			if (type2 != null)
			{
				return type2;
			}
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(68, 1);
			defaultInterpolatedStringHandler.AppendLiteral("AccessTools.TypeSearch: Could not find type with regular expression ");
			defaultInterpolatedStringHandler.AppendFormatted<Regex>(search);
			FileLog.Debug(defaultInterpolatedStringHandler.ToStringAndClear());
			return null;
		}

		public static void ClearTypeSearchCache()
		{
			AccessTools.allTypesCached = null;
		}

		public static Type[] GetTypesFromAssembly(Assembly assembly)
		{
			Type[] array;
			try
			{
				array = assembly.GetTypes();
			}
			catch (ReflectionTypeLoadException ex)
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(47, 2);
				defaultInterpolatedStringHandler.AppendLiteral("AccessTools.GetTypesFromAssembly: assembly ");
				defaultInterpolatedStringHandler.AppendFormatted<Assembly>(assembly);
				defaultInterpolatedStringHandler.AppendLiteral(" => ");
				defaultInterpolatedStringHandler.AppendFormatted<ReflectionTypeLoadException>(ex);
				FileLog.Debug(defaultInterpolatedStringHandler.ToStringAndClear());
				array = ex.Types.Where<Type>((Type type) => type != null).ToArray<Type>();
			}
			return array;
		}

		public static IEnumerable<Type> AllTypes()
		{
			IEnumerable<Assembly> enumerable = AccessTools.AllAssemblies();
			Func<Assembly, IEnumerable<Type>> func;
			if ((func = AccessTools.<>O.<0>__GetTypesFromAssembly) == null)
			{
				func = (AccessTools.<>O.<0>__GetTypesFromAssembly = new Func<Assembly, IEnumerable<Type>>(AccessTools.GetTypesFromAssembly));
			}
			return enumerable.SelectMany<Assembly, Type>(func);
		}

		public static IEnumerable<Type> InnerTypes(Type type)
		{
			return type.GetNestedTypes(AccessTools.all);
		}

		public static T FindIncludingBaseTypes<T>(Type type, Func<Type, T> func) where T : class
		{
			T t;
			for (;;)
			{
				t = func(type);
				if (t != null)
				{
					break;
				}
				type = type.BaseType;
				if (type == null)
				{
					goto Block_1;
				}
			}
			return t;
			Block_1:
			return default(T);
		}

		public static T FindIncludingInnerTypes<T>(Type type, Func<Type, T> func) where T : class
		{
			T t = func(type);
			if (t != null)
			{
				return t;
			}
			foreach (Type type2 in type.GetNestedTypes(AccessTools.all))
			{
				t = AccessTools.FindIncludingInnerTypes<T>(type2, func);
				if (t != null)
				{
					break;
				}
			}
			return t;
		}

		public static MethodInfo Identifiable(this MethodInfo method)
		{
			return (PlatformTriple.Current.GetIdentifiable(method) as MethodInfo) ?? method;
		}

		public static FieldInfo DeclaredField(Type type, string name)
		{
			if (type == null)
			{
				FileLog.Debug("AccessTools.DeclaredField: type is null");
				return null;
			}
			if (string.IsNullOrEmpty(name))
			{
				FileLog.Debug("AccessTools.DeclaredField: name is null/empty");
				return null;
			}
			FieldInfo field = type.GetField(name, AccessTools.allDeclared);
			if (field == null)
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(67, 2);
				defaultInterpolatedStringHandler.AppendLiteral("AccessTools.DeclaredField: Could not find field for type ");
				defaultInterpolatedStringHandler.AppendFormatted<Type>(type);
				defaultInterpolatedStringHandler.AppendLiteral(" and name ");
				defaultInterpolatedStringHandler.AppendFormatted(name);
				FileLog.Debug(defaultInterpolatedStringHandler.ToStringAndClear());
			}
			return field;
		}

		public static FieldInfo DeclaredField(string typeColonName)
		{
			Tools.TypeAndName typeAndName = Tools.TypColonName(typeColonName);
			FieldInfo field = typeAndName.type.GetField(typeAndName.name, AccessTools.allDeclared);
			if (field == null)
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(67, 2);
				defaultInterpolatedStringHandler.AppendLiteral("AccessTools.DeclaredField: Could not find field for type ");
				defaultInterpolatedStringHandler.AppendFormatted<Type>(typeAndName.type);
				defaultInterpolatedStringHandler.AppendLiteral(" and name ");
				defaultInterpolatedStringHandler.AppendFormatted(typeAndName.name);
				FileLog.Debug(defaultInterpolatedStringHandler.ToStringAndClear());
			}
			return field;
		}

		public static FieldInfo Field(Type type, string name)
		{
			if (type == null)
			{
				FileLog.Debug("AccessTools.Field: type is null");
				return null;
			}
			if (string.IsNullOrEmpty(name))
			{
				FileLog.Debug("AccessTools.Field: name is null/empty");
				return null;
			}
			FieldInfo fieldInfo = AccessTools.FindIncludingBaseTypes<FieldInfo>(type, (Type t) => t.GetField(name, AccessTools.all));
			if (fieldInfo == null)
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(59, 2);
				defaultInterpolatedStringHandler.AppendLiteral("AccessTools.Field: Could not find field for type ");
				defaultInterpolatedStringHandler.AppendFormatted<Type>(type);
				defaultInterpolatedStringHandler.AppendLiteral(" and name ");
				defaultInterpolatedStringHandler.AppendFormatted(name);
				FileLog.Debug(defaultInterpolatedStringHandler.ToStringAndClear());
			}
			return fieldInfo;
		}

		public static FieldInfo Field(string typeColonName)
		{
			Tools.TypeAndName info = Tools.TypColonName(typeColonName);
			FieldInfo fieldInfo = AccessTools.FindIncludingBaseTypes<FieldInfo>(info.type, (Type t) => t.GetField(info.name, AccessTools.all));
			if (fieldInfo == null)
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(59, 2);
				defaultInterpolatedStringHandler.AppendLiteral("AccessTools.Field: Could not find field for type ");
				defaultInterpolatedStringHandler.AppendFormatted<Type>(info.type);
				defaultInterpolatedStringHandler.AppendLiteral(" and name ");
				defaultInterpolatedStringHandler.AppendFormatted(info.name);
				FileLog.Debug(defaultInterpolatedStringHandler.ToStringAndClear());
			}
			return fieldInfo;
		}

		public static FieldInfo DeclaredField(Type type, int idx)
		{
			if (type == null)
			{
				FileLog.Debug("AccessTools.DeclaredField: type is null");
				return null;
			}
			FieldInfo fieldInfo = AccessTools.GetDeclaredFields(type).ElementAtOrDefault<FieldInfo>(idx);
			if (fieldInfo == null)
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(66, 2);
				defaultInterpolatedStringHandler.AppendLiteral("AccessTools.DeclaredField: Could not find field for type ");
				defaultInterpolatedStringHandler.AppendFormatted<Type>(type);
				defaultInterpolatedStringHandler.AppendLiteral(" and idx ");
				defaultInterpolatedStringHandler.AppendFormatted<int>(idx);
				FileLog.Debug(defaultInterpolatedStringHandler.ToStringAndClear());
			}
			return fieldInfo;
		}

		public static PropertyInfo DeclaredProperty(Type type, string name)
		{
			if (type == null)
			{
				FileLog.Debug("AccessTools.DeclaredProperty: type is null");
				return null;
			}
			if (string.IsNullOrEmpty(name))
			{
				FileLog.Debug("AccessTools.DeclaredProperty: name is null/empty");
				return null;
			}
			PropertyInfo property = type.GetProperty(name, AccessTools.allDeclared);
			if (property == null)
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(73, 2);
				defaultInterpolatedStringHandler.AppendLiteral("AccessTools.DeclaredProperty: Could not find property for type ");
				defaultInterpolatedStringHandler.AppendFormatted<Type>(type);
				defaultInterpolatedStringHandler.AppendLiteral(" and name ");
				defaultInterpolatedStringHandler.AppendFormatted(name);
				FileLog.Debug(defaultInterpolatedStringHandler.ToStringAndClear());
			}
			return property;
		}

		public static PropertyInfo DeclaredProperty(string typeColonName)
		{
			Tools.TypeAndName typeAndName = Tools.TypColonName(typeColonName);
			PropertyInfo property = typeAndName.type.GetProperty(typeAndName.name, AccessTools.allDeclared);
			if (property == null)
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(73, 2);
				defaultInterpolatedStringHandler.AppendLiteral("AccessTools.DeclaredProperty: Could not find property for type ");
				defaultInterpolatedStringHandler.AppendFormatted<Type>(typeAndName.type);
				defaultInterpolatedStringHandler.AppendLiteral(" and name ");
				defaultInterpolatedStringHandler.AppendFormatted(typeAndName.name);
				FileLog.Debug(defaultInterpolatedStringHandler.ToStringAndClear());
			}
			return property;
		}

		public static PropertyInfo DeclaredIndexer(Type type, Type[] parameters = null)
		{
			if (type == null)
			{
				FileLog.Debug("AccessTools.DeclaredIndexer: type is null");
				return null;
			}
			PropertyInfo propertyInfo3;
			try
			{
				PropertyInfo propertyInfo;
				if (parameters != null)
				{
					propertyInfo = type.GetProperties(AccessTools.allDeclared).FirstOrDefault<PropertyInfo>((PropertyInfo property) => (from param in property.GetIndexParameters()
						select param.ParameterType).SequenceEqual<Type>(parameters));
				}
				else
				{
					propertyInfo = type.GetProperties(AccessTools.allDeclared).SingleOrDefault<PropertyInfo>((PropertyInfo property) => property.GetIndexParameters().Length != 0);
				}
				PropertyInfo propertyInfo2 = propertyInfo;
				if (propertyInfo2 == null)
				{
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(77, 2);
					defaultInterpolatedStringHandler.AppendLiteral("AccessTools.DeclaredIndexer: Could not find indexer for type ");
					defaultInterpolatedStringHandler.AppendFormatted<Type>(type);
					defaultInterpolatedStringHandler.AppendLiteral(" and parameters ");
					Type[] parameters2 = parameters;
					defaultInterpolatedStringHandler.AppendFormatted((parameters2 != null) ? parameters2.Description() : null);
					FileLog.Debug(defaultInterpolatedStringHandler.ToStringAndClear());
				}
				propertyInfo3 = propertyInfo2;
			}
			catch (InvalidOperationException ex)
			{
				throw new AmbiguousMatchException("Multiple possible indexers were found.", ex);
			}
			return propertyInfo3;
		}

		public static MethodInfo DeclaredPropertyGetter(Type type, string name)
		{
			PropertyInfo propertyInfo = AccessTools.DeclaredProperty(type, name);
			if (propertyInfo == null)
			{
				return null;
			}
			return propertyInfo.GetGetMethod(true);
		}

		public static MethodInfo DeclaredPropertyGetter(string typeColonName)
		{
			PropertyInfo propertyInfo = AccessTools.DeclaredProperty(typeColonName);
			if (propertyInfo == null)
			{
				return null;
			}
			return propertyInfo.GetGetMethod(true);
		}

		public static MethodInfo DeclaredIndexerGetter(Type type, Type[] parameters = null)
		{
			PropertyInfo propertyInfo = AccessTools.DeclaredIndexer(type, parameters);
			if (propertyInfo == null)
			{
				return null;
			}
			return propertyInfo.GetGetMethod(true);
		}

		public static MethodInfo DeclaredPropertySetter(Type type, string name)
		{
			PropertyInfo propertyInfo = AccessTools.DeclaredProperty(type, name);
			if (propertyInfo == null)
			{
				return null;
			}
			return propertyInfo.GetSetMethod(true);
		}

		public static MethodInfo DeclaredPropertySetter(string typeColonName)
		{
			PropertyInfo propertyInfo = AccessTools.DeclaredProperty(typeColonName);
			if (propertyInfo == null)
			{
				return null;
			}
			return propertyInfo.GetSetMethod(true);
		}

		public static MethodInfo DeclaredIndexerSetter(Type type, Type[] parameters)
		{
			PropertyInfo propertyInfo = AccessTools.DeclaredIndexer(type, parameters);
			if (propertyInfo == null)
			{
				return null;
			}
			return propertyInfo.GetSetMethod(true);
		}

		public static PropertyInfo Property(Type type, string name)
		{
			if (type == null)
			{
				FileLog.Debug("AccessTools.Property: type is null");
				return null;
			}
			if (string.IsNullOrEmpty(name))
			{
				FileLog.Debug("AccessTools.Property: name is null/empty");
				return null;
			}
			PropertyInfo propertyInfo = AccessTools.FindIncludingBaseTypes<PropertyInfo>(type, (Type t) => t.GetProperty(name, AccessTools.all));
			if (propertyInfo == null)
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(65, 2);
				defaultInterpolatedStringHandler.AppendLiteral("AccessTools.Property: Could not find property for type ");
				defaultInterpolatedStringHandler.AppendFormatted<Type>(type);
				defaultInterpolatedStringHandler.AppendLiteral(" and name ");
				defaultInterpolatedStringHandler.AppendFormatted(name);
				FileLog.Debug(defaultInterpolatedStringHandler.ToStringAndClear());
			}
			return propertyInfo;
		}

		public static PropertyInfo Property(string typeColonName)
		{
			Tools.TypeAndName info = Tools.TypColonName(typeColonName);
			PropertyInfo propertyInfo = AccessTools.FindIncludingBaseTypes<PropertyInfo>(info.type, (Type t) => t.GetProperty(info.name, AccessTools.all));
			if (propertyInfo == null)
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(65, 2);
				defaultInterpolatedStringHandler.AppendLiteral("AccessTools.Property: Could not find property for type ");
				defaultInterpolatedStringHandler.AppendFormatted<Type>(info.type);
				defaultInterpolatedStringHandler.AppendLiteral(" and name ");
				defaultInterpolatedStringHandler.AppendFormatted(info.name);
				FileLog.Debug(defaultInterpolatedStringHandler.ToStringAndClear());
			}
			return propertyInfo;
		}

		public static PropertyInfo Indexer(Type type, Type[] parameters = null)
		{
			if (type == null)
			{
				FileLog.Debug("AccessTools.Indexer: type is null");
				return null;
			}
			Func<Type, PropertyInfo> func;
			if (parameters != null)
			{
				Func<PropertyInfo, bool> <>9__3;
				func = delegate(Type t)
				{
					IEnumerable<PropertyInfo> properties = t.GetProperties(AccessTools.all);
					Func<PropertyInfo, bool> func3;
					if ((func3 = <>9__3) == null)
					{
						func3 = (<>9__3 = (PropertyInfo property) => (from param in property.GetIndexParameters()
							select param.ParameterType).SequenceEqual<Type>(parameters));
					}
					return properties.FirstOrDefault<PropertyInfo>(func3);
				};
			}
			else
			{
				func = (Type t) => t.GetProperties(AccessTools.all).SingleOrDefault<PropertyInfo>((PropertyInfo property) => property.GetIndexParameters().Length != 0);
			}
			Func<Type, PropertyInfo> func2 = func;
			PropertyInfo propertyInfo2;
			try
			{
				PropertyInfo propertyInfo = AccessTools.FindIncludingBaseTypes<PropertyInfo>(type, func2);
				if (propertyInfo == null)
				{
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(69, 2);
					defaultInterpolatedStringHandler.AppendLiteral("AccessTools.Indexer: Could not find indexer for type ");
					defaultInterpolatedStringHandler.AppendFormatted<Type>(type);
					defaultInterpolatedStringHandler.AppendLiteral(" and parameters ");
					Type[] parameters2 = parameters;
					defaultInterpolatedStringHandler.AppendFormatted((parameters2 != null) ? parameters2.Description() : null);
					FileLog.Debug(defaultInterpolatedStringHandler.ToStringAndClear());
				}
				propertyInfo2 = propertyInfo;
			}
			catch (InvalidOperationException ex)
			{
				throw new AmbiguousMatchException("Multiple possible indexers were found.", ex);
			}
			return propertyInfo2;
		}

		public static MethodInfo PropertyGetter(Type type, string name)
		{
			PropertyInfo propertyInfo = AccessTools.Property(type, name);
			if (propertyInfo == null)
			{
				return null;
			}
			return propertyInfo.GetGetMethod(true);
		}

		public static MethodInfo PropertyGetter(string typeColonName)
		{
			PropertyInfo propertyInfo = AccessTools.Property(typeColonName);
			if (propertyInfo == null)
			{
				return null;
			}
			return propertyInfo.GetGetMethod(true);
		}

		public static MethodInfo IndexerGetter(Type type, Type[] parameters = null)
		{
			PropertyInfo propertyInfo = AccessTools.Indexer(type, parameters);
			if (propertyInfo == null)
			{
				return null;
			}
			return propertyInfo.GetGetMethod(true);
		}

		public static MethodInfo PropertySetter(Type type, string name)
		{
			PropertyInfo propertyInfo = AccessTools.Property(type, name);
			if (propertyInfo == null)
			{
				return null;
			}
			return propertyInfo.GetSetMethod(true);
		}

		public static MethodInfo PropertySetter(string typeColonName)
		{
			PropertyInfo propertyInfo = AccessTools.Property(typeColonName);
			if (propertyInfo == null)
			{
				return null;
			}
			return propertyInfo.GetSetMethod(true);
		}

		public static MethodInfo IndexerSetter(Type type, Type[] parameters = null)
		{
			PropertyInfo propertyInfo = AccessTools.Indexer(type, parameters);
			if (propertyInfo == null)
			{
				return null;
			}
			return propertyInfo.GetSetMethod(true);
		}

		public static EventInfo DeclaredEvent(Type type, string name)
		{
			if (type == null)
			{
				FileLog.Debug("AccessTools.DeclaredEvent: type is null");
				return null;
			}
			if (string.IsNullOrEmpty(name))
			{
				FileLog.Debug("AccessTools.DeclaredEvent: name is null/empty");
				return null;
			}
			EventInfo @event = type.GetEvent(name, AccessTools.allDeclared);
			if (@event == null)
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(67, 2);
				defaultInterpolatedStringHandler.AppendLiteral("AccessTools.DeclaredEvent: Could not find event for type ");
				defaultInterpolatedStringHandler.AppendFormatted<Type>(type);
				defaultInterpolatedStringHandler.AppendLiteral(" and name ");
				defaultInterpolatedStringHandler.AppendFormatted(name);
				FileLog.Debug(defaultInterpolatedStringHandler.ToStringAndClear());
			}
			return @event;
		}

		public static EventInfo DeclaredEvent(string typeColonName)
		{
			Tools.TypeAndName typeAndName = Tools.TypColonName(typeColonName);
			EventInfo @event = typeAndName.type.GetEvent(typeAndName.name, AccessTools.allDeclared);
			if (@event == null)
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(67, 2);
				defaultInterpolatedStringHandler.AppendLiteral("AccessTools.DeclaredEvent: Could not find event for type ");
				defaultInterpolatedStringHandler.AppendFormatted<Type>(typeAndName.type);
				defaultInterpolatedStringHandler.AppendLiteral(" and name ");
				defaultInterpolatedStringHandler.AppendFormatted(typeAndName.name);
				FileLog.Debug(defaultInterpolatedStringHandler.ToStringAndClear());
			}
			return @event;
		}

		public static EventInfo Event(Type type, string name)
		{
			if (type == null)
			{
				FileLog.Debug("AccessTools.Event: type is null");
				return null;
			}
			if (string.IsNullOrEmpty(name))
			{
				FileLog.Debug("AccessTools.Event: name is null/empty");
				return null;
			}
			EventInfo eventInfo = AccessTools.FindIncludingBaseTypes<EventInfo>(type, (Type t) => t.GetEvent(name, AccessTools.all));
			if (eventInfo == null)
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(59, 2);
				defaultInterpolatedStringHandler.AppendLiteral("AccessTools.Event: Could not find event for type ");
				defaultInterpolatedStringHandler.AppendFormatted<Type>(type);
				defaultInterpolatedStringHandler.AppendLiteral(" and name ");
				defaultInterpolatedStringHandler.AppendFormatted(name);
				FileLog.Debug(defaultInterpolatedStringHandler.ToStringAndClear());
			}
			return eventInfo;
		}

		public static EventInfo Event(string typeColonName)
		{
			Tools.TypeAndName info = Tools.TypColonName(typeColonName);
			EventInfo eventInfo = AccessTools.FindIncludingBaseTypes<EventInfo>(info.type, (Type t) => t.GetEvent(info.name, AccessTools.all));
			if (eventInfo == null)
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(59, 2);
				defaultInterpolatedStringHandler.AppendLiteral("AccessTools.Event: Could not find event for type ");
				defaultInterpolatedStringHandler.AppendFormatted<Type>(info.type);
				defaultInterpolatedStringHandler.AppendLiteral(" and name ");
				defaultInterpolatedStringHandler.AppendFormatted(info.name);
				FileLog.Debug(defaultInterpolatedStringHandler.ToStringAndClear());
			}
			return eventInfo;
		}

		public static MethodInfo DeclaredEventAdder(Type type, string name)
		{
			EventInfo eventInfo = AccessTools.DeclaredEvent(type, name);
			if (eventInfo == null)
			{
				return null;
			}
			return eventInfo.GetAddMethod(true);
		}

		public static MethodInfo DeclaredEventAdder(string typeColonName)
		{
			EventInfo eventInfo = AccessTools.DeclaredEvent(typeColonName);
			if (eventInfo == null)
			{
				return null;
			}
			return eventInfo.GetAddMethod(true);
		}

		public static MethodInfo EventAdder(Type type, string name)
		{
			EventInfo eventInfo = AccessTools.Event(type, name);
			if (eventInfo == null)
			{
				return null;
			}
			return eventInfo.GetAddMethod(true);
		}

		public static MethodInfo EventAdder(string typeColonName)
		{
			EventInfo eventInfo = AccessTools.Event(typeColonName);
			if (eventInfo == null)
			{
				return null;
			}
			return eventInfo.GetAddMethod(true);
		}

		public static MethodInfo DeclaredEventRemover(Type type, string name)
		{
			EventInfo eventInfo = AccessTools.DeclaredEvent(type, name);
			if (eventInfo == null)
			{
				return null;
			}
			return eventInfo.GetRemoveMethod(true);
		}

		public static MethodInfo DeclaredEventRemover(string typeColonName)
		{
			EventInfo eventInfo = AccessTools.DeclaredEvent(typeColonName);
			if (eventInfo == null)
			{
				return null;
			}
			return eventInfo.GetRemoveMethod(true);
		}

		public static MethodInfo EventRemover(Type type, string name)
		{
			EventInfo eventInfo = AccessTools.Event(type, name);
			if (eventInfo == null)
			{
				return null;
			}
			return eventInfo.GetRemoveMethod(true);
		}

		public static MethodInfo EventRemover(string typeColonName)
		{
			EventInfo eventInfo = AccessTools.Event(typeColonName);
			if (eventInfo == null)
			{
				return null;
			}
			return eventInfo.GetRemoveMethod(true);
		}

		public static MethodInfo DeclaredMethod(Type type, string name, Type[] parameters = null, Type[] generics = null)
		{
			if (type == null)
			{
				FileLog.Debug("AccessTools.DeclaredMethod: type is null");
				return null;
			}
			if (string.IsNullOrEmpty(name))
			{
				FileLog.Debug("AccessTools.DeclaredMethod: name is null/empty");
				return null;
			}
			ParameterModifier[] array = new ParameterModifier[0];
			MethodInfo methodInfo;
			if (parameters == null)
			{
				methodInfo = type.GetMethod(name, AccessTools.allDeclared);
			}
			else
			{
				methodInfo = type.GetMethod(name, AccessTools.allDeclared, null, parameters, array);
			}
			if (methodInfo == null)
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(85, 3);
				defaultInterpolatedStringHandler.AppendLiteral("AccessTools.DeclaredMethod: Could not find method for type ");
				defaultInterpolatedStringHandler.AppendFormatted<Type>(type);
				defaultInterpolatedStringHandler.AppendLiteral(" and name ");
				defaultInterpolatedStringHandler.AppendFormatted(name);
				defaultInterpolatedStringHandler.AppendLiteral(" and parameters ");
				defaultInterpolatedStringHandler.AppendFormatted((parameters != null) ? parameters.Description() : null);
				FileLog.Debug(defaultInterpolatedStringHandler.ToStringAndClear());
				return null;
			}
			if (generics != null)
			{
				methodInfo = methodInfo.MakeGenericMethod(generics);
			}
			return methodInfo;
		}

		public static MethodInfo DeclaredMethod(string typeColonName, Type[] parameters = null, Type[] generics = null)
		{
			Tools.TypeAndName typeAndName = Tools.TypColonName(typeColonName);
			return AccessTools.DeclaredMethod(typeAndName.type, typeAndName.name, parameters, generics);
		}

		public static MethodInfo Method(Type type, string name, Type[] parameters = null, Type[] generics = null)
		{
			if (type == null)
			{
				FileLog.Debug("AccessTools.Method: type is null");
				return null;
			}
			if (string.IsNullOrEmpty(name))
			{
				FileLog.Debug("AccessTools.Method: name is null/empty");
				return null;
			}
			ParameterModifier[] modifiers = new ParameterModifier[0];
			MethodInfo methodInfo;
			if (parameters == null)
			{
				try
				{
					methodInfo = AccessTools.FindIncludingBaseTypes<MethodInfo>(type, (Type t) => t.GetMethod(name, AccessTools.all));
					goto IL_00D6;
				}
				catch (AmbiguousMatchException ex)
				{
					methodInfo = AccessTools.FindIncludingBaseTypes<MethodInfo>(type, (Type t) => t.GetMethod(name, AccessTools.all, null, Array.Empty<Type>(), modifiers));
					if (methodInfo == null)
					{
						DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(38, 2);
						defaultInterpolatedStringHandler.AppendLiteral("Ambiguous match in Harmony patch for ");
						defaultInterpolatedStringHandler.AppendFormatted<Type>(type);
						defaultInterpolatedStringHandler.AppendLiteral(":");
						defaultInterpolatedStringHandler.AppendFormatted(name);
						throw new AmbiguousMatchException(defaultInterpolatedStringHandler.ToStringAndClear(), ex);
					}
					goto IL_00D6;
				}
			}
			methodInfo = AccessTools.FindIncludingBaseTypes<MethodInfo>(type, (Type t) => t.GetMethod(name, AccessTools.all, null, parameters, modifiers));
			IL_00D6:
			if (methodInfo == null)
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(77, 3);
				defaultInterpolatedStringHandler.AppendLiteral("AccessTools.Method: Could not find method for type ");
				defaultInterpolatedStringHandler.AppendFormatted<Type>(type);
				defaultInterpolatedStringHandler.AppendLiteral(" and name ");
				defaultInterpolatedStringHandler.AppendFormatted(name);
				defaultInterpolatedStringHandler.AppendLiteral(" and parameters ");
				Type[] parameters2 = parameters;
				defaultInterpolatedStringHandler.AppendFormatted((parameters2 != null) ? parameters2.Description() : null);
				FileLog.Debug(defaultInterpolatedStringHandler.ToStringAndClear());
				return null;
			}
			if (generics != null)
			{
				methodInfo = methodInfo.MakeGenericMethod(generics);
			}
			return methodInfo;
		}

		public static MethodInfo Method(string typeColonName, Type[] parameters = null, Type[] generics = null)
		{
			Tools.TypeAndName typeAndName = Tools.TypColonName(typeColonName);
			return AccessTools.Method(typeAndName.type, typeAndName.name, parameters, generics);
		}

		public static MethodInfo EnumeratorMoveNext(MethodBase method)
		{
			if (method == null)
			{
				FileLog.Debug("AccessTools.EnumeratorMoveNext: method is null");
				return null;
			}
			IEnumerable<KeyValuePair<OpCode, object>> enumerable = from pair in PatchProcessor.ReadMethodBody(method)
				where pair.Key == OpCodes.Newobj
				select pair;
			if (enumerable.Count<KeyValuePair<OpCode, object>>() != 1)
			{
				FileLog.Debug("AccessTools.EnumeratorMoveNext: " + method.FullDescription() + " contains no Newobj opcode");
				return null;
			}
			ConstructorInfo constructorInfo = enumerable.First<KeyValuePair<OpCode, object>>().Value as ConstructorInfo;
			if (constructorInfo == null)
			{
				FileLog.Debug("AccessTools.EnumeratorMoveNext: " + method.FullDescription() + " contains no constructor");
				return null;
			}
			Type declaringType = constructorInfo.DeclaringType;
			if (declaringType == null)
			{
				FileLog.Debug("AccessTools.EnumeratorMoveNext: " + method.FullDescription() + " refers to a global type");
				return null;
			}
			return AccessTools.Method(declaringType, "MoveNext", null, null);
		}

		public static MethodInfo AsyncMoveNext(MethodBase method)
		{
			if (method == null)
			{
				FileLog.Debug("AccessTools.AsyncMoveNext: method is null");
				return null;
			}
			AsyncStateMachineAttribute customAttribute = method.GetCustomAttribute<AsyncStateMachineAttribute>();
			if (customAttribute == null)
			{
				FileLog.Debug("AccessTools.AsyncMoveNext: Could not find AsyncStateMachine for " + method.FullDescription());
				return null;
			}
			Type stateMachineType = customAttribute.StateMachineType;
			MethodInfo methodInfo = AccessTools.DeclaredMethod(stateMachineType, "MoveNext", null, null);
			if (methodInfo == null)
			{
				FileLog.Debug("AccessTools.AsyncMoveNext: Could not find async method body for " + method.FullDescription());
				return null;
			}
			return methodInfo;
		}

		public static MethodInfo Finalizer(Type type)
		{
			return AccessTools.Method(type, "Finalize", null, null);
		}

		public static MethodInfo DeclaredFinalizer(Type type)
		{
			return AccessTools.DeclaredMethod(type, "Finalize", null, null);
		}

		public static List<string> GetMethodNames(Type type)
		{
			if (type == null)
			{
				FileLog.Debug("AccessTools.GetMethodNames: type is null");
				return new List<string>();
			}
			return (from m in AccessTools.GetDeclaredMethods(type)
				select m.Name).ToList<string>();
		}

		public static List<string> GetMethodNames(object instance)
		{
			if (instance == null)
			{
				FileLog.Debug("AccessTools.GetMethodNames: instance is null");
				return new List<string>();
			}
			return AccessTools.GetMethodNames(instance.GetType());
		}

		public static List<string> GetFieldNames(Type type)
		{
			if (type == null)
			{
				FileLog.Debug("AccessTools.GetFieldNames: type is null");
				return new List<string>();
			}
			return (from f in AccessTools.GetDeclaredFields(type)
				select f.Name).ToList<string>();
		}

		public static List<string> GetFieldNames(object instance)
		{
			if (instance == null)
			{
				FileLog.Debug("AccessTools.GetFieldNames: instance is null");
				return new List<string>();
			}
			return AccessTools.GetFieldNames(instance.GetType());
		}

		public static List<string> GetPropertyNames(Type type)
		{
			if (type == null)
			{
				FileLog.Debug("AccessTools.GetPropertyNames: type is null");
				return new List<string>();
			}
			return (from f in AccessTools.GetDeclaredProperties(type)
				select f.Name).ToList<string>();
		}

		public static List<string> GetPropertyNames(object instance)
		{
			if (instance == null)
			{
				FileLog.Debug("AccessTools.GetPropertyNames: instance is null");
				return new List<string>();
			}
			return AccessTools.GetPropertyNames(instance.GetType());
		}

		public static Type GetUnderlyingType(this MemberInfo member)
		{
			MemberTypes memberType = member.MemberType;
			if (memberType <= MemberTypes.Field)
			{
				if (memberType == MemberTypes.Event)
				{
					return ((EventInfo)member).EventHandlerType;
				}
				if (memberType == MemberTypes.Field)
				{
					return ((FieldInfo)member).FieldType;
				}
			}
			else
			{
				if (memberType == MemberTypes.Method)
				{
					return ((MethodInfo)member).ReturnType;
				}
				if (memberType == MemberTypes.Property)
				{
					return ((PropertyInfo)member).PropertyType;
				}
			}
			throw new ArgumentException("Member must be of type EventInfo, FieldInfo, MethodInfo, or PropertyInfo");
		}

		public static MethodInfo GetMethodByModuleAndToken(string moduleGUID, int token)
		{
			Module module = (from a in AppDomain.CurrentDomain.GetAssemblies()
				where !a.FullName.StartsWith("Microsoft.VisualStudio")
				select a).SelectMany<Assembly, Module>((Assembly a) => a.GetLoadedModules()).First<Module>((Module m) => m.ModuleVersionId.ToString() == moduleGUID);
			if (!(module == null))
			{
				return (MethodInfo)module.ResolveMethod(token);
			}
			return null;
		}

		public static bool IsDeclaredMember<T>(this T member) where T : MemberInfo
		{
			return member.DeclaringType == member.ReflectedType;
		}

		public static T GetDeclaredMember<T>(this T member) where T : MemberInfo
		{
			if (member.DeclaringType == null || member.IsDeclaredMember<T>())
			{
				return member;
			}
			int metadataToken = member.MetadataToken;
			Type declaringType = member.DeclaringType;
			MemberInfo[] array = ((declaringType != null) ? declaringType.GetMembers(AccessTools.all) : null) ?? Array.Empty<MemberInfo>();
			foreach (MemberInfo memberInfo in array)
			{
				if (memberInfo.MetadataToken == metadataToken)
				{
					return (T)((object)memberInfo);
				}
			}
			return member;
		}

		public static ConstructorInfo DeclaredConstructor(Type type, Type[] parameters = null, bool searchForStatic = false)
		{
			if (type == null)
			{
				FileLog.Debug("AccessTools.DeclaredConstructor: type is null");
				return null;
			}
			if (parameters == null)
			{
				parameters = Array.Empty<Type>();
			}
			BindingFlags bindingFlags = (searchForStatic ? (AccessTools.allDeclared & ~BindingFlags.Instance) : (AccessTools.allDeclared & ~BindingFlags.Static));
			return type.GetConstructor(bindingFlags, null, parameters, Array.Empty<ParameterModifier>());
		}

		public static ConstructorInfo Constructor(Type type, Type[] parameters = null, bool searchForStatic = false)
		{
			if (type == null)
			{
				FileLog.Debug("AccessTools.ConstructorInfo: type is null");
				return null;
			}
			if (parameters == null)
			{
				parameters = Array.Empty<Type>();
			}
			BindingFlags flags = (searchForStatic ? (AccessTools.all & ~BindingFlags.Instance) : (AccessTools.all & ~BindingFlags.Static));
			return AccessTools.FindIncludingBaseTypes<ConstructorInfo>(type, (Type t) => t.GetConstructor(flags, null, parameters, Array.Empty<ParameterModifier>()));
		}

		public static List<ConstructorInfo> GetDeclaredConstructors(Type type, bool? searchForStatic = null)
		{
			if (type == null)
			{
				FileLog.Debug("AccessTools.GetDeclaredConstructors: type is null");
				return new List<ConstructorInfo>();
			}
			BindingFlags bindingFlags = AccessTools.allDeclared;
			if (searchForStatic != null)
			{
				bindingFlags = (searchForStatic.Value ? (bindingFlags & ~BindingFlags.Instance) : (bindingFlags & ~BindingFlags.Static));
			}
			return (from method in type.GetConstructors(bindingFlags)
				where method.DeclaringType == type
				select method).ToList<ConstructorInfo>();
		}

		public static List<MethodInfo> GetDeclaredMethods(Type type)
		{
			if (type == null)
			{
				FileLog.Debug("AccessTools.GetDeclaredMethods: type is null");
				return new List<MethodInfo>();
			}
			return type.GetMethods(AccessTools.allDeclared).ToList<MethodInfo>();
		}

		public static List<PropertyInfo> GetDeclaredProperties(Type type)
		{
			if (type == null)
			{
				FileLog.Debug("AccessTools.GetDeclaredProperties: type is null");
				return new List<PropertyInfo>();
			}
			return type.GetProperties(AccessTools.allDeclared).ToList<PropertyInfo>();
		}

		public static List<FieldInfo> GetDeclaredFields(Type type)
		{
			if (type == null)
			{
				FileLog.Debug("AccessTools.GetDeclaredFields: type is null");
				return new List<FieldInfo>();
			}
			return type.GetFields(AccessTools.allDeclared).ToList<FieldInfo>();
		}

		public static Type GetReturnedType(MethodBase methodOrConstructor)
		{
			if (methodOrConstructor == null)
			{
				FileLog.Debug("AccessTools.GetReturnedType: methodOrConstructor is null");
				return null;
			}
			ConstructorInfo constructorInfo = methodOrConstructor as ConstructorInfo;
			if (constructorInfo != null)
			{
				return typeof(void);
			}
			return ((MethodInfo)methodOrConstructor).ReturnType;
		}

		public static Type Inner(Type type, string name)
		{
			if (type == null)
			{
				FileLog.Debug("AccessTools.Inner: type is null");
				return null;
			}
			if (string.IsNullOrEmpty(name))
			{
				FileLog.Debug("AccessTools.Inner: name is null/empty");
				return null;
			}
			return AccessTools.FindIncludingBaseTypes<Type>(type, (Type t) => t.GetNestedType(name, AccessTools.all));
		}

		public static Type FirstInner(Type type, Func<Type, bool> predicate)
		{
			if (type == null)
			{
				FileLog.Debug("AccessTools.FirstInner: type is null");
				return null;
			}
			if (predicate == null)
			{
				FileLog.Debug("AccessTools.FirstInner: predicate is null");
				return null;
			}
			return type.GetNestedTypes(AccessTools.all).FirstOrDefault<Type>((Type subType) => predicate(subType));
		}

		public static MethodInfo FirstMethod(Type type, Func<MethodInfo, bool> predicate)
		{
			if (type == null)
			{
				FileLog.Debug("AccessTools.FirstMethod: type is null");
				return null;
			}
			if (predicate == null)
			{
				FileLog.Debug("AccessTools.FirstMethod: predicate is null");
				return null;
			}
			return type.GetMethods(AccessTools.allDeclared).FirstOrDefault<MethodInfo>((MethodInfo method) => predicate(method));
		}

		public static ConstructorInfo FirstConstructor(Type type, Func<ConstructorInfo, bool> predicate)
		{
			if (type == null)
			{
				FileLog.Debug("AccessTools.FirstConstructor: type is null");
				return null;
			}
			if (predicate == null)
			{
				FileLog.Debug("AccessTools.FirstConstructor: predicate is null");
				return null;
			}
			return type.GetConstructors(AccessTools.allDeclared).FirstOrDefault<ConstructorInfo>((ConstructorInfo constructor) => predicate(constructor));
		}

		public static PropertyInfo FirstProperty(Type type, Func<PropertyInfo, bool> predicate)
		{
			if (type == null)
			{
				FileLog.Debug("AccessTools.FirstProperty: type is null");
				return null;
			}
			if (predicate == null)
			{
				FileLog.Debug("AccessTools.FirstProperty: predicate is null");
				return null;
			}
			return type.GetProperties(AccessTools.allDeclared).FirstOrDefault<PropertyInfo>((PropertyInfo property) => predicate(property));
		}

		public static Type[] GetTypes(object[] parameters)
		{
			if (parameters == null)
			{
				return Array.Empty<Type>();
			}
			return parameters.Select<object, Type>(delegate(object p)
			{
				if (p != null)
				{
					return p.GetType();
				}
				return typeof(object);
			}).ToArray<Type>();
		}

		public static object[] ActualParameters(MethodBase method, object[] inputs)
		{
			List<Type> inputTypes = inputs.Select<object, Type>(delegate(object obj)
			{
				if (obj == null)
				{
					return null;
				}
				return obj.GetType();
			}).ToList<Type>();
			return (from p in method.GetParameters()
				select p.ParameterType).Select<Type, object>(delegate(Type pType)
			{
				int num = inputTypes.FindIndex((Type inType) => inType != null && pType.IsAssignableFrom(inType));
				if (num >= 0)
				{
					return inputs[num];
				}
				return AccessTools.GetDefaultValue(pType);
			}).ToArray<object>();
		}

		public static AccessTools.FieldRef<T, F> FieldRefAccess<T, F>(string fieldName)
		{
			if (fieldName == null)
			{
				throw new ArgumentNullException("fieldName");
			}
			AccessTools.FieldRef<T, F> fieldRef;
			try
			{
				Type typeFromHandle = typeof(T);
				if (typeFromHandle.IsValueType)
				{
					throw new ArgumentException("T (FieldRefAccess instance type) must not be a value type");
				}
				fieldRef = Tools.FieldRefAccess<T, F>(Tools.GetInstanceField(typeFromHandle, fieldName), false);
			}
			catch (Exception ex)
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(43, 3);
				defaultInterpolatedStringHandler.AppendLiteral("FieldRefAccess<");
				defaultInterpolatedStringHandler.AppendFormatted<Type>(typeof(T));
				defaultInterpolatedStringHandler.AppendLiteral(", ");
				defaultInterpolatedStringHandler.AppendFormatted<Type>(typeof(F));
				defaultInterpolatedStringHandler.AppendLiteral("> for ");
				defaultInterpolatedStringHandler.AppendFormatted(fieldName);
				defaultInterpolatedStringHandler.AppendLiteral(" caused an exception");
				throw new ArgumentException(defaultInterpolatedStringHandler.ToStringAndClear(), ex);
			}
			return fieldRef;
		}

		public static ref F FieldRefAccess<T, F>(T instance, string fieldName)
		{
			if (instance == null)
			{
				throw new ArgumentNullException("instance");
			}
			if (fieldName == null)
			{
				throw new ArgumentNullException("fieldName");
			}
			ref F ptr;
			try
			{
				Type typeFromHandle = typeof(T);
				if (typeFromHandle.IsValueType)
				{
					throw new ArgumentException("T (FieldRefAccess instance type) must not be a value type");
				}
				ptr = Tools.FieldRefAccess<T, F>(Tools.GetInstanceField(typeFromHandle, fieldName), false)(instance);
			}
			catch (Exception ex)
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(45, 4);
				defaultInterpolatedStringHandler.AppendLiteral("FieldRefAccess<");
				defaultInterpolatedStringHandler.AppendFormatted<Type>(typeof(T));
				defaultInterpolatedStringHandler.AppendLiteral(", ");
				defaultInterpolatedStringHandler.AppendFormatted<Type>(typeof(F));
				defaultInterpolatedStringHandler.AppendLiteral("> for ");
				defaultInterpolatedStringHandler.AppendFormatted<T>(instance);
				defaultInterpolatedStringHandler.AppendLiteral(", ");
				defaultInterpolatedStringHandler.AppendFormatted(fieldName);
				defaultInterpolatedStringHandler.AppendLiteral(" caused an exception");
				throw new ArgumentException(defaultInterpolatedStringHandler.ToStringAndClear(), ex);
			}
			return ref ptr;
		}

		public static AccessTools.FieldRef<object, F> FieldRefAccess<F>(Type type, string fieldName)
		{
			if (type == null)
			{
				throw new ArgumentNullException("type");
			}
			if (fieldName == null)
			{
				throw new ArgumentNullException("fieldName");
			}
			AccessTools.FieldRef<object, F> fieldRef;
			try
			{
				FieldInfo fieldInfo = AccessTools.Field(type, fieldName);
				if (fieldInfo == null)
				{
					throw new MissingFieldException(type.Name, fieldName);
				}
				if (!fieldInfo.IsStatic)
				{
					Type declaringType = fieldInfo.DeclaringType;
					if (declaringType != null && declaringType.IsValueType)
					{
						throw new ArgumentException("Either FieldDeclaringType must be a class or field must be static");
					}
				}
				fieldRef = Tools.FieldRefAccess<object, F>(fieldInfo, true);
			}
			catch (Exception ex)
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(43, 3);
				defaultInterpolatedStringHandler.AppendLiteral("FieldRefAccess<");
				defaultInterpolatedStringHandler.AppendFormatted<Type>(typeof(F));
				defaultInterpolatedStringHandler.AppendLiteral("> for ");
				defaultInterpolatedStringHandler.AppendFormatted<Type>(type);
				defaultInterpolatedStringHandler.AppendLiteral(", ");
				defaultInterpolatedStringHandler.AppendFormatted(fieldName);
				defaultInterpolatedStringHandler.AppendLiteral(" caused an exception");
				throw new ArgumentException(defaultInterpolatedStringHandler.ToStringAndClear(), ex);
			}
			return fieldRef;
		}

		public static AccessTools.FieldRef<object, F> FieldRefAccess<F>(string typeColonName)
		{
			Tools.TypeAndName typeAndName = Tools.TypColonName(typeColonName);
			return AccessTools.FieldRefAccess<F>(typeAndName.type, typeAndName.name);
		}

		public static AccessTools.FieldRef<T, F> FieldRefAccess<T, F>(FieldInfo fieldInfo)
		{
			if (fieldInfo == null)
			{
				throw new ArgumentNullException("fieldInfo");
			}
			AccessTools.FieldRef<T, F> fieldRef;
			try
			{
				Type typeFromHandle = typeof(T);
				if (typeFromHandle.IsValueType)
				{
					throw new ArgumentException("T (FieldRefAccess instance type) must not be a value type");
				}
				bool flag = false;
				if (!fieldInfo.IsStatic)
				{
					Type declaringType = fieldInfo.DeclaringType;
					if (declaringType != null)
					{
						if (declaringType.IsValueType)
						{
							throw new ArgumentException("Either FieldDeclaringType must be a class or field must be static");
						}
						flag = Tools.FieldRefNeedsClasscast(typeFromHandle, declaringType);
					}
				}
				fieldRef = Tools.FieldRefAccess<T, F>(fieldInfo, flag);
			}
			catch (Exception ex)
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(43, 3);
				defaultInterpolatedStringHandler.AppendLiteral("FieldRefAccess<");
				defaultInterpolatedStringHandler.AppendFormatted<Type>(typeof(T));
				defaultInterpolatedStringHandler.AppendLiteral(", ");
				defaultInterpolatedStringHandler.AppendFormatted<Type>(typeof(F));
				defaultInterpolatedStringHandler.AppendLiteral("> for ");
				defaultInterpolatedStringHandler.AppendFormatted<FieldInfo>(fieldInfo);
				defaultInterpolatedStringHandler.AppendLiteral(" caused an exception");
				throw new ArgumentException(defaultInterpolatedStringHandler.ToStringAndClear(), ex);
			}
			return fieldRef;
		}

		public static ref F FieldRefAccess<T, F>(T instance, FieldInfo fieldInfo)
		{
			if (instance == null)
			{
				throw new ArgumentNullException("instance");
			}
			if (fieldInfo == null)
			{
				throw new ArgumentNullException("fieldInfo");
			}
			ref F ptr;
			try
			{
				Type typeFromHandle = typeof(T);
				if (typeFromHandle.IsValueType)
				{
					throw new ArgumentException("T (FieldRefAccess instance type) must not be a value type");
				}
				if (fieldInfo.IsStatic)
				{
					throw new ArgumentException("Field must not be static");
				}
				bool flag = false;
				Type declaringType = fieldInfo.DeclaringType;
				if (declaringType != null)
				{
					if (declaringType.IsValueType)
					{
						throw new ArgumentException("FieldDeclaringType must be a class");
					}
					flag = Tools.FieldRefNeedsClasscast(typeFromHandle, declaringType);
				}
				ptr = Tools.FieldRefAccess<T, F>(fieldInfo, flag)(instance);
			}
			catch (Exception ex)
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(45, 4);
				defaultInterpolatedStringHandler.AppendLiteral("FieldRefAccess<");
				defaultInterpolatedStringHandler.AppendFormatted<Type>(typeof(T));
				defaultInterpolatedStringHandler.AppendLiteral(", ");
				defaultInterpolatedStringHandler.AppendFormatted<Type>(typeof(F));
				defaultInterpolatedStringHandler.AppendLiteral("> for ");
				defaultInterpolatedStringHandler.AppendFormatted<T>(instance);
				defaultInterpolatedStringHandler.AppendLiteral(", ");
				defaultInterpolatedStringHandler.AppendFormatted<FieldInfo>(fieldInfo);
				defaultInterpolatedStringHandler.AppendLiteral(" caused an exception");
				throw new ArgumentException(defaultInterpolatedStringHandler.ToStringAndClear(), ex);
			}
			return ref ptr;
		}

		public static AccessTools.StructFieldRef<T, F> StructFieldRefAccess<T, F>(string fieldName) where T : struct
		{
			if (fieldName == null)
			{
				throw new ArgumentNullException("fieldName");
			}
			AccessTools.StructFieldRef<T, F> structFieldRef;
			try
			{
				structFieldRef = Tools.StructFieldRefAccess<T, F>(Tools.GetInstanceField(typeof(T), fieldName));
			}
			catch (Exception ex)
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(49, 3);
				defaultInterpolatedStringHandler.AppendLiteral("StructFieldRefAccess<");
				defaultInterpolatedStringHandler.AppendFormatted<Type>(typeof(T));
				defaultInterpolatedStringHandler.AppendLiteral(", ");
				defaultInterpolatedStringHandler.AppendFormatted<Type>(typeof(F));
				defaultInterpolatedStringHandler.AppendLiteral("> for ");
				defaultInterpolatedStringHandler.AppendFormatted(fieldName);
				defaultInterpolatedStringHandler.AppendLiteral(" caused an exception");
				throw new ArgumentException(defaultInterpolatedStringHandler.ToStringAndClear(), ex);
			}
			return structFieldRef;
		}

		public static ref F StructFieldRefAccess<T, F>(ref T instance, string fieldName) where T : struct
		{
			if (fieldName == null)
			{
				throw new ArgumentNullException("fieldName");
			}
			ref F ptr;
			try
			{
				ptr = Tools.StructFieldRefAccess<T, F>(Tools.GetInstanceField(typeof(T), fieldName))(ref instance);
			}
			catch (Exception ex)
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(51, 4);
				defaultInterpolatedStringHandler.AppendLiteral("StructFieldRefAccess<");
				defaultInterpolatedStringHandler.AppendFormatted<Type>(typeof(T));
				defaultInterpolatedStringHandler.AppendLiteral(", ");
				defaultInterpolatedStringHandler.AppendFormatted<Type>(typeof(F));
				defaultInterpolatedStringHandler.AppendLiteral("> for ");
				defaultInterpolatedStringHandler.AppendFormatted<T>(instance);
				defaultInterpolatedStringHandler.AppendLiteral(", ");
				defaultInterpolatedStringHandler.AppendFormatted(fieldName);
				defaultInterpolatedStringHandler.AppendLiteral(" caused an exception");
				throw new ArgumentException(defaultInterpolatedStringHandler.ToStringAndClear(), ex);
			}
			return ref ptr;
		}

		public static AccessTools.StructFieldRef<T, F> StructFieldRefAccess<T, F>(FieldInfo fieldInfo) where T : struct
		{
			if (fieldInfo == null)
			{
				throw new ArgumentNullException("fieldInfo");
			}
			AccessTools.StructFieldRef<T, F> structFieldRef;
			try
			{
				Tools.ValidateStructField<T, F>(fieldInfo);
				structFieldRef = Tools.StructFieldRefAccess<T, F>(fieldInfo);
			}
			catch (Exception ex)
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(49, 3);
				defaultInterpolatedStringHandler.AppendLiteral("StructFieldRefAccess<");
				defaultInterpolatedStringHandler.AppendFormatted<Type>(typeof(T));
				defaultInterpolatedStringHandler.AppendLiteral(", ");
				defaultInterpolatedStringHandler.AppendFormatted<Type>(typeof(F));
				defaultInterpolatedStringHandler.AppendLiteral("> for ");
				defaultInterpolatedStringHandler.AppendFormatted<FieldInfo>(fieldInfo);
				defaultInterpolatedStringHandler.AppendLiteral(" caused an exception");
				throw new ArgumentException(defaultInterpolatedStringHandler.ToStringAndClear(), ex);
			}
			return structFieldRef;
		}

		public static ref F StructFieldRefAccess<T, F>(ref T instance, FieldInfo fieldInfo) where T : struct
		{
			if (fieldInfo == null)
			{
				throw new ArgumentNullException("fieldInfo");
			}
			ref F ptr;
			try
			{
				Tools.ValidateStructField<T, F>(fieldInfo);
				ptr = Tools.StructFieldRefAccess<T, F>(fieldInfo)(ref instance);
			}
			catch (Exception ex)
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(51, 4);
				defaultInterpolatedStringHandler.AppendLiteral("StructFieldRefAccess<");
				defaultInterpolatedStringHandler.AppendFormatted<Type>(typeof(T));
				defaultInterpolatedStringHandler.AppendLiteral(", ");
				defaultInterpolatedStringHandler.AppendFormatted<Type>(typeof(F));
				defaultInterpolatedStringHandler.AppendLiteral("> for ");
				defaultInterpolatedStringHandler.AppendFormatted<T>(instance);
				defaultInterpolatedStringHandler.AppendLiteral(", ");
				defaultInterpolatedStringHandler.AppendFormatted<FieldInfo>(fieldInfo);
				defaultInterpolatedStringHandler.AppendLiteral(" caused an exception");
				throw new ArgumentException(defaultInterpolatedStringHandler.ToStringAndClear(), ex);
			}
			return ref ptr;
		}

		public static ref F StaticFieldRefAccess<T, F>(string fieldName)
		{
			return AccessTools.StaticFieldRefAccess<F>(typeof(T), fieldName);
		}

		public static ref F StaticFieldRefAccess<F>(Type type, string fieldName)
		{
			ref F ptr;
			try
			{
				FieldInfo fieldInfo = AccessTools.Field(type, fieldName);
				if (fieldInfo == null)
				{
					throw new MissingFieldException(type.Name, fieldName);
				}
				ptr = Tools.StaticFieldRefAccess<F>(fieldInfo)();
			}
			catch (Exception ex)
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(49, 3);
				defaultInterpolatedStringHandler.AppendLiteral("StaticFieldRefAccess<");
				defaultInterpolatedStringHandler.AppendFormatted<Type>(typeof(F));
				defaultInterpolatedStringHandler.AppendLiteral("> for ");
				defaultInterpolatedStringHandler.AppendFormatted<Type>(type);
				defaultInterpolatedStringHandler.AppendLiteral(", ");
				defaultInterpolatedStringHandler.AppendFormatted(fieldName);
				defaultInterpolatedStringHandler.AppendLiteral(" caused an exception");
				throw new ArgumentException(defaultInterpolatedStringHandler.ToStringAndClear(), ex);
			}
			return ref ptr;
		}

		public static ref F StaticFieldRefAccess<F>(string typeColonName)
		{
			Tools.TypeAndName typeAndName = Tools.TypColonName(typeColonName);
			return AccessTools.StaticFieldRefAccess<F>(typeAndName.type, typeAndName.name);
		}

		public static ref F StaticFieldRefAccess<T, F>(FieldInfo fieldInfo)
		{
			if (fieldInfo == null)
			{
				throw new ArgumentNullException("fieldInfo");
			}
			ref F ptr;
			try
			{
				ptr = Tools.StaticFieldRefAccess<F>(fieldInfo)();
			}
			catch (Exception ex)
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(49, 3);
				defaultInterpolatedStringHandler.AppendLiteral("StaticFieldRefAccess<");
				defaultInterpolatedStringHandler.AppendFormatted<Type>(typeof(T));
				defaultInterpolatedStringHandler.AppendLiteral(", ");
				defaultInterpolatedStringHandler.AppendFormatted<Type>(typeof(F));
				defaultInterpolatedStringHandler.AppendLiteral("> for ");
				defaultInterpolatedStringHandler.AppendFormatted<FieldInfo>(fieldInfo);
				defaultInterpolatedStringHandler.AppendLiteral(" caused an exception");
				throw new ArgumentException(defaultInterpolatedStringHandler.ToStringAndClear(), ex);
			}
			return ref ptr;
		}

		public static AccessTools.FieldRef<F> StaticFieldRefAccess<F>(FieldInfo fieldInfo)
		{
			if (fieldInfo == null)
			{
				throw new ArgumentNullException("fieldInfo");
			}
			AccessTools.FieldRef<F> fieldRef;
			try
			{
				fieldRef = Tools.StaticFieldRefAccess<F>(fieldInfo);
			}
			catch (Exception ex)
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(47, 2);
				defaultInterpolatedStringHandler.AppendLiteral("StaticFieldRefAccess<");
				defaultInterpolatedStringHandler.AppendFormatted<Type>(typeof(F));
				defaultInterpolatedStringHandler.AppendLiteral("> for ");
				defaultInterpolatedStringHandler.AppendFormatted<FieldInfo>(fieldInfo);
				defaultInterpolatedStringHandler.AppendLiteral(" caused an exception");
				throw new ArgumentException(defaultInterpolatedStringHandler.ToStringAndClear(), ex);
			}
			return fieldRef;
		}

		[Obsolete("This overload only exists for runtime backwards compatibility and will be removed in Harmony 3. Use MethodDelegate(MethodInfo, object, bool, Type[]) instead")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public static DelegateType MethodDelegate<DelegateType>(MethodInfo method, object instance, bool virtualCall) where DelegateType : Delegate
		{
			return AccessTools.MethodDelegate<DelegateType>(method, instance, virtualCall, null);
		}

		public static DelegateType MethodDelegate<DelegateType>(MethodInfo method, object instance = null, bool virtualCall = true, Type[] delegateArgs = null) where DelegateType : Delegate
		{
			if (method == null)
			{
				throw new ArgumentNullException("method");
			}
			Type typeFromHandle = typeof(DelegateType);
			if (method.IsStatic)
			{
				return (DelegateType)((object)Delegate.CreateDelegate(typeFromHandle, method));
			}
			Type type = method.DeclaringType;
			if (type != null && type.IsInterface && !virtualCall)
			{
				throw new ArgumentException("Interface methods must be called virtually");
			}
			if (instance == null)
			{
				ParameterInfo[] parameters = typeFromHandle.GetMethod("Invoke").GetParameters();
				if (parameters.Length == 0)
				{
					Delegate.CreateDelegate(typeof(DelegateType), method);
					throw new ArgumentException("Invalid delegate type");
				}
				Type parameterType = parameters[0].ParameterType;
				if (type != null && type.IsInterface && parameterType.IsValueType)
				{
					InterfaceMapping interfaceMap = parameterType.GetInterfaceMap(type);
					method = interfaceMap.TargetMethods[Array.IndexOf<MethodInfo>(interfaceMap.InterfaceMethods, method)];
					type = parameterType;
				}
				if (type != null && virtualCall)
				{
					if (type.IsInterface)
					{
						return (DelegateType)((object)Delegate.CreateDelegate(typeFromHandle, method));
					}
					if (parameterType.IsInterface)
					{
						InterfaceMapping interfaceMap2 = type.GetInterfaceMap(parameterType);
						MethodInfo methodInfo = interfaceMap2.InterfaceMethods[Array.IndexOf<MethodInfo>(interfaceMap2.TargetMethods, method)];
						return (DelegateType)((object)Delegate.CreateDelegate(typeFromHandle, methodInfo));
					}
					if (!type.IsValueType)
					{
						return (DelegateType)((object)Delegate.CreateDelegate(typeFromHandle, method.GetBaseDefinition()));
					}
				}
				ParameterInfo[] parameters2 = method.GetParameters();
				int num = parameters2.Length;
				Type[] array = new Type[num + 1];
				array[0] = type;
				for (int i = 0; i < num; i++)
				{
					array[i + 1] = parameters2[i].ParameterType;
				}
				Type[] array2 = delegateArgs ?? typeFromHandle.GetGenericArguments();
				Type[] array3 = ((array2.Length < array.Length) ? array : array2);
				DynamicMethodDefinition dynamicMethodDefinition = new DynamicMethodDefinition("OpenInstanceDelegate_" + method.Name, method.ReturnType, array3);
				ILGenerator ilgenerator = dynamicMethodDefinition.GetILGenerator();
				if (type != null && type.IsValueType && array2.Length != 0 && !array2[0].IsByRef)
				{
					ilgenerator.Emit(OpCodes.Ldarga_S, 0);
				}
				else
				{
					ilgenerator.Emit(OpCodes.Ldarg_0);
				}
				for (int j = 1; j < array.Length; j++)
				{
					ilgenerator.Emit(OpCodes.Ldarg, j);
					if (array[j].IsValueType && j < array2.Length && !array2[j].IsValueType)
					{
						ilgenerator.Emit(OpCodes.Unbox_Any, array[j]);
					}
				}
				ilgenerator.Emit(OpCodes.Call, method);
				ilgenerator.Emit(OpCodes.Ret);
				return (DelegateType)((object)dynamicMethodDefinition.Generate().CreateDelegate(typeFromHandle));
			}
			else
			{
				if (virtualCall)
				{
					return (DelegateType)((object)Delegate.CreateDelegate(typeFromHandle, instance, method.GetBaseDefinition()));
				}
				if (type != null && !type.IsInstanceOfType(instance))
				{
					Delegate.CreateDelegate(typeof(DelegateType), instance, method);
					throw new ArgumentException("Invalid delegate type");
				}
				if (AccessTools.IsMonoRuntime)
				{
					DynamicMethodDefinition dynamicMethodDefinition2 = new DynamicMethodDefinition("LdftnDelegate_" + method.Name, typeFromHandle, new Type[] { typeof(object) });
					ILGenerator ilgenerator2 = dynamicMethodDefinition2.GetILGenerator();
					ilgenerator2.Emit(OpCodes.Ldarg_0);
					ilgenerator2.Emit(OpCodes.Ldftn, method);
					ilgenerator2.Emit(OpCodes.Newobj, typeFromHandle.GetConstructor(new Type[]
					{
						typeof(object),
						typeof(IntPtr)
					}));
					ilgenerator2.Emit(OpCodes.Ret);
					return (DelegateType)((object)dynamicMethodDefinition2.Generate().Invoke(null, new object[] { instance }));
				}
				return (DelegateType)((object)Activator.CreateInstance(typeFromHandle, new object[]
				{
					instance,
					method.MethodHandle.GetFunctionPointer()
				}));
			}
		}

		[Obsolete("This overload only exists for runtime backwards compatibility and will be removed in Harmony 3. Use MethodDelegate(string, object, bool, Type[]) instead")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public static DelegateType MethodDelegate<DelegateType>(string typeColonName, object instance, bool virtualCall) where DelegateType : Delegate
		{
			return AccessTools.MethodDelegate<DelegateType>(typeColonName, instance, virtualCall, null);
		}

		public static DelegateType MethodDelegate<DelegateType>(string typeColonName, object instance = null, bool virtualCall = true, Type[] delegateArgs = null) where DelegateType : Delegate
		{
			return AccessTools.MethodDelegate<DelegateType>(AccessTools.DeclaredMethod(typeColonName, null, null), instance, virtualCall, delegateArgs);
		}

		public static DelegateType HarmonyDelegate<DelegateType>(object instance = null) where DelegateType : Delegate
		{
			HarmonyMethod mergedFromType = HarmonyMethodExtensions.GetMergedFromType(typeof(DelegateType));
			HarmonyMethod harmonyMethod = mergedFromType;
			MethodType methodType = harmonyMethod.methodType.GetValueOrDefault();
			if (harmonyMethod.methodType == null)
			{
				methodType = MethodType.Normal;
				harmonyMethod.methodType = new MethodType?(methodType);
			}
			MethodInfo methodInfo = mergedFromType.GetOriginalMethod() as MethodInfo;
			if (methodInfo == null)
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(40, 1);
				defaultInterpolatedStringHandler.AppendLiteral("Delegate ");
				defaultInterpolatedStringHandler.AppendFormatted<Type>(typeof(DelegateType));
				defaultInterpolatedStringHandler.AppendLiteral(" has no defined original method");
				throw new NullReferenceException(defaultInterpolatedStringHandler.ToStringAndClear());
			}
			return AccessTools.MethodDelegate<DelegateType>(methodInfo, instance, !mergedFromType.nonVirtualDelegate, null);
		}

		public static MethodBase GetOutsideCaller()
		{
			StackTrace stackTrace = new StackTrace(true);
			foreach (StackFrame stackFrame in stackTrace.GetFrames())
			{
				MethodBase method = stackFrame.GetMethod();
				Type declaringType = method.DeclaringType;
				if (((declaringType != null) ? declaringType.Namespace : null) != typeof(Harmony).Namespace)
				{
					return method;
				}
			}
			throw new Exception("Unexpected end of stack trace");
		}

		public static void RethrowException(Exception exception)
		{
			ExceptionDispatchInfo.Capture(exception).Throw();
			throw exception;
		}

		public static bool IsMonoRuntime { get; } = Type.GetType("Mono.Runtime") != null;

		public static bool IsNetFrameworkRuntime { get; }

		public static bool IsNetCoreRuntime { get; }

		public static void ThrowMissingMemberException(Type type, params string[] names)
		{
			string text = string.Join(",", AccessTools.GetFieldNames(type).ToArray());
			string text2 = string.Join(",", AccessTools.GetPropertyNames(type).ToArray());
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(44, 3);
			defaultInterpolatedStringHandler.AppendFormatted(string.Join(",", names));
			defaultInterpolatedStringHandler.AppendLiteral("; available fields: ");
			defaultInterpolatedStringHandler.AppendFormatted(text);
			defaultInterpolatedStringHandler.AppendLiteral("; available properties: ");
			defaultInterpolatedStringHandler.AppendFormatted(text2);
			throw new MissingMemberException(defaultInterpolatedStringHandler.ToStringAndClear());
		}

		public static object GetDefaultValue(Type type)
		{
			if (type == null)
			{
				FileLog.Debug("AccessTools.GetDefaultValue: type is null");
				return null;
			}
			if (type == typeof(void))
			{
				return null;
			}
			if (type.IsValueType)
			{
				return Activator.CreateInstance(type);
			}
			return null;
		}

		public static object CreateInstance(Type type)
		{
			if (type == null)
			{
				throw new ArgumentNullException("type");
			}
			ConstructorInfo constructor = type.GetConstructor(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, CallingConventions.Any, Array.Empty<Type>(), null);
			if (constructor != null)
			{
				return constructor.Invoke(null);
			}
			return FormatterServices.GetUninitializedObject(type);
		}

		public static T CreateInstance<T>()
		{
			object obj = AccessTools.CreateInstance(typeof(T));
			if (obj is T)
			{
				return (T)((object)obj);
			}
			return default(T);
		}

		public static T MakeDeepCopy<T>(object source) where T : class
		{
			return AccessTools.MakeDeepCopy(source, typeof(T), null, "") as T;
		}

		public static void MakeDeepCopy<T>(object source, out T result, Func<string, Traverse, Traverse, object> processor = null, string pathRoot = "")
		{
			result = (T)((object)AccessTools.MakeDeepCopy(source, typeof(T), processor, pathRoot));
		}

		public static object MakeDeepCopy(object source, Type resultType, Func<string, Traverse, Traverse, object> processor = null, string pathRoot = "")
		{
			if (source == null || resultType == null)
			{
				return null;
			}
			resultType = Nullable.GetUnderlyingType(resultType) ?? resultType;
			Type type = source.GetType();
			if (type.IsPrimitive)
			{
				return source;
			}
			if (type.IsEnum)
			{
				return Enum.ToObject(resultType, (int)source);
			}
			if (type.IsGenericType && resultType.IsGenericType)
			{
				AccessTools.addHandlerCacheLock.EnterUpgradeableReadLock();
				try
				{
					FastInvokeHandler handler;
					if (!AccessTools.addHandlerCache.TryGetValue(resultType, out handler))
					{
						MethodInfo methodInfo = AccessTools.FirstMethod(resultType, (MethodInfo m) => m.Name == "Add" && m.GetParameters().Length == 1);
						if (methodInfo != null)
						{
							handler = MethodInvoker.GetHandler(methodInfo, false);
						}
						AccessTools.addHandlerCacheLock.EnterWriteLock();
						try
						{
							AccessTools.addHandlerCache[resultType] = handler;
						}
						finally
						{
							AccessTools.addHandlerCacheLock.ExitWriteLock();
						}
					}
					if (handler != null)
					{
						object obj = Activator.CreateInstance(resultType);
						Type type2 = resultType.GetGenericArguments()[0];
						int num = 0;
						foreach (object obj2 in (source as IEnumerable))
						{
							string text = num++.ToString();
							string text2 = ((pathRoot.Length > 0) ? (pathRoot + "." + text) : text);
							object obj3 = AccessTools.MakeDeepCopy(obj2, type2, processor, text2);
							handler(obj, new object[] { obj3 });
						}
						return obj;
					}
				}
				finally
				{
					AccessTools.addHandlerCacheLock.ExitUpgradeableReadLock();
				}
			}
			if (type.IsArray && resultType.IsArray)
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
				return array;
			}
			string @namespace = type.Namespace;
			if (@namespace == "System" || (@namespace != null && @namespace.StartsWith("System.")))
			{
				return source;
			}
			object obj4 = AccessTools.CreateInstance((resultType == typeof(object)) ? type : resultType);
			Traverse.IterateFields(source, obj4, delegate(string name, Traverse src, Traverse dst)
			{
				string text5 = ((pathRoot.Length > 0) ? (pathRoot + "." + name) : name);
				object obj5 = ((processor != null) ? processor(text5, src, dst) : src.GetValue());
				if (dst.IsWriteable)
				{
					dst.SetValue(AccessTools.MakeDeepCopy(obj5, dst.GetValueType(), processor, text5));
				}
			});
			return obj4;
		}

		public static bool IsStruct(Type type)
		{
			return !(type == null) && (type.IsValueType && !AccessTools.IsValue(type)) && !AccessTools.IsVoid(type);
		}

		public static bool IsClass(Type type)
		{
			return !(type == null) && !type.IsValueType;
		}

		public static bool IsValue(Type type)
		{
			return !(type == null) && (type.IsPrimitive || type.IsEnum);
		}

		public static bool IsInteger(Type type)
		{
			if (type == null)
			{
				return false;
			}
			TypeCode typeCode = Type.GetTypeCode(type);
			return typeCode - TypeCode.SByte <= 7;
		}

		public static bool IsFloatingPoint(Type type)
		{
			if (type == null)
			{
				return false;
			}
			TypeCode typeCode = Type.GetTypeCode(type);
			return typeCode - TypeCode.Single <= 2;
		}

		public static bool IsNumber(Type type)
		{
			return AccessTools.IsInteger(type) || AccessTools.IsFloatingPoint(type);
		}

		public static bool IsVoid(Type type)
		{
			return type == typeof(void);
		}

		public static bool IsOfNullableType<T>(T instance)
		{
			return Nullable.GetUnderlyingType(typeof(T)) != null;
		}

		public static bool IsStatic(MemberInfo member)
		{
			if (member == null)
			{
				throw new ArgumentNullException("member");
			}
			MemberTypes memberType = member.MemberType;
			if (memberType <= MemberTypes.Method)
			{
				switch (memberType)
				{
				case MemberTypes.Constructor:
					break;
				case MemberTypes.Event:
					return AccessTools.IsStatic((EventInfo)member);
				case MemberTypes.Constructor | MemberTypes.Event:
					goto IL_0091;
				case MemberTypes.Field:
					return ((FieldInfo)member).IsStatic;
				default:
					if (memberType != MemberTypes.Method)
					{
						goto IL_0091;
					}
					break;
				}
				return ((MethodBase)member).IsStatic;
			}
			if (memberType == MemberTypes.Property)
			{
				return AccessTools.IsStatic((PropertyInfo)member);
			}
			if (memberType == MemberTypes.TypeInfo || memberType == MemberTypes.NestedType)
			{
				return AccessTools.IsStatic((Type)member);
			}
			IL_0091:
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(21, 1);
			defaultInterpolatedStringHandler.AppendLiteral("Unknown member type: ");
			defaultInterpolatedStringHandler.AppendFormatted<MemberTypes>(member.MemberType);
			throw new ArgumentException(defaultInterpolatedStringHandler.ToStringAndClear());
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public static bool IsStatic(Type type)
		{
			return type != null && type.IsAbstract && type.IsSealed;
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public static bool IsStatic(PropertyInfo propertyInfo)
		{
			if (propertyInfo == null)
			{
				throw new ArgumentNullException("propertyInfo");
			}
			return propertyInfo.GetAccessors(true)[0].IsStatic;
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public static bool IsStatic(EventInfo eventInfo)
		{
			if (eventInfo == null)
			{
				throw new ArgumentNullException("eventInfo");
			}
			return eventInfo.GetAddMethod(true).IsStatic;
		}

		public static int CombinedHashCode(IEnumerable<object> objects)
		{
			int num = 352654597;
			int num2 = num;
			int num3 = 0;
			foreach (object obj in objects)
			{
				if (num3 % 2 == 0)
				{
					num = ((num << 5) + num + (num >> 27)) ^ obj.GetHashCode();
				}
				else
				{
					num2 = ((num2 << 5) + num2 + (num2 >> 27)) ^ obj.GetHashCode();
				}
				num3++;
			}
			return num + num2 * 1566083941;
		}

		// Note: this type is marked as 'beforefieldinit'.
		static AccessTools()
		{
			Type type = Type.GetType("System.Runtime.InteropServices.RuntimeInformation", false);
			AccessTools.IsNetFrameworkRuntime = ((type != null) ? type.GetProperty("FrameworkDescription").GetValue(null, null).ToString()
				.StartsWith(".NET Framework") : (!AccessTools.IsMonoRuntime));
			Type type2 = Type.GetType("System.Runtime.InteropServices.RuntimeInformation", false);
			AccessTools.IsNetCoreRuntime = type2 != null && type2.GetProperty("FrameworkDescription").GetValue(null, null).ToString()
				.StartsWith(".NET Core");
			AccessTools.addHandlerCache = new Dictionary<Type, FastInvokeHandler>();
			AccessTools.addHandlerCacheLock = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);
		}

		private static Type[] allTypesCached = null;

		public static readonly BindingFlags all = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.GetField | BindingFlags.SetField | BindingFlags.GetProperty | BindingFlags.SetProperty;

		public static readonly BindingFlags allDeclared = AccessTools.all | BindingFlags.DeclaredOnly;

		private static readonly Dictionary<Type, FastInvokeHandler> addHandlerCache;

		private static readonly ReaderWriterLockSlim addHandlerCacheLock;

		public delegate ref F FieldRef<in T, F>(T instance = default(T));

		public delegate ref F StructFieldRef<T, F>(ref T instance) where T : struct;

		public delegate ref F FieldRef<F>();

		[CompilerGenerated]
		private static class <>O
		{
			public static Func<Assembly, IEnumerable<Type>> <0>__GetTypesFromAssembly;
		}
	}
}
