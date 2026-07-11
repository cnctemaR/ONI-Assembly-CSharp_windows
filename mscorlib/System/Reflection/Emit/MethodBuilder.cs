using System;
using System.Diagnostics.SymbolStore;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Permissions;
using System.Text;

namespace System.Reflection.Emit
{
	[ComDefaultInterface(typeof(_MethodBuilder))]
	[ComVisible(true)]
	[ClassInterface(ClassInterfaceType.None)]
	public sealed class MethodBuilder : MethodInfo, _MethodBuilder
	{
		internal MethodBuilder(TypeBuilder tb, string name, MethodAttributes attributes, CallingConventions callingConvention, Type returnType, Type[] returnModReq, Type[] returnModOpt, Type[] parameterTypes, Type[][] paramModReq, Type[][] paramModOpt)
		{
			this.name = name;
			this.attrs = attributes;
			this.call_conv = callingConvention;
			this.rtype = returnType;
			this.returnModReq = returnModReq;
			this.returnModOpt = returnModOpt;
			this.paramModReq = paramModReq;
			this.paramModOpt = paramModOpt;
			if ((attributes & MethodAttributes.Static) == MethodAttributes.PrivateScope)
			{
				this.call_conv |= CallingConventions.HasThis;
			}
			if (parameterTypes != null)
			{
				for (int i = 0; i < parameterTypes.Length; i++)
				{
					if (parameterTypes[i] == null)
					{
						throw new ArgumentException("Elements of the parameterTypes array cannot be null", "parameterTypes");
					}
				}
				this.parameters = new Type[parameterTypes.Length];
				Array.Copy(parameterTypes, this.parameters, parameterTypes.Length);
			}
			this.type = tb;
			this.table_idx = this.get_next_table_index(this, 6, true);
			((ModuleBuilder)tb.Module).RegisterToken(this, this.GetToken().Token);
		}

		internal MethodBuilder(TypeBuilder tb, string name, MethodAttributes attributes, CallingConventions callingConvention, Type returnType, Type[] returnModReq, Type[] returnModOpt, Type[] parameterTypes, Type[][] paramModReq, Type[][] paramModOpt, string dllName, string entryName, CallingConvention nativeCConv, CharSet nativeCharset)
			: this(tb, name, attributes, callingConvention, returnType, returnModReq, returnModOpt, parameterTypes, paramModReq, paramModOpt)
		{
			this.pi_dll = dllName;
			this.pi_entry = entryName;
			this.native_cc = nativeCConv;
			this.charset = nativeCharset;
		}

		void _MethodBuilder.GetIDsOfNames([In] ref Guid riid, IntPtr rgszNames, uint cNames, uint lcid, IntPtr rgDispId)
		{
			throw new NotImplementedException();
		}

		void _MethodBuilder.GetTypeInfo(uint iTInfo, uint lcid, IntPtr ppTInfo)
		{
			throw new NotImplementedException();
		}

		void _MethodBuilder.GetTypeInfoCount(out uint pcTInfo)
		{
			throw new NotImplementedException();
		}

		void _MethodBuilder.Invoke(uint dispIdMember, [In] ref Guid riid, uint lcid, short wFlags, IntPtr pDispParams, IntPtr pVarResult, IntPtr pExcepInfo, IntPtr puArgErr)
		{
			throw new NotImplementedException();
		}

		public override bool ContainsGenericParameters
		{
			get
			{
				throw new NotSupportedException();
			}
		}

		public bool InitLocals
		{
			get
			{
				return this.init_locals;
			}
			set
			{
				this.init_locals = value;
			}
		}

		internal TypeBuilder TypeBuilder
		{
			get
			{
				return this.type;
			}
		}

		public override RuntimeMethodHandle MethodHandle
		{
			get
			{
				throw this.NotSupported();
			}
		}

		public override Type ReturnType
		{
			get
			{
				return this.rtype;
			}
		}

		public override Type ReflectedType
		{
			get
			{
				return this.type;
			}
		}

		public override Type DeclaringType
		{
			get
			{
				return this.type;
			}
		}

		public override string Name
		{
			get
			{
				return this.name;
			}
		}

		public override MethodAttributes Attributes
		{
			get
			{
				return this.attrs;
			}
		}

		public override ICustomAttributeProvider ReturnTypeCustomAttributes
		{
			get
			{
				return null;
			}
		}

		public override CallingConventions CallingConvention
		{
			get
			{
				return this.call_conv;
			}
		}

		[MonoTODO("Not implemented")]
		public string Signature
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		internal bool BestFitMapping
		{
			set
			{
				this.extra_flags = (uint)(((ulong)this.extra_flags & 18446744073709551567UL) | ((!value) ? 32UL : 16UL));
			}
		}

