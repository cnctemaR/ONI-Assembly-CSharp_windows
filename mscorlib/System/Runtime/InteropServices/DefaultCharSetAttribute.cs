using System;

namespace System.Runtime.InteropServices
{
	[AttributeUsage(AttributeTargets.Module, Inherited = false)]
	[ComVisible(true)]
	public sealed class DefaultCharSetAttribute : Attribute
	{
		public DefaultCharSetAttribute(CharSet charSet)
		{
			this._CharSet = charSet;
		}

		public CharSet CharSet
		{
			get
			{
				return this._CharSet;
			}
		}

		internal CharSet _CharSet;
	}
}
