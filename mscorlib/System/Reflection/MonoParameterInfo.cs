using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace System.Reflection
{
	[ComDefaultInterface(typeof(_ParameterInfo))]
	[ClassInterface(ClassInterfaceType.None)]
	[ComVisible(true)]
	[Serializable]
	[StructLayout(LayoutKind.Sequential)]
	internal class MonoParameterInfo : RuntimeParameterInfo
	{
		internal MonoParameterInfo(ParameterBuilder pb, Type type, MemberInfo member, int position)
		{
			this.ClassImpl = type;
			this.MemberImpl = member;
			if (pb != null)
			{
				this.NameImpl = pb.Name;
				this.PositionImpl = pb.Position - 1;
				this.AttrsImpl = (ParameterAttributes)pb.Attributes;
				return;
			}
			this.NameImpl = null;
			this.PositionImpl = position - 1;
			this.AttrsImpl = ParameterAttributes.None;
		}

		internal MonoParameterInfo(ParameterInfo pinfo, Type type, MemberInfo member, int position)
		{
			this.ClassImpl = type;
			this.MemberImpl = member;
			if (pinfo != null)
			{
				this.NameImpl = pinfo.Name;
				this.PositionImpl = pinfo.Position - 1;
				this.AttrsImpl = pinfo.Attributes;
				return;
			}
			this.NameImpl = null;
			this.PositionImpl = position - 1;
			this.AttrsImpl = ParameterAttributes.None;
		}

		internal MonoParameterInfo(ParameterInfo pinfo, MemberInfo member)
		{
			this.ClassImpl = pinfo.ParameterType;
			this.MemberImpl = member;
			this.NameImpl = pinfo.Name;
			this.PositionImpl = pinfo.Position;
			this.AttrsImpl = pinfo.Attributes;
			this.DefaultValueImpl = pinfo.GetDefaultValueImpl();
		}

		internal MonoParameterInfo(Type type, MemberInfo member, MarshalAsAttribute marshalAs)
		{
			this.ClassImpl = type;
			this.MemberImpl = member;
			this.NameImpl = "";
			this.PositionImpl = -1;
			this.AttrsImpl = ParameterAttributes.Retval;
			this.marshalAs = marshalAs;
		}

		public override object DefaultValue
		{
			get
			{
				if (this.ClassImpl == typeof(decimal))
				{
					DecimalConstantAttribute[] array = (DecimalConstantAttribute[])this.GetCustomAttributes(typeof(DecimalConstantAttribute), false);
					if (array.Length != 0)
					{
						return array[0].Value;
					}
				}
				else if (this.ClassImpl == typeof(DateTime))
				{
					DateTimeConstantAttribute[] array2 = (DateTimeConstantAttribute[])this.GetCustomAttributes(typeof(DateTimeConstantAttribute), false);
					if (array2.Length != 0)
					{
						return array2[0].Value;
					}
				}
				return this.DefaultValueImpl;
			}
		}

		public override object RawDefaultValue
		{
			get
			{
				return this.DefaultValue;
			}
		}

		public override int MetadataToken
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
					return methodInfo.GetParametersInternal()[this.PositionImpl].MetadataToken;
				}
				if (this.MemberImpl is MethodBase)
				{
					return base.GetMetadataToken();
				}
				throw new ArgumentException("Can't produce MetadataToken for member of type " + this.MemberImpl.GetType());
			}
		}

		public override object[] GetCustomAttributes(bool inherit)
		{
			return MonoCustomAttrs.GetCustomAttributes(this, inherit);
		}

		public override object[] GetCustomAttributes(Type attributeType, bool inherit)
		{
			return MonoCustomAttrs.GetCustomAttributes(this, attributeType, inherit);
		}

		public override bool IsDefined(Type attributeType, bool inherit)
		{
			return MonoCustomAttrs.IsDefined(this, attributeType, inherit);
		}

		public override IList<CustomAttributeData> GetCustomAttributesData()
		{
			return CustomAttributeData.GetCustomAttributes(this);
		}

		public override Type[] GetOptionalCustomModifiers()
		{
			Type[] typeModifiers = base.GetTypeModifiers(true);
			if (typeModifiers == null)
			{
				return Type.EmptyTypes;
			}
			return typeModifiers;
		}

		public override Type[] GetRequiredCustomModifiers()
		{
			Type[] typeModifiers = base.GetTypeModifiers(false);
			if (typeModifiers == null)
			{
				return Type.EmptyTypes;
			}
			return typeModifiers;
		}

		public override bool HasDefaultValue
		{
			get
			{
				object defaultValue = this.DefaultValue;
				return defaultValue == null || (!(defaultValue.GetType() == typeof(DBNull)) && !(defaultValue.GetType() == typeof(Missing)));
			}
		}
	}
}
