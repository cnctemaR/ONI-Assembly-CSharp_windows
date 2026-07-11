using System;

namespace System.Runtime.InteropServices
{
	[AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
	[ComVisible(true)]
	[Obsolete]
	public sealed class IDispatchImplAttribute : Attribute
	{
		public IDispatchImplAttribute(IDispatchImplType implType)
		{
			this.Impl = implType;
		}

		public IDispatchImplAttribute(short implType)
		{
			this.Impl = (IDispatchImplType)implType;
		}

		public IDispatchImplType Value
		{
			get
			{
				return this.Impl;
			}
		}

		private IDispatchImplType Impl;
	}
}
