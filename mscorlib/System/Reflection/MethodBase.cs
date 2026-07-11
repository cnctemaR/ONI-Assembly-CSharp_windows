using System;
using System.Diagnostics;
using System.Globalization;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace System.Reflection
{
	[ComDefaultInterface(typeof(_MethodBase))]
	[ClassInterface(ClassInterfaceType.None)]
	[ComVisible(true)]
	[Serializable]
	public abstract class MethodBase : MemberInfo, _MethodBase
	{
		void _MethodBase.GetIDsOfNames([In] ref Guid riid, IntPtr rgszNames, uint cNames, uint lcid, IntPtr rgDispId)
		{
			throw new NotImplementedException();
		}

		void _MethodBase.GetTypeInfo(uint iTInfo, uint lcid, IntPtr ppTInfo)
		{
			throw new NotImplementedException();
		}

		void _MethodBase.GetTypeInfoCount(out uint pcTInfo)
		{
			throw new NotImplementedException();
		}

		void _MethodBase.Invoke(uint dispIdMember, [In] ref Guid riid, uint lcid, short wFlags, IntPtr pDispParams, IntPtr pVarResult, IntPtr pExcepInfo, IntPtr puArgErr)
		{
			throw new NotImplementedException();
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern MethodBase GetCurrentMethod();

		internal static MethodBase GetMethodFromHandleNoGenericCheck(RuntimeMethodHandle handle)
		{
			return MethodBase.GetMethodFromIntPtr(handle.Value, IntPtr.Zero);
		}

		private static MethodBase GetMethodFromIntPtr(IntPtr handle, IntPtr declaringType)
		{
			if (handle == IntPtr.Zero)
			{
				throw new ArgumentException("The handle is invalid.");
			}
			MethodBase methodFromHandleInternalType = MethodBase.GetMethodFromHandleInternalType(handle, declaringType);
			if (methodFromHandleInternalType == null)
			{
				throw new ArgumentException("The handle is invalid.");
			}
			return methodFromHandleInternalType;
		}

		public static MethodBase GetMethodFromHandle(RuntimeMethodHandle handle)
		{
			MethodBase methodFromIntPtr = MethodBase.GetMethodFromIntPtr(handle.Value, IntPtr.Zero);
			Type declaringType = methodFromIntPtr.DeclaringType;
			if (declaringType.IsGenericType || declaringType.IsGenericTypeDefinition)
			{
				throw new ArgumentException("Cannot resolve method because it's declared in a generic class.");
			}
			return methodFromIntPtr;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern MethodBase GetMethodFromHandleInternalType(IntPtr method_handle, IntPtr type_handle);

		[ComVisible(false)]
		public static MethodBase GetMethodFromHandle(RuntimeMethodHandle handle, RuntimeTypeHandle declaringType)
		{
			return MethodBase.GetMethodFromIntPtr(handle.Value, declaringType.Value);
		}

		public abstract MethodImplAttributes GetMethodImplementationFlags();

		public abstract ParameterInfo[] GetParameters();

		internal virtual int GetParameterCount()
		{
			ParameterInfo[] parameters = this.GetParameters();
			if (parameters == null)
			{
				return 0;
			}
			return parameters.Length;
		}

		[DebuggerHidden]
		[DebuggerStepThrough]
		public object Invoke(object obj, object[] parameters)
		{
			return this.Invoke(obj, BindingFlags.Default, null, parameters, null);
		}

		public abstract object Invoke(object obj, BindingFlags invokeAttr, Binder binder, object[] parameters, CultureInfo culture);

		public abstract RuntimeMethodHandle MethodHandle { get; }

		public abstract MethodAttributes Attributes { get; }

		public virtual CallingConventions CallingConvention
		{
			get
			{
				return CallingConventions.Standard;
			}
		}

		public bool IsPublic
		{
			get
			{
				return (this.Attributes & MethodAttributes.MemberAccessMask) == MethodAttributes.Public;
			}
		}

		public bool IsPrivate
		{
			get
			{
				return (this.Attributes & MethodAttributes.MemberAccessMask) == MethodAttributes.Private;
			}
		}

		public bool IsFamily
		{
			get
			{
				return (this.Attributes & MethodAttributes.MemberAccessMask) == MethodAttributes.Family;
			}
		}

		public bool IsAssembly
		{
			get
			{
				return (this.Attributes & MethodAttributes.MemberAccessMask) == MethodAttributes.Assembly;
			}
		}

		public bool IsFamilyAndAssembly
		{
			get
			{
				return (this.Attributes & MethodAttributes.MemberAccessMask) == MethodAttributes.FamANDAssem;
			}
		}

		public bool IsFamilyOrAssembly
		{
			get
			{
				return (this.Attributes & MethodAttributes.MemberAccessMask) == MethodAttributes.FamORAssem;
			}
		}

		public bool IsStatic
		{
			get
			{
				return (this.Attributes & MethodAttributes.Static) != MethodAttributes.PrivateScope;
			}
		}

		public bool IsFinal
		{
			get
			{
				return (this.Attributes & MethodAttributes.Final) != MethodAttributes.PrivateScope;
			}
		}

		public bool IsVirtual
		{
			get
			{
				return (this.Attributes & MethodAttributes.Virtual) != MethodAttributes.PrivateScope;
			}
		}

		public bool IsHideBySig
		{
			get
			{
				return (this.Attributes & MethodAttributes.HideBySig) != MethodAttributes.PrivateScope;
			}
		}

		public bool IsAbstract
		{
			get
			{
				return (this.Attributes & MethodAttributes.Abstract) != MethodAttributes.PrivateScope;
			}
		}

		public bool IsSpecialName
		{
			get
			{
				int attributes = (int)this.Attributes;
				return (attributes & 2048) != 0;
			}
		}

		[ComVisible(true)]
		public bool IsConstructor
		{
			get
			{
				int attributes = (int)this.Attributes;
				return (attributes & 4096) != 0 && this.Name == ".ctor";
			}
		}

		internal virtual int get_next_table_index(object obj, int table, bool inc)
		{
			if (this is MethodBuilder)
			{
				MethodBuilder methodBuilder = (MethodBuilder)this;
				return methodBuilder.get_next_table_index(obj, table, inc);
			}
			if (this is ConstructorBuilder)
			{
				ConstructorBuilder constructorBuilder = (ConstructorBuilder)this;
				return constructorBuilder.get_next_table_index(obj, table, inc);
			}
			throw new Exception("Method is not a builder method");
		}

		[ComVisible(true)]
		public virtual Type[] GetGenericArguments()
		{
			throw new NotSupportedException();
		}

		public virtual bool ContainsGenericParameters
		{
			get
			{
				return false;
			}
		}

		public virtual bool IsGenericMethodDefinition
		{
			get
			{
				return false;
			}
		}

		public virtual bool IsGenericMethod
		{
			get
			{
				return false;
			}
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern MethodBody GetMethodBodyInternal(IntPtr handle);

		internal static MethodBody GetMethodBody(IntPtr handle)
		{
			return MethodBase.GetMethodBodyInternal(handle);
		}

		public virtual MethodBody GetMethodBody()
		{
			throw new NotSupportedException();
		}

		virtual Type System.Runtime.InteropServices._MethodBase.GetType()
		{
			return base.GetType();
		}
	}
}
