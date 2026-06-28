using System;
using System.Runtime.InteropServices;

namespace System.Reflection.Emit
{
	[ComDefaultInterface(typeof(_ParameterBuilder))]
	[ClassInterface(ClassInterfaceType.None)]
	[ComVisible(true)]
	public class ParameterBuilder : _ParameterBuilder
	{
		internal ParameterBuilder(MethodBase mb, int pos, ParameterAttributes attributes, string strParamName)
		{
			this.name = strParamName;
			this.position = pos;
			this.attrs = attributes;
			this.methodb = mb;
			if (mb is DynamicMethod)
			{
				this.table_idx = 0;
			}
			else
			{
				this.table_idx = mb.get_next_table_index(this, 8, true);
			}
		}

		void _ParameterBuilder.GetIDsOfNames([In] ref Guid riid, IntPtr rgszNames, uint cNames, uint lcid, IntPtr rgDispId)
		{
			throw new NotImplementedException();
		}

		void _ParameterBuilder.GetTypeInfo(uint iTInfo, uint lcid, IntPtr ppTInfo)
		{
			throw new NotImplementedException();
		}

		void _ParameterBuilder.GetTypeInfoCount(out uint pcTInfo)
		{
			throw new NotImplementedException();
		}

		void _ParameterBuilder.Invoke(uint dispIdMember, [In] ref Guid riid, uint lcid, short wFlags, IntPtr pDispParams, IntPtr pVarResult, IntPtr pExcepInfo, IntPtr puArgErr)
		{
			throw new NotImplementedException();
		}

		public virtual int Attributes
		{
			get
			{
				return (int)this.attrs;
			}
		}

		public bool IsIn
		{
			get
			{
				return (this.attrs & ParameterAttributes.In) != ParameterAttributes.None;
			}
		}

		public bool IsOut
		{
			get
			{
				return (this.attrs & ParameterAttributes.Out) != ParameterAttributes.None;
			}
		}

		public bool IsOptional
		{
			get
			{
				return (this.attrs & ParameterAttributes.Optional) != ParameterAttributes.None;
			}
		}

		public virtual string Name
		{
			get
			{
				return this.name;
			}
		}

		public virtual int Position
		{
			get
			{
				return this.position;
			}
		}

		public virtual ParameterToken GetToken()
		{
			return new ParameterToken(8 | this.table_idx);
		}

		public virtual void SetConstant(object defaultValue)
		{
			this.def_value = defaultValue;
			this.attrs |= ParameterAttributes.HasDefault;
		}

		public void SetCustomAttribute(CustomAttributeBuilder customBuilder)
		{
			string fullName = customBuilder.Ctor.ReflectedType.FullName;
			if (fullName == "System.Runtime.InteropServices.InAttribute")
			{
				this.attrs |= ParameterAttributes.In;
				return;
			}
			if (fullName == "System.Runtime.InteropServices.OutAttribute")
			{
				this.attrs |= ParameterAttributes.Out;
				return;
			}
			if (fullName == "System.Runtime.InteropServices.OptionalAttribute")
			{
				this.attrs |= ParameterAttributes.Optional;
				return;
			}
			if (fullName == "System.Runtime.InteropServices.MarshalAsAttribute")
			{
				this.attrs |= ParameterAttributes.HasFieldMarshal;
				this.marshal_info = CustomAttributeBuilder.get_umarshal(customBuilder, false);
				return;
			}
			if (fullName == "System.Runtime.InteropServices.DefaultParameterValueAttribute")
			{
				this.SetConstant(CustomAttributeBuilder.decode_cattr(customBuilder).ctorArgs[0]);
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
			this.SetCustomAttribute(new CustomAttributeBuilder(con, binaryAttribute));
		}

		[Obsolete("An alternate API is available: Emit the MarshalAs custom attribute instead.")]
		public virtual void SetMarshal(UnmanagedMarshal unmanagedMarshal)
		{
			this.marshal_info = unmanagedMarshal;
			this.attrs |= ParameterAttributes.HasFieldMarshal;
		}

		private MethodBase methodb;

		private string name;

		private CustomAttributeBuilder[] cattrs;

		private UnmanagedMarshal marshal_info;

		private ParameterAttributes attrs;

		private int position;

		private int table_idx;

		private object def_value;
	}
}