		internal bool ThrowOnUnmappableChar
		{
			set
			{
				this.extra_flags = (uint)(((ulong)this.extra_flags & 18446744073709539327UL) | ((!value) ? 8192UL : 4096UL));
			}
		}

		internal bool ExactSpelling
		{
			set
			{
				this.extra_flags = (uint)(((ulong)this.extra_flags & 18446744073709551614UL) | ((!value) ? 0UL : 1UL));
			}
		}

		internal bool SetLastError
		{
			set
			{
				this.extra_flags = (uint)(((ulong)this.extra_flags & 18446744073709551551UL) | ((!value) ? 0UL : 64UL));
			}
		}

		public MethodToken GetToken()
		{
			return new MethodToken(100663296 | this.table_idx);
		}

		public override MethodInfo GetBaseDefinition()
		{
			return this;
		}

		public override MethodImplAttributes GetMethodImplementationFlags()
		{
			return this.iattrs;
		}

		public override ParameterInfo[] GetParameters()
		{
			if (!this.type.is_created)
			{
				throw this.NotSupported();
			}
			if (this.parameters == null)
			{
				return null;
			}
			ParameterInfo[] array = new ParameterInfo[this.parameters.Length];
			for (int i = 0; i < this.parameters.Length; i++)
			{
				array[i] = new ParameterInfo((this.pinfo != null) ? this.pinfo[i + 1] : null, this.parameters[i], this, i + 1);
			}
			return array;
		}

		internal override int GetParameterCount()
		{
			if (this.parameters == null)
			{
				return 0;
			}
			return this.parameters.Length;
		}

		public Module GetModule()
		{
			return this.type.Module;
		}

		public void CreateMethodBody(byte[] il, int count)
		{
			if (il != null && (count < 0 || count > il.Length))
			{
				throw new ArgumentOutOfRangeException("Index was out of range.  Must be non-negative and less than the size of the collection.");
			}
			if (this.code != null || this.type.is_created)
			{
				throw new InvalidOperationException("Type definition of the method is complete.");
			}
			if (il == null)
			{
				this.code = null;
			}
			else
			{
				this.code = new byte[count];
				Array.Copy(il, this.code, count);
			}
		}

		public override object Invoke(object obj, BindingFlags invokeAttr, Binder binder, object[] parameters, CultureInfo culture)
		{
			throw this.NotSupported();
		}

		public override bool IsDefined(Type attributeType, bool inherit)
		{
			throw this.NotSupported();
		}

		public override object[] GetCustomAttributes(bool inherit)
		{
			if (this.type.is_created)
			{
				return MonoCustomAttrs.GetCustomAttributes(this, inherit);
			}
			throw this.NotSupported();
		}

		public override object[] GetCustomAttributes(Type attributeType, bool inherit)
		{
			if (this.type.is_created)
			{
				return MonoCustomAttrs.GetCustomAttributes(this, attributeType, inherit);
			}
			throw this.NotSupported();
		}

		public ILGenerator GetILGenerator()
		{
			return this.GetILGenerator(64);
		}

		public ILGenerator GetILGenerator(int size)
		{
			if ((this.iattrs & MethodImplAttributes.CodeTypeMask) != MethodImplAttributes.IL || (this.iattrs & MethodImplAttributes.ManagedMask) != MethodImplAttributes.IL)
			{
				throw new InvalidOperationException("Method body should not exist.");
			}
			if (this.ilgen != null)
			{
				return this.ilgen;
			}
			this.ilgen = new ILGenerator(this.type.Module, ((ModuleBuilder)this.type.Module).GetTokenGenerator(), size);
			return this.ilgen;
		}

		public ParameterBuilder DefineParameter(int position, ParameterAttributes attributes, string strParamName)
		{
			this.RejectIfCreated();
			if (position < 0 || position > this.parameters.Length)
			{
				throw new ArgumentOutOfRangeException("position");
			}
			ParameterBuilder parameterBuilder = new ParameterBuilder(this, position, attributes, strParamName);
			if (this.pinfo == null)
			{
				this.pinfo = new ParameterBuilder[this.parameters.Length + 1];
			}
			this.pinfo[position] = parameterBuilder;
			return parameterBuilder;
		}

		internal void check_override()
		{
			if (this.override_method != null && this.override_method.IsVirtual && !this.IsVirtual)
			{
				throw new TypeLoadException(string.Format("Method '{0}' override '{1}' but it is not virtual", this.name, this.override_method));
			}
		}

