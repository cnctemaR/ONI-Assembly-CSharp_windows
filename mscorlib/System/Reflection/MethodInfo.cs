using System;
using System.Runtime.InteropServices;

namespace System.Reflection
{
	[ClassInterface(ClassInterfaceType.None)]
	[ComVisible(true)]
	[ComDefaultInterface(typeof(_MethodInfo))]
	[Serializable]
	public abstract class MethodInfo : MethodBase, _MethodInfo
	{
		public override MemberTypes MemberType
		{
			get
			{
				return MemberTypes.Method;
			}
		}

		public virtual ParameterInfo ReturnParameter
		{
			get
			{
				throw NotImplemented.ByDesign;
			}
		}

		public virtual Type ReturnType
		{
			get
			{
				throw NotImplemented.ByDesign;
			}
		}

		public override Type[] GetGenericArguments()
		{
			throw new NotSupportedException("Derived classes must provide an implementation.");
		}

		public virtual MethodInfo GetGenericMethodDefinition()
		{
			throw new NotSupportedException("Derived classes must provide an implementation.");
		}

		public virtual MethodInfo MakeGenericMethod(params Type[] typeArguments)
		{
			throw new NotSupportedException("Derived classes must provide an implementation.");
		}

		public abstract MethodInfo GetBaseDefinition();

		public abstract ICustomAttributeProvider ReturnTypeCustomAttributes { get; }

		public virtual Delegate CreateDelegate(Type delegateType)
		{
			throw new NotSupportedException("Derived classes must provide an implementation.");
		}

		public virtual Delegate CreateDelegate(Type delegateType, object target)
		{
			throw new NotSupportedException("Derived classes must provide an implementation.");
		}

		public override bool Equals(object obj)
		{
			return base.Equals(obj);
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public static bool operator ==(MethodInfo left, MethodInfo right)
		{
			return left == right || (left != null && right != null && left.Equals(right));
		}

		public static bool operator !=(MethodInfo left, MethodInfo right)
		{
			return !(left == right);
		}

		void _MethodInfo.GetIDsOfNames([In] ref Guid riid, IntPtr rgszNames, uint cNames, uint lcid, IntPtr rgDispId)
		{
			throw new NotImplementedException();
		}

		void _MethodInfo.GetTypeInfo(uint iTInfo, uint lcid, IntPtr ppTInfo)
		{
			throw new NotImplementedException();
		}

		void _MethodInfo.GetTypeInfoCount(out uint pcTInfo)
		{
			throw new NotImplementedException();
		}

		void _MethodInfo.Invoke(uint dispIdMember, [In] ref Guid riid, uint lcid, short wFlags, IntPtr pDispParams, IntPtr pVarResult, IntPtr pExcepInfo, IntPtr puArgErr)
		{
			throw new NotImplementedException();
		}

		Type _MethodInfo.GetType()
		{
			return base.GetType();
		}

		internal virtual int GenericParameterCount
		{
			get
			{
				return this.GetGenericArguments().Length;
			}
		}
	}
}
