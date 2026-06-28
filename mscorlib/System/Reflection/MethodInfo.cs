using System;
using System.Runtime.InteropServices;

namespace System.Reflection
{
	[ComVisible(true)]
	[ComDefaultInterface(typeof(_MethodInfo))]
	[ClassInterface(ClassInterfaceType.None)]
	[Serializable]
	public abstract class MethodInfo : MethodBase, _MethodInfo
	{
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

		public abstract MethodInfo GetBaseDefinition();

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
				return null;
			}
		}

		public abstract ICustomAttributeProvider ReturnTypeCustomAttributes { get; }

		[ComVisible(true)]
		public virtual MethodInfo GetGenericMethodDefinition()
		{
			throw new NotSupportedException();
		}

		public virtual MethodInfo MakeGenericMethod(params Type[] typeArguments)
		{
			throw new NotSupportedException(base.GetType().ToString());
		}

		[ComVisible(true)]
		public override Type[] GetGenericArguments()
		{
			return Type.EmptyTypes;
		}

		public override bool IsGenericMethod
		{
			get
			{
				return false;
			}
		}

		public override bool IsGenericMethodDefinition
		{
			get
			{
				return false;
			}
		}

		public override bool ContainsGenericParameters
		{
			get
			{
				return false;
			}
		}

		public virtual ParameterInfo ReturnParameter
		{
			get
			{
				throw new NotSupportedException();
			}
		}

		virtual Type System.Runtime.InteropServices._MethodInfo.GetType()
		{
			return base.GetType();
		}
	}
}
