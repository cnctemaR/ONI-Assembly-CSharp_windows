using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.SymbolStore;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Permissions;
using Unity;

namespace System.Reflection.Emit
{
	[ComVisible(true)]
	[ComDefaultInterface(typeof(_TypeBuilder))]
	[ClassInterface(ClassInterfaceType.None)]
	[StructLayout(LayoutKind.Sequential)]
	public sealed class TypeBuilder : TypeInfo, _TypeBuilder
	{
		protected override TypeAttributes GetAttributeFlagsImpl()
		{
			return this.attrs;
		}

		internal TypeBuilder(ModuleBuilder mb, TypeAttributes attr, int table_idx)
		{
			this.parent = null;
			this.attrs = attr;
			this.class_size = 0;
			this.table_idx = table_idx;
			this.tname = ((table_idx == 1) ? "<Module>" : ("type_" + table_idx.ToString()));
			this.nspace = string.Empty;
			this.fullname = TypeIdentifiers.WithoutEscape(this.tname);
			this.pmodule = mb;
		}

		internal TypeBuilder(ModuleBuilder mb, string name, TypeAttributes attr, Type parent, Type[] interfaces, PackingSize packing_size, int type_size, Type nesting_type)
		{
			this.parent = TypeBuilder.ResolveUserType(parent);
			this.attrs = attr;
			this.class_size = type_size;
			this.packing_size = packing_size;
			this.nesting_type = nesting_type;
			this.check_name("fullname", name);
			if (parent == null && (attr & TypeAttributes.ClassSemanticsMask) != TypeAttributes.NotPublic && (attr & TypeAttributes.Abstract) == TypeAttributes.NotPublic)
			{
				throw new InvalidOperationException("Interface must be declared abstract.");
			}
			int num = name.LastIndexOf('.');
			if (num != -1)
			{
				this.tname = name.Substring(num + 1);
				this.nspace = name.Substring(0, num);
			}
			else
			{
				this.tname = name;
				this.nspace = string.Empty;
			}
			if (interfaces != null)
			{
				this.interfaces = new Type[interfaces.Length];
				Array.Copy(interfaces, this.interfaces, interfaces.Length);
			}
			this.pmodule = mb;
			if ((attr & TypeAttributes.ClassSemanticsMask) == TypeAttributes.NotPublic && parent == null)
			{
				this.parent = typeof(object);
			}
			this.table_idx = mb.get_next_table_index(this, 2, true);
			this.fullname = this.GetFullName();
		}

		public override Assembly Assembly
		{
			get
			{
				return this.pmodule.Assembly;
			}
		}

		public override string AssemblyQualifiedName
		{
			get
			{
				return this.fullname.DisplayName + ", " + this.Assembly.FullName;
			}
		}

		public override Type BaseType
		{
			get
			{
				return this.parent;
			}
		}

		public override Type DeclaringType
		{
			get
			{
				return this.nesting_type;
			}
		}

		[ComVisible(true)]
		public override bool IsSubclassOf(Type c)
		{
			if (c == null)
			{
				return false;
			}
			if (c == this)
			{
				return false;
			}
			Type baseType = this.parent;
			while (baseType != null)
			{
				if (c == baseType)
				{
					return true;
				}
				baseType = baseType.BaseType;
			}
			return false;
		}

		public override Type UnderlyingSystemType
		{
			get
			{
				if (this.is_created)
				{
					return this.created.UnderlyingSystemType;
				}
				if (!this.IsEnum)
				{
					return this;
				}
				if (this.underlying_type != null)
				{
					return this.underlying_type;
				}
				throw new InvalidOperationException("Enumeration type is not defined.");
			}
		}

		private TypeName GetFullName()
		{
			TypeIdentifier typeIdentifier = TypeIdentifiers.FromInternal(this.tname);
			if (this.nesting_type != null)
			{
				return TypeNames.FromDisplay(this.nesting_type.FullName).NestedName(typeIdentifier);
			}
			if (this.nspace != null && this.nspace.Length > 0)
			{
				return TypeIdentifiers.FromInternal(this.nspace, typeIdentifier);
			}
			return typeIdentifier;
		}

		public override string FullName
		{
			get
			{
				return this.fullname.DisplayName;
			}
		}

		public override Guid GUID
		{
			get
			{
				this.check_created();
				return this.created.GUID;
			}
		}

		public override Module Module
		{
			get
			{
				return this.pmodule;
			}
		}

		public override string Name
		{
			get
			{
				return this.tname;
			}
		}

		public override string Namespace
		{
			get
			{
				return this.nspace;
			}
		}

		public PackingSize PackingSize
		{
			get
			{
				return this.packing_size;
			}
		}

		public int Size
		{
			get
			{
				return this.class_size;
			}
		}

		public override Type ReflectedType
		{
			get
			{
				return this.nesting_type;
			}
		}

		public void AddDeclarativeSecurity(SecurityAction action, PermissionSet pset)
		{
			if (pset == null)
			{
				throw new ArgumentNullException("pset");
			}
			if (action == SecurityAction.RequestMinimum || action == SecurityAction.RequestOptional || action == SecurityAction.RequestRefuse)
			{
				throw new ArgumentOutOfRangeException("Request* values are not permitted", "action");
			}
			this.check_not_created();
			if (this.permissions != null)
			{
				RefEmitPermissionSet[] array = this.permissions;
				for (int i = 0; i < array.Length; i++)
				{
					if (array[i].action == action)
					{
						throw new InvalidOperationException("Multiple permission sets specified with the same SecurityAction.");
					}
				}
				RefEmitPermissionSet[] array2 = new RefEmitPermissionSet[this.permissions.Length + 1];
				this.permissions.CopyTo(array2, 0);
				this.permissions = array2;
			}
			else
			{
				this.permissions = new RefEmitPermissionSet[1];
			}
			this.permissions[this.permissions.Length - 1] = new RefEmitPermissionSet(action, pset.ToXml().ToString());
			this.attrs |= TypeAttributes.HasSecurity;
		}

		[ComVisible(true)]
		public void AddInterfaceImplementation(Type interfaceType)
		{
			if (interfaceType == null)
			{
				throw new ArgumentNullException("interfaceType");
			}
			this.check_not_created();
			if (this.interfaces != null)
			{
				Type[] array = this.interfaces;
				for (int i = 0; i < array.Length; i++)
				{
					if (array[i] == interfaceType)
					{
						return;
					}
				}
				Type[] array2 = new Type[this.interfaces.Length + 1];
				this.interfaces.CopyTo(array2, 0);
				array2[this.interfaces.Length] = interfaceType;
				this.interfaces = array2;
				return;
			}
			this.interfaces = new Type[1];
			this.interfaces[0] = interfaceType;
		}

		protected override ConstructorInfo GetConstructorImpl(BindingFlags bindingAttr, Binder binder, CallingConventions callConvention, Type[] types, ParameterModifier[] modifiers)
		{
			this.check_created();
			if (!(this.created == typeof(object)))
			{
				return this.created.GetConstructor(bindingAttr, binder, callConvention, types, modifiers);
			}
			if (this.ctors == null)
			{
				return null;
			}
			ConstructorBuilder constructorBuilder = null;
			int num = 0;
			foreach (ConstructorBuilder constructorBuilder2 in this.ctors)
			{
				if (callConvention == CallingConventions.Any || constructorBuilder2.CallingConvention == callConvention)
				{
					constructorBuilder = constructorBuilder2;
					num++;
				}
			}
			if (num == 0)
			{
				return null;
			}
			if (types != null)
			{
				MethodBase[] array2 = new MethodBase[num];
				if (num == 1)
				{
					array2[0] = constructorBuilder;
				}
				else
				{
					num = 0;
					foreach (ConstructorBuilder constructorInfo in this.ctors)
					{
						if (callConvention == CallingConventions.Any || constructorInfo.CallingConvention == callConvention)
						{
							array2[num++] = constructorInfo;
						}
					}
				}
				if (binder == null)
				{
					binder = Type.DefaultBinder;
				}
				return (ConstructorInfo)binder.SelectMethod(bindingAttr, array2, types, modifiers);
			}
			if (num > 1)
			{
				throw new AmbiguousMatchException();
			}
			return constructorBuilder;
		}

