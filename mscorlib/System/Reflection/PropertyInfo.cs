using System;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.InteropServices;

namespace System.Reflection
{
	[ClassInterface(ClassInterfaceType.None)]
	[ComVisible(true)]
	[ComDefaultInterface(typeof(_PropertyInfo))]
	[Serializable]
	public abstract class PropertyInfo : MemberInfo, _PropertyInfo
	{
		void _PropertyInfo.GetIDsOfNames([In] ref Guid riid, IntPtr rgszNames, uint cNames, uint lcid, IntPtr rgDispId)
		{
			throw new NotImplementedException();
		}

		void _PropertyInfo.GetTypeInfo(uint iTInfo, uint lcid, IntPtr ppTInfo)
		{
			throw new NotImplementedException();
		}

		void _PropertyInfo.GetTypeInfoCount(out uint pcTInfo)
		{
			throw new NotImplementedException();
		}

		void _PropertyInfo.Invoke(uint dispIdMember, [In] ref Guid riid, uint lcid, short wFlags, IntPtr pDispParams, IntPtr pVarResult, IntPtr pExcepInfo, IntPtr puArgErr)
		{
			throw new NotImplementedException();
		}

		public abstract PropertyAttributes Attributes { get; }

		public abstract bool CanRead { get; }

		public abstract bool CanWrite { get; }

		public bool IsSpecialName
		{
			get
			{
				return (this.Attributes & PropertyAttributes.SpecialName) != PropertyAttributes.None;
			}
		}

		public override MemberTypes MemberType
		{
			get
			{
				return MemberTypes.Property;
			}
		}

		public abstract Type PropertyType { get; }

		public MethodInfo[] GetAccessors()
		{
			return this.GetAccessors(false);
		}

		public abstract MethodInfo[] GetAccessors(bool nonPublic);

		public MethodInfo GetGetMethod()
		{
			return this.GetGetMethod(false);
		}

		public abstract MethodInfo GetGetMethod(bool nonPublic);

		public abstract ParameterInfo[] GetIndexParameters();

		public MethodInfo GetSetMethod()
		{
			return this.GetSetMethod(false);
		}

		public abstract MethodInfo GetSetMethod(bool nonPublic);

		[DebuggerStepThrough]
		[DebuggerHidden]
		public virtual object GetValue(object obj, object[] index)
		{
			return this.GetValue(obj, BindingFlags.Default, null, index, null);
		}

		public abstract object GetValue(object obj, BindingFlags invokeAttr, Binder binder, object[] index, CultureInfo culture);

		[DebuggerStepThrough]
		[DebuggerHidden]
		public virtual void SetValue(object obj, object value, object[] index)
		{
			this.SetValue(obj, value, BindingFlags.Default, null, index, null);
		}

		public abstract void SetValue(object obj, object value, BindingFlags invokeAttr, Binder binder, object[] index, CultureInfo culture);

		public virtual Type[] GetOptionalCustomModifiers()
		{
			return Type.EmptyTypes;
		}

		public virtual Type[] GetRequiredCustomModifiers()
		{
			return Type.EmptyTypes;
		}

		[MonoTODO("Not implemented")]
		public virtual object GetConstantValue()
		{
			throw new NotImplementedException();
		}

		[MonoTODO("Not implemented")]
		public virtual object GetRawConstantValue()
		{
			throw new NotImplementedException();
		}

		virtual Type System.Runtime.InteropServices._PropertyInfo.GetType()
		{
			return base.GetType();
		}
	}
}