		internal void fixup()
		{
			if ((this.attrs & (MethodAttributes.Abstract | MethodAttributes.PinvokeImpl)) == MethodAttributes.PrivateScope && (this.iattrs & (MethodImplAttributes)4099) == MethodImplAttributes.IL && (this.ilgen == null || ILGenerator.Mono_GetCurrentOffset(this.ilgen) == 0) && (this.code == null || this.code.Length == 0))
			{
				throw new InvalidOperationException(string.Format("Method '{0}.{1}' does not have a method body.", this.DeclaringType.FullName, this.Name));
			}
			if (this.ilgen != null)
			{
				this.ilgen.label_fixup();
			}
		}

		internal void GenerateDebugInfo(ISymbolWriter symbolWriter)
		{
			if (this.ilgen != null && this.ilgen.HasDebugInfo)
			{
				SymbolToken symbolToken = new SymbolToken(this.GetToken().Token);
				symbolWriter.OpenMethod(symbolToken);
				symbolWriter.SetSymAttribute(symbolToken, "__name", Encoding.UTF8.GetBytes(this.Name));
				this.ilgen.GenerateDebugInfo(symbolWriter);
				symbolWriter.CloseMethod();
			}
		}

		public void SetCustomAttribute(CustomAttributeBuilder customBuilder)
		{
			if (customBuilder == null)
			{
				throw new ArgumentNullException("customBuilder");
			}
			string fullName = customBuilder.Ctor.ReflectedType.FullName;
			switch (fullName)
			{
			case "System.Runtime.CompilerServices.MethodImplAttribute":
			{
				byte[] data = customBuilder.Data;
				int num2 = (int)data[2];
				num2 |= (int)data[3] << 8;
				this.iattrs |= (MethodImplAttributes)num2;
				return;
			}
			case "System.Runtime.InteropServices.DllImportAttribute":
			{
				CustomAttributeBuilder.CustomAttributeInfo customAttributeInfo = CustomAttributeBuilder.decode_cattr(customBuilder);
				bool flag = true;
				this.pi_dll = (string)customAttributeInfo.ctorArgs[0];
				if (this.pi_dll == null || this.pi_dll.Length == 0)
				{
					throw new ArgumentException("DllName cannot be empty");
				}
				this.native_cc = global::System.Runtime.InteropServices.CallingConvention.Winapi;
				for (int i = 0; i < customAttributeInfo.namedParamNames.Length; i++)
				{
					string text = customAttributeInfo.namedParamNames[i];
					object obj = customAttributeInfo.namedParamValues[i];
					if (text == "CallingConvention")
					{
						this.native_cc = (CallingConvention)((int)obj);
					}
					else if (text == "CharSet")
					{
						this.charset = (CharSet)((int)obj);
					}
					else if (text == "EntryPoint")
					{
						this.pi_entry = (string)obj;
					}
					else if (text == "ExactSpelling")
					{
						this.ExactSpelling = (bool)obj;
					}
					else if (text == "SetLastError")
					{
						this.SetLastError = (bool)obj;
					}
					else if (text == "PreserveSig")
					{
						flag = (bool)obj;
					}
					else if (text == "BestFitMapping")
					{
						this.BestFitMapping = (bool)obj;
					}
					else if (text == "ThrowOnUnmappableChar")
					{
						this.ThrowOnUnmappableChar = (bool)obj;
					}
				}
				this.attrs |= MethodAttributes.PinvokeImpl;
				if (flag)
				{
					this.iattrs |= MethodImplAttributes.PreserveSig;
				}
				return;
			}
			case "System.Runtime.InteropServices.PreserveSigAttribute":
				this.iattrs |= MethodImplAttributes.PreserveSig;
				return;
			case "System.Runtime.CompilerServices.SpecialNameAttribute":
				this.attrs |= MethodAttributes.SpecialName;
				return;
			case "System.Security.SuppressUnmanagedCodeSecurityAttribute":
				this.attrs |= MethodAttributes.HasSecurity;
				break;
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

		[ComVisible(true)]
		public void SetCustomAttribute(ConstructorInfo con, byte[] binaryAttribute)
		{
			if (con == null)
			{
				throw new ArgumentNullException("con");
			}
			if (binaryAttribute == null)
			{
				throw new ArgumentNullException("binaryAttribute");
			}
			this.SetCustomAttribute(new CustomAttributeBuilder(con, binaryAttribute));
		}

		public void SetImplementationFlags(MethodImplAttributes attributes)
		{
			this.RejectIfCreated();
			this.iattrs = attributes;
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
			this.RejectIfCreated();
			if (this.permissions != null)
			{
				foreach (RefEmitPermissionSet refEmitPermissionSet in this.permissions)
				{
					if (refEmitPermissionSet.action == action)
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
			this.attrs |= MethodAttributes.HasSecurity;
		}

		[Obsolete("An alternate API is available: Emit the MarshalAs custom attribute instead.")]
		public void SetMarshal(UnmanagedMarshal unmanagedMarshal)
		{
			this.RejectIfCreated();
			throw new NotImplementedException();
		}

		[MonoTODO]
		public void SetSymCustomAttribute(string name, byte[] data)
		{
			this.RejectIfCreated();
			throw new NotImplementedException();
		}

		public override string ToString()
		{
			return string.Concat(new string[]
			{
				"MethodBuilder [",
				this.type.Name,
				"::",
				this.name,
				"]"
			});
		}

		[MonoTODO]
		public override bool Equals(object obj)
		{
			return base.Equals(obj);
		}

		public override int GetHashCode()
		{
			return this.name.GetHashCode();
		}

		internal override int get_next_table_index(object obj, int table, bool inc)
		{
			return this.type.get_next_table_index(obj, table, inc);
		}

		internal void set_override(MethodInfo mdecl)
		{
			this.override_method = mdecl;
		}

		private void RejectIfCreated()
		{
			if (this.type.is_created)
			{
				throw new InvalidOperationException("Type definition of the method is complete.");
			}
		}

		private Exception NotSupported()
		{
			return new NotSupportedException("The invoked member is not supported in a dynamic module.");
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		public override extern MethodInfo MakeGenericMethod(params Type[] typeArguments);

		public override bool IsGenericMethodDefinition
		{
			get
			{
				return this.generic_params != null;
			}
		}

		public override bool IsGenericMethod
		{
			get
			{
				return this.generic_params != null;
			}
		}

		public override MethodInfo GetGenericMethodDefinition()
		{
			if (!this.IsGenericMethodDefinition)
			{
				throw new InvalidOperationException();
			}
			return this;
		}

		public override Type[] GetGenericArguments()
		{
			if (this.generic_params == null)
			{
				return Type.EmptyTypes;
			}
			Type[] array = new Type[this.generic_params.Length];
			for (int i = 0; i < this.generic_params.Length; i++)
			{
				array[i] = this.generic_params[i];
			}
			return array;
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
				this.generic_params[i] = new GenericTypeParameterBuilder(this.type, this, text, i);
			}
			return this.generic_params;
		}

		public void SetReturnType(Type returnType)
		{
			this.rtype = returnType;
		}

		public void SetParameters(params Type[] parameterTypes)
		{
			if (parameterTypes != null)
			{
				for (int i = 0; i < parameterTypes.Length; i++)
				{
					if (parameterTypes[i] == null)
					{
						throw new ArgumentException("Elements of the parameterTypes array cannot be null", "parameterTypes");
					}
				}
				this.parameters = new Type[parameterTypes.Length];
				Array.Copy(parameterTypes, this.parameters, parameterTypes.Length);
			}
		}

		public void SetSignature(Type returnType, Type[] returnTypeRequiredCustomModifiers, Type[] returnTypeOptionalCustomModifiers, Type[] parameterTypes, Type[][] parameterTypeRequiredCustomModifiers, Type[][] parameterTypeOptionalCustomModifiers)
		{
			this.SetReturnType(returnType);
			this.SetParameters(parameterTypes);
			this.returnModReq = returnTypeRequiredCustomModifiers;
			this.returnModOpt = returnTypeOptionalCustomModifiers;
			this.paramModReq = parameterTypeRequiredCustomModifiers;
			this.paramModOpt = parameterTypeOptionalCustomModifiers;
		}

		public override Module Module
		{
			get
			{
				return base.Module;
			}
		}

		private RuntimeMethodHandle mhandle;

		private Type rtype;

		internal Type[] parameters;

		private MethodAttributes attrs;

		private MethodImplAttributes iattrs;

		private string name;

		private int table_idx;

		private byte[] code;

		private ILGenerator ilgen;

		private TypeBuilder type;

		internal ParameterBuilder[] pinfo;

		private CustomAttributeBuilder[] cattrs;

		private MethodInfo override_method;

		private string pi_dll;

		private string pi_entry;

		private CharSet charset;

		private uint extra_flags;

		private CallingConvention native_cc;

		private CallingConventions call_conv;

		private bool init_locals = true;

		private IntPtr generic_container;

		internal GenericTypeParameterBuilder[] generic_params;

		private Type[] returnModReq;

		private Type[] returnModOpt;

		private Type[][] paramModReq;

		private Type[][] paramModOpt;

		private RefEmitPermissionSet[] permissions;
	}
}