		[SecuritySafeCritical]
		public override bool IsDefined(Type attributeType, bool inherit)
		{
			if (!this.is_created)
			{
				throw new NotSupportedException();
			}
			return MonoCustomAttrs.IsDefined(this, attributeType, inherit);
		}

		[SecuritySafeCritical]
		public override object[] GetCustomAttributes(bool inherit)
		{
			this.check_created();
			return this.created.GetCustomAttributes(inherit);
		}

		[SecuritySafeCritical]
		public override object[] GetCustomAttributes(Type attributeType, bool inherit)
		{
			this.check_created();
			return this.created.GetCustomAttributes(attributeType, inherit);
		}

		public TypeBuilder DefineNestedType(string name)
		{
			return this.DefineNestedType(name, TypeAttributes.NestedPrivate, this.pmodule.assemblyb.corlib_object_type, null);
		}

		public TypeBuilder DefineNestedType(string name, TypeAttributes attr)
		{
			return this.DefineNestedType(name, attr, this.pmodule.assemblyb.corlib_object_type, null);
		}

		public TypeBuilder DefineNestedType(string name, TypeAttributes attr, Type parent)
		{
			return this.DefineNestedType(name, attr, parent, null);
		}

		private TypeBuilder DefineNestedType(string name, TypeAttributes attr, Type parent, Type[] interfaces, PackingSize packSize, int typeSize)
		{
			if (interfaces != null)
			{
				for (int i = 0; i < interfaces.Length; i++)
				{
					if (interfaces[i] == null)
					{
						throw new ArgumentNullException("interfaces");
					}
				}
			}
			TypeBuilder typeBuilder = new TypeBuilder(this.pmodule, name, attr, parent, interfaces, packSize, typeSize, this);
			typeBuilder.fullname = typeBuilder.GetFullName();
			this.pmodule.RegisterTypeName(typeBuilder, typeBuilder.fullname);
			if (this.subtypes != null)
			{
				TypeBuilder[] array = new TypeBuilder[this.subtypes.Length + 1];
				Array.Copy(this.subtypes, array, this.subtypes.Length);
				array[this.subtypes.Length] = typeBuilder;
				this.subtypes = array;
			}
			else
			{
				this.subtypes = new TypeBuilder[1];
				this.subtypes[0] = typeBuilder;
			}
			return typeBuilder;
		}

		[ComVisible(true)]
		public TypeBuilder DefineNestedType(string name, TypeAttributes attr, Type parent, Type[] interfaces)
		{
			return this.DefineNestedType(name, attr, parent, interfaces, PackingSize.Unspecified, 0);
		}

		public TypeBuilder DefineNestedType(string name, TypeAttributes attr, Type parent, int typeSize)
		{
			return this.DefineNestedType(name, attr, parent, null, PackingSize.Unspecified, typeSize);
		}

		public TypeBuilder DefineNestedType(string name, TypeAttributes attr, Type parent, PackingSize packSize)
		{
			return this.DefineNestedType(name, attr, parent, null, packSize, 0);
		}

		public TypeBuilder DefineNestedType(string name, TypeAttributes attr, Type parent, PackingSize packSize, int typeSize)
		{
			return this.DefineNestedType(name, attr, parent, null, packSize, typeSize);
		}

		[ComVisible(true)]
		public ConstructorBuilder DefineConstructor(MethodAttributes attributes, CallingConventions callingConvention, Type[] parameterTypes)
		{
			return this.DefineConstructor(attributes, callingConvention, parameterTypes, null, null);
		}

		[ComVisible(true)]
		public ConstructorBuilder DefineConstructor(MethodAttributes attributes, CallingConventions callingConvention, Type[] parameterTypes, Type[][] requiredCustomModifiers, Type[][] optionalCustomModifiers)
		{
			this.check_not_created();
			ConstructorBuilder constructorBuilder = new ConstructorBuilder(this, attributes, callingConvention, parameterTypes, requiredCustomModifiers, optionalCustomModifiers);
			if (this.ctors != null)
			{
				ConstructorBuilder[] array = new ConstructorBuilder[this.ctors.Length + 1];
				Array.Copy(this.ctors, array, this.ctors.Length);
				array[this.ctors.Length] = constructorBuilder;
				this.ctors = array;
			}
			else
			{
				this.ctors = new ConstructorBuilder[1];
				this.ctors[0] = constructorBuilder;
			}
			return constructorBuilder;
		}

		[ComVisible(true)]
		public ConstructorBuilder DefineDefaultConstructor(MethodAttributes attributes)
		{
			Type type;
			if (this.parent != null)
			{
				type = this.parent;
			}
			else
			{
				type = this.pmodule.assemblyb.corlib_object_type;
			}
			Type type2 = type;
			type = type.InternalResolve();
			if (type == typeof(object) || type == typeof(ValueType))
			{
				type = type2;
			}
			ConstructorInfo constructor = type.GetConstructor(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, Type.EmptyTypes, null);
			if (constructor == null)
			{
				throw new NotSupportedException("Parent does not have a default constructor. The default constructor must be explicitly defined.");
			}
			ConstructorBuilder constructorBuilder = this.DefineConstructor(attributes, CallingConventions.Standard, Type.EmptyTypes);
			ILGenerator ilgenerator = constructorBuilder.GetILGenerator();
			ilgenerator.Emit(OpCodes.Ldarg_0);
			ilgenerator.Emit(OpCodes.Call, constructor);
			ilgenerator.Emit(OpCodes.Ret);
			return constructorBuilder;
		}

		private void append_method(MethodBuilder mb)
		{
			if (this.methods != null)
			{
				if (this.methods.Length == this.num_methods)
				{
					MethodBuilder[] array = new MethodBuilder[this.methods.Length * 2];
					Array.Copy(this.methods, array, this.num_methods);
					this.methods = array;
				}
			}
			else
			{
				this.methods = new MethodBuilder[1];
			}
			this.methods[this.num_methods] = mb;
			this.num_methods++;
		}

		public MethodBuilder DefineMethod(string name, MethodAttributes attributes, Type returnType, Type[] parameterTypes)
		{
			return this.DefineMethod(name, attributes, CallingConventions.Standard, returnType, parameterTypes);
		}

		public MethodBuilder DefineMethod(string name, MethodAttributes attributes, CallingConventions callingConvention, Type returnType, Type[] parameterTypes)
		{
			return this.DefineMethod(name, attributes, callingConvention, returnType, null, null, parameterTypes, null, null);
		}

		public MethodBuilder DefineMethod(string name, MethodAttributes attributes, CallingConventions callingConvention, Type returnType, Type[] returnTypeRequiredCustomModifiers, Type[] returnTypeOptionalCustomModifiers, Type[] parameterTypes, Type[][] parameterTypeRequiredCustomModifiers, Type[][] parameterTypeOptionalCustomModifiers)
		{
			this.check_name("name", name);
			this.check_not_created();
			if (base.IsInterface && ((attributes & MethodAttributes.Abstract) == MethodAttributes.PrivateScope || (attributes & MethodAttributes.Virtual) == MethodAttributes.PrivateScope) && (attributes & MethodAttributes.Static) == MethodAttributes.PrivateScope)
			{
				throw new ArgumentException("Interface method must be abstract and virtual.");
			}
			if (returnType == null)
			{
				returnType = this.pmodule.assemblyb.corlib_void_type;
			}
			MethodBuilder methodBuilder = new MethodBuilder(this, name, attributes, callingConvention, returnType, returnTypeRequiredCustomModifiers, returnTypeOptionalCustomModifiers, parameterTypes, parameterTypeRequiredCustomModifiers, parameterTypeOptionalCustomModifiers);
			this.append_method(methodBuilder);
			return methodBuilder;
		}

