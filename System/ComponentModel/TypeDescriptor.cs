using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Globalization;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security.Permissions;

namespace System.ComponentModel
{
	public sealed class TypeDescriptor
	{
		private TypeDescriptor()
		{
		}

		public static event RefreshEventHandler Refreshed;

		[global::System.MonoNotSupported("Mono does not support COM")]
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public static Type ComObjectType
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public static TypeDescriptionProvider AddAttributes(object instance, params Attribute[] attributes)
		{
			if (instance == null)
			{
				throw new ArgumentNullException("instance");
			}
			if (attributes == null)
			{
				throw new ArgumentNullException("attributes");
			}
			TypeDescriptor.AttributeProvider attributeProvider = new TypeDescriptor.AttributeProvider(attributes, TypeDescriptor.GetProvider(instance));
			TypeDescriptor.AddProvider(attributeProvider, instance);
			return attributeProvider;
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public static TypeDescriptionProvider AddAttributes(Type type, params Attribute[] attributes)
		{
			if (type == null)
			{
				throw new ArgumentNullException("type");
			}
			if (attributes == null)
			{
				throw new ArgumentNullException("attributes");
			}
			TypeDescriptor.AttributeProvider attributeProvider = new TypeDescriptor.AttributeProvider(attributes, TypeDescriptor.GetProvider(type));
			TypeDescriptor.AddProvider(attributeProvider, type);
			return attributeProvider;
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public static void AddProvider(TypeDescriptionProvider provider, object instance)
		{
			if (provider == null)
			{
				throw new ArgumentNullException("provider");
			}
			if (instance == null)
			{
				throw new ArgumentNullException("instance");
			}
			object obj = TypeDescriptor.componentDescriptionProvidersLock;
			lock (obj)
			{
				WeakObjectWrapper weakObjectWrapper = new WeakObjectWrapper(instance);
				global::System.Collections.Generic.LinkedList<TypeDescriptionProvider> linkedList;
				if (!TypeDescriptor.componentDescriptionProviders.TryGetValue(weakObjectWrapper, out linkedList))
				{
					linkedList = new global::System.Collections.Generic.LinkedList<TypeDescriptionProvider>();
					TypeDescriptor.componentDescriptionProviders.Add(new WeakObjectWrapper(instance), linkedList);
				}
				linkedList.AddLast(provider);
				TypeDescriptor.Refresh(instance);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public static void AddProvider(TypeDescriptionProvider provider, Type type)
		{
			if (provider == null)
			{
				throw new ArgumentNullException("provider");
			}
			if (type == null)
			{
				throw new ArgumentNullException("type");
			}
			object obj = TypeDescriptor.typeDescriptionProvidersLock;
			lock (obj)
			{
				global::System.Collections.Generic.LinkedList<TypeDescriptionProvider> linkedList;
				if (!TypeDescriptor.typeDescriptionProviders.TryGetValue(type, out linkedList))
				{
					linkedList = new global::System.Collections.Generic.LinkedList<TypeDescriptionProvider>();
					TypeDescriptor.typeDescriptionProviders.Add(type, linkedList);
				}
				linkedList.AddLast(provider);
				TypeDescriptor.Refresh(type);
			}
		}

		[global::System.MonoTODO]
		public static object CreateInstance(IServiceProvider provider, Type objectType, Type[] argTypes, object[] args)
		{
			if (objectType == null)
			{
				throw new ArgumentNullException("objectType");
			}
			object obj = null;
			if (provider != null)
			{
				TypeDescriptionProvider typeDescriptionProvider = provider.GetService(typeof(TypeDescriptionProvider)) as TypeDescriptionProvider;
				if (typeDescriptionProvider != null)
				{
					obj = typeDescriptionProvider.CreateInstance(provider, objectType, argTypes, args);
				}
			}
			if (obj == null)
			{
				obj = Activator.CreateInstance(objectType, args);
			}
			return obj;
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public static void AddEditorTable(Type editorBaseType, Hashtable table)
		{
			if (editorBaseType == null)
			{
				throw new ArgumentNullException("editorBaseType");
			}
			if (TypeDescriptor.editors == null)
			{
				TypeDescriptor.editors = new Hashtable();
			}
			if (!TypeDescriptor.editors.ContainsKey(editorBaseType))
			{
				TypeDescriptor.editors[editorBaseType] = table;
			}
		}

		public static global::System.ComponentModel.Design.IDesigner CreateDesigner(IComponent component, Type designerBaseType)
		{
			string assemblyQualifiedName = designerBaseType.AssemblyQualifiedName;
			AttributeCollection attributes = TypeDescriptor.GetAttributes(component);
			foreach (object obj in attributes)
			{
				Attribute attribute = (Attribute)obj;
				DesignerAttribute designerAttribute = attribute as DesignerAttribute;
				if (designerAttribute != null && assemblyQualifiedName == designerAttribute.DesignerBaseTypeName)
				{
					Type typeFromName = TypeDescriptor.GetTypeFromName(component, designerAttribute.DesignerTypeName);
					if (typeFromName != null)
					{
						return (global::System.ComponentModel.Design.IDesigner)Activator.CreateInstance(typeFromName);
					}
				}
			}
			return null;
		}

		[PermissionSet(SecurityAction.LinkDemand, XML = "<PermissionSet class=\"System.Security.PermissionSet\"\nversion=\"1\">\n<IPermission class=\"System.Security.Permissions.ReflectionPermission, mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089\"\nversion=\"1\"\nFlags=\"MemberAccess, TypeInformation\"/>\n</PermissionSet>\n")]
		public static EventDescriptor CreateEvent(Type componentType, string name, Type type, params Attribute[] attributes)
		{
			return new ReflectionEventDescriptor(componentType, name, type, attributes);
		}

		[PermissionSet(SecurityAction.LinkDemand, XML = "<PermissionSet class=\"System.Security.PermissionSet\"\nversion=\"1\">\n<IPermission class=\"System.Security.Permissions.ReflectionPermission, mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089\"\nversion=\"1\"\nFlags=\"MemberAccess, TypeInformation\"/>\n</PermissionSet>\n")]
		public static EventDescriptor CreateEvent(Type componentType, EventDescriptor oldEventDescriptor, params Attribute[] attributes)
		{
			return new ReflectionEventDescriptor(componentType, oldEventDescriptor, attributes);
		}

		[PermissionSet(SecurityAction.LinkDemand, XML = "<PermissionSet class=\"System.Security.PermissionSet\"\nversion=\"1\">\n<IPermission class=\"System.Security.Permissions.ReflectionPermission, mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089\"\nversion=\"1\"\nFlags=\"MemberAccess, TypeInformation\"/>\n</PermissionSet>\n")]
		public static PropertyDescriptor CreateProperty(Type componentType, string name, Type type, params Attribute[] attributes)
		{
			return new ReflectionPropertyDescriptor(componentType, name, type, attributes);
		}

		[PermissionSet(SecurityAction.LinkDemand, XML = "<PermissionSet class=\"System.Security.PermissionSet\"\nversion=\"1\">\n<IPermission class=\"System.Security.Permissions.ReflectionPermission, mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089\"\nversion=\"1\"\nFlags=\"MemberAccess, TypeInformation\"/>\n</PermissionSet>\n")]
		public static PropertyDescriptor CreateProperty(Type componentType, PropertyDescriptor oldPropertyDescriptor, params Attribute[] attributes)
		{
			return new ReflectionPropertyDescriptor(componentType, oldPropertyDescriptor, attributes);
		}

		public static AttributeCollection GetAttributes(Type componentType)
		{
			if (componentType == null)
			{
				return AttributeCollection.Empty;
			}
			return TypeDescriptor.GetTypeInfo(componentType).GetAttributes();
		}

		public static AttributeCollection GetAttributes(object component)
		{
			return TypeDescriptor.GetAttributes(component, false);
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public static AttributeCollection GetAttributes(object component, bool noCustomTypeDesc)
		{
			if (component == null)
			{
				return AttributeCollection.Empty;
			}
			if (!noCustomTypeDesc && component is ICustomTypeDescriptor)
			{
				return ((ICustomTypeDescriptor)component).GetAttributes();
			}
			IComponent component2 = component as IComponent;
			if (component2 != null && component2.Site != null)
			{
				return TypeDescriptor.GetComponentInfo(component2).GetAttributes();
			}
			return TypeDescriptor.GetTypeInfo(component.GetType()).GetAttributes();
		}

		public static string GetClassName(object component)
		{
			return TypeDescriptor.GetClassName(component, false);
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public static string GetClassName(object component, bool noCustomTypeDesc)
		{
			if (component == null)
			{
				throw new ArgumentNullException("component", "component cannot be null");
			}
			if (!noCustomTypeDesc && component is ICustomTypeDescriptor)
			{
				string text = ((ICustomTypeDescriptor)component).GetClassName();
				if (text == null)
				{
					text = ((ICustomTypeDescriptor)component).GetComponentName();
				}
				if (text == null)
				{
					text = component.GetType().FullName;
				}
				return text;
			}
			return component.GetType().FullName;
		}

		public static string GetComponentName(object component)
		{
			return TypeDescriptor.GetComponentName(component, false);
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public static string GetComponentName(object component, bool noCustomTypeDesc)
		{
			if (component == null)
			{
				throw new ArgumentNullException("component", "component cannot be null");
			}
			if (!noCustomTypeDesc && component is ICustomTypeDescriptor)
			{
				return ((ICustomTypeDescriptor)component).GetComponentName();
			}
			IComponent component2 = component as IComponent;
			if (component2 != null && component2.Site != null)
			{
				return component2.Site.Name;
			}
			return null;
		}

		[global::System.MonoNotSupported("")]
		public static string GetFullComponentName(object component)
		{
			throw new NotImplementedException();
		}

		[global::System.MonoNotSupported("")]
		public static string GetClassName(Type componentType)
		{
			throw new NotImplementedException();
		}

		public static TypeConverter GetConverter(object component)
		{
			return TypeDescriptor.GetConverter(component, false);
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public static TypeConverter GetConverter(object component, bool noCustomTypeDesc)
		{
			if (component == null)
			{
				throw new ArgumentNullException("component", "component cannot be null");
			}
			if (!noCustomTypeDesc && component is ICustomTypeDescriptor)
			{
				return ((ICustomTypeDescriptor)component).GetConverter();
			}
			Type type = null;
			AttributeCollection attributes = TypeDescriptor.GetAttributes(component, false);
			TypeConverterAttribute typeConverterAttribute = (TypeConverterAttribute)attributes[typeof(TypeConverterAttribute)];
			if (typeConverterAttribute != null && typeConverterAttribute.ConverterTypeName.Length > 0)
			{
				type = TypeDescriptor.GetTypeFromName(component as IComponent, typeConverterAttribute.ConverterTypeName);
			}
			if (type == null)
			{
				type = TypeDescriptor.FindDefaultConverterType(component.GetType());
			}
			if (type == null)
			{
				return null;
			}
			ConstructorInfo constructor = type.GetConstructor(new Type[] { typeof(Type) });
			if (constructor != null)
			{
				return (TypeConverter)constructor.Invoke(new object[] { component.GetType() });
			}
			return (TypeConverter)Activator.CreateInstance(type);
		}

		private static ArrayList DefaultConverters
		{
			get
			{
				object obj = TypeDescriptor.creatingDefaultConverters;
				lock (obj)
				{
					if (TypeDescriptor.defaultConverters != null)
					{
						return TypeDescriptor.defaultConverters;
					}
					TypeDescriptor.defaultConverters = new ArrayList();
					TypeDescriptor.defaultConverters.Add(new DictionaryEntry(typeof(bool), typeof(BooleanConverter)));
					TypeDescriptor.defaultConverters.Add(new DictionaryEntry(typeof(byte), typeof(ByteConverter)));
					TypeDescriptor.defaultConverters.Add(new DictionaryEntry(typeof(sbyte), typeof(SByteConverter)));
					TypeDescriptor.defaultConverters.Add(new DictionaryEntry(typeof(string), typeof(StringConverter)));
					TypeDescriptor.defaultConverters.Add(new DictionaryEntry(typeof(char), typeof(CharConverter)));
					TypeDescriptor.defaultConverters.Add(new DictionaryEntry(typeof(short), typeof(Int16Converter)));
					TypeDescriptor.defaultConverters.Add(new DictionaryEntry(typeof(int), typeof(Int32Converter)));
					TypeDescriptor.defaultConverters.Add(new DictionaryEntry(typeof(long), typeof(Int64Converter)));
					TypeDescriptor.defaultConverters.Add(new DictionaryEntry(typeof(ushort), typeof(UInt16Converter)));
					TypeDescriptor.defaultConverters.Add(new DictionaryEntry(typeof(uint), typeof(UInt32Converter)));
					TypeDescriptor.defaultConverters.Add(new DictionaryEntry(typeof(ulong), typeof(UInt64Converter)));
					TypeDescriptor.defaultConverters.Add(new DictionaryEntry(typeof(float), typeof(SingleConverter)));
					TypeDescriptor.defaultConverters.Add(new DictionaryEntry(typeof(double), typeof(DoubleConverter)));
					TypeDescriptor.defaultConverters.Add(new DictionaryEntry(typeof(decimal), typeof(DecimalConverter)));
					TypeDescriptor.defaultConverters.Add(new DictionaryEntry(typeof(void), typeof(TypeConverter)));
					TypeDescriptor.defaultConverters.Add(new DictionaryEntry(typeof(Array), typeof(ArrayConverter)));
					TypeDescriptor.defaultConverters.Add(new DictionaryEntry(typeof(CultureInfo), typeof(CultureInfoConverter)));
					TypeDescriptor.defaultConverters.Add(new DictionaryEntry(typeof(DateTime), typeof(DateTimeConverter)));
					TypeDescriptor.defaultConverters.Add(new DictionaryEntry(typeof(Guid), typeof(GuidConverter)));
					TypeDescriptor.defaultConverters.Add(new DictionaryEntry(typeof(TimeSpan), typeof(TimeSpanConverter)));
					TypeDescriptor.defaultConverters.Add(new DictionaryEntry(typeof(ICollection), typeof(CollectionConverter)));
					TypeDescriptor.defaultConverters.Add(new DictionaryEntry(typeof(Enum), typeof(EnumConverter)));
				}
				return TypeDescriptor.defaultConverters;
			}
		}

		public static TypeConverter GetConverter(Type type)
		{
			if (type == null)
			{
				throw new ArgumentNullException("type");
			}
			Type type2 = null;
			AttributeCollection attributes = TypeDescriptor.GetAttributes(type);
			TypeConverterAttribute typeConverterAttribute = (TypeConverterAttribute)attributes[typeof(TypeConverterAttribute)];
			if (typeConverterAttribute != null && typeConverterAttribute.ConverterTypeName.Length > 0)
			{
				type2 = TypeDescriptor.GetTypeFromName(null, typeConverterAttribute.ConverterTypeName);
			}
			if (type2 == null)
			{
				type2 = TypeDescriptor.FindDefaultConverterType(type);
			}
			if (type2 == null)
			{
				return null;
			}
			ConstructorInfo constructor = type2.GetConstructor(new Type[] { typeof(Type) });
			if (constructor != null)
			{
				return (TypeConverter)constructor.Invoke(new object[] { type });
			}
			return (TypeConverter)Activator.CreateInstance(type2);
		}

		private static Type FindDefaultConverterType(Type type)
		{
			Type type2 = null;
			if (type != null)
			{
				if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
				{
					return typeof(NullableConverter);
				}
				foreach (object obj in TypeDescriptor.DefaultConverters)
				{
					DictionaryEntry dictionaryEntry = (DictionaryEntry)obj;
					if ((Type)dictionaryEntry.Key == type)
					{
						return (Type)dictionaryEntry.Value;
					}
				}
			}
			Type type3 = type;
			while (type3 != null && type3 != typeof(object))
			{
				foreach (object obj2 in TypeDescriptor.DefaultConverters)
				{
					DictionaryEntry dictionaryEntry2 = (DictionaryEntry)obj2;
					Type type4 = (Type)dictionaryEntry2.Key;
					if (type4.IsAssignableFrom(type3))
					{
						type2 = (Type)dictionaryEntry2.Value;
						break;
					}
				}
				type3 = type3.BaseType;
			}
			if (type2 == null)
			{
				if (type != null && type.IsInterface)
				{
					type2 = typeof(ReferenceConverter);
				}
				else
				{
					type2 = typeof(TypeConverter);
				}
			}
			return type2;
		}

		public static EventDescriptor GetDefaultEvent(Type componentType)
		{
			return TypeDescriptor.GetTypeInfo(componentType).GetDefaultEvent();
		}

		public static EventDescriptor GetDefaultEvent(object component)
		{
			return TypeDescriptor.GetDefaultEvent(component, false);
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public static EventDescriptor GetDefaultEvent(object component, bool noCustomTypeDesc)
		{
			if (!noCustomTypeDesc && component is ICustomTypeDescriptor)
			{
				return ((ICustomTypeDescriptor)component).GetDefaultEvent();
			}
			IComponent component2 = component as IComponent;
			if (component2 != null && component2.Site != null)
			{
				return TypeDescriptor.GetComponentInfo(component2).GetDefaultEvent();
			}
			return TypeDescriptor.GetTypeInfo(component.GetType()).GetDefaultEvent();
		}

		public static PropertyDescriptor GetDefaultProperty(Type componentType)
		{
			return TypeDescriptor.GetTypeInfo(componentType).GetDefaultProperty();
		}

		public static PropertyDescriptor GetDefaultProperty(object component)
		{
			return TypeDescriptor.GetDefaultProperty(component, false);
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public static PropertyDescriptor GetDefaultProperty(object component, bool noCustomTypeDesc)
		{
			if (!noCustomTypeDesc && component is ICustomTypeDescriptor)
			{
				return ((ICustomTypeDescriptor)component).GetDefaultProperty();
			}
			IComponent component2 = component as IComponent;
			if (component2 != null && component2.Site != null)
			{
				return TypeDescriptor.GetComponentInfo(component2).GetDefaultProperty();
			}
			return TypeDescriptor.GetTypeInfo(component.GetType()).GetDefaultProperty();
		}

		internal static object CreateEditor(Type t, Type componentType)
		{
			if (t == null)
			{
				return null;
			}
			try
			{
				return Activator.CreateInstance(t);
			}
			catch
			{
			}
			try
			{
				return Activator.CreateInstance(t, new object[] { componentType });
			}
			catch
			{
			}
			return null;
		}

		private static object FindEditorInTable(Type componentType, Type editorBaseType, Hashtable table)
		{
			object obj = null;
			object obj2 = null;
			if (componentType == null || editorBaseType == null || table == null)
			{
				return null;
			}
			for (Type type = componentType; type != null; type = type.BaseType)
			{
				obj = table[type];
				if (obj != null)
				{
					break;
				}
			}
			if (obj == null)
			{
				foreach (Type type2 in componentType.GetInterfaces())
				{
					obj = table[type2];
					if (obj != null)
					{
						break;
					}
				}
			}
			if (obj == null)
			{
				return null;
			}
			if (obj is string)
			{
				obj2 = TypeDescriptor.CreateEditor(Type.GetType((string)obj), componentType);
			}
			else if (obj is Type)
			{
				obj2 = TypeDescriptor.CreateEditor((Type)obj, componentType);
			}
			else if (obj.GetType().IsSubclassOf(editorBaseType))
			{
				obj2 = obj;
			}
			if (obj2 != null)
			{
				table[componentType] = obj2;
			}
			return obj2;
		}

		public static object GetEditor(Type componentType, Type editorBaseType)
		{
			Type type = null;
			object obj = null;
			object[] customAttributes = componentType.GetCustomAttributes(typeof(EditorAttribute), true);
			if (customAttributes != null && customAttributes.Length != 0)
			{
				foreach (EditorAttribute editorAttribute in customAttributes)
				{
					type = TypeDescriptor.GetTypeFromName(null, editorAttribute.EditorTypeName);
					if (type != null && type.IsSubclassOf(editorBaseType))
					{
						break;
					}
				}
			}
			if (type != null)
			{
				obj = TypeDescriptor.CreateEditor(type, componentType);
			}
			if (type == null || obj == null)
			{
				RuntimeHelpers.RunClassConstructor(editorBaseType.TypeHandle);
				if (TypeDescriptor.editors != null)
				{
					obj = TypeDescriptor.FindEditorInTable(componentType, editorBaseType, TypeDescriptor.editors[editorBaseType] as Hashtable);
				}
			}
			return obj;
		}

		public static object GetEditor(object component, Type editorBaseType)
		{
			return TypeDescriptor.GetEditor(component, editorBaseType, false);
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public static object GetEditor(object component, Type editorBaseType, bool noCustomTypeDesc)
		{
			if (component == null)
			{
				throw new ArgumentNullException("component");
			}
			if (editorBaseType == null)
			{
				throw new ArgumentNullException("editorBaseType");
			}
			if (!noCustomTypeDesc && component is ICustomTypeDescriptor)
			{
				return ((ICustomTypeDescriptor)component).GetEditor(editorBaseType);
			}
			object[] customAttributes = component.GetType().GetCustomAttributes(typeof(EditorAttribute), true);
			if (customAttributes.Length == 0)
			{
				return null;
			}
			string assemblyQualifiedName = editorBaseType.AssemblyQualifiedName;
			foreach (EditorAttribute editorAttribute in customAttributes)
			{
				if (editorAttribute.EditorBaseTypeName == assemblyQualifiedName)
				{
					Type type = Type.GetType(editorAttribute.EditorTypeName, true);
					return Activator.CreateInstance(type);
				}
			}
			return null;
		}

		public static EventDescriptorCollection GetEvents(object component)
		{
			return TypeDescriptor.GetEvents(component, false);
		}

		public static EventDescriptorCollection GetEvents(Type componentType)
		{
			return TypeDescriptor.GetEvents(componentType, null);
		}

		public static EventDescriptorCollection GetEvents(object component, Attribute[] attributes)
		{
			return TypeDescriptor.GetEvents(component, attributes, false);
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public static EventDescriptorCollection GetEvents(object component, bool noCustomTypeDesc)
		{
			if (!noCustomTypeDesc && component is ICustomTypeDescriptor)
			{
				return ((ICustomTypeDescriptor)component).GetEvents();
			}
			IComponent component2 = component as IComponent;
			if (component2 != null && component2.Site != null)
			{
				return TypeDescriptor.GetComponentInfo(component2).GetEvents();
			}
			return TypeDescriptor.GetTypeInfo(component.GetType()).GetEvents();
		}

		public static EventDescriptorCollection GetEvents(Type componentType, Attribute[] attributes)
		{
			return TypeDescriptor.GetTypeInfo(componentType).GetEvents(attributes);
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public static EventDescriptorCollection GetEvents(object component, Attribute[] attributes, bool noCustomTypeDesc)
		{
			if (!noCustomTypeDesc && component is ICustomTypeDescriptor)
			{
				return ((ICustomTypeDescriptor)component).GetEvents(attributes);
			}
			IComponent component2 = component as IComponent;
			if (component2 != null && component2.Site != null)
			{
				return TypeDescriptor.GetComponentInfo(component2).GetEvents(attributes);
			}
			return TypeDescriptor.GetTypeInfo(component.GetType()).GetEvents(attributes);
		}

		public static PropertyDescriptorCollection GetProperties(object component)
		{
			return TypeDescriptor.GetProperties(component, false);
		}

		public static PropertyDescriptorCollection GetProperties(Type componentType)
		{
			return TypeDescriptor.GetProperties(componentType, null);
		}

		public static PropertyDescriptorCollection GetProperties(object component, Attribute[] attributes)
		{
			return TypeDescriptor.GetProperties(component, attributes, false);
		}

		public static PropertyDescriptorCollection GetProperties(object component, Attribute[] attributes, bool noCustomTypeDesc)
		{
			if (component == null)
			{
				return PropertyDescriptorCollection.Empty;
			}
			if (!noCustomTypeDesc && component is ICustomTypeDescriptor)
			{
				return ((ICustomTypeDescriptor)component).GetProperties(attributes);
			}
			IComponent component2 = component as IComponent;
			if (component2 != null && component2.Site != null)
			{
				return TypeDescriptor.GetComponentInfo(component2).GetProperties(attributes);
			}
			return TypeDescriptor.GetTypeInfo(component.GetType()).GetProperties(attributes);
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public static PropertyDescriptorCollection GetProperties(object component, bool noCustomTypeDesc)
		{
			if (component == null)
			{
				return PropertyDescriptorCollection.Empty;
			}
			if (!noCustomTypeDesc && component is ICustomTypeDescriptor)
			{
				return ((ICustomTypeDescriptor)component).GetProperties();
			}
			IComponent component2 = component as IComponent;
			if (component2 != null && component2.Site != null)
			{
				return TypeDescriptor.GetComponentInfo(component2).GetProperties();
			}
			return TypeDescriptor.GetTypeInfo(component.GetType()).GetProperties();
		}

		public static PropertyDescriptorCollection GetProperties(Type componentType, Attribute[] attributes)
		{
			return TypeDescriptor.GetTypeInfo(componentType).GetProperties(attributes);
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public static TypeDescriptionProvider GetProvider(object instance)
		{
			if (instance == null)
			{
				throw new ArgumentNullException("instance");
			}
			TypeDescriptionProvider typeDescriptionProvider = null;
			object obj = TypeDescriptor.componentDescriptionProvidersLock;
			lock (obj)
			{
				WeakObjectWrapper weakObjectWrapper = new WeakObjectWrapper(instance);
				global::System.Collections.Generic.LinkedList<TypeDescriptionProvider> linkedList;
				if (TypeDescriptor.componentDescriptionProviders.TryGetValue(weakObjectWrapper, out linkedList) && linkedList.Count > 0)
				{
					typeDescriptionProvider = linkedList.Last.Value;
				}
			}
			if (typeDescriptionProvider == null)
			{
				typeDescriptionProvider = TypeDescriptor.GetProvider(instance.GetType());
			}
			if (typeDescriptionProvider == null)
			{
				return new TypeDescriptor.DefaultTypeDescriptionProvider();
			}
			return new TypeDescriptor.WrappedTypeDescriptionProvider(typeDescriptionProvider);
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public static TypeDescriptionProvider GetProvider(Type type)
		{
			if (type == null)
			{
				throw new ArgumentNullException("type");
			}
			TypeDescriptionProvider typeDescriptionProvider = null;
			object obj = TypeDescriptor.typeDescriptionProvidersLock;
			lock (obj)
			{
				global::System.Collections.Generic.LinkedList<TypeDescriptionProvider> linkedList;
				while (!TypeDescriptor.typeDescriptionProviders.TryGetValue(type, out linkedList))
				{
					linkedList = null;
					type = type.BaseType;
					if (type == null)
					{
						break;
					}
				}
				if (linkedList != null && linkedList.Count > 0)
				{
					typeDescriptionProvider = linkedList.Last.Value;
				}
			}
			if (typeDescriptionProvider == null)
			{
				return new TypeDescriptor.DefaultTypeDescriptionProvider();
			}
			return new TypeDescriptor.WrappedTypeDescriptionProvider(typeDescriptionProvider);
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public static Type GetReflectionType(object instance)
		{
			if (instance == null)
			{
				throw new ArgumentNullException("instance");
			}
			return instance.GetType();
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public static Type GetReflectionType(Type type)
		{
			if (type == null)
			{
				throw new ArgumentNullException("type");
			}
			return type;
		}

		[global::System.MonoNotSupported("Associations not supported")]
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public static void CreateAssociation(object primary, object secondary)
		{
			throw new NotImplementedException();
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		[global::System.MonoNotSupported("Associations not supported")]
		public static object GetAssociation(Type type, object primary)
		{
			throw new NotImplementedException();
		}

		[global::System.MonoNotSupported("Associations not supported")]
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public static void RemoveAssociation(object primary, object secondary)
		{
			throw new NotImplementedException();
		}

		[global::System.MonoNotSupported("Associations not supported")]
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public static void RemoveAssociations(object primary)
		{
			throw new NotImplementedException();
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public static void RemoveProvider(TypeDescriptionProvider provider, object instance)
		{
			if (provider == null)
			{
				throw new ArgumentNullException("provider");
			}
			if (instance == null)
			{
				throw new ArgumentNullException("instance");
			}
			object obj = TypeDescriptor.componentDescriptionProvidersLock;
			lock (obj)
			{
				WeakObjectWrapper weakObjectWrapper = new WeakObjectWrapper(instance);
				global::System.Collections.Generic.LinkedList<TypeDescriptionProvider> linkedList;
				if (TypeDescriptor.componentDescriptionProviders.TryGetValue(weakObjectWrapper, out linkedList) && linkedList.Count > 0)
				{
					TypeDescriptor.RemoveProvider(provider, linkedList);
				}
			}
			RefreshEventHandler refreshed = TypeDescriptor.Refreshed;
			if (refreshed != null)
			{
				refreshed(new RefreshEventArgs(instance));
			}
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public static void RemoveProvider(TypeDescriptionProvider provider, Type type)
		{
			if (provider == null)
			{
				throw new ArgumentNullException("provider");
			}
			if (type == null)
			{
				throw new ArgumentNullException("type");
			}
			object obj = TypeDescriptor.typeDescriptionProvidersLock;
			lock (obj)
			{
				global::System.Collections.Generic.LinkedList<TypeDescriptionProvider> linkedList;
				if (TypeDescriptor.typeDescriptionProviders.TryGetValue(type, out linkedList) && linkedList.Count > 0)
				{
					TypeDescriptor.RemoveProvider(provider, linkedList);
				}
			}
			RefreshEventHandler refreshed = TypeDescriptor.Refreshed;
			if (refreshed != null)
			{
				refreshed(new RefreshEventArgs(type));
			}
		}

		private static void RemoveProvider(TypeDescriptionProvider provider, global::System.Collections.Generic.LinkedList<TypeDescriptionProvider> plist)
		{
			global::System.Collections.Generic.LinkedListNode<TypeDescriptionProvider> linkedListNode = plist.Last;
			global::System.Collections.Generic.LinkedListNode<TypeDescriptionProvider> first = plist.First;
			for (;;)
			{
				TypeDescriptionProvider value = linkedListNode.Value;
				if (value == provider)
				{
					break;
				}
				if (linkedListNode == first)
				{
					return;
				}
				linkedListNode = linkedListNode.Previous;
			}
			plist.Remove(linkedListNode);
		}

		public static void SortDescriptorArray(IList infos)
		{
			string[] array = new string[infos.Count];
			object[] array2 = new object[infos.Count];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = ((MemberDescriptor)infos[i]).Name;
				array2[i] = infos[i];
			}
			Array.Sort<string, object>(array, array2);
			infos.Clear();
			foreach (object obj in array2)
			{
				infos.Add(obj);
			}
		}

		[Obsolete("Use ComObjectType")]
		public static IComNativeDescriptorHandler ComNativeDescriptorHandler
		{
			[PermissionSet((SecurityAction)14, XML = "<PermissionSet class=\"System.Security.PermissionSet\"\nversion=\"1\"\nUnrestricted=\"true\"/>\n")]
			get
			{
				return TypeDescriptor.descriptorHandler;
			}
			[PermissionSet((SecurityAction)14, XML = "<PermissionSet class=\"System.Security.PermissionSet\"\nversion=\"1\"\nUnrestricted=\"true\"/>\n")]
			set
			{
				TypeDescriptor.descriptorHandler = value;
			}
		}

		public static void Refresh(Assembly assembly)
		{
			foreach (Type type in assembly.GetTypes())
			{
				TypeDescriptor.Refresh(type);
			}
		}

		public static void Refresh(Module module)
		{
			foreach (Type type in module.GetTypes())
			{
				TypeDescriptor.Refresh(type);
			}
		}

		public static void Refresh(object component)
		{
			Hashtable hashtable = TypeDescriptor.componentTable;
			lock (hashtable)
			{
				TypeDescriptor.componentTable.Remove(component);
			}
			if (TypeDescriptor.Refreshed != null)
			{
				TypeDescriptor.Refreshed(new RefreshEventArgs(component));
			}
		}

		public static void Refresh(Type type)
		{
			Hashtable hashtable = TypeDescriptor.typeTable;
			lock (hashtable)
			{
				TypeDescriptor.typeTable.Remove(type);
			}
			if (TypeDescriptor.Refreshed != null)
			{
				TypeDescriptor.Refreshed(new RefreshEventArgs(type));
			}
		}

		private static void OnComponentDisposed(object sender, EventArgs args)
		{
			Hashtable hashtable = TypeDescriptor.componentTable;
			lock (hashtable)
			{
				TypeDescriptor.componentTable.Remove(sender);
			}
		}

		internal static ComponentInfo GetComponentInfo(IComponent com)
		{
			Hashtable hashtable = TypeDescriptor.componentTable;
			ComponentInfo componentInfo2;
			lock (hashtable)
			{
				ComponentInfo componentInfo = (ComponentInfo)TypeDescriptor.componentTable[com];
				if (componentInfo == null)
				{
					if (TypeDescriptor.onDispose == null)
					{
						TypeDescriptor.onDispose = new EventHandler(TypeDescriptor.OnComponentDisposed);
					}
					com.Disposed += TypeDescriptor.onDispose;
					componentInfo = new ComponentInfo(com);
					TypeDescriptor.componentTable[com] = componentInfo;
				}
				componentInfo2 = componentInfo;
			}
			return componentInfo2;
		}

		internal static TypeInfo GetTypeInfo(Type type)
		{
			Hashtable hashtable = TypeDescriptor.typeTable;
			TypeInfo typeInfo2;
			lock (hashtable)
			{
				TypeInfo typeInfo = (TypeInfo)TypeDescriptor.typeTable[type];
				if (typeInfo == null)
				{
					typeInfo = new TypeInfo(type);
					TypeDescriptor.typeTable[type] = typeInfo;
				}
				typeInfo2 = typeInfo;
			}
			return typeInfo2;
		}

		private static Type GetTypeFromName(IComponent component, string typeName)
		{
			Type type = null;
			if (component != null && component.Site != null)
			{
				global::System.ComponentModel.Design.ITypeResolutionService typeResolutionService = (global::System.ComponentModel.Design.ITypeResolutionService)component.Site.GetService(typeof(global::System.ComponentModel.Design.ITypeResolutionService));
				if (typeResolutionService != null)
				{
					type = typeResolutionService.GetType(typeName);
				}
			}
			if (type == null)
			{
				type = Type.GetType(typeName);
			}
			return type;
		}

		private static readonly object creatingDefaultConverters = new object();

		private static ArrayList defaultConverters;

		private static IComNativeDescriptorHandler descriptorHandler;

		private static Hashtable componentTable = new Hashtable();

		private static Hashtable typeTable = new Hashtable();

		private static Hashtable editors;

		private static object typeDescriptionProvidersLock = new object();

		private static Dictionary<Type, global::System.Collections.Generic.LinkedList<TypeDescriptionProvider>> typeDescriptionProviders = new Dictionary<Type, global::System.Collections.Generic.LinkedList<TypeDescriptionProvider>>();

		private static object componentDescriptionProvidersLock = new object();

		private static Dictionary<WeakObjectWrapper, global::System.Collections.Generic.LinkedList<TypeDescriptionProvider>> componentDescriptionProviders = new Dictionary<WeakObjectWrapper, global::System.Collections.Generic.LinkedList<TypeDescriptionProvider>>(new WeakObjectWrapperComparer());

		private static EventHandler onDispose;

		private sealed class AttributeProvider : TypeDescriptionProvider
		{
			public AttributeProvider(Attribute[] attributes, TypeDescriptionProvider parent)
				: base(parent)
			{
				this.attributes = attributes;
			}

			public override ICustomTypeDescriptor GetTypeDescriptor(Type type, object instance)
			{
				return new TypeDescriptor.AttributeProvider.AttributeTypeDescriptor(base.GetTypeDescriptor(type, instance), this.attributes);
			}

			private Attribute[] attributes;

			private sealed class AttributeTypeDescriptor : CustomTypeDescriptor
			{
				public AttributeTypeDescriptor(ICustomTypeDescriptor parent, Attribute[] attributes)
					: base(parent)
				{
					this.attributes = attributes;
				}

				public override AttributeCollection GetAttributes()
				{
					AttributeCollection attributeCollection = base.GetAttributes();
					if (attributeCollection != null && attributeCollection.Count > 0)
					{
						return AttributeCollection.FromExisting(attributeCollection, this.attributes);
					}
					return new AttributeCollection(this.attributes);
				}

				private Attribute[] attributes;
			}
		}

		private sealed class WrappedTypeDescriptionProvider : TypeDescriptionProvider
		{
			public WrappedTypeDescriptionProvider(TypeDescriptionProvider wrapped)
			{
				this.Wrapped = wrapped;
			}

			public TypeDescriptionProvider Wrapped { get; private set; }

			public override object CreateInstance(IServiceProvider provider, Type objectType, Type[] argTypes, object[] args)
			{
				TypeDescriptionProvider wrapped = this.Wrapped;
				if (wrapped == null)
				{
					return base.CreateInstance(provider, objectType, argTypes, args);
				}
				return wrapped.CreateInstance(provider, objectType, argTypes, args);
			}

			public override IDictionary GetCache(object instance)
			{
				TypeDescriptionProvider wrapped = this.Wrapped;
				if (wrapped == null)
				{
					return base.GetCache(instance);
				}
				return wrapped.GetCache(instance);
			}

			public override ICustomTypeDescriptor GetExtendedTypeDescriptor(object instance)
			{
				return new TypeDescriptor.DefaultTypeDescriptor(this, null, instance);
			}

			public override string GetFullComponentName(object component)
			{
				TypeDescriptionProvider wrapped = this.Wrapped;
				if (wrapped == null)
				{
					return base.GetFullComponentName(component);
				}
				return wrapped.GetFullComponentName(component);
			}

			public override Type GetReflectionType(Type type, object instance)
			{
				TypeDescriptionProvider wrapped = this.Wrapped;
				if (wrapped == null)
				{
					return base.GetReflectionType(type, instance);
				}
				return wrapped.GetReflectionType(type, instance);
			}

			public override ICustomTypeDescriptor GetTypeDescriptor(Type objectType, object instance)
			{
				TypeDescriptionProvider wrapped = this.Wrapped;
				if (wrapped == null)
				{
					return new TypeDescriptor.DefaultTypeDescriptor(this, objectType, instance);
				}
				return wrapped.GetTypeDescriptor(objectType, instance);
			}
		}

		private sealed class DefaultTypeDescriptor : CustomTypeDescriptor
		{
			public DefaultTypeDescriptor(TypeDescriptionProvider owner, Type objectType, object instance)
			{
				this.owner = owner;
				this.objectType = objectType;
				this.instance = instance;
			}

			public override AttributeCollection GetAttributes()
			{
				TypeDescriptor.WrappedTypeDescriptionProvider wrappedTypeDescriptionProvider = this.owner as TypeDescriptor.WrappedTypeDescriptionProvider;
				if (wrappedTypeDescriptionProvider != null)
				{
					return wrappedTypeDescriptionProvider.Wrapped.GetTypeDescriptor(this.objectType, this.instance).GetAttributes();
				}
				if (this.instance != null)
				{
					return TypeDescriptor.GetAttributes(this.instance, false);
				}
				if (this.objectType != null)
				{
					return TypeDescriptor.GetTypeInfo(this.objectType).GetAttributes();
				}
				return base.GetAttributes();
			}

			public override string GetClassName()
			{
				TypeDescriptor.WrappedTypeDescriptionProvider wrappedTypeDescriptionProvider = this.owner as TypeDescriptor.WrappedTypeDescriptionProvider;
				if (wrappedTypeDescriptionProvider != null)
				{
					return wrappedTypeDescriptionProvider.Wrapped.GetTypeDescriptor(this.objectType, this.instance).GetClassName();
				}
				return base.GetClassName();
			}

			public override PropertyDescriptor GetDefaultProperty()
			{
				TypeDescriptor.WrappedTypeDescriptionProvider wrappedTypeDescriptionProvider = this.owner as TypeDescriptor.WrappedTypeDescriptionProvider;
				if (wrappedTypeDescriptionProvider != null)
				{
					return wrappedTypeDescriptionProvider.Wrapped.GetTypeDescriptor(this.objectType, this.instance).GetDefaultProperty();
				}
				PropertyDescriptor propertyDescriptor;
				if (this.objectType != null)
				{
					propertyDescriptor = TypeDescriptor.GetTypeInfo(this.objectType).GetDefaultProperty();
				}
				else if (this.instance != null)
				{
					propertyDescriptor = TypeDescriptor.GetTypeInfo(this.instance.GetType()).GetDefaultProperty();
				}
				else
				{
					propertyDescriptor = base.GetDefaultProperty();
				}
				return propertyDescriptor;
			}

			public override PropertyDescriptorCollection GetProperties()
			{
				TypeDescriptor.WrappedTypeDescriptionProvider wrappedTypeDescriptionProvider = this.owner as TypeDescriptor.WrappedTypeDescriptionProvider;
				if (wrappedTypeDescriptionProvider != null)
				{
					return wrappedTypeDescriptionProvider.Wrapped.GetTypeDescriptor(this.objectType, this.instance).GetProperties();
				}
				if (this.instance != null)
				{
					return TypeDescriptor.GetProperties(this.instance, null, false);
				}
				if (this.objectType != null)
				{
					return TypeDescriptor.GetTypeInfo(this.objectType).GetProperties(null);
				}
				return base.GetProperties();
			}

			private TypeDescriptionProvider owner;

			private Type objectType;

			private object instance;
		}

		private sealed class DefaultTypeDescriptionProvider : TypeDescriptionProvider
		{
			public override ICustomTypeDescriptor GetExtendedTypeDescriptor(object instance)
			{
				return new TypeDescriptor.DefaultTypeDescriptor(this, null, instance);
			}

			public override ICustomTypeDescriptor GetTypeDescriptor(Type objectType, object instance)
			{
				return new TypeDescriptor.DefaultTypeDescriptor(this, objectType, instance);
			}
		}
	}
}
