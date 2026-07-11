using System;
using System.Globalization;
using System.Runtime.InteropServices;
using Unity;

namespace System.Reflection.Emit
{
	[ClassInterface(ClassInterfaceType.None)]
	[ComDefaultInterface(typeof(_PropertyBuilder))]
	[ComVisible(true)]
	[StructLayout(LayoutKind.Sequential)]
	public sealed class PropertyBuilder : PropertyInfo, _PropertyBuilder
	{
		internal PropertyBuilder(TypeBuilder tb, string name, PropertyAttributes attributes, CallingConventions callingConvention, Type returnType, Type[] returnModReq, Type[] returnModOpt, Type[] parameterTypes, Type[][] paramModReq, Type[][] paramModOpt)
		{
			this.name = name;
			this.attrs = attributes;
			this.callingConvention = callingConvention;
			this.type = returnType;
			this.returnModReq = returnModReq;
			this.returnModOpt = returnModOpt;
			this.paramModReq = paramModReq;
			this.paramModOpt = paramModOpt;
			if (parameterTypes != null)
			{
				this.parameters = new Type[parameterTypes.Length];
				Array.Copy(parameterTypes, this.parameters, this.parameters.Length);
			}
			this.typeb = tb;
			this.table_idx = tb.get_next_table_index(this, 23, true);
		}

		public override PropertyAttributes Attributes
		{
			get
			{
				return this.attrs;
			}
		}

		public override bool CanRead
		{
			get
			{
				return this.get_method != null;
			}
		}

		public override bool CanWrite
		{
			get
			{
				return this.set_method != null;
			}
		}

		public override Type DeclaringType
		{
			get
			{
				return this.typeb;
			}
		}

		public override string Name
		{
			get
			{
				return this.name;
			}
		}

		public PropertyToken PropertyToken
		{
			get
			{
				return default(PropertyToken);
			}
		}

		public override Type PropertyType
		{
			get
			{
				return this.type;
			}
		}

		public override Type ReflectedType
		{
			get
			{
				return this.typeb;
			}
		}

		public void AddOtherMethod(MethodBuilder mdBuilder)
		{
		}

		public override MethodInfo[] GetAccessors(bool nonPublic)
		{
			return null;
		}

		public override object[] GetCustomAttributes(bool inherit)
		{
			throw this.not_supported();
		}

		public override object[] GetCustomAttributes(Type attributeType, bool inherit)
		{
			throw this.not_supported();
		}

		public override MethodInfo GetGetMethod(bool nonPublic)
		{
			return this.get_method;
		}

		public override ParameterInfo[] GetIndexParameters()
		{
			throw this.not_supported();
		}

		public override MethodInfo GetSetMethod(bool nonPublic)
		{
			return this.set_method;
		}

		public override object GetValue(object obj, object[] index)
		{
			return null;
		}

		public override object GetValue(object obj, BindingFlags invokeAttr, Binder binder, object[] index, CultureInfo culture)
		{
			throw this.not_supported();
		}

		public override bool IsDefined(Type attributeType, bool inherit)
		{
			throw this.not_supported();
		}

		public void SetConstant(object defaultValue)
		{
			this.def_value = defaultValue;
		}

		public void SetCustomAttribute(CustomAttributeBuilder customBuilder)
		{
			if (customBuilder.Ctor.ReflectedType.FullName == "System.Runtime.CompilerServices.SpecialNameAttribute")
			{
				this.attrs |= PropertyAttributes.SpecialName;
				return;
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

		public void SetGetMethod(MethodBuilder mdBuilder)
		{
			this.get_method = mdBuilder;
		}

		public void SetSetMethod(MethodBuilder mdBuilder)
		{
			this.set_method = mdBuilder;
		}

		public override void SetValue(object obj, object value, object[] index)
		{
		}

		public override void SetValue(object obj, object value, BindingFlags invokeAttr, Binder binder, object[] index, CultureInfo culture)
		{
		}

		public override Module Module
		{
			get
			{
				return base.Module;
			}
		}

		void _PropertyBuilder.GetIDsOfNames([In] ref Guid riid, IntPtr rgszNames, uint cNames, uint lcid, IntPtr rgDispId)
		{
			throw new NotImplementedException();
		}

		void _PropertyBuilder.GetTypeInfo(uint iTInfo, uint lcid, IntPtr ppTInfo)
		{
			throw new NotImplementedException();
		}

		void _PropertyBuilder.GetTypeInfoCount(out uint pcTInfo)
		{
			throw new NotImplementedException();
		}

		void _PropertyBuilder.Invoke(uint dispIdMember, [In] ref Guid riid, uint lcid, short wFlags, IntPtr pDispParams, IntPtr pVarResult, IntPtr pExcepInfo, IntPtr puArgErr)
		{
			throw new NotImplementedException();
		}

		private Exception not_supported()
		{
			return new NotSupportedException("The invoked member is not supported in a dynamic module.");
		}

		internal PropertyBuilder()
		{
			ThrowStub.ThrowNotSupportedException();
		}

		private PropertyAttributes attrs;

		private string name;

		private Type type;

		private Type[] parameters;

		private CustomAttributeBuilder[] cattrs;

		private object def_value;

		private MethodBuilder set_method;

		private MethodBuilder get_method;

		private int table_idx;

		internal TypeBuilder typeb;

		private Type[] returnModReq;

		private Type[] returnModOpt;

		private Type[][] paramModReq;

		private Type[][] paramModOpt;

		private CallingConventions callingConvention;
	}
}