		public MethodBuilder DefinePInvokeMethod(string name, string dllName, string entryName, MethodAttributes attributes, CallingConventions callingConvention, Type returnType, Type[] parameterTypes, CallingConvention nativeCallConv, CharSet nativeCharSet)
		{
			return this.DefinePInvokeMethod(name, dllName, entryName, attributes, callingConvention, returnType, null, null, parameterTypes, null, null, nativeCallConv, nativeCharSet);
		}

		public MethodBuilder DefinePInvokeMethod(string name, string dllName, string entryName, MethodAttributes attributes, CallingConventions callingConvention, Type returnType, Type[] returnTypeRequiredCustomModifiers, Type[] returnTypeOptionalCustomModifiers, Type[] parameterTypes, Type[][] parameterTypeRequiredCustomModifiers, Type[][] parameterTypeOptionalCustomModifiers, CallingConvention nativeCallConv, CharSet nativeCharSet)
		{
			this.check_name("name", name);
			this.check_name("dllName", dllName);
			this.check_name("entryName", entryName);
			if ((attributes & MethodAttributes.Abstract) != MethodAttributes.PrivateScope)
			{
				throw new ArgumentException("PInvoke methods must be static and native and cannot be abstract.");
			}
			if (base.IsInterface)
			{
				throw new ArgumentException("PInvoke methods cannot exist on interfaces.");
			}
			this.check_not_created();
			MethodBuilder methodBuilder = new MethodBuilder(this, name, attributes, callingConvention, returnType, returnTypeRequiredCustomModifiers, returnTypeOptionalCustomModifiers, parameterTypes, parameterTypeRequiredCustomModifiers, parameterTypeOptionalCustomModifiers, dllName, entryName, nativeCallConv, nativeCharSet);
			this.append_method(methodBuilder);
			return methodBuilder;
		}

		public MethodBuilder DefinePInvokeMethod(string name, string dllName, MethodAttributes attributes, CallingConventions callingConvention, Type returnType, Type[] parameterTypes, CallingConvention nativeCallConv, CharSet nativeCharSet)
		{
			return this.DefinePInvokeMethod(name, dllName, name, attributes, callingConvention, returnType, parameterTypes, nativeCallConv, nativeCharSet);
		}

		public MethodBuilder DefineMethod(string name, MethodAttributes attributes)
		{
			return this.DefineMethod(name, attributes, CallingConventions.Standard);
		}

		public MethodBuilder DefineMethod(string name, MethodAttributes attributes, CallingConventions callingConvention)
		{
			return this.DefineMethod(name, attributes, callingConvention, null, null);
		}

		public void DefineMethodOverride(MethodInfo methodInfoBody, MethodInfo methodInfoDeclaration)
		{
			if (methodInfoBody == null)
			{
				throw new ArgumentNullException("methodInfoBody");
			}
			if (methodInfoDeclaration == null)
			{
				throw new ArgumentNullException("methodInfoDeclaration");
			}
			this.check_not_created();
			if (methodInfoBody.DeclaringType != this)
			{
				throw new ArgumentException("method body must belong to this type");
			}
			if (methodInfoBody is MethodBuilder)
			{
				((MethodBuilder)methodInfoBody).set_override(methodInfoDeclaration);
			}
		}

		public FieldBuilder DefineField(string fieldName, Type type, FieldAttributes attributes)
		{
			return this.DefineField(fieldName, type, null, null, attributes);
		}

		public FieldBuilder DefineField(string fieldName, Type type, Type[] requiredCustomModifiers, Type[] optionalCustomModifiers, FieldAttributes attributes)
		{
			this.check_name("fieldName", fieldName);
			if (type == typeof(void))
			{
				throw new ArgumentException("Bad field type in defining field.");
			}
			this.check_not_created();
			FieldBuilder fieldBuilder = new FieldBuilder(this, fieldName, type, attributes, requiredCustomModifiers, optionalCustomModifiers);
			if (this.fields != null)
			{
				if (this.fields.Length == this.num_fields)
				{
					FieldBuilder[] array = new FieldBuilder[this.fields.Length * 2];
					Array.Copy(this.fields, array, this.num_fields);
					this.fields = array;
				}
				this.fields[this.num_fields] = fieldBuilder;
				this.num_fields++;
			}
			else
			{
				this.fields = new FieldBuilder[1];
				this.fields[0] = fieldBuilder;
				this.num_fields++;
			}
			if (this.IsEnum && this.underlying_type == null && (attributes & FieldAttributes.Static) == FieldAttributes.PrivateScope)
			{
				this.underlying_type = type;
			}
			return fieldBuilder;
		}

		public PropertyBuilder DefineProperty(string name, PropertyAttributes attributes, Type returnType, Type[] parameterTypes)
		{
			return this.DefineProperty(name, attributes, (CallingConventions)0, returnType, null, null, parameterTypes, null, null);
		}

		public PropertyBuilder DefineProperty(string name, PropertyAttributes attributes, CallingConventions callingConvention, Type returnType, Type[] parameterTypes)
		{
			return this.DefineProperty(name, attributes, callingConvention, returnType, null, null, parameterTypes, null, null);
		}

		public PropertyBuilder DefineProperty(string name, PropertyAttributes attributes, Type returnType, Type[] returnTypeRequiredCustomModifiers, Type[] returnTypeOptionalCustomModifiers, Type[] parameterTypes, Type[][] parameterTypeRequiredCustomModifiers, Type[][] parameterTypeOptionalCustomModifiers)
		{
			return this.DefineProperty(name, attributes, (CallingConventions)0, returnType, returnTypeRequiredCustomModifiers, returnTypeOptionalCustomModifiers, parameterTypes, parameterTypeRequiredCustomModifiers, parameterTypeOptionalCustomModifiers);
		}

		public PropertyBuilder DefineProperty(string name, PropertyAttributes attributes, CallingConventions callingConvention, Type returnType, Type[] returnTypeRequiredCustomModifiers, Type[] returnTypeOptionalCustomModifiers, Type[] parameterTypes, Type[][] parameterTypeRequiredCustomModifiers, Type[][] parameterTypeOptionalCustomModifiers)
		{
			this.check_name("name", name);
			if (parameterTypes != null)
			{
				for (int i = 0; i < parameterTypes.Length; i++)
				{
					if (parameterTypes[i] == null)
					{
						throw new ArgumentNullException("parameterTypes");
					}
				}
			}
			this.check_not_created();
			PropertyBuilder propertyBuilder = new PropertyBuilder(this, name, attributes, callingConvention, returnType, returnTypeRequiredCustomModifiers, returnTypeOptionalCustomModifiers, parameterTypes, parameterTypeRequiredCustomModifiers, parameterTypeOptionalCustomModifiers);
			if (this.properties != null)
			{
				Array.Resize<PropertyBuilder>(ref this.properties, this.properties.Length + 1);
				this.properties[this.properties.Length - 1] = propertyBuilder;
			}
			else
			{
				this.properties = new PropertyBuilder[] { propertyBuilder };
			}
			return propertyBuilder;
		}

