using System;
using System.Diagnostics.SymbolStore;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Permissions;
using System.Text;

namespace System.Reflection.Emit
{
	[ComVisible(true)]
	[ComDefaultInterface(typeof(_ConstructorBuilder))]
	[ClassInterface(ClassInterfaceType.None)]
	public sealed class ConstructorBuilder : ConstructorInfo, _ConstructorBuilder
	{
		internal ConstructorBuilder(TypeBuilder tb, MethodAttributes attributes, CallingConventions callingConvention, Type[] parameterTypes, Type[][] paramModReq, Type[][] paramModOpt)
		{
			this.attrs = attributes | MethodAttributes.SpecialName | MethodAttributes.RTSpecialName;
			this.call_conv = callingConvention;
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
			this.paramModReq = paramModReq;
			this.paramModOpt = paramModOpt;
			this.table_idx = this.get_next_table_index(this, 6, true);
			((ModuleBuilder)tb.Module).RegisterToken(this, this.GetToken().Token);
		}

		void _ConstructorBuilder.GetIDsOfNames([In] ref Guid riid, IntPtr rgszNames, uint cNames, uint lcid, IntPtr rgDispId)
		{
			throw new NotImplementedException();
		}

		void _ConstructorBuilder.GetTypeInfo(uint iTInfo, uint lcid, IntPtr ppTInfo)
		{
			throw new NotImplementedException();
		}

		void _ConstructorBuilder.GetTypeInfoCount(out uint pcTInfo)
		{
			throw new NotImplementedException();
		}

		void _ConstructorBuilder.Invoke(uint dispIdMember, [In] ref Guid riid, uint lcid, short wFlags, IntPtr pDispParams, IntPtr pVarResult, IntPtr pExcepInfo, IntPtr puArgErr)
		{
			throw new NotImplementedException();
		}

