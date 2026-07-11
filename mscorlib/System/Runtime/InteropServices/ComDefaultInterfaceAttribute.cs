using System;

namespace System.Runtime.InteropServices
{
	[AttributeUsage(AttributeTargets.Class, Inherited = false)]
	[ComVisible(true)]
	public sealed class ComDefaultInterfaceAttribute : Attribute
	{
		public ComDefaultInterfaceAttribute(Type defaultInterface)
		{
			this._val = defaultInterface;
		}

		public Type Value
		{
			get
			{
				return this._val;
			}
		}

		internal Type _val;
	}
}
