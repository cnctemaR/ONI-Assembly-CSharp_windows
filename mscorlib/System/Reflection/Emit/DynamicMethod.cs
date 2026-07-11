using System;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;

namespace System.Reflection.Emit
{
	[ComVisible(true)]
	[StructLayout(LayoutKind.Sequential)]
	public sealed class DynamicMethod : MethodInfo
	{
		public DynamicMethod(string name, Type returnType, Type[] parameterTypes, Module m)
			: this(name, returnType, parameterTypes, m, false)
		{
		}

		public DynamicMethod(string name, Type returnType, Type[] parameterTypes, Type owner)
			: this(name, returnType, parameterTypes, owner, false)
		{
		}

		public DynamicMethod(string name, Type returnType, Type[] parameterTypes, Module m, bool skipVisibility)
			: this(name, MethodAttributes.FamANDAssem | MethodAttributes.Family | MethodAttributes.Static, CallingConventions.Standard, returnType, parameterTypes, m, skipVisibility)
		{
		}

		public DynamicMethod(string name, Type returnType, Type[] parameterTypes, Type owner, bool skipVisibility)
			: this(name, MethodAttributes.FamANDAssem | MethodAttributes.Family | MethodAttributes.Static, CallingConventions.Standard, returnType, parameterTypes, owner, skipVisibility)
		{
		}

		public DynamicMethod(string name, MethodAttributes attributes, CallingConventions callingConvention, Type returnType, Type[] parameterTypes, Type owner, bool skipVisibility)
			: this(name, attributes, callingConvention, returnType, parameterTypes, owner, owner.Module, skipVisibility, false)
		{
		}

		public DynamicMethod(string name, MethodAttributes attributes, CallingConventions callingConvention, Type returnType, Type[] parameterTypes, Module m, bool skipVisibility)
			: this(name, attributes, callingConvention, returnType, parameterTypes, null, m, skipVisibility, false)
		{
		}

		public DynamicMethod(string name, Type returnType, Type[] parameterTypes)
			: this(name, returnType, parameterTypes, false)
		{
		}

		[MonoTODO("Visibility is not restricted")]
		public DynamicMethod(string name, Type returnType, Type[] parameterTypes, bool restrictedSkipVisibility)
			: this(name, MethodAttributes.FamANDAssem | MethodAttributes.Family | MethodAttributes.Static, CallingConventions.Standard, returnType, parameterTypes, null, null, restrictedSkipVisibility, true)
		{
		}

