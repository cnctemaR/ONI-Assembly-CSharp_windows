using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security.Permissions;

namespace System.Reflection
{
	[ComVisible(true)]
	[ClassInterface(ClassInterfaceType.None)]
	[ComDefaultInterface(typeof(_MemberInfo))]
	[PermissionSet(SecurityAction.InheritanceDemand, Name = "FullTrust")]
	[Serializable]
	public abstract class MemberInfo : ICustomAttributeProvider, _MemberInfo
	{
		internal virtual bool CacheEquals(object o)
		{
			throw new NotImplementedException();
		}

		public abstract MemberTypes MemberType { get; }

		public abstract string Name { get; }

		public abstract Type DeclaringType { get; }

		public abstract Type ReflectedType { get; }

		public virtual IEnumerable<CustomAttributeData> CustomAttributes
		{
			get
			{
				return this.GetCustomAttributesData();
			}
		}

		public abstract object[] GetCustomAttributes(bool inherit);

		public abstract object[] GetCustomAttributes(Type attributeType, bool inherit);

		public abstract bool IsDefined(Type attributeType, bool inherit);

		public virtual IList<CustomAttributeData> GetCustomAttributesData()
		{
			throw new NotImplementedException();
		}

		public virtual extern int MetadataToken
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public virtual Module Module
		{
			get
			{
				if (this is Type)
				{
					return ((Type)this).Module;
				}
				throw new NotImplementedException();
			}
		}

		public static bool operator ==(MemberInfo left, MemberInfo right)
		{
			if (left == right)
			{
				return true;
			}
			if (left == null || right == null)
			{
				return false;
			}
			Type type;
			Type type2;
			if ((type = left as Type) != null && (type2 = right as Type) != null)
			{
				return type == type2;
			}
			MethodBase methodBase;
			MethodBase methodBase2;
			if ((methodBase = left as MethodBase) != null && (methodBase2 = right as MethodBase) != null)
			{
				return methodBase == methodBase2;
			}
			FieldInfo fieldInfo;
			FieldInfo fieldInfo2;
			if ((fieldInfo = left as FieldInfo) != null && (fieldInfo2 = right as FieldInfo) != null)
			{
				return fieldInfo == fieldInfo2;
			}
			EventInfo eventInfo;
			EventInfo eventInfo2;
			if ((eventInfo = left as EventInfo) != null && (eventInfo2 = right as EventInfo) != null)
			{
				return eventInfo == eventInfo2;
			}
			PropertyInfo propertyInfo;
			PropertyInfo propertyInfo2;
			return (propertyInfo = left as PropertyInfo) != null && (propertyInfo2 = right as PropertyInfo) != null && propertyInfo == propertyInfo2;
		}

		public static bool operator !=(MemberInfo left, MemberInfo right)
		{
			return !(left == right);
		}

		public override bool Equals(object obj)
		{
			return base.Equals(obj);
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		Type _MemberInfo.GetType()
		{
			return base.GetType();
		}

		void _MemberInfo.GetTypeInfoCount(out uint pcTInfo)
		{
			throw new NotImplementedException();
		}

		void _MemberInfo.GetTypeInfo(uint iTInfo, uint lcid, IntPtr ppTInfo)
		{
			throw new NotImplementedException();
		}

		void _MemberInfo.GetIDsOfNames([In] ref Guid riid, IntPtr rgszNames, uint cNames, uint lcid, IntPtr rgDispId)
		{
			throw new NotImplementedException();
		}

		void _MemberInfo.Invoke(uint dispIdMember, [In] ref Guid riid, uint lcid, short wFlags, IntPtr pDispParams, IntPtr pVarResult, IntPtr pExcepInfo, IntPtr puArgErr)
		{
			throw new NotImplementedException();
		}
	}
}
