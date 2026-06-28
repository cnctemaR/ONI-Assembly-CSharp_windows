using System;

namespace System.Runtime.InteropServices
{
	[AttributeUsage(AttributeTargets.Interface, Inherited = false)]
	[ComVisible(true)]
	public sealed class CoClassAttribute : Attribute
	{
		public CoClassAttribute(Type coClass)
		{
			this.klass = coClass;
		}

		public Type CoClass
		{
			get
			{
				return this.klass;
			}
		}

		private Type klass;
	}
}
