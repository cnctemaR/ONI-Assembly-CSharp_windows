using System;

namespace System.Runtime.InteropServices
{
	[ComVisible(true)]
	[AttributeUsage(AttributeTargets.Module, Inherited = false)]
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
