using System;

namespace System.Runtime.InteropServices
{
	[AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class, Inherited = false)]
	[ComVisible(true)]
	[Obsolete("This attribute is deprecated and will be removed in a future version.", false)]
	public sealed class IDispatchImplAttribute : Attribute
	{
		public IDispatchImplAttribute(IDispatchImplType implType)
		{
			this._val = implType;
		}

		public IDispatchImplAttribute(short implType)
		{
			this._val = (IDispatchImplType)implType;
		}

		public IDispatchImplType Value
		{
			get
			{
				return this._val;
			}
		}

		internal IDispatchImplType _val;
	}
}
