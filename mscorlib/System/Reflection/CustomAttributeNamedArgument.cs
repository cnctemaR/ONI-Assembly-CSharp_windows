using System;
using System.Runtime.InteropServices;

namespace System.Reflection
{
	[ComVisible(true)]
	[Serializable]
	public struct CustomAttributeNamedArgument
	{
		public CustomAttributeNamedArgument(MemberInfo memberInfo, object value)
		{
			this.memberInfo = memberInfo;
			this.typedArgument = (CustomAttributeTypedArgument)value;
		}

		public CustomAttributeNamedArgument(MemberInfo memberInfo, CustomAttributeTypedArgument typedArgument)
		{
			this.memberInfo = memberInfo;
			this.typedArgument = typedArgument;
		}

		public MemberInfo MemberInfo
		{
			get
			{
				return this.memberInfo;
			}
		}

		public CustomAttributeTypedArgument TypedValue
		{
			get
			{
				return this.typedArgument;
			}
		}

		public bool IsField
		{
			get
			{
				return this.memberInfo.MemberType == MemberTypes.Field;
			}
		}

		public string MemberName
		{
			get
			{
				return this.memberInfo.Name;
			}
		}

		public override string ToString()
		{
			return this.memberInfo.Name + " = " + this.typedArgument.ToString();
		}

		public override bool Equals(object obj)
		{
			if (!(obj is CustomAttributeNamedArgument))
			{
				return false;
			}
			CustomAttributeNamedArgument customAttributeNamedArgument = (CustomAttributeNamedArgument)obj;
			return customAttributeNamedArgument.memberInfo == this.memberInfo && this.typedArgument.Equals(customAttributeNamedArgument.typedArgument);
		}

		public override int GetHashCode()
		{
			return (this.memberInfo.GetHashCode() << 16) + this.typedArgument.GetHashCode();
		}

		public static bool operator ==(CustomAttributeNamedArgument left, CustomAttributeNamedArgument right)
		{
			return left.Equals(right);
		}

		public static bool operator !=(CustomAttributeNamedArgument left, CustomAttributeNamedArgument right)
		{
			return !left.Equals(right);
		}

		private CustomAttributeTypedArgument typedArgument;

		private MemberInfo memberInfo;
	}
}
