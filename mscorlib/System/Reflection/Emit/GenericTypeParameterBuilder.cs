using System;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace System.Reflection.Emit
{
	[ComVisible(true)]
	public sealed class GenericTypeParameterBuilder : Type
	{
		internal GenericTypeParameterBuilder(TypeBuilder tbuilder, MethodBuilder mbuilder, string name, int index)
		{
			this.tbuilder = tbuilder;
			this.mbuilder = mbuilder;
			this.name = name;
			this.index = index;
			this.initialize();
		}

		public void SetBaseTypeConstraint(Type baseTypeConstraint)
		{
			this.base_type = baseTypeConstraint ?? typeof(object);
		}

		[ComVisible(true)]
		public void SetInterfaceConstraints(params Type[] interfaceConstraints)
		{
			this.iface_constraints = interfaceConstraints;
		}

		public void SetGenericParameterAttributes(GenericParameterAttributes genericParameterAttributes)
		{
			this.attrs = genericParameterAttributes;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void initialize();

		[ComVisible(true)]
		public override bool IsSubclassOf(Type c)
		{
			if (!((ModuleBuilder)this.tbuilder.Module).assemblyb.IsCompilerContext)
			{
				throw this.not_supported();
			}
			return this.BaseType != null && (this.BaseType == c || this.BaseType.IsSubclassOf(c));
		}

		protected override TypeAttributes GetAttributeFlagsImpl()
		{
			if (((ModuleBuilder)this.tbuilder.Module).assemblyb.IsCompilerContext)
			{
				return TypeAttributes.Public;
			}
			throw this.not_supported();
		}

		protected override ConstructorInfo GetConstructorImpl(BindingFlags bindingAttr, Binder binder, CallingConventions callConvention, Type[] types, ParameterModifier[] modifiers)
		{
			throw this.not_supported();
		}

		[ComVisible(true)]
		public override ConstructorInfo[] GetConstructors(BindingFlags bindingAttr)
		{
			throw this.not_supported();
		}

		public override EventInfo GetEvent(string name, BindingFlags bindingAttr)
		{
			throw this.not_supported();
		}

		public override EventInfo[] GetEvents()
		{
			throw this.not_supported();
		}

		public override EventInfo[] GetEvents(BindingFlags bindingAttr)
		{
			throw this.not_supported();
		}

		public override FieldInfo GetField(string name, BindingFlags bindingAttr)
		{
			throw this.not_supported();
		}

		public override FieldInfo[] GetFields(BindingFlags bindingAttr)
		{
			throw this.not_supported();
		}

		public override Type GetInterface(string name, bool ignoreCase)
		{
			throw this.not_supported();
		}

		public override Type[] GetInterfaces()
		{
			throw this.not_supported();
		}

		public override MemberInfo[] GetMembers(BindingFlags bindingAttr)
		{
			throw this.not_supported();
		}

		public override MemberInfo[] GetMember(string name, MemberTypes type, BindingFlags bindingAttr)
		{
			throw this.not_supported();
		}

		public override MethodInfo[] GetMethods(BindingFlags bindingAttr)
		{
			throw this.not_supported();
		}

		protected override MethodInfo GetMethodImpl(string name, BindingFlags bindingAttr, Binder binder, CallingConventions callConvention, Type[] types, ParameterModifier[] modifiers)
		{
			throw this.not_supported();
		}

		public override Type GetNestedType(string name, BindingFlags bindingAttr)
		{
			throw this.not_supported();
		}

		public override Type[] GetNestedTypes(BindingFlags bindingAttr)
		{
			throw this.not_supported();
		}

		public override PropertyInfo[] GetProperties(BindingFlags bindingAttr)
		{
			throw this.not_supported();
		}

		protected override PropertyInfo GetPropertyImpl(string name, BindingFlags bindingAttr, Binder binder, Type returnType, Type[] types, ParameterModifier[] modifiers)
		{
			throw this.not_supported();
		}

		protected override bool HasElementTypeImpl()
		{
			return false;
		}

		public override bool IsAssignableFrom(Type c)
		{
			throw this.not_supported();
		}

		public override bool IsInstanceOfType(object o)
		{
			throw this.not_supported();
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
			return false;
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
			return this.base_type != null && this.base_type.IsValueType;
		}

		public override object InvokeMember(string name, BindingFlags invokeAttr, Binder binder, object target, object[] args, ParameterModifier[] modifiers, CultureInfo culture, string[] namedParameters)
		{
			throw this.not_supported();
		}

		public override Type GetElementType()
		{
			throw this.not_supported();
		}

		public override Type UnderlyingSystemType
		{
			get
			{
				return this;
			}
		}

		public override Assembly Assembly
		{
			get
			{
				return this.tbuilder.Assembly;
			}
		}

		public override string AssemblyQualifiedName
		{
			get
			{
				return null;
			}
		}

		public override Type BaseType
		{
			get
			{
				return this.base_type;
			}
		}

		public override string FullName
		{
			get
			{
				return null;
			}
		}

		public override Guid GUID
		{
			get
			{
				throw this.not_supported();
			}
		}

		public override bool IsDefined(Type attributeType, bool inherit)
		{
			throw this.not_supported();
		}

		public override object[] GetCustomAttributes(bool inherit)
		{
			throw this.not_supported();
		}

		public override object[] GetCustomAttributes(Type attributeType, bool inherit)
		{
			throw this.not_supported();
		}

		[ComVisible(true)]
		public override InterfaceMapping GetInterfaceMap(Type interfaceType)
		{
			throw this.not_supported();
		}

		public override string Name
		{
			get
			{
				return this.name;
			}
		}

		public override string Namespace
		{
			get
			{
				return null;
			}
		}

		public override Module Module
		{
			get
			{
				return this.tbuilder.Module;
			}
		}

		public override Type DeclaringType
		{
			get
			{
				return (this.mbuilder == null) ? this.tbuilder : this.mbuilder.DeclaringType;
			}
		}

		public override Type ReflectedType
		{
			get
			{
				return this.DeclaringType;
			}
		}

		public override RuntimeTypeHandle TypeHandle
		{
			get
			{
				throw this.not_supported();
			}
		}

		public override Type[] GetGenericArguments()
		{
			throw new InvalidOperationException();
		}

		public override Type GetGenericTypeDefinition()
		{
			throw new InvalidOperationException();
		}

		public override bool ContainsGenericParameters
		{
			get
			{
				return true;
			}
		}

		public override bool IsGenericParameter
		{
			get
			{
				return true;
			}
		}

		public override bool IsGenericType
		{
			get
			{
				return false;
			}
		}

		public override bool IsGenericTypeDefinition
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
				if (((ModuleBuilder)this.tbuilder.Module).assemblyb.IsCompilerContext)
				{
					return this.attrs;
				}
				throw new NotSupportedException();
			}
		}

		public override int GenericParameterPosition
		{
			get
			{
				return this.index;
			}
		}

		public override Type[] GetGenericParameterConstraints()
		{
			if (!((ModuleBuilder)this.tbuilder.Module).assemblyb.IsCompilerContext)
			{
				throw new InvalidOperationException();
			}
			if (this.base_type == null)
			{
				if (this.iface_constraints != null)
				{
					return this.iface_constraints;
				}
				return Type.EmptyTypes;
			}
			else
			{
				if (this.iface_constraints == null)
				{
					return new Type[] { this.base_type };
				}
				Type[] array = new Type[this.iface_constraints.Length + 1];
				array[0] = this.base_type;
				this.iface_constraints.CopyTo(array, 1);
				return array;
			}
		}

		public override MethodBase DeclaringMethod
		{
			get
			{
				return this.mbuilder;
			}
		}

		public void SetCustomAttribute(CustomAttributeBuilder customBuilder)
		{
			if (customBuilder == null)
			{
				throw new ArgumentNullException("customBuilder");
			}
			if (this.cattrs != null)
			{
				CustomAttributeBuilder[] array = new CustomAttributeBuilder[this.cattrs.Length + 1];
				this.cattrs.CopyTo(array, 0);
				array[this.cattrs.Length] = customBuilder;
				this.cattrs = array;
			}
			else
			{
				this.cattrs = new CustomAttributeBuilder[1];
				this.cattrs[0] = customBuilder;
			}
		}

		[MonoTODO("unverified implementation")]
		public void SetCustomAttribute(ConstructorInfo con, byte[] binaryAttribute)
		{
			this.SetCustomAttribute(new CustomAttributeBuilder(con, binaryAttribute));
		}

		private Exception not_supported()
		{
			return new NotSupportedException();
		}

		public override string ToString()
		{
			return this.name;
		}

		[MonoTODO]
		public override bool Equals(object o)
		{
			return base.Equals(o);
		}

		[MonoTODO]
		public override int GetHashCode()
		{
			return base.GetHashCode();
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

		[MonoTODO]
		public override Type MakeGenericType(params Type[] typeArguments)
		{
			return base.MakeGenericType(typeArguments);
		}

		public override Type MakePointerType()
		{
			return new PointerType(this);
		}

		private TypeBuilder tbuilder;

		private MethodBuilder mbuilder;

		private string name;

		private int index;

		private Type base_type;

		private Type[] iface_constraints;

		private CustomAttributeBuilder[] cattrs;

		private GenericParameterAttributes attrs;
	}
}