		[ComVisible(true)]
		public ConstructorBuilder DefineTypeInitializer()
		{
			return this.DefineConstructor(MethodAttributes.FamANDAssem | MethodAttributes.Family | MethodAttributes.Static | MethodAttributes.SpecialName | MethodAttributes.RTSpecialName, CallingConventions.Standard, null);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern TypeInfo create_runtime_class();

		private bool is_nested_in(Type t)
		{
			while (t != null)
			{
				if (t == this)
				{
					return true;
				}
				t = t.DeclaringType;
			}
			return false;
		}

		private bool has_ctor_method()
		{
			MethodAttributes methodAttributes = MethodAttributes.SpecialName | MethodAttributes.RTSpecialName;
			for (int i = 0; i < this.num_methods; i++)
			{
				MethodBuilder methodBuilder = this.methods[i];
				if (methodBuilder.Name == ConstructorInfo.ConstructorName && (methodBuilder.Attributes & methodAttributes) == methodAttributes)
				{
					return true;
				}
			}
			return false;
		}

		public Type CreateType()
		{
			return this.CreateTypeInfo();
		}

		public TypeInfo CreateTypeInfo()
		{
			if (this.createTypeCalled)
			{
				return this.created;
			}
			if (!base.IsInterface && this.parent == null && this != this.pmodule.assemblyb.corlib_object_type && this.FullName != "<Module>")
			{
				this.SetParent(this.pmodule.assemblyb.corlib_object_type);
			}
			if (this.fields != null)
			{
				foreach (FieldBuilder fieldBuilder in this.fields)
				{
					if (!(fieldBuilder == null))
					{
						Type fieldType = fieldBuilder.FieldType;
						if (!fieldBuilder.IsStatic && fieldType is TypeBuilder && fieldType.IsValueType && fieldType != this && this.is_nested_in(fieldType))
						{
							TypeBuilder typeBuilder = (TypeBuilder)fieldType;
							if (!typeBuilder.is_created)
							{
								AppDomain.CurrentDomain.DoTypeResolve(typeBuilder);
								bool is_created = typeBuilder.is_created;
							}
						}
					}
				}
			}
			if (!base.IsInterface && !base.IsValueType && this.ctors == null && this.tname != "<Module>" && ((this.GetAttributeFlagsImpl() & TypeAttributes.Abstract) | TypeAttributes.Sealed) != (TypeAttributes.Abstract | TypeAttributes.Sealed) && !this.has_ctor_method())
			{
				this.DefineDefaultConstructor(MethodAttributes.Public);
			}
			this.createTypeCalled = true;
			if (this.parent != null && this.parent.IsSealed)
			{
				throw new TypeLoadException(string.Concat(new object[] { "Could not load type '", this.FullName, "' from assembly '", this.Assembly, "' because the parent type is sealed." }));
			}
			if (this.parent == this.pmodule.assemblyb.corlib_enum_type && this.methods != null)
			{
				throw new TypeLoadException(string.Concat(new object[] { "Could not load type '", this.FullName, "' from assembly '", this.Assembly, "' because it is an enum with methods." }));
			}
			if (this.interfaces != null)
			{
				foreach (Type type in this.interfaces)
				{
					if (type.IsNestedPrivate && type.Assembly != this.Assembly)
					{
						throw new TypeLoadException(string.Concat(new object[] { "Could not load type '", this.FullName, "' from assembly '", this.Assembly, "' because it is implements the inaccessible interface '", type.FullName, "'." }));
					}
				}
			}
			if (this.methods != null)
			{
				bool flag = !base.IsAbstract;
				for (int j = 0; j < this.num_methods; j++)
				{
					MethodBuilder methodBuilder = this.methods[j];
					if (flag && methodBuilder.IsAbstract)
					{
						throw new InvalidOperationException("Type is concrete but has abstract method " + methodBuilder);
					}
					methodBuilder.check_override();
					methodBuilder.fixup();
				}
			}
			if (this.ctors != null)
			{
				ConstructorBuilder[] array3 = this.ctors;
				for (int i = 0; i < array3.Length; i++)
				{
					array3[i].fixup();
				}
			}
			this.ResolveUserTypes();
			this.created = this.create_runtime_class();
			if (this.created != null)
			{
				return this.created;
			}
			return this;
		}

		private void ResolveUserTypes()
		{
			this.parent = TypeBuilder.ResolveUserType(this.parent);
			TypeBuilder.ResolveUserTypes(this.interfaces);
			if (this.fields != null)
			{
				foreach (FieldBuilder fieldBuilder in this.fields)
				{
					if (fieldBuilder != null)
					{
						fieldBuilder.ResolveUserTypes();
					}
				}
			}
			if (this.methods != null)
			{
				foreach (MethodBuilder methodBuilder in this.methods)
				{
					if (methodBuilder != null)
					{
						methodBuilder.ResolveUserTypes();
					}
				}
			}
			if (this.ctors != null)
			{
				foreach (ConstructorBuilder constructorBuilder in this.ctors)
				{
					if (constructorBuilder != null)
					{
						constructorBuilder.ResolveUserTypes();
					}
				}
			}
		}

		internal static void ResolveUserTypes(Type[] types)
		{
			if (types != null)
			{
				for (int i = 0; i < types.Length; i++)
				{
					types[i] = TypeBuilder.ResolveUserType(types[i]);
				}
			}
		}

		internal static Type ResolveUserType(Type t)
		{
			if (!(t != null) || (!(t.GetType().Assembly != typeof(int).Assembly) && !(t is TypeDelegator)))
			{
				return t;
			}
			t = t.UnderlyingSystemType;
			if (t != null && (t.GetType().Assembly != typeof(int).Assembly || t is TypeDelegator))
			{
				throw new NotSupportedException("User defined subclasses of System.Type are not yet supported.");
			}
			return t;
		}

		internal void FixupTokens(Dictionary<int, int> token_map, Dictionary<int, MemberInfo> member_map)
		{
			if (this.methods != null)
			{
				for (int i = 0; i < this.num_methods; i++)
				{
					this.methods[i].FixupTokens(token_map, member_map);
				}
			}
			if (this.ctors != null)
			{
				ConstructorBuilder[] array = this.ctors;
				for (int j = 0; j < array.Length; j++)
				{
					array[j].FixupTokens(token_map, member_map);
				}
			}
			if (this.subtypes != null)
			{
				TypeBuilder[] array2 = this.subtypes;
				for (int j = 0; j < array2.Length; j++)
				{
					array2[j].FixupTokens(token_map, member_map);
				}
			}
		}

		internal void GenerateDebugInfo(ISymbolWriter symbolWriter)
		{
			symbolWriter.OpenNamespace(this.Namespace);
			if (this.methods != null)
			{
				for (int i = 0; i < this.num_methods; i++)
				{
					this.methods[i].GenerateDebugInfo(symbolWriter);
				}
			}
			if (this.ctors != null)
			{
				ConstructorBuilder[] array = this.ctors;
				for (int j = 0; j < array.Length; j++)
				{
					array[j].GenerateDebugInfo(symbolWriter);
				}
			}
			symbolWriter.CloseNamespace();
			if (this.subtypes != null)
			{
				for (int k = 0; k < this.subtypes.Length; k++)
				{
					this.subtypes[k].GenerateDebugInfo(symbolWriter);
				}
			}
		}

		[ComVisible(true)]
		public override ConstructorInfo[] GetConstructors(BindingFlags bindingAttr)
		{
			if (this.is_created)
			{
				return this.created.GetConstructors(bindingAttr);
			}
			throw new NotSupportedException();
		}

		internal ConstructorInfo[] GetConstructorsInternal(BindingFlags bindingAttr)
		{
			if (this.ctors == null)
			{
				return new ConstructorInfo[0];
			}
			ArrayList arrayList = new ArrayList();
			foreach (ConstructorBuilder constructorBuilder in this.ctors)
			{
				bool flag = false;
				MethodAttributes attributes = constructorBuilder.Attributes;
				if ((attributes & MethodAttributes.MemberAccessMask) == MethodAttributes.Public)
				{
					if ((bindingAttr & BindingFlags.Public) != BindingFlags.Default)
					{
						flag = true;
					}
				}
				else if ((bindingAttr & BindingFlags.NonPublic) != BindingFlags.Default)
				{
					flag = true;
				}
				if (flag)
				{
					flag = false;
					if ((attributes & MethodAttributes.Static) != MethodAttributes.PrivateScope)
					{
						if ((bindingAttr & BindingFlags.Static) != BindingFlags.Default)
						{
							flag = true;
						}
					}
					else if ((bindingAttr & BindingFlags.Instance) != BindingFlags.Default)
					{
						flag = true;
					}
					if (flag)
					{
						arrayList.Add(constructorBuilder);
					}
				}
			}
			ConstructorInfo[] array2 = new ConstructorInfo[arrayList.Count];
			arrayList.CopyTo(array2);
			return array2;
		}

		public override Type GetElementType()
		{
			throw new NotSupportedException();
		}

		public override EventInfo GetEvent(string name, BindingFlags bindingAttr)
		{
			this.check_created();
			return this.created.GetEvent(name, bindingAttr);
		}

		public override EventInfo[] GetEvents()
		{
			return this.GetEvents(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public);
		}

		public override EventInfo[] GetEvents(BindingFlags bindingAttr)
		{
			if (this.is_created)
			{
				return this.created.GetEvents(bindingAttr);
			}
			throw new NotSupportedException();
		}

		public override FieldInfo GetField(string name, BindingFlags bindingAttr)
		{
			if (this.created != null)
			{
				return this.created.GetField(name, bindingAttr);
			}
			if (this.fields == null)
			{
				return null;
			}
			foreach (FieldBuilder fieldInfo in this.fields)
			{
				if (!(fieldInfo == null) && !(fieldInfo.Name != name))
				{
					bool flag = false;
					FieldAttributes attributes = fieldInfo.Attributes;
					if ((attributes & FieldAttributes.FieldAccessMask) == FieldAttributes.Public)
					{
						if ((bindingAttr & BindingFlags.Public) != BindingFlags.Default)
						{
							flag = true;
						}
					}
					else if ((bindingAttr & BindingFlags.NonPublic) != BindingFlags.Default)
					{
						flag = true;
					}
					if (flag)
					{
						flag = false;
						if ((attributes & FieldAttributes.Static) != FieldAttributes.PrivateScope)
						{
							if ((bindingAttr & BindingFlags.Static) != BindingFlags.Default)
							{
								flag = true;
							}
						}
						else if ((bindingAttr & BindingFlags.Instance) != BindingFlags.Default)
						{
							flag = true;
						}
						if (flag)
						{
							return fieldInfo;
						}
					}
				}
			}
			return null;
		}

		public override FieldInfo[] GetFields(BindingFlags bindingAttr)
		{
			if (this.created != null)
			{
				return this.created.GetFields(bindingAttr);
			}
			if (this.fields == null)
			{
				return new FieldInfo[0];
			}
			ArrayList arrayList = new ArrayList();
			foreach (FieldBuilder fieldInfo in this.fields)
			{
				if (!(fieldInfo == null))
				{
					bool flag = false;
					FieldAttributes attributes = fieldInfo.Attributes;
					if ((attributes & FieldAttributes.FieldAccessMask) == FieldAttributes.Public)
					{
						if ((bindingAttr & BindingFlags.Public) != BindingFlags.Default)
						{
							flag = true;
						}
					}
					else if ((bindingAttr & BindingFlags.NonPublic) != BindingFlags.Default)
					{
						flag = true;
					}
					if (flag)
					{
						flag = false;
						if ((attributes & FieldAttributes.Static) != FieldAttributes.PrivateScope)
						{
							if ((bindingAttr & BindingFlags.Static) != BindingFlags.Default)
							{
								flag = true;
							}
						}
						else if ((bindingAttr & BindingFlags.Instance) != BindingFlags.Default)
						{
							flag = true;
						}
						if (flag)
						{
							arrayList.Add(fieldInfo);
						}
					}
				}
			}
			FieldInfo[] array2 = new FieldInfo[arrayList.Count];
			arrayList.CopyTo(array2);
			return array2;
		}

		public override Type GetInterface(string name, bool ignoreCase)
		{
			this.check_created();
			return this.created.GetInterface(name, ignoreCase);
		}

		public override Type[] GetInterfaces()
		{
			if (this.is_created)
			{
				return this.created.GetInterfaces();
			}
			if (this.interfaces != null)
			{
				Type[] array = new Type[this.interfaces.Length];
				this.interfaces.CopyTo(array, 0);
				return array;
			}
			return Type.EmptyTypes;
		}

		public override MemberInfo[] GetMember(string name, MemberTypes type, BindingFlags bindingAttr)
		{
			this.check_created();
			return this.created.GetMember(name, type, bindingAttr);
		}

		public override MemberInfo[] GetMembers(BindingFlags bindingAttr)
		{
			this.check_created();
			return this.created.GetMembers(bindingAttr);
		}

		private MethodInfo[] GetMethodsByName(string name, BindingFlags bindingAttr, bool ignoreCase, Type reflected_type)
		{
			MethodInfo[] array2;
			if ((bindingAttr & BindingFlags.DeclaredOnly) == BindingFlags.Default && this.parent != null)
			{
				MethodInfo[] array = this.parent.GetMethods(bindingAttr);
				ArrayList arrayList = new ArrayList(array.Length);
				bool flag = (bindingAttr & BindingFlags.FlattenHierarchy) > BindingFlags.Default;
				foreach (MethodInfo methodInfo in array)
				{
					MethodAttributes methodAttributes = methodInfo.Attributes;
					if (!methodInfo.IsStatic || flag)
					{
						MethodAttributes methodAttributes2 = methodAttributes & MethodAttributes.MemberAccessMask;
						bool flag2;
						if (methodAttributes2 != MethodAttributes.Private)
						{
							if (methodAttributes2 != MethodAttributes.Assembly)
							{
								if (methodAttributes2 == MethodAttributes.Public)
								{
									flag2 = (bindingAttr & BindingFlags.Public) > BindingFlags.Default;
								}
								else
								{
									flag2 = (bindingAttr & BindingFlags.NonPublic) > BindingFlags.Default;
								}
							}
							else
							{
								flag2 = (bindingAttr & BindingFlags.NonPublic) > BindingFlags.Default;
							}
						}
						else
						{
							flag2 = false;
						}
						if (flag2)
						{
							arrayList.Add(methodInfo);
						}
					}
				}
				if (this.methods == null)
				{
					array2 = new MethodInfo[arrayList.Count];
					arrayList.CopyTo(array2);
				}
				else
				{
					array2 = new MethodInfo[this.methods.Length + arrayList.Count];
					arrayList.CopyTo(array2, 0);
					this.methods.CopyTo(array2, arrayList.Count);
				}
			}
			else
			{
				array2 = this.methods;
			}
			if (array2 == null)
			{
				return new MethodInfo[0];
			}
			ArrayList arrayList2 = new ArrayList();
			foreach (MethodInfo methodInfo2 in array2)
			{
				if (!(methodInfo2 == null) && (name == null || string.Compare(methodInfo2.Name, name, ignoreCase) == 0))
				{
					bool flag2 = false;
					MethodAttributes methodAttributes = methodInfo2.Attributes;
					if ((methodAttributes & MethodAttributes.MemberAccessMask) == MethodAttributes.Public)
					{
						if ((bindingAttr & BindingFlags.Public) != BindingFlags.Default)
						{
							flag2 = true;
						}
					}
					else if ((bindingAttr & BindingFlags.NonPublic) != BindingFlags.Default)
					{
						flag2 = true;
					}
					if (flag2)
					{
						flag2 = false;
						if ((methodAttributes & MethodAttributes.Static) != MethodAttributes.PrivateScope)
						{
							if ((bindingAttr & BindingFlags.Static) != BindingFlags.Default)
							{
								flag2 = true;
							}
						}
						else if ((bindingAttr & BindingFlags.Instance) != BindingFlags.Default)
						{
							flag2 = true;
						}
						if (flag2)
						{
							arrayList2.Add(methodInfo2);
						}
					}
				}
			}
			MethodInfo[] array4 = new MethodInfo[arrayList2.Count];
			arrayList2.CopyTo(array4);
			return array4;
		}

		public override MethodInfo[] GetMethods(BindingFlags bindingAttr)
		{
			return this.GetMethodsByName(null, bindingAttr, false, this);
		}

		protected override MethodInfo GetMethodImpl(string name, BindingFlags bindingAttr, Binder binder, CallingConventions callConvention, Type[] types, ParameterModifier[] modifiers)
		{
			this.check_created();
			if (types == null)
			{
				return this.created.GetMethod(name, bindingAttr);
			}
			return this.created.GetMethod(name, bindingAttr, binder, callConvention, types, modifiers);
		}

		public override Type GetNestedType(string name, BindingFlags bindingAttr)
		{
			this.check_created();
			if (this.subtypes == null)
			{
				return null;
			}
			foreach (TypeBuilder typeBuilder in this.subtypes)
			{
				if (typeBuilder.is_created)
				{
					if ((typeBuilder.attrs & TypeAttributes.VisibilityMask) == TypeAttributes.NestedPublic)
					{
						if ((bindingAttr & BindingFlags.Public) == BindingFlags.Default)
						{
							goto IL_0055;
						}
					}
					else if ((bindingAttr & BindingFlags.NonPublic) == BindingFlags.Default)
					{
						goto IL_0055;
					}
					if (typeBuilder.Name == name)
					{
						return typeBuilder.created;
					}
				}
				IL_0055:;
			}
			return null;
		}

		public override Type[] GetNestedTypes(BindingFlags bindingAttr)
		{
			if (!this.is_created)
			{
				throw new NotSupportedException();
			}
			ArrayList arrayList = new ArrayList();
			if (this.subtypes == null)
			{
				return Type.EmptyTypes;
			}
			foreach (TypeBuilder typeBuilder in this.subtypes)
			{
				bool flag = false;
				if ((typeBuilder.attrs & TypeAttributes.VisibilityMask) == TypeAttributes.NestedPublic)
				{
					if ((bindingAttr & BindingFlags.Public) != BindingFlags.Default)
					{
						flag = true;
					}
				}
				else if ((bindingAttr & BindingFlags.NonPublic) != BindingFlags.Default)
				{
					flag = true;
				}
				if (flag)
				{
					arrayList.Add(typeBuilder);
				}
			}
			Type[] array2 = new Type[arrayList.Count];
			arrayList.CopyTo(array2);
			return array2;
		}

		public override PropertyInfo[] GetProperties(BindingFlags bindingAttr)
		{
			if (this.is_created)
			{
				return this.created.GetProperties(bindingAttr);
			}
			if (this.properties == null)
			{
				return new PropertyInfo[0];
			}
			ArrayList arrayList = new ArrayList();
			foreach (PropertyBuilder propertyInfo in this.properties)
			{
				bool flag = false;
				MethodInfo methodInfo = propertyInfo.GetGetMethod(true);
				if (methodInfo == null)
				{
					methodInfo = propertyInfo.GetSetMethod(true);
				}
				if (!(methodInfo == null))
				{
					MethodAttributes attributes = methodInfo.Attributes;
					if ((attributes & MethodAttributes.MemberAccessMask) == MethodAttributes.Public)
					{
						if ((bindingAttr & BindingFlags.Public) != BindingFlags.Default)
						{
							flag = true;
						}
					}
					else if ((bindingAttr & BindingFlags.NonPublic) != BindingFlags.Default)
					{
						flag = true;
					}
					if (flag)
					{
						flag = false;
						if ((attributes & MethodAttributes.Static) != MethodAttributes.PrivateScope)
						{
							if ((bindingAttr & BindingFlags.Static) != BindingFlags.Default)
							{
								flag = true;
							}
						}
						else if ((bindingAttr & BindingFlags.Instance) != BindingFlags.Default)
						{
							flag = true;
						}
						if (flag)
						{
							arrayList.Add(propertyInfo);
						}
					}
				}
			}
			PropertyInfo[] array2 = new PropertyInfo[arrayList.Count];
			arrayList.CopyTo(array2);
			return array2;
		}

		protected override PropertyInfo GetPropertyImpl(string name, BindingFlags bindingAttr, Binder binder, Type returnType, Type[] types, ParameterModifier[] modifiers)
		{
			throw this.not_supported();
		}

		protected override bool HasElementTypeImpl()
		{
			return this.is_created && this.created.HasElementType;
		}

		public override object InvokeMember(string name, BindingFlags invokeAttr, Binder binder, object target, object[] args, ParameterModifier[] modifiers, CultureInfo culture, string[] namedParameters)
		{
			this.check_created();
			return this.created.InvokeMember(name, invokeAttr, binder, target, args, modifiers, culture, namedParameters);
		}

		protected override bool IsArrayImpl()
		{
			return false;
		}

		protected override bool IsByRefImpl()
		{
			return false;
		}

		protected override bool IsCOMObjectImpl()
		{
			return (this.GetAttributeFlagsImpl() & TypeAttributes.Import) > TypeAttributes.NotPublic;
		}

		protected override bool IsPointerImpl()
		{
			return false;
		}

		protected override bool IsPrimitiveImpl()
		{
			return false;
		}

		protected override bool IsValueTypeImpl()
		{
			if (this == this.pmodule.assemblyb.corlib_value_type || this == this.pmodule.assemblyb.corlib_enum_type)
			{
				return false;
			}
			Type baseType = this.parent;
			while (baseType != null)
			{
				if (baseType == this.pmodule.assemblyb.corlib_value_type)
				{
					return true;
				}
				baseType = baseType.BaseType;
			}
			return false;
		}

		public override Type MakeArrayType()
		{
			return new ArrayType(this, 0);
		}

		public override Type MakeArrayType(int rank)
		{
			if (rank < 1)
			{
				throw new IndexOutOfRangeException();
			}
			return new ArrayType(this, rank);
		}

		public override Type MakeByRefType()
		{
			return new ByRefType(this);
		}

		public override Type MakeGenericType(params Type[] typeArguments)
		{
			if (!this.IsGenericTypeDefinition)
			{
				throw new InvalidOperationException("not a generic type definition");
			}
			if (typeArguments == null)
			{
				throw new ArgumentNullException("typeArguments");
			}
			if (this.generic_params.Length != typeArguments.Length)
			{
				throw new ArgumentException(string.Format("The type or method has {0} generic parameter(s) but {1} generic argument(s) where provided. A generic argument must be provided for each generic parameter.", this.generic_params.Length, typeArguments.Length), "typeArguments");
			}
			for (int i = 0; i < typeArguments.Length; i++)
			{
				if (typeArguments[i] == null)
				{
					throw new ArgumentNullException("typeArguments");
				}
			}
			Type[] array = new Type[typeArguments.Length];
			typeArguments.CopyTo(array, 0);
			return this.pmodule.assemblyb.MakeGenericType(this, array);
		}

		public override Type MakePointerType()
		{
			return new PointerType(this);
		}

		public override RuntimeTypeHandle TypeHandle
		{
			get
			{
				this.check_created();
				return this.created.TypeHandle;
			}
		}

		public void SetCustomAttribute(CustomAttributeBuilder customBuilder)
		{
			if (customBuilder == null)
			{
				throw new ArgumentNullException("customBuilder");
			}
			string fullName = customBuilder.Ctor.ReflectedType.FullName;
			if (fullName == "System.Runtime.InteropServices.StructLayoutAttribute")
			{
				byte[] data = customBuilder.Data;
				int num = (int)data[2] | ((int)data[3] << 8);
				this.attrs &= ~TypeAttributes.LayoutMask;
				switch (num)
				{
				case 0:
					this.attrs |= TypeAttributes.SequentialLayout;
					goto IL_00A5;
				case 2:
					this.attrs |= TypeAttributes.ExplicitLayout;
					goto IL_00A5;
				case 3:
					this.attrs |= TypeAttributes.NotPublic;
					goto IL_00A5;
				}
				throw new Exception("Error in customattr");
				IL_00A5:
				Type type = ((customBuilder.Ctor is ConstructorBuilder) ? ((ConstructorBuilder)customBuilder.Ctor).parameters[0] : customBuilder.Ctor.GetParametersInternal()[0].ParameterType);
				int num2 = 6;
				if (type.FullName == "System.Int16")
				{
					num2 = 4;
				}
				int num3 = (int)data[num2++];
				num3 |= (int)data[num2++] << 8;
				for (int i = 0; i < num3; i++)
				{
					num2++;
					int num4;
					if (data[num2++] == 85)
					{
						num4 = CustomAttributeBuilder.decode_len(data, num2, out num2);
						CustomAttributeBuilder.string_from_bytes(data, num2, num4);
						num2 += num4;
					}
					num4 = CustomAttributeBuilder.decode_len(data, num2, out num2);
					string text = CustomAttributeBuilder.string_from_bytes(data, num2, num4);
					num2 += num4;
					int num5 = (int)data[num2++];
					num5 |= (int)data[num2++] << 8;
					num5 |= (int)data[num2++] << 16;
					num5 |= (int)data[num2++] << 24;
					if (!(text == "CharSet"))
					{
						if (!(text == "Pack"))
						{
							if (text == "Size")
							{
								this.class_size = num5;
							}
						}
						else
						{
							this.packing_size = (PackingSize)num5;
						}
					}
					else
					{
						switch (num5)
						{
						case 1:
						case 2:
							this.attrs &= ~TypeAttributes.StringFormatMask;
							break;
						case 3:
							this.attrs &= ~TypeAttributes.AutoClass;
							this.attrs |= TypeAttributes.UnicodeClass;
							break;
						case 4:
							this.attrs &= ~TypeAttributes.UnicodeClass;
							this.attrs |= TypeAttributes.AutoClass;
							break;
						}
					}
				}
				return;
			}
			if (fullName == "System.Runtime.CompilerServices.SpecialNameAttribute")
			{
				this.attrs |= TypeAttributes.SpecialName;
				return;
			}
			if (fullName == "System.SerializableAttribute")
			{
				this.attrs |= TypeAttributes.Serializable;
				return;
			}
			if (fullName == "System.Runtime.InteropServices.ComImportAttribute")
			{
				this.attrs |= TypeAttributes.Import;
				return;
			}
			if (fullName == "System.Security.SuppressUnmanagedCodeSecurityAttribute")
			{
				this.attrs |= TypeAttributes.HasSecurity;
			}
			if (this.cattrs != null)
			{
				CustomAttributeBuilder[] array = new CustomAttributeBuilder[this.cattrs.Length + 1];
				this.cattrs.CopyTo(array, 0);
				array[this.cattrs.Length] = customBuilder;
				this.cattrs = array;
				return;
			}
			this.cattrs = new CustomAttributeBuilder[1];
			this.cattrs[0] = customBuilder;
		}

		[ComVisible(true)]
		public void SetCustomAttribute(ConstructorInfo con, byte[] binaryAttribute)
		{
			this.SetCustomAttribute(new CustomAttributeBuilder(con, binaryAttribute));
		}

		public EventBuilder DefineEvent(string name, EventAttributes attributes, Type eventtype)
		{
			this.check_name("name", name);
			if (eventtype == null)
			{
				throw new ArgumentNullException("type");
			}
			this.check_not_created();
			EventBuilder eventBuilder = new EventBuilder(this, name, attributes, eventtype);
			if (this.events != null)
			{
				EventBuilder[] array = new EventBuilder[this.events.Length + 1];
				Array.Copy(this.events, array, this.events.Length);
				array[this.events.Length] = eventBuilder;
				this.events = array;
			}
			else
			{
				this.events = new EventBuilder[1];
				this.events[0] = eventBuilder;
			}
			return eventBuilder;
		}

		public FieldBuilder DefineInitializedData(string name, byte[] data, FieldAttributes attributes)
		{
			if (data == null)
			{
				throw new ArgumentNullException("data");
			}
			FieldBuilder fieldBuilder = this.DefineUninitializedData(name, data.Length, attributes);
			fieldBuilder.SetRVAData(data);
			return fieldBuilder;
		}

		public FieldBuilder DefineUninitializedData(string name, int size, FieldAttributes attributes)
		{
			if (name == null)
			{
				throw new ArgumentNullException("name");
			}
			if (name.Length == 0)
			{
				throw new ArgumentException("Empty name is not legal", "name");
			}
			if (size <= 0 || size > 4128768)
			{
				throw new ArgumentException("Data size must be > 0 and < 0x3f0000");
			}
			this.check_not_created();
			string text = "$ArrayType$" + size;
			TypeIdentifier typeIdentifier = TypeIdentifiers.WithoutEscape(text);
			Type type = this.pmodule.GetRegisteredType(this.fullname.NestedName(typeIdentifier));
			if (type == null)
			{
				TypeBuilder typeBuilder = this.DefineNestedType(text, TypeAttributes.Public | TypeAttributes.NestedPublic | TypeAttributes.ExplicitLayout | TypeAttributes.Sealed, this.pmodule.assemblyb.corlib_value_type, null, PackingSize.Size1, size);
				typeBuilder.CreateType();
				type = typeBuilder;
			}
			return this.DefineField(name, type, attributes | FieldAttributes.Static | FieldAttributes.HasFieldRVA);
		}

		public TypeToken TypeToken
		{
			get
			{
				return new TypeToken(33554432 | this.table_idx);
			}
		}

		public void SetParent(Type parent)
		{
			this.check_not_created();
			if (parent == null)
			{
				if ((this.attrs & TypeAttributes.ClassSemanticsMask) != TypeAttributes.NotPublic)
				{
					if ((this.attrs & TypeAttributes.Abstract) == TypeAttributes.NotPublic)
					{
						throw new InvalidOperationException("Interface must be declared abstract.");
					}
					this.parent = null;
				}
				else
				{
					this.parent = typeof(object);
				}
			}
			else
			{
				this.parent = parent;
			}
			this.parent = TypeBuilder.ResolveUserType(this.parent);
		}

		internal int get_next_table_index(object obj, int table, bool inc)
		{
			return this.pmodule.get_next_table_index(obj, table, inc);
		}

		[ComVisible(true)]
		public override InterfaceMapping GetInterfaceMap(Type interfaceType)
		{
			if (this.created == null)
			{
				throw new NotSupportedException("This method is not implemented for incomplete types.");
			}
			return this.created.GetInterfaceMap(interfaceType);
		}

		internal override Type InternalResolve()
		{
			this.check_created();
			return this.created;
		}

		internal override Type RuntimeResolve()
		{
			this.check_created();
			return this.created;
		}

		internal bool is_created
		{
			get
			{
				return this.createTypeCalled;
			}
		}

		private Exception not_supported()
		{
			return new NotSupportedException("The invoked member is not supported in a dynamic module.");
		}

		private void check_not_created()
		{
			if (this.is_created)
			{
				throw new InvalidOperationException("Unable to change after type has been created.");
			}
		}

		private void check_created()
		{
			if (!this.is_created)
			{
				throw this.not_supported();
			}
		}

		private void check_name(string argName, string name)
		{
			if (name == null)
			{
				throw new ArgumentNullException(argName);
			}
			if (name.Length == 0)
			{
				throw new ArgumentException("Empty name is not legal", argName);
			}
			if (name[0] == '\0')
			{
				throw new ArgumentException("Illegal name", argName);
			}
		}

		public override string ToString()
		{
			return this.FullName;
		}

		[MonoTODO]
		public override bool IsAssignableFrom(Type c)
		{
			return base.IsAssignableFrom(c);
		}

		[MonoTODO("arrays")]
		internal bool IsAssignableTo(Type c)
		{
			if (c == this)
			{
				return true;
			}
			if (c.IsInterface)
			{
				if (this.parent != null && this.is_created && c.IsAssignableFrom(this.parent))
				{
					return true;
				}
				if (this.interfaces == null)
				{
					return false;
				}
				foreach (Type type in this.interfaces)
				{
					if (c.IsAssignableFrom(type))
					{
						return true;
					}
				}
				if (!this.is_created)
				{
					return false;
				}
			}
			if (this.parent == null)
			{
				return c == typeof(object);
			}
			return c.IsAssignableFrom(this.parent);
		}

		public bool IsCreated()
		{
			return this.is_created;
		}

		public override Type[] GetGenericArguments()
		{
			if (this.generic_params == null)
			{
				return null;
			}
			Type[] array = new Type[this.generic_params.Length];
			this.generic_params.CopyTo(array, 0);
			return array;
		}

		public override Type GetGenericTypeDefinition()
		{
			if (this.generic_params == null)
			{
				throw new InvalidOperationException("Type is not generic");
			}
			return this;
		}

		public override bool ContainsGenericParameters
		{
			get
			{
				return this.generic_params != null;
			}
		}

		public override bool IsGenericParameter
		{
			get
			{
				return false;
			}
		}

		public override GenericParameterAttributes GenericParameterAttributes
		{
			get
			{
				return GenericParameterAttributes.None;
			}
		}

		public override bool IsGenericTypeDefinition
		{
			get
			{
				return this.generic_params != null;
			}
		}

		public override bool IsGenericType
		{
			get
			{
				return this.IsGenericTypeDefinition;
			}
		}

		[MonoTODO]
		public override int GenericParameterPosition
		{
			get
			{
				return 0;
			}
		}

		public override MethodBase DeclaringMethod
		{
			get
			{
				return null;
			}
		}

		public GenericTypeParameterBuilder[] DefineGenericParameters(params string[] names)
		{
			if (names == null)
			{
				throw new ArgumentNullException("names");
			}
			if (names.Length == 0)
			{
				throw new ArgumentException("names");
			}
			this.generic_params = new GenericTypeParameterBuilder[names.Length];
			for (int i = 0; i < names.Length; i++)
			{
				string text = names[i];
				if (text == null)
				{
					throw new ArgumentNullException("names");
				}
				this.generic_params[i] = new GenericTypeParameterBuilder(this, null, text, i);
			}
			return this.generic_params;
		}

		public static ConstructorInfo GetConstructor(Type type, ConstructorInfo constructor)
		{
			if (type == null)
			{
				throw new ArgumentException("Type is not generic", "type");
			}
			if (!type.IsGenericType)
			{
				throw new ArgumentException("Type is not a generic type", "type");
			}
			if (type.IsGenericTypeDefinition)
			{
				throw new ArgumentException("Type cannot be a generic type definition", "type");
			}
			if (constructor == null)
			{
				throw new NullReferenceException();
			}
			if (!constructor.DeclaringType.IsGenericTypeDefinition)
			{
				throw new ArgumentException("constructor declaring type is not a generic type definition", "constructor");
			}
			if (constructor.DeclaringType != type.GetGenericTypeDefinition())
			{
				throw new ArgumentException("constructor declaring type is not the generic type definition of type", "constructor");
			}
			ConstructorInfo constructor2 = type.GetConstructor(constructor);
			if (constructor2 == null)
			{
				throw new ArgumentException("constructor not found");
			}
			return constructor2;
		}

		private static bool IsValidGetMethodType(Type type)
		{
			if (type is TypeBuilder || type is TypeBuilderInstantiation)
			{
				return true;
			}
			if (type.Module is ModuleBuilder)
			{
				return true;
			}
			if (type.IsGenericParameter)
			{
				return false;
			}
			Type[] genericArguments = type.GetGenericArguments();
			if (genericArguments == null)
			{
				return false;
			}
			for (int i = 0; i < genericArguments.Length; i++)
			{
				if (TypeBuilder.IsValidGetMethodType(genericArguments[i]))
				{
					return true;
				}
			}
			return false;
		}

		public static MethodInfo GetMethod(Type type, MethodInfo method)
		{
			if (!TypeBuilder.IsValidGetMethodType(type))
			{
				throw new ArgumentException("type is not TypeBuilder but " + type.GetType(), "type");
			}
			if (type is TypeBuilder && type.ContainsGenericParameters)
			{
				type = type.MakeGenericType(type.GetGenericArguments());
			}
			if (!type.IsGenericType)
			{
				throw new ArgumentException("type is not a generic type", "type");
			}
			if (!method.DeclaringType.IsGenericTypeDefinition)
			{
				throw new ArgumentException("method declaring type is not a generic type definition", "method");
			}
			if (method.DeclaringType != type.GetGenericTypeDefinition())
			{
				throw new ArgumentException("method declaring type is not the generic type definition of type", "method");
			}
			if (method == null)
			{
				throw new NullReferenceException();
			}
			MethodInfo method2 = type.GetMethod(method);
			if (method2 == null)
			{
				throw new ArgumentException(string.Format("method {0} not found in type {1}", method.Name, type));
			}
			return method2;
		}

		public static FieldInfo GetField(Type type, FieldInfo field)
		{
			if (!type.IsGenericType)
			{
				throw new ArgumentException("Type is not a generic type", "type");
			}
			if (type.IsGenericTypeDefinition)
			{
				throw new ArgumentException("Type cannot be a generic type definition", "type");
			}
			if (field is FieldOnTypeBuilderInst)
			{
				throw new ArgumentException("The specified field must be declared on a generic type definition.", "field");
			}
			if (field.DeclaringType != type.GetGenericTypeDefinition())
			{
				throw new ArgumentException("field declaring type is not the generic type definition of type", "method");
			}
			FieldInfo field2 = type.GetField(field);
			if (field2 == null)
			{
				throw new Exception("field not found");
			}
			return field2;
		}

		void _TypeBuilder.GetIDsOfNames([In] ref Guid riid, IntPtr rgszNames, uint cNames, uint lcid, IntPtr rgDispId)
		{
			throw new NotImplementedException();
		}

		void _TypeBuilder.GetTypeInfo(uint iTInfo, uint lcid, IntPtr ppTInfo)
		{
			throw new NotImplementedException();
		}

		void _TypeBuilder.GetTypeInfoCount(out uint pcTInfo)
		{
			throw new NotImplementedException();
		}

		void _TypeBuilder.Invoke(uint dispIdMember, [In] ref Guid riid, uint lcid, short wFlags, IntPtr pDispParams, IntPtr pVarResult, IntPtr pExcepInfo, IntPtr puArgErr)
		{
			throw new NotImplementedException();
		}

		internal override bool IsUserType
		{
			get
			{
				return false;
			}
		}

		public override bool IsConstructedGenericType
		{
			get
			{
				return false;
			}
		}

		public override bool IsAssignableFrom(TypeInfo typeInfo)
		{
			return base.IsAssignableFrom(typeInfo);
		}

		internal TypeBuilder()
		{
			ThrowStub.ThrowNotSupportedException();
		}

		private string tname;

		private string nspace;

		private Type parent;

		private Type nesting_type;

		internal Type[] interfaces;

		internal int num_methods;

		internal MethodBuilder[] methods;

		internal ConstructorBuilder[] ctors;

		internal PropertyBuilder[] properties;

		internal int num_fields;

		internal FieldBuilder[] fields;

		internal EventBuilder[] events;

		private CustomAttributeBuilder[] cattrs;

		internal TypeBuilder[] subtypes;

		internal TypeAttributes attrs;

		private int table_idx;

		private ModuleBuilder pmodule;

		private int class_size;

		private PackingSize packing_size;

		private IntPtr generic_container;

		private GenericTypeParameterBuilder[] generic_params;

		private RefEmitPermissionSet[] permissions;

		private TypeInfo created;

		private int state;

		private TypeName fullname;

		private bool createTypeCalled;

		private Type underlying_type;

		public const int UnspecifiedTypeSize = 0;
	}
}
