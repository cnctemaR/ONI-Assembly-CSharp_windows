using System;
using System.Collections;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace System
{
	[ComVisible(true)]
	[ComDefaultInterface(typeof(_Type))]
	[ClassInterface(ClassInterfaceType.None)]
	[Serializable]
	public abstract class Type : MemberInfo, IReflect, _Type
	{
		void _Type.GetIDsOfNames([In] ref Guid riid, IntPtr rgszNames, uint cNames, uint lcid, IntPtr rgDispId)
		{
			throw new NotImplementedException();
		}

		void _Type.GetTypeInfo(uint iTInfo, uint lcid, IntPtr ppTInfo)
		{
			throw new NotImplementedException();
		}

		void _Type.GetTypeInfoCount(out uint pcTInfo)
		{
			throw new NotImplementedException();
		}

		void _Type.Invoke(uint dispIdMember, [In] ref Guid riid, uint lcid, short wFlags, IntPtr pDispParams, IntPtr pVarResult, IntPtr pExcepInfo, IntPtr puArgErr)
		{
			throw new NotImplementedException();
		}

		private static bool FilterName_impl(MemberInfo m, object filterCriteria)
		{
			string text = (string)filterCriteria;
			if (text == null || text.Length == 0)
			{
				return false;
			}
			if (text[text.Length - 1] == '*')
			{
				return string.Compare(text, 0, m.Name, 0, text.Length - 1, false, CultureInfo.InvariantCulture) == 0;
			}
			return text.Equals(m.Name);
		}

		private static bool FilterNameIgnoreCase_impl(MemberInfo m, object filterCriteria)
		{
			string text = (string)filterCriteria;
			if (text == null || text.Length == 0)
			{
				return false;
			}
			if (text[text.Length - 1] == '*')
			{
				return string.Compare(text, 0, m.Name, 0, text.Length - 1, true, CultureInfo.InvariantCulture) == 0;
			}
			return string.Compare(text, m.Name, true, CultureInfo.InvariantCulture) == 0;
		}

		private static bool FilterAttribute_impl(MemberInfo m, object filterCriteria)
		{
			int num = ((IConvertible)filterCriteria).ToInt32(null);
			if (m is MethodInfo)
			{
				return (((MethodInfo)m).Attributes & (MethodAttributes)num) != MethodAttributes.PrivateScope;
			}
			if (m is FieldInfo)
			{
				return (((FieldInfo)m).Attributes & (FieldAttributes)num) != FieldAttributes.PrivateScope;
			}
			if (m is PropertyInfo)
			{
				return (((PropertyInfo)m).Attributes & (PropertyAttributes)num) != PropertyAttributes.None;
			}
			return m is EventInfo && (((EventInfo)m).Attributes & (EventAttributes)num) != EventAttributes.None;
		}

		public abstract Assembly Assembly { get; }

		public abstract string AssemblyQualifiedName { get; }

		public TypeAttributes Attributes
		{
			get
			{
				return this.GetAttributeFlagsImpl();
			}
		}

		public abstract Type BaseType { get; }

		public override Type DeclaringType
		{
			get
			{
				return null;
			}
		}

		public static Binder DefaultBinder
		{
			get
			{
				return Binder.DefaultBinder;
			}
		}

		public abstract string FullName { get; }

		public abstract Guid GUID { get; }

		public bool HasElementType
		{
			get
			{
				return this.HasElementTypeImpl();
			}
		}

		public bool IsAbstract
		{
			get
			{
				return (this.Attributes & TypeAttributes.Abstract) != TypeAttributes.NotPublic;
			}
		}

		public bool IsAnsiClass
		{
			get
			{
				return (this.Attributes & TypeAttributes.StringFormatMask) == TypeAttributes.NotPublic;
			}
		}

		public bool IsArray
		{
			get
			{
				return this.IsArrayImpl();
			}
		}

		public bool IsAutoClass
		{
			get
			{
				return (this.Attributes & TypeAttributes.StringFormatMask) == TypeAttributes.AutoClass;
			}
		}

		public bool IsAutoLayout
		{
			get
			{
				return (this.Attributes & TypeAttributes.LayoutMask) == TypeAttributes.NotPublic;
			}
		}

		public bool IsByRef
		{
			get
			{
				return this.IsByRefImpl();
			}
		}

		public bool IsClass
		{
			get
			{
				return !this.IsInterface && !this.IsValueType;
			}
		}

		public bool IsCOMObject
		{
			get
			{
				return this.IsCOMObjectImpl();
			}
		}

		public bool IsContextful
		{
			get
			{
				return this.IsContextfulImpl();
			}
		}

		public bool IsEnum
		{
			get
			{
				return this.IsSubclassOf(typeof(Enum));
			}
		}

		public bool IsExplicitLayout
		{
			get
			{
				return (this.Attributes & TypeAttributes.LayoutMask) == TypeAttributes.ExplicitLayout;
			}
		}

		public bool IsImport
		{
			get
			{
				return (this.Attributes & TypeAttributes.Import) != TypeAttributes.NotPublic;
			}
		}

		public bool IsInterface
		{
			get
			{
				return (this.Attributes & TypeAttributes.ClassSemanticsMask) == TypeAttributes.ClassSemanticsMask;
			}
		}

		public bool IsLayoutSequential
		{
			get
			{
				return (this.Attributes & TypeAttributes.LayoutMask) == TypeAttributes.SequentialLayout;
			}
		}

		public bool IsMarshalByRef
		{
			get
			{
				return this.IsMarshalByRefImpl();
			}
		}

		public bool IsNestedAssembly
		{
			get
			{
				return (this.Attributes & TypeAttributes.VisibilityMask) == TypeAttributes.NestedAssembly;
			}
		}

		public bool IsNestedFamANDAssem
		{
			get
			{
				return (this.Attributes & TypeAttributes.VisibilityMask) == TypeAttributes.NestedFamANDAssem;
			}
		}

		public bool IsNestedFamily
		{
			get
			{
				return (this.Attributes & TypeAttributes.VisibilityMask) == TypeAttributes.NestedFamily;
			}
		}

		public bool IsNestedFamORAssem
		{
			get
			{
				return (this.Attributes & TypeAttributes.VisibilityMask) == TypeAttributes.VisibilityMask;
			}
		}

		public bool IsNestedPrivate
		{
			get
			{
				return (this.Attributes & TypeAttributes.VisibilityMask) == TypeAttributes.NestedPrivate;
			}
		}

		public bool IsNestedPublic
		{
			get
			{
				return (this.Attributes & TypeAttributes.VisibilityMask) == TypeAttributes.NestedPublic;
			}
		}

		public bool IsNotPublic
		{
			get
			{
				return (this.Attributes & TypeAttributes.VisibilityMask) == TypeAttributes.NotPublic;
			}
		}

		public bool IsPointer
		{
			get
			{
				return this.IsPointerImpl();
			}
		}

		public bool IsPrimitive
		{
			get
			{
				return this.IsPrimitiveImpl();
			}
		}

		public bool IsPublic
		{
			get
			{
				return (this.Attributes & TypeAttributes.VisibilityMask) == TypeAttributes.Public;
			}
		}

		public bool IsSealed
		{
			get
			{
				return (this.Attributes & TypeAttributes.Sealed) != TypeAttributes.NotPublic;
			}
		}

		public bool IsSerializable
		{
			get
			{
				if ((this.Attributes & TypeAttributes.Serializable) != TypeAttributes.NotPublic)
				{
					return true;
				}
				Type type = this.UnderlyingSystemType;
				if (type == null)
				{
					return false;
				}
				if (type.IsSystemType)
				{
					return Type.type_is_subtype_of(type, typeof(Enum), false) || Type.type_is_subtype_of(type, typeof(Delegate), false);
				}
				while (type != typeof(Enum) && type != typeof(Delegate))
				{
					type = type.BaseType;
					if (type == null)
					{
						return false;
					}
				}
				return true;
			}
		}

		public bool IsSpecialName
		{
			get
			{
				return (this.Attributes & TypeAttributes.SpecialName) != TypeAttributes.NotPublic;
			}
		}

		public bool IsUnicodeClass
		{
			get
			{
				return (this.Attributes & TypeAttributes.StringFormatMask) == TypeAttributes.UnicodeClass;
			}
		}

		public bool IsValueType
		{
			get
			{
				return this.IsValueTypeImpl();
			}
		}

		public override MemberTypes MemberType
		{
			get
			{
				return MemberTypes.TypeInfo;
			}
		}

		public abstract override Module Module { get; }

		public abstract string Namespace { get; }

		public override Type ReflectedType
		{
			get
			{
				return null;
			}
		}

		public virtual RuntimeTypeHandle TypeHandle
		{
			get
			{
				return default(RuntimeTypeHandle);
			}
		}

		[ComVisible(true)]
		public ConstructorInfo TypeInitializer
		{
			get
			{
				return this.GetConstructorImpl(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic, null, CallingConventions.Any, Type.EmptyTypes, null);
			}
		}

		public abstract Type UnderlyingSystemType { get; }

		public override bool Equals(object o)
		{
			if (o == null)
			{
				return false;
			}
			Type type = o as Type;
			return type != null && this.Equals(type);
		}

		public bool Equals(Type o)
		{
			return o != null && this.UnderlyingSystemType.EqualsInternal(o.UnderlyingSystemType);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern bool EqualsInternal(Type type);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern Type internal_from_handle(IntPtr handle);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern Type internal_from_name(string name, bool throwOnError, bool ignoreCase);

		public static Type GetType(string typeName)
		{
			if (typeName == null)
			{
				throw new ArgumentNullException("TypeName");
			}
			return Type.internal_from_name(typeName, false, false);
		}

		public static Type GetType(string typeName, bool throwOnError)
		{
			if (typeName == null)
			{
				throw new ArgumentNullException("TypeName");
			}
			Type type = Type.internal_from_name(typeName, throwOnError, false);
			if (throwOnError && type == null)
			{
				throw new TypeLoadException("Error loading '" + typeName + "'");
			}
			return type;
		}

		public static Type GetType(string typeName, bool throwOnError, bool ignoreCase)
		{
			if (typeName == null)
			{
				throw new ArgumentNullException("TypeName");
			}
			Type type = Type.internal_from_name(typeName, throwOnError, ignoreCase);
			if (throwOnError && type == null)
			{
				throw new TypeLoadException("Error loading '" + typeName + "'");
			}
			return type;
		}

		public static Type[] GetTypeArray(object[] args)
		{
			if (args == null)
			{
				throw new ArgumentNullException("args");
			}
			Type[] array = new Type[args.Length];
			for (int i = 0; i < args.Length; i++)
			{
				array[i] = args[i].GetType();
			}
			return array;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern TypeCode GetTypeCodeInternal(Type type);

		public static TypeCode GetTypeCode(Type type)
		{
			if (type is MonoType)
			{
				return Type.GetTypeCodeInternal(type);
			}
			if (type == null)
			{
				return TypeCode.Empty;
			}
			type = type.UnderlyingSystemType;
			if (!type.IsSystemType)
			{
				return TypeCode.Object;
			}
			return Type.GetTypeCodeInternal(type);
		}

		[MonoTODO("This operation is currently not supported by Mono")]
		public static Type GetTypeFromCLSID(Guid clsid)
		{
			throw new NotImplementedException();
		}

		[MonoTODO("This operation is currently not supported by Mono")]
		public static Type GetTypeFromCLSID(Guid clsid, bool throwOnError)
		{
			throw new NotImplementedException();
		}

		[MonoTODO("This operation is currently not supported by Mono")]
		public static Type GetTypeFromCLSID(Guid clsid, string server)
		{
			throw new NotImplementedException();
		}

		[MonoTODO("This operation is currently not supported by Mono")]
		public static Type GetTypeFromCLSID(Guid clsid, string server, bool throwOnError)
		{
			throw new NotImplementedException();
		}

		public static Type GetTypeFromHandle(RuntimeTypeHandle handle)
		{
			if (handle.Value == IntPtr.Zero)
			{
				return null;
			}
			return Type.internal_from_handle(handle.Value);
		}

		[MonoTODO("Mono does not support COM")]
		public static Type GetTypeFromProgID(string progID)
		{
			throw new NotImplementedException();
		}

		[MonoTODO("Mono does not support COM")]
		public static Type GetTypeFromProgID(string progID, bool throwOnError)
		{
			throw new NotImplementedException();
		}

		[MonoTODO("Mono does not support COM")]
		public static Type GetTypeFromProgID(string progID, string server)
		{
			throw new NotImplementedException();
		}

		[MonoTODO("Mono does not support COM")]
		public static Type GetTypeFromProgID(string progID, string server, bool throwOnError)
		{
			throw new NotImplementedException();
		}

		public static RuntimeTypeHandle GetTypeHandle(object o)
		{
			if (o == null)
			{
				throw new ArgumentNullException();
			}
			return o.GetType().TypeHandle;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern bool type_is_subtype_of(Type a, Type b, bool check_interfaces);

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern bool type_is_assignable_from(Type a, Type b);

		public new Type GetType()
		{
			return base.GetType();
		}

		[ComVisible(true)]
		public virtual bool IsSubclassOf(Type c)
		{
			if (c == null || c == this)
			{
				return false;
			}
			if (this.IsSystemType)
			{
				return c.IsSystemType && Type.type_is_subtype_of(this, c, false);
			}
			for (Type type = this.BaseType; type != null; type = type.BaseType)
			{
				if (type == c)
				{
					return true;
				}
			}
			return false;
		}

		public virtual Type[] FindInterfaces(TypeFilter filter, object filterCriteria)
		{
			if (filter == null)
			{
				throw new ArgumentNullException("filter");
			}
			ArrayList arrayList = new ArrayList();
			foreach (Type type in this.GetInterfaces())
			{
				if (filter(type, filterCriteria))
				{
					arrayList.Add(type);
				}
			}
			return (Type[])arrayList.ToArray(typeof(Type));
		}

		public Type GetInterface(string name)
		{
			return this.GetInterface(name, false);
		}

		public abstract Type GetInterface(string name, bool ignoreCase);

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void GetInterfaceMapData(Type t, Type iface, out MethodInfo[] targets, out MethodInfo[] methods);

		[ComVisible(true)]
		public virtual InterfaceMapping GetInterfaceMap(Type interfaceType)
		{
			if (interfaceType == null)
			{
				throw new ArgumentNullException("interfaceType");
			}
			if (!interfaceType.IsInterface)
			{
				throw new ArgumentException(Locale.GetText("Argument must be an interface."), "interfaceType");
			}
			if (this.IsInterface)
			{
				throw new ArgumentException("'this' type cannot be an interface itself");
			}
			InterfaceMapping interfaceMapping;
			interfaceMapping.TargetType = this;
			interfaceMapping.InterfaceType = interfaceType;
			Type.GetInterfaceMapData(this, interfaceType, out interfaceMapping.TargetMethods, out interfaceMapping.InterfaceMethods);
			if (interfaceMapping.TargetMethods == null)
			{
				throw new ArgumentException(Locale.GetText("Interface not found"), "interfaceType");
			}
			return interfaceMapping;
		}

		public abstract Type[] GetInterfaces();

		public virtual bool IsAssignableFrom(Type c)
		{
			if (c == null)
			{
				return false;
			}
			if (this.Equals(c))
			{
				return true;
			}
			if (c is TypeBuilder)
			{
				return ((TypeBuilder)c).IsAssignableTo(this);
			}
			if (!this.IsSystemType)
			{
				Type underlyingSystemType = this.UnderlyingSystemType;
				return underlyingSystemType.IsSystemType && underlyingSystemType.IsAssignableFrom(c);
			}
			if (!c.IsSystemType)
			{
				Type underlyingSystemType2 = c.UnderlyingSystemType;
				return underlyingSystemType2.IsSystemType && this.IsAssignableFrom(underlyingSystemType2);
			}
			return Type.type_is_assignable_from(this, c);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		public virtual extern bool IsInstanceOfType(object o);

		public virtual int GetArrayRank()
		{
			throw new NotSupportedException();
		}

		public abstract Type GetElementType();

		public EventInfo GetEvent(string name)
		{
			return this.GetEvent(name, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public);
		}

		public abstract EventInfo GetEvent(string name, BindingFlags bindingAttr);

		public virtual EventInfo[] GetEvents()
		{
			return this.GetEvents(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public);
		}

		public abstract EventInfo[] GetEvents(BindingFlags bindingAttr);

		public FieldInfo GetField(string name)
		{
			return this.GetField(name, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public);
		}

		public abstract FieldInfo GetField(string name, BindingFlags bindingAttr);

		public FieldInfo[] GetFields()
		{
			return this.GetFields(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public);
		}

		public abstract FieldInfo[] GetFields(BindingFlags bindingAttr);

		public override int GetHashCode()
		{
			Type underlyingSystemType = this.UnderlyingSystemType;
			if (underlyingSystemType != null && underlyingSystemType != this)
			{
				return underlyingSystemType.GetHashCode();
			}
			return (int)this._impl.Value;
		}

		public MemberInfo[] GetMember(string name)
		{
			return this.GetMember(name, MemberTypes.All, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public);
		}

		public virtual MemberInfo[] GetMember(string name, BindingFlags bindingAttr)
		{
			return this.GetMember(name, MemberTypes.All, bindingAttr);
		}

		public virtual MemberInfo[] GetMember(string name, MemberTypes type, BindingFlags bindingAttr)
		{
			if (name == null)
			{
				throw new ArgumentNullException("name");
			}
			if ((bindingAttr & BindingFlags.IgnoreCase) != BindingFlags.Default)
			{
				return this.FindMembers(type, bindingAttr, Type.FilterNameIgnoreCase, name);
			}
			return this.FindMembers(type, bindingAttr, Type.FilterName, name);
		}

		public MemberInfo[] GetMembers()
		{
			return this.GetMembers(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public);
		}

		public abstract MemberInfo[] GetMembers(BindingFlags bindingAttr);

		public MethodInfo GetMethod(string name)
		{
			if (name == null)
			{
				throw new ArgumentNullException("name");
			}
			return this.GetMethodImpl(name, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public, null, CallingConventions.Any, null, null);
		}

		public MethodInfo GetMethod(string name, BindingFlags bindingAttr)
		{
			if (name == null)
			{
				throw new ArgumentNullException("name");
			}
			return this.GetMethodImpl(name, bindingAttr, null, CallingConventions.Any, null, null);
		}

		public MethodInfo GetMethod(string name, Type[] types)
		{
			return this.GetMethod(name, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public, null, CallingConventions.Any, types, null);
		}

		public MethodInfo GetMethod(string name, Type[] types, ParameterModifier[] modifiers)
		{
			return this.GetMethod(name, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public, null, CallingConventions.Any, types, modifiers);
		}

		public MethodInfo GetMethod(string name, BindingFlags bindingAttr, Binder binder, Type[] types, ParameterModifier[] modifiers)
		{
			return this.GetMethod(name, bindingAttr, binder, CallingConventions.Any, types, modifiers);
		}

		public MethodInfo GetMethod(string name, BindingFlags bindingAttr, Binder binder, CallingConventions callConvention, Type[] types, ParameterModifier[] modifiers)
		{
			if (name == null)
			{
				throw new ArgumentNullException("name");
			}
			if (types == null)
			{
				throw new ArgumentNullException("types");
			}
			for (int i = 0; i < types.Length; i++)
			{
				if (types[i] == null)
				{
					throw new ArgumentNullException("types");
				}
			}
			return this.GetMethodImpl(name, bindingAttr, binder, callConvention, types, modifiers);
		}

		protected abstract MethodInfo GetMethodImpl(string name, BindingFlags bindingAttr, Binder binder, CallingConventions callConvention, Type[] types, ParameterModifier[] modifiers);

		internal MethodInfo GetMethodImplInternal(string name, BindingFlags bindingAttr, Binder binder, CallingConventions callConvention, Type[] types, ParameterModifier[] modifiers)
		{
			return this.GetMethodImpl(name, bindingAttr, binder, callConvention, types, modifiers);
		}

		internal virtual MethodInfo GetMethod(MethodInfo fromNoninstanciated)
		{
			throw new InvalidOperationException("can only be called in generic type");
		}

		internal virtual ConstructorInfo GetConstructor(ConstructorInfo fromNoninstanciated)
		{
			throw new InvalidOperationException("can only be called in generic type");
		}

		internal virtual FieldInfo GetField(FieldInfo fromNoninstanciated)
		{
			throw new InvalidOperationException("can only be called in generic type");
		}

		public MethodInfo[] GetMethods()
		{
			return this.GetMethods(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public);
		}

		public abstract MethodInfo[] GetMethods(BindingFlags bindingAttr);

		public Type GetNestedType(string name)
		{
			return this.GetNestedType(name, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public);
		}

		public abstract Type GetNestedType(string name, BindingFlags bindingAttr);

		public Type[] GetNestedTypes()
		{
			return this.GetNestedTypes(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public);
		}

		public abstract Type[] GetNestedTypes(BindingFlags bindingAttr);

		public PropertyInfo[] GetProperties()
		{
			return this.GetProperties(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public);
		}

		public abstract PropertyInfo[] GetProperties(BindingFlags bindingAttr);

		public PropertyInfo GetProperty(string name)
		{
			if (name == null)
			{
				throw new ArgumentNullException("name");
			}
			return this.GetPropertyImpl(name, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public, null, null, null, null);
		}

		public PropertyInfo GetProperty(string name, BindingFlags bindingAttr)
		{
			if (name == null)
			{
				throw new ArgumentNullException("name");
			}
			return this.GetPropertyImpl(name, bindingAttr, null, null, null, null);
		}

		public PropertyInfo GetProperty(string name, Type returnType)
		{
			if (name == null)
			{
				throw new ArgumentNullException("name");
			}
			return this.GetPropertyImpl(name, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public, null, returnType, null, null);
		}

		public PropertyInfo GetProperty(string name, Type[] types)
		{
			return this.GetProperty(name, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public, null, null, types, null);
		}

		public PropertyInfo GetProperty(string name, Type returnType, Type[] types)
		{
			return this.GetProperty(name, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public, null, returnType, types, null);
		}

		public PropertyInfo GetProperty(string name, Type returnType, Type[] types, ParameterModifier[] modifiers)
		{
			return this.GetProperty(name, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public, null, returnType, types, modifiers);
		}

		public PropertyInfo GetProperty(string name, BindingFlags bindingAttr, Binder binder, Type returnType, Type[] types, ParameterModifier[] modifiers)
		{
			if (name == null)
			{
				throw new ArgumentNullException("name");
			}
			if (types == null)
			{
				throw new ArgumentNullException("types");
			}
			for (int i = 0; i < types.Length; i++)
			{
				if (types[i] == null)
				{
					throw new ArgumentNullException("types");
				}
			}
			return this.GetPropertyImpl(name, bindingAttr, binder, returnType, types, modifiers);
		}

		protected abstract PropertyInfo GetPropertyImpl(string name, BindingFlags bindingAttr, Binder binder, Type returnType, Type[] types, ParameterModifier[] modifiers);

		internal PropertyInfo GetPropertyImplInternal(string name, BindingFlags bindingAttr, Binder binder, Type returnType, Type[] types, ParameterModifier[] modifiers)
		{
			return this.GetPropertyImpl(name, bindingAttr, binder, returnType, types, modifiers);
		}

		protected abstract ConstructorInfo GetConstructorImpl(BindingFlags bindingAttr, Binder binder, CallingConventions callConvention, Type[] types, ParameterModifier[] modifiers);

		protected abstract TypeAttributes GetAttributeFlagsImpl();

		protected abstract bool HasElementTypeImpl();

		protected abstract bool IsArrayImpl();

		protected abstract bool IsByRefImpl();

		protected abstract bool IsCOMObjectImpl();

		protected abstract bool IsPointerImpl();

		protected abstract bool IsPrimitiveImpl();

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern bool IsArrayImpl(Type type);

		protected virtual bool IsValueTypeImpl()
		{
			return this != typeof(ValueType) && this != typeof(Enum) && this.IsSubclassOf(typeof(ValueType));
		}

		protected virtual bool IsContextfulImpl()
		{
			return typeof(ContextBoundObject).IsAssignableFrom(this);
		}

		protected virtual bool IsMarshalByRefImpl()
		{
			return typeof(MarshalByRefObject).IsAssignableFrom(this);
		}

		[ComVisible(true)]
		public ConstructorInfo GetConstructor(Type[] types)
		{
			return this.GetConstructor(BindingFlags.Instance | BindingFlags.Public, null, CallingConventions.Any, types, null);
		}

		[ComVisible(true)]
		public ConstructorInfo GetConstructor(BindingFlags bindingAttr, Binder binder, Type[] types, ParameterModifier[] modifiers)
		{
			return this.GetConstructor(bindingAttr, binder, CallingConventions.Any, types, modifiers);
		}

		[ComVisible(true)]
		public ConstructorInfo GetConstructor(BindingFlags bindingAttr, Binder binder, CallingConventions callConvention, Type[] types, ParameterModifier[] modifiers)
		{
			if (types == null)
			{
				throw new ArgumentNullException("types");
			}
			for (int i = 0; i < types.Length; i++)
			{
				if (types[i] == null)
				{
					throw new ArgumentNullException("types");
				}
			}
			return this.GetConstructorImpl(bindingAttr, binder, callConvention, types, modifiers);
		}

		[ComVisible(true)]
		public ConstructorInfo[] GetConstructors()
		{
			return this.GetConstructors(BindingFlags.Instance | BindingFlags.Public);
		}

		[ComVisible(true)]
		public abstract ConstructorInfo[] GetConstructors(BindingFlags bindingAttr);

		public virtual MemberInfo[] GetDefaultMembers()
		{
			object[] customAttributes = this.GetCustomAttributes(typeof(DefaultMemberAttribute), true);
			if (customAttributes.Length == 0)
			{
				return new MemberInfo[0];
			}
			MemberInfo[] member = this.GetMember(((DefaultMemberAttribute)customAttributes[0]).MemberName);
			return (member == null) ? new MemberInfo[0] : member;
		}

		public virtual MemberInfo[] FindMembers(MemberTypes memberType, BindingFlags bindingAttr, MemberFilter filter, object filterCriteria)
		{
			ArrayList arrayList = new ArrayList();
			if ((memberType & MemberTypes.Method) != (MemberTypes)0)
			{
				MethodInfo[] methods = this.GetMethods(bindingAttr);
				if (filter != null)
				{
					foreach (MethodInfo memberInfo in methods)
					{
						if (filter(memberInfo, filterCriteria))
						{
							arrayList.Add(memberInfo);
						}
					}
				}
				else
				{
					arrayList.AddRange(methods);
				}
			}
			if ((memberType & MemberTypes.Constructor) != (MemberTypes)0)
			{
				ConstructorInfo[] constructors = this.GetConstructors(bindingAttr);
				if (filter != null)
				{
					foreach (ConstructorInfo memberInfo2 in constructors)
					{
						if (filter(memberInfo2, filterCriteria))
						{
							arrayList.Add(memberInfo2);
						}
					}
				}
				else
				{
					arrayList.AddRange(constructors);
				}
			}
			if ((memberType & MemberTypes.Property) != (MemberTypes)0)
			{
				int count = arrayList.Count;
				if (filter != null)
				{
					Type type = this;
					while (arrayList.Count == count && type != null)
					{
						PropertyInfo[] array3 = type.GetProperties(bindingAttr);
						foreach (PropertyInfo memberInfo3 in array3)
						{
							if (filter(memberInfo3, filterCriteria))
							{
								arrayList.Add(memberInfo3);
							}
						}
						type = type.BaseType;
					}
				}
				else
				{
					PropertyInfo[] array3 = this.GetProperties(bindingAttr);
					arrayList.AddRange(array3);
				}
			}
			if ((memberType & MemberTypes.Event) != (MemberTypes)0)
			{
				EventInfo[] events = this.GetEvents(bindingAttr);
				if (filter != null)
				{
					foreach (EventInfo memberInfo4 in events)
					{
						if (filter(memberInfo4, filterCriteria))
						{
							arrayList.Add(memberInfo4);
						}
					}
				}
				else
				{
					arrayList.AddRange(events);
				}
			}
			if ((memberType & MemberTypes.Field) != (MemberTypes)0)
			{
				FieldInfo[] fields = this.GetFields(bindingAttr);
				if (filter != null)
				{
					foreach (FieldInfo memberInfo5 in fields)
					{
						if (filter(memberInfo5, filterCriteria))
						{
							arrayList.Add(memberInfo5);
						}
					}
				}
				else
				{
					arrayList.AddRange(fields);
				}
			}
			if ((memberType & MemberTypes.NestedType) != (MemberTypes)0)
			{
				Type[] nestedTypes = this.GetNestedTypes(bindingAttr);
				if (filter != null)
				{
					foreach (Type memberInfo6 in nestedTypes)
					{
						if (filter(memberInfo6, filterCriteria))
						{
							arrayList.Add(memberInfo6);
						}
					}
				}
				else
				{
					arrayList.AddRange(nestedTypes);
				}
			}
			MemberInfo[] array8;
			switch (memberType)
			{
			case MemberTypes.Constructor:
				array8 = new ConstructorInfo[arrayList.Count];
				break;
			case MemberTypes.Event:
				array8 = new EventInfo[arrayList.Count];
				break;
			default:
				if (memberType != MemberTypes.Property)
				{
					if (memberType != MemberTypes.TypeInfo && memberType != MemberTypes.NestedType)
					{
						array8 = new MemberInfo[arrayList.Count];
					}
					else
					{
						array8 = new Type[arrayList.Count];
					}
				}
				else
				{
					array8 = new PropertyInfo[arrayList.Count];
				}
				break;
			case MemberTypes.Field:
				array8 = new FieldInfo[arrayList.Count];
				break;
			case MemberTypes.Method:
				array8 = new MethodInfo[arrayList.Count];
				break;
			}
			arrayList.CopyTo(array8);
			return array8;
		}

		[DebuggerStepThrough]
		[DebuggerHidden]
		public object InvokeMember(string name, BindingFlags invokeAttr, Binder binder, object target, object[] args)
		{
			return this.InvokeMember(name, invokeAttr, binder, target, args, null, null, null);
		}

		[DebuggerStepThrough]
		[DebuggerHidden]
		public object InvokeMember(string name, BindingFlags invokeAttr, Binder binder, object target, object[] args, CultureInfo culture)
		{
			return this.InvokeMember(name, invokeAttr, binder, target, args, null, culture, null);
		}

		public abstract object InvokeMember(string name, BindingFlags invokeAttr, Binder binder, object target, object[] args, ParameterModifier[] modifiers, CultureInfo culture, string[] namedParameters);

		public override string ToString()
		{
			return this.FullName;
		}

		internal bool IsSystemType
		{
			get
			{
				return this._impl.Value != IntPtr.Zero;
			}
		}

		public virtual Type[] GetGenericArguments()
		{
			throw new NotSupportedException();
		}

		public virtual bool ContainsGenericParameters
		{
			get
			{
				return false;
			}
		}

		public virtual extern bool IsGenericTypeDefinition
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern Type GetGenericTypeDefinition_impl();

		public virtual Type GetGenericTypeDefinition()
		{
			throw new NotSupportedException("Derived classes must provide an implementation.");
		}

		public virtual extern bool IsGenericType
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern Type MakeGenericType(Type gt, Type[] types);

		public virtual Type MakeGenericType(params Type[] typeArguments)
		{
			if (!this.IsGenericTypeDefinition)
			{
				throw new InvalidOperationException("not a generic type definition");
			}
			if (typeArguments == null)
			{
				throw new ArgumentNullException("typeArguments");
			}
			if (this.GetGenericArguments().Length != typeArguments.Length)
			{
				throw new ArgumentException(string.Format("The type or method has {0} generic parameter(s) but {1} generic argument(s) where provided. A generic argument must be provided for each generic parameter.", this.GetGenericArguments().Length, typeArguments.Length), "typeArguments");
			}
			Type[] array = new Type[typeArguments.Length];
			for (int i = 0; i < typeArguments.Length; i++)
			{
				Type type = typeArguments[i];
				if (type == null)
				{
					throw new ArgumentNullException("typeArguments");
				}
				if (!(type is EnumBuilder) && !(type is TypeBuilder))
				{
					type = type.UnderlyingSystemType;
				}
				if (type == null || !type.IsSystemType)
				{
					throw new ArgumentNullException("typeArguments");
				}
				array[i] = type;
			}
			Type type2 = Type.MakeGenericType(this, array);
			if (type2 == null)
			{
				throw new TypeLoadException();
			}
			return type2;
		}

		public virtual bool IsGenericParameter
		{
			get
			{
				return false;
			}
		}

		public bool IsNested
		{
			get
			{
				return this.DeclaringType != null;
			}
		}

		public bool IsVisible
		{
			get
			{
				if (this.IsNestedPublic)
				{
					return this.DeclaringType.IsVisible;
				}
				return this.IsPublic;
			}
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern int GetGenericParameterPosition();

		public virtual int GenericParameterPosition
		{
			get
			{
				int genericParameterPosition = this.GetGenericParameterPosition();
				if (genericParameterPosition < 0)
				{
					throw new InvalidOperationException();
				}
				return genericParameterPosition;
			}
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern GenericParameterAttributes GetGenericParameterAttributes();

		public virtual GenericParameterAttributes GenericParameterAttributes
		{
			get
			{
				if (!this.IsGenericParameter)
				{
					throw new InvalidOperationException();
				}
				return this.GetGenericParameterAttributes();
			}
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern Type[] GetGenericParameterConstraints_impl();

		public virtual Type[] GetGenericParameterConstraints()
		{
			if (!this.IsGenericParameter)
			{
				throw new InvalidOperationException();
			}
			return this.GetGenericParameterConstraints_impl();
		}

		public virtual MethodBase DeclaringMethod
		{
			get
			{
				return null;
			}
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern Type make_array_type(int rank);

		public virtual Type MakeArrayType()
		{
			return this.make_array_type(0);
		}

		public virtual Type MakeArrayType(int rank)
		{
			if (rank < 1)
			{
				throw new IndexOutOfRangeException();
			}
			return this.make_array_type(rank);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern Type make_byref_type();

		public virtual Type MakeByRefType()
		{
			return this.make_byref_type();
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		public virtual extern Type MakePointerType();

		public static Type ReflectionOnlyGetType(string typeName, bool throwIfNotFound, bool ignoreCase)
		{
			if (typeName == null)
			{
				throw new ArgumentNullException("typeName");
			}
			int num = typeName.IndexOf(',');
			if (num < 0 || num == 0 || num == typeName.Length - 1)
			{
				throw new ArgumentException("Assembly qualifed type name is required", "typeName");
			}
			string text = typeName.Substring(num + 1);
			Assembly assembly;
			try
			{
				assembly = Assembly.ReflectionOnlyLoad(text);
			}
			catch
			{
				if (throwIfNotFound)
				{
					throw;
				}
				return null;
			}
			return assembly.GetType(typeName.Substring(0, num), throwIfNotFound, ignoreCase);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void GetPacking(out int packing, out int size);

		public virtual StructLayoutAttribute StructLayoutAttribute
		{
			get
			{
				LayoutKind layoutKind;
				if (this.IsLayoutSequential)
				{
					layoutKind = LayoutKind.Sequential;
				}
				else if (this.IsExplicitLayout)
				{
					layoutKind = LayoutKind.Explicit;
				}
				else
				{
					layoutKind = LayoutKind.Auto;
				}
				StructLayoutAttribute structLayoutAttribute = new StructLayoutAttribute(layoutKind);
				if (this.IsUnicodeClass)
				{
					structLayoutAttribute.CharSet = CharSet.Unicode;
				}
				else if (this.IsAnsiClass)
				{
					structLayoutAttribute.CharSet = CharSet.Ansi;
				}
				else
				{
					structLayoutAttribute.CharSet = CharSet.Auto;
				}
				if (layoutKind != LayoutKind.Auto)
				{
					this.GetPacking(out structLayoutAttribute.Pack, out structLayoutAttribute.Size);
				}
				return structLayoutAttribute;
			}
		}

		internal object[] GetPseudoCustomAttributes()
		{
			int num = 0;
			if ((this.Attributes & TypeAttributes.Serializable) != TypeAttributes.NotPublic)
			{
				num++;
			}
			if ((this.Attributes & TypeAttributes.Import) != TypeAttributes.NotPublic)
			{
				num++;
			}
			if (num == 0)
			{
				return null;
			}
			object[] array = new object[num];
			num = 0;
			if ((this.Attributes & TypeAttributes.Serializable) != TypeAttributes.NotPublic)
			{
				array[num++] = new SerializableAttribute();
			}
			if ((this.Attributes & TypeAttributes.Import) != TypeAttributes.NotPublic)
			{
				array[num++] = new ComImportAttribute();
			}
			return array;
		}

		internal bool IsUserType
		{
			get
			{
				return this._impl.Value == IntPtr.Zero && (this.GetType().Assembly != typeof(Type).Assembly || this.GetType() == typeof(TypeDelegator));
			}
		}

		internal const BindingFlags DefaultBindingFlags = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public;

		internal RuntimeTypeHandle _impl;

		public static readonly char Delimiter = '.';

		public static readonly Type[] EmptyTypes = new Type[0];

		public static readonly MemberFilter FilterAttribute = new MemberFilter(Type.FilterAttribute_impl);

		public static readonly MemberFilter FilterName = new MemberFilter(Type.FilterName_impl);

		public static readonly MemberFilter FilterNameIgnoreCase = new MemberFilter(Type.FilterNameIgnoreCase_impl);

		public static readonly object Missing = global::System.Reflection.Missing.Value;
	}
}
