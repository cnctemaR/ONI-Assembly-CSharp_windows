using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Security;
using System.Text;

namespace System.Reflection
{
	[ComDefaultInterface(typeof(_ParameterInfo))]
	[ClassInterface(ClassInterfaceType.None)]
	[ComVisible(true)]
	[Serializable]
	[StructLayout(LayoutKind.Sequential)]
	public class ParameterInfo : ICustomAttributeProvider, _ParameterInfo, IObjectReference
	{
		protected ParameterInfo()
		{
		}

		public override string ToString()
		{
			Type type = this.ClassImpl;
			while (type.HasElementType)
			{
				type = type.GetElementType();
			}
			string text = ((type.IsPrimitive || this.ClassImpl == typeof(void) || this.ClassImpl.Namespace == this.MemberImpl.DeclaringType.Namespace) ? this.ClassImpl.Name : this.ClassImpl.FullName);
			if (!this.IsRetval)
			{
				text += " ";
				text += this.NameImpl;
			}
			return text;
		}

		internal static void FormatParameters(StringBuilder sb, ParameterInfo[] p, CallingConventions callingConvention, bool serialization)
		{
			for (int i = 0; i < p.Length; i++)
			{
				if (i > 0)
				{
					sb.Append(", ");
				}
				Type parameterType = p[i].ParameterType;
				string text = parameterType.FormatTypeName(serialization);
				if (parameterType.IsByRef && !serialization)
				{
					sb.Append(text.TrimEnd(new char[] { '&' }));
					sb.Append(" ByRef");
				}
				else
				{
					sb.Append(text);
				}
			}
			if ((callingConvention & CallingConventions.VarArgs) != (CallingConventions)0)
			{
				if (p.Length != 0)
				{
					sb.Append(", ");
				}
				sb.Append("...");
			}
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

		public bool IsIn
		{
			get
			{
				return (this.Attributes & ParameterAttributes.In) > ParameterAttributes.None;
			}
		}

		public bool IsLcid
		{
			get
			{
				return (this.Attributes & ParameterAttributes.Lcid) > ParameterAttributes.None;
			}
		}

		public bool IsOptional
		{
			get
			{
				return (this.Attributes & ParameterAttributes.Optional) > ParameterAttributes.None;
			}
		}

		public bool IsOut
		{
			get
			{
				return (this.Attributes & ParameterAttributes.Out) > ParameterAttributes.None;
			}
		}

		public bool IsRetval
		{
			get
			{
				return (this.Attributes & ParameterAttributes.Retval) > ParameterAttributes.None;
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
		internal extern int GetMetadataToken();

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
				array[num++] = this.marshalAs.Copy();
			}
			return array;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern Type[] GetTypeModifiers(bool optional);

		internal object GetDefaultValueImpl()
		{
			return this.DefaultValueImpl;
		}

		public virtual IEnumerable<CustomAttributeData> CustomAttributes
		{
			get
			{
				return this.GetCustomAttributesData();
			}
		}

		public virtual bool HasDefaultValue
		{
			get
			{
				throw new NotImplementedException();
			}
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

		public virtual object DefaultValue
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public virtual object RawDefaultValue
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public virtual int MetadataToken
		{
			get
			{
				return 134217728;
			}
		}

		public virtual object[] GetCustomAttributes(bool inherit)
		{
			return new object[0];
		}

		public virtual object[] GetCustomAttributes(Type attributeType, bool inherit)
		{
			return new object[0];
		}

		[SecurityCritical]
		public object GetRealObject(StreamingContext context)
		{
			throw new NotImplementedException();
		}

		public virtual bool IsDefined(Type attributeType, bool inherit)
		{
			return false;
		}

		public virtual Type[] GetRequiredCustomModifiers()
		{
			return new Type[0];
		}

		public virtual Type[] GetOptionalCustomModifiers()
		{
			return new Type[0];
		}

		public virtual IList<CustomAttributeData> GetCustomAttributesData()
		{
			throw new NotImplementedException();
		}

		internal static ParameterInfo New(ParameterBuilder pb, Type type, MemberInfo member, int position)
		{
			return new MonoParameterInfo(pb, type, member, position);
		}

		internal static ParameterInfo New(ParameterInfo pinfo, Type type, MemberInfo member, int position)
		{
			return new MonoParameterInfo(pinfo, type, member, position);
		}

		internal static ParameterInfo New(ParameterInfo pinfo, MemberInfo member)
		{
			return new MonoParameterInfo(pinfo, member);
		}

		internal static ParameterInfo New(Type type, MemberInfo member, MarshalAsAttribute marshalAs)
		{
			return new MonoParameterInfo(type, member, marshalAs);
		}

		protected Type ClassImpl;

		protected object DefaultValueImpl;

		protected MemberInfo MemberImpl;

		protected string NameImpl;

		protected int PositionImpl;

		protected ParameterAttributes AttrsImpl;

		internal MarshalAsAttribute marshalAs;
	}
}
