using System;

namespace System.Runtime.InteropServices
{
	[ComVisible(true)]
	[AttributeUsage(AttributeTargets.Class, Inherited = false)]
	public sealed class ComDefaultInterfaceAttribute : Attribute
	{
		public ComDefaultInterfaceAttribute(Type defaultInterface)
		{
			this._type = defaultInterface;
		}

		public Type Value
		{
			get
			{
				return this._type;
			}
		}

		private Type _type;
	}
}
