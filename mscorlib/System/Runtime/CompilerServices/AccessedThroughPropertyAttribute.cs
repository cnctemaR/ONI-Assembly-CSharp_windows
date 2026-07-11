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
			this.name = propertyName;
		}

		public string PropertyName
		{
			get
			{
				return this.name;
			}
		}

		private string name;
	}
}
