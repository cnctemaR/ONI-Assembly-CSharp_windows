using System;
using System.Runtime.InteropServices;
using System.Security.Permissions;

namespace System.Reflection
{
	[ComDefaultInterface(typeof(_MethodInfo))]
	[ComVisible(true)]
	[ClassInterface(ClassInterfaceType.None)]
	[PermissionSet(SecurityAction.InheritanceDemand, Name = "FullTrust")]
	[Serializable]
	public abstract class MethodInfo : MethodBase, _MethodInfo
	{
		public static bool operator ==(MethodInfo left, MethodInfo right)
		{
			return left == right || (left != null && right != null && !(left is RuntimeMethodInfo) && !(right is RuntimeMethodInfo) && left.Equals(right));
		}

		public static bool operator !=(MethodInfo left, MethodInfo right)
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

		public override MemberTypes MemberType
		{
			get
			{
				return MemberTypes.Method;
			}
		}

		public virtual Type ReturnType
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public virtual ParameterInfo ReturnParameter
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public abstract ICustomAttributeProvider ReturnTypeCustomAttributes { get; }

		public abstract MethodInfo GetBaseDefinition();

		[ComVisible(true)]
		public override Type[] GetGenericArguments()
		{
			throw new NotSupportedException(Environment.GetResourceString("Derived classes must provide an implementation."));
		}

		[ComVisible(true)]
		public virtual MethodInfo GetGenericMethodDefinition()
		{
			throw new NotSupportedException(Environment.GetResourceString("Derived classes must provide an implementation."));
		}

		public virtual MethodInfo MakeGenericMethod(params Type[] typeArguments)
		{
			throw new NotSupportedException(Environment.GetResourceString("Derived classes must provide an implementation."));
		}

		public virtual Delegate CreateDelegate(Type delegateType)
		{
			throw new NotSupportedException(Environment.GetResourceString("Derived classes must provide an implementation."));
		}

		public virtual Delegate CreateDelegate(Type delegateType, object target)
		{
			throw new NotSupportedException(Environment.GetResourceString("Derived classes must provide an implementation."));
		}

		Type _MethodInfo.GetType()
		{
			return base.GetType();
		}

		void _MethodInfo.GetTypeInfoCount(out uint pcTInfo)
		{
			throw new NotImplementedException();
		}

		void _MethodInfo.GetTypeInfo(uint iTInfo, uint lcid, IntPtr ppTInfo)
		{
			throw new NotImplementedException();
		}

		void _MethodInfo.GetIDsOfNames([In] ref Guid riid, IntPtr rgszNames, uint cNames, uint lcid, IntPtr rgDispId)
		{
			throw new NotImplementedException();
		}

		void _MethodInfo.Invoke(uint dispIdMember, [In] ref Guid riid, uint lcid, short wFlags, IntPtr pDispParams, IntPtr pVarResult, IntPtr pExcepInfo, IntPtr puArgErr)
		{
			throw new NotImplementedException();
		}

		internal virtual MethodInfo GetBaseMethod()
		{
			return this;
		}
	}
}