		private DynamicMethod(string name, MethodAttributes attributes, CallingConventions callingConvention, Type returnType, Type[] parameterTypes, Type owner, Module m, bool skipVisibility, bool anonHosted)
		{
			if (name == null)
			{
				throw new ArgumentNullException("name");
			}
			if (returnType == null)
			{
				returnType = typeof(void);
			}
			if (m == null && !anonHosted)
			{
				throw new ArgumentNullException("m");
			}
			if (returnType.IsByRef)
			{
				throw new ArgumentException("Return type can't be a byref type", "returnType");
			}
			if (parameterTypes != null)
			{
				for (int i = 0; i < parameterTypes.Length; i++)
				{
					if (parameterTypes[i] == null)
					{
						throw new ArgumentException("Parameter " + i + " is null", "parameterTypes");
					}
				}
			}
			if (owner != null && (owner.IsArray || owner.IsInterface))
			{
				throw new ArgumentException("Owner can't be an array or an interface.");
			}
			if (m == null)
			{
				m = DynamicMethod.AnonHostModuleHolder.AnonHostModule;
			}
			this.name = name;
			this.attributes = attributes | MethodAttributes.Static;
			this.callingConvention = callingConvention;
			this.returnType = returnType;
			this.parameters = parameterTypes;
			this.owner = owner;
			this.module = m;
			this.skipVisibility = skipVisibility;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void create_dynamic_method(DynamicMethod m);

		private void CreateDynMethod()
		{
			if (this.mhandle.Value == IntPtr.Zero)
			{
				if (this.ilgen == null || this.ilgen.ILOffset == 0)
				{
					throw new InvalidOperationException("Method '" + this.name + "' does not have a method body.");
				}
				this.ilgen.label_fixup(this);
				try
				{
					this.creating = true;
					if (this.refs != null)
					{
						for (int i = 0; i < this.refs.Length; i++)
						{
							if (this.refs[i] is DynamicMethod)
							{
								DynamicMethod dynamicMethod = (DynamicMethod)this.refs[i];
								if (!dynamicMethod.creating)
								{
									dynamicMethod.CreateDynMethod();
								}
							}
						}
					}
				}
				finally
				{
					this.creating = false;
				}
				DynamicMethod.create_dynamic_method(this);
			}
		}

		[ComVisible(true)]
		public sealed override Delegate CreateDelegate(Type delegateType)
		{
			if (delegateType == null)
			{
				throw new ArgumentNullException("delegateType");
			}
			if (this.deleg != null)
			{
				return this.deleg;
			}
			this.CreateDynMethod();
			this.deleg = Delegate.CreateDelegate(delegateType, null, this);
			return this.deleg;
		}

		[ComVisible(true)]
		public sealed override Delegate CreateDelegate(Type delegateType, object target)
		{
			if (delegateType == null)
			{
				throw new ArgumentNullException("delegateType");
			}
			this.CreateDynMethod();
			return Delegate.CreateDelegate(delegateType, target, this);
		}

		public ParameterBuilder DefineParameter(int position, ParameterAttributes attributes, string parameterName)
		{
			if (position < 0 || position > this.parameters.Length)
			{
				throw new ArgumentOutOfRangeException("position");
			}
			this.RejectIfCreated();
			ParameterBuilder parameterBuilder = new ParameterBuilder(this, position, attributes, parameterName);
			if (this.pinfo == null)
			{
				this.pinfo = new ParameterBuilder[this.parameters.Length + 1];
			}
			this.pinfo[position] = parameterBuilder;
			return parameterBuilder;
		}

		public override MethodInfo GetBaseDefinition()
		{
			return this;
		}

		public override object[] GetCustomAttributes(bool inherit)
		{
			return new object[]
			{
				new MethodImplAttribute(this.GetMethodImplementationFlags())
			};
		}

		public override object[] GetCustomAttributes(Type attributeType, bool inherit)
		{
			if (attributeType == null)
			{
				throw new ArgumentNullException("attributeType");
			}
			if (attributeType.IsAssignableFrom(typeof(MethodImplAttribute)))
			{
				return new object[]
				{
					new MethodImplAttribute(this.GetMethodImplementationFlags())
				};
			}
			return EmptyArray<object>.Value;
		}

		public DynamicILInfo GetDynamicILInfo()
		{
			if (this.il_info == null)
			{
				this.il_info = new DynamicILInfo(this);
			}
			return this.il_info;
		}

		public ILGenerator GetILGenerator()
		{
			return this.GetILGenerator(64);
		}

		public ILGenerator GetILGenerator(int streamSize)
		{
			if ((this.GetMethodImplementationFlags() & MethodImplAttributes.CodeTypeMask) != MethodImplAttributes.IL || (this.GetMethodImplementationFlags() & MethodImplAttributes.ManagedMask) != MethodImplAttributes.IL)
			{
				throw new InvalidOperationException("Method body should not exist.");
			}
			if (this.ilgen != null)
			{
				return this.ilgen;
			}
			this.ilgen = new ILGenerator(this.Module, new DynamicMethodTokenGenerator(this), streamSize);
			return this.ilgen;
		}

		public override MethodImplAttributes GetMethodImplementationFlags()
		{
			return MethodImplAttributes.NoInlining;
		}

		public override ParameterInfo[] GetParameters()
		{
			return this.GetParametersInternal();
		}

		internal override ParameterInfo[] GetParametersInternal()
		{
			if (this.parameters == null)
			{
				return EmptyArray<ParameterInfo>.Value;
			}
			ParameterInfo[] array = new ParameterInfo[this.parameters.Length];
			for (int i = 0; i < this.parameters.Length; i++)
			{
				array[i] = ParameterInfo.New((this.pinfo == null) ? null : this.pinfo[i + 1], this.parameters[i], this, i + 1);
			}
			return array;
		}

		internal override int GetParametersCount()
		{
			if (this.parameters != null)
			{
				return this.parameters.Length;
			}
			return 0;
		}

		internal override Type GetParameterType(int pos)
		{
			return this.parameters[pos];
		}

		[SecuritySafeCritical]
		public override object Invoke(object obj, BindingFlags invokeAttr, Binder binder, object[] parameters, CultureInfo culture)
		{
			object obj2;
			try
			{
				this.CreateDynMethod();
				if (this.method == null)
				{
					this.method = new MonoMethod(this.mhandle);
				}
				obj2 = this.method.Invoke(obj, parameters);
			}
			catch (MethodAccessException ex)
			{
				throw new TargetInvocationException("Method cannot be invoked.", ex);
			}
			return obj2;
		}

		public override bool IsDefined(Type attributeType, bool inherit)
		{
			if (attributeType == null)
			{
				throw new ArgumentNullException("attributeType");
			}
			return attributeType.IsAssignableFrom(typeof(MethodImplAttribute));
		}

		public override string ToString()
		{
			string text = string.Empty;
			ParameterInfo[] parametersInternal = this.GetParametersInternal();
			for (int i = 0; i < parametersInternal.Length; i++)
			{
				if (i > 0)
				{
					text += ", ";
				}
				text += parametersInternal[i].ParameterType.Name;
			}
			return string.Concat(new string[]
			{
				this.ReturnType.Name,
				" ",
				this.Name,
				"(",
				text,
				")"
			});
		}

		public override MethodAttributes Attributes
		{
			get
			{
				return this.attributes;
			}
		}

		public override CallingConventions CallingConvention
		{
			get
			{
				return this.callingConvention;
			}
		}

		public override Type DeclaringType
		{
			get
			{
				return null;
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

		public override RuntimeMethodHandle MethodHandle
		{
			get
			{
				return this.mhandle;
			}
		}

		public override Module Module
		{
			get
			{
				return this.module;
			}
		}

		public override string Name
		{
			get
			{
				return this.name;
			}
		}

		public override Type ReflectedType
		{
			get
			{
				return null;
			}
		}

		[MonoTODO("Not implemented")]
		public override ParameterInfo ReturnParameter
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public override Type ReturnType
		{
			get
			{
				return this.returnType;
			}
		}

		[MonoTODO("Not implemented")]
		public override ICustomAttributeProvider ReturnTypeCustomAttributes
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		private void RejectIfCreated()
		{
			if (this.mhandle.Value != IntPtr.Zero)
			{
				throw new InvalidOperationException("Type definition of the method is complete.");
			}
		}

		internal int AddRef(object reference)
		{
			if (this.refs == null)
			{
				this.refs = new object[4];
			}
			if (this.nrefs >= this.refs.Length - 1)
			{
				object[] array = new object[this.refs.Length * 2];
				Array.Copy(this.refs, array, this.refs.Length);
				this.refs = array;
			}
			this.refs[this.nrefs] = reference;
			this.refs[this.nrefs + 1] = null;
			this.nrefs += 2;
			return this.nrefs - 1;
		}

		private RuntimeMethodHandle mhandle;

		private string name;

		private Type returnType;

		private Type[] parameters;

		private MethodAttributes attributes;

		private CallingConventions callingConvention;

		private Module module;

		private bool skipVisibility;

		private bool init_locals = true;

		private ILGenerator ilgen;

		private int nrefs;

		private object[] refs;

		private IntPtr referenced_by;

		private Type owner;

		private Delegate deleg;

		private MonoMethod method;

		private ParameterBuilder[] pinfo;

		internal bool creating;

		private DynamicILInfo il_info;

		private class AnonHostModuleHolder
		{
			static AnonHostModuleHolder()
			{
				AssemblyName assemblyName = new AssemblyName();
				assemblyName.Name = "Anonymously Hosted DynamicMethods Assembly";
				DynamicMethod.AnonHostModuleHolder.anon_host_module = AppDomain.CurrentDomain.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run).GetManifestModule();
			}

			public static Module AnonHostModule
			{
				get
				{
					return DynamicMethod.AnonHostModuleHolder.anon_host_module;
				}
			}

			public static Module anon_host_module;
		}
	}
}
