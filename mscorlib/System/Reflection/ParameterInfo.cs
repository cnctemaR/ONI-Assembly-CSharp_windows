using System;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace System.Reflection
{
	[ClassInterface(ClassInterfaceType.None)]
	[ComVisible(true)]
	[ComDefaultInterface(typeof(_ParameterInfo))]
	[Serializable]
	public class ParameterInfo : ICustomAttributeProvider, _ParameterInfo
	{
		protected ParameterInfo()
		{
		}

		internal ParameterInfo(ParameterBuilder pb, Type type, MemberInfo member, int position)
		{
			this.ClassImpl = type;
			this.MemberImpl = member;
			if (pb != null)
			{
				this.NameImpl = pb.Name;
				this.PositionImpl = pb.Position - 1;
				this.AttrsImpl = (ParameterAttributes)pb.Attributes;
			}
			else
			{
				this.NameImpl = null;
				this.PositionImpl = position - 1;
				this.AttrsImpl = ParameterAttributes.None;
			}
		}

		internal ParameterInfo(ParameterInfo pinfo, MemberInfo member)
		{
			this.ClassImpl = pinfo.ParameterType;
			this.MemberImpl = member;
			this.NameImpl = pinfo.Name;
			this.PositionImpl = pinfo.Position;
			this.AttrsImpl = pinfo.Attributes;
		}

		internal ParameterInfo(Type type, MemberInfo member, UnmanagedMarshal marshalAs)
		{
			this.ClassImpl = type;
			this.MemberImpl = member;
			this.NameImpl = string.Empty;
			this.PositionImpl = -1;
			this.AttrsImpl = ParameterAttributes.Retval;
			this.marshalAs = marshalAs;
		}

		void _ParameterInfo.GetIDsOfNames([In] ref Guid riid, IntPtr rgszNames, uint cNames, uint lcid, IntPtr rgDispId)
		{
			throw new NotImplementedException();
		}

		void _ParameterInfo.GetTypeInfo(uint iTInfo, uint lcid, IntPtr ppTInfo)
		{
			throw new NotImplementedException();
		}

		void _ParameterInfo.GetTypeInfoCount(out uint pcTInfo)
		{
			throw new NotImplementedException();
		}

		void _ParameterInfo.Invoke(uint dispIdMember, [In] ref Guid riid, uint lcid, short wFlags, IntPtr pDispParams, IntPtr pVarResult, IntPtr pExcepInfo, IntPtr puArgErr)
		{
			throw new NotImplementedException();
		}

		public override string ToString()
		{
			Type type = this.ClassImpl;
			while (type.HasElementType)
			{
				type = type.GetElementType();
			}
			bool flag = type.IsPrimitive || this.ClassImpl == typeof(void) || this.ClassImpl.Namespace == this.MemberImpl.DeclaringType.Namespace;
			string text = ((!flag) ? this.ClassImpl.FullName : this.ClassImpl.Name);
			if (!this.IsRetval)
			{
				text += ' ';
				text += this.NameImpl;
			}
			return text;
		}

		public virtual Type ParameterType
		{
			get
			{
				return this.ClassImpl;
			}
		}

		public virtual ParameterAttributes Attributes
		{
			get
			{
				return this.AttrsImpl;
			}
		}

		public virtual object DefaultValue
		{
			get
			{
				if (this.ClassImpl == typeof(decimal))
				{
					DecimalConstantAttribute[] array = (DecimalConstantAttribute[])this.GetCustomAttributes(typeof(DecimalConstantAttribute), false);
					if (array.Length > 0)
					{
						return array[0].Value;
					}
				}
				else if (this.ClassImpl == typeof(DateTime))
				{
					DateTimeConstantAttribute[] array2 = (DateTimeConstantAttribute[])this.GetCustomAttributes(typeof(DateTimeConstantAttribute), false);
					if (array2.Length > 0)
					{
						return new DateTime(array2[0].Ticks);
					}
				}
				return this.DefaultValueImpl;
			}
		}

		public bool IsIn
		{
			get
			{
				return (this.Attributes & ParameterAttributes.In) != ParameterAttributes.None;
			}
		}

		public bool IsLcid
		{
			get
			{
				return (this.Attributes & ParameterAttributes.Lcid) != ParameterAttributes.None;
			}
		}

		public bool IsOptional
		{
			get
			{
				return (this.Attributes & ParameterAttributes.Optional) != ParameterAttributes.None;
			}
		}

		public bool IsOut
		{
			get
			{
				return (this.Attributes & ParameterAttributes.Out) != ParameterAttributes.None;
			}
		}

		public bool IsRetval
		{
			get
			{
				return (this.Attributes & ParameterAttributes.Retval) != ParameterAttributes.None;
			}
		}

		public virtual MemberInfo Member
		{
			get
			{
				return this.MemberImpl;
			}
		}

		public virtual string Name
		{
			get
			{
				return this.NameImpl;
			}
		}

		public virtual int Position
		{
			get
			{
				return this.PositionImpl;
			}
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern int GetMetadataToken();

		public int MetadataToken
		{
			get
			{
				if (this.MemberImpl is PropertyInfo)
				{
					PropertyInfo propertyInfo = (PropertyInfo)this.MemberImpl;
					MethodInfo methodInfo = propertyInfo.GetGetMethod(true);
					if (methodInfo == null)
					{
						methodInfo = propertyInfo.GetSetMethod(true);
					}
					return methodInfo.GetParameters()[this.PositionImpl].MetadataToken;
				}
				if (this.MemberImpl is MethodBase)
				{
					return this.GetMetadataToken();
				}
				throw new ArgumentException("Can't produce MetadataToken for member of type " + this.MemberImpl.GetType());
			}
		}

		public virtual object[] GetCustomAttributes(bool inherit)
		{
			return MonoCustomAttrs.GetCustomAttributes(this, inherit);
		}

		public virtual object[] GetCustomAttributes(Type attributeType, bool inherit)
		{
			return MonoCustomAttrs.GetCustomAttributes(this, attributeType, inherit);
		}

		public virtual bool IsDefined(Type attributeType, bool inherit)
		{
			return MonoCustomAttrs.IsDefined(this, attributeType, inherit);
		}

		internal object[] GetPseudoCustomAttributes()
		{
			int num = 0;
			if (this.IsIn)
			{
				num++;
			}
			if (this.IsOut)
			{
				num++;
			}
			if (this.IsOptional)
			{
				num++;
			}
			if (this.marshalAs != null)
			{
				num++;
			}
			if (num == 0)
			{
				return null;
			}
			object[] array = new object[num];
			num = 0;
			if (this.IsIn)
			{
				array[num++] = new InAttribute();
			}
			if (this.IsOptional)
			{
				array[num++] = new OptionalAttribute();
			}
			if (this.IsOut)
			{
				array[num++] = new OutAttribute();
			}
			if (this.marshalAs != null)
			{
				array[num++] = this.marshalAs.ToMarshalAsAttribute();
			}
			return array;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern Type[] GetTypeModifiers(bool optional);

		public virtual Type[] GetOptionalCustomModifiers()
		{
			Type[] typeModifiers = this.GetTypeModifiers(true);
			if (typeModifiers == null)
			{
				return Type.EmptyTypes;
			}
			return typeModifiers;
		}

		public virtual Type[] GetRequiredCustomModifiers()
		{
			Type[] typeModifiers = this.GetTypeModifiers(false);
			if (typeModifiers == null)
			{
				return Type.EmptyTypes;
			}
			return typeModifiers;
		}

		public virtual object RawDefaultValue
		{
			get
			{
				return this.DefaultValue;
			}
		}

		protected Type ClassImpl;

		protected object DefaultValueImpl;

		protected MemberInfo MemberImpl;

		protected string NameImpl;

		protected int PositionImpl;

		protected ParameterAttributes AttrsImpl;

		private UnmanagedMarshal marshalAs;
	}
}