		[MonoTODO]
		public override CallingConventions CallingConvention
		{
			get
			{
				return this.call_conv;
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

		public override MethodImplAttributes GetMethodImplementationFlags()
		{
			return this.iattrs;
		}

		public override ParameterInfo[] GetParameters()
		{
			if (!this.type.is_created && !this.IsCompilerContext)
			{
				throw this.not_created();
			}
			return this.GetParametersInternal();
		}

		internal ParameterInfo[] GetParametersInternal()
		{
			if (this.parameters == null)
			{
				return new ParameterInfo[0];
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

		public override object Invoke(object obj, BindingFlags invokeAttr, Binder binder, object[] parameters, CultureInfo culture)
		{
			throw this.not_supported();
		}

		public override object Invoke(BindingFlags invokeAttr, Binder binder, object[] parameters, CultureInfo culture)
		{
			throw this.not_supported();
		}

		public override RuntimeMethodHandle MethodHandle
		{
			get
			{
				throw this.not_supported();
			}
		}

		public override MethodAttributes Attributes
		{
			get
			{
				return this.attrs;
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

		public Type ReturnType
		{
			get
			{
				return null;
			}
		}

		public override string Name
		{
			get
			{
				return ((this.attrs & MethodAttributes.Static) == MethodAttributes.PrivateScope) ? ConstructorInfo.ConstructorName : ConstructorInfo.TypeConstructorName;
			}
		}

		public string Signature
		{
			get
			{
				return "constructor signature";
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
				throw new ArgumentOutOfRangeException("action", "Request* values are not permitted");
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

		public ParameterBuilder DefineParameter(int iSequence, ParameterAttributes attributes, string strParamName)
		{
			if (iSequence < 1 || iSequence > this.GetParameterCount())
			{
				throw new ArgumentOutOfRangeException("iSequence");
			}
			if (this.type.is_created)
			{
				throw this.not_after_created();
			}
			ParameterBuilder parameterBuilder = new ParameterBuilder(this, iSequence, attributes, strParamName);
			if (this.pinfo == null)
			{
				this.pinfo = new ParameterBuilder[this.parameters.Length + 1];
			}
			this.pinfo[iSequence] = parameterBuilder;
			return parameterBuilder;
		}

		public override bool IsDefined(Type attributeType, bool inherit)
		{
			throw this.not_supported();
		}

		public override object[] GetCustomAttributes(bool inherit)
		{
			if (this.type.is_created && this.IsCompilerContext)
			{
				return MonoCustomAttrs.GetCustomAttributes(this, inherit);
			}
			throw this.not_supported();
		}

		public override object[] GetCustomAttributes(Type attributeType, bool inherit)
		{
			if (this.type.is_created && this.IsCompilerContext)
			{
				return MonoCustomAttrs.GetCustomAttributes(this, attributeType, inherit);
			}
			throw this.not_supported();
		}

		public ILGenerator GetILGenerator()
		{
			return this.GetILGenerator(64);
		}

		public ILGenerator GetILGenerator(int streamSize)
		{
			if (this.ilgen != null)
			{
				return this.ilgen;
			}
			this.ilgen = new ILGenerator(this.type.Module, ((ModuleBuilder)this.type.Module).GetTokenGenerator(), streamSize);
			return this.ilgen;
		}

		public void SetCustomAttribute(CustomAttributeBuilder customBuilder)
		{
			if (customBuilder == null)
			{
				throw new ArgumentNullException("customBuilder");
			}
			string fullName = customBuilder.Ctor.ReflectedType.FullName;
			if (fullName == "System.Runtime.CompilerServices.MethodImplAttribute")
			{
				byte[] data = customBuilder.Data;
				int num = (int)data[2];
				num |= (int)data[3] << 8;
				this.SetImplementationFlags((MethodImplAttributes)num);
				return;
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
			if (this.type.is_created)
			{
				throw this.not_after_created();
			}
			this.iattrs = attributes;
		}

		public Module GetModule()
		{
			return this.type.Module;
		}

		public MethodToken GetToken()
		{
			return new MethodToken(100663296 | this.table_idx);
		}

		[MonoTODO]
		public void SetSymCustomAttribute(string name, byte[] data)
		{
			if (this.type.is_created)
			{
				throw this.not_after_created();
			}
		}

		public override Module Module
		{
			get
			{
				return base.Module;
			}
		}

		public override string ToString()
		{
			return "ConstructorBuilder ['" + this.type.Name + "']";
		}

		internal void fixup()
		{
			if ((this.attrs & (MethodAttributes.Abstract | MethodAttributes.PinvokeImpl)) == MethodAttributes.PrivateScope && (this.iattrs & (MethodImplAttributes)4099) == MethodImplAttributes.IL && (this.ilgen == null || ILGenerator.Mono_GetCurrentOffset(this.ilgen) == 0))
			{
				throw new InvalidOperationException("Method '" + this.Name + "' does not have a method body.");
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

		internal override int get_next_table_index(object obj, int table, bool inc)
		{
			return this.type.get_next_table_index(obj, table, inc);
		}

		private bool IsCompilerContext
		{
			get
			{
				ModuleBuilder moduleBuilder = (ModuleBuilder)this.TypeBuilder.Module;
				AssemblyBuilder assemblyBuilder = (AssemblyBuilder)moduleBuilder.Assembly;
				return assemblyBuilder.IsCompilerContext;
			}
		}

		private void RejectIfCreated()
		{
			if (this.type.is_created)
			{
				throw new InvalidOperationException("Type definition of the method is complete.");
			}
		}

		private Exception not_supported()
		{
			return new NotSupportedException("The invoked member is not supported in a dynamic module.");
		}

		private Exception not_after_created()
		{
			return new InvalidOperationException("Unable to change after type has been created.");
		}

		private Exception not_created()
		{
			return new NotSupportedException("The type is not yet created.");
		}

		private RuntimeMethodHandle mhandle;

		private ILGenerator ilgen;

		internal Type[] parameters;

		private MethodAttributes attrs;

		private MethodImplAttributes iattrs;

		private int table_idx;

		private CallingConventions call_conv;

		private TypeBuilder type;

		internal ParameterBuilder[] pinfo;

		private CustomAttributeBuilder[] cattrs;

		private bool init_locals = true;

		private Type[][] paramModReq;

		private Type[][] paramModOpt;

		private RefEmitPermissionSet[] permissions;
	}
}
