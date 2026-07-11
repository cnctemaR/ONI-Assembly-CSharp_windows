using System;

namespace System.Runtime.InteropServices
{
	[AttributeUsage(AttributeTargets.Interface, Inherited = false)]
	[ComVisible(true)]
	public sealed class CoClassAttribute : Attribute
	{
		public CoClassAttribute(Type coClass)
		{
			this._CoClass = coClass;
		}

		public Type CoClass
		{
			get
			{
				return this._CoClass;
			}
		}

		internal Type _CoClass;
	}
}
