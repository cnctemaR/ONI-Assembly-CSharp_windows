using System;
using System.Runtime.InteropServices;

namespace System.Runtime.CompilerServices
{
	[ComVisible(true)]
	[AttributeUsage(AttributeTargets.Field)]
	public sealed class AccessedThroughPropertyAttribute : Attribute
	{
		public AccessedThroughPropertyAttribute(string propertyName)
		{
			this.propertyName = propertyName;
		}

		public string PropertyName
		{
			get
			{
				return this.propertyName;
			}
		}

		private readonly string propertyName;
	}
}
