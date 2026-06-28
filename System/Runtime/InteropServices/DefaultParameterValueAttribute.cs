using System;

namespace System.Runtime.InteropServices
{
	[AttributeUsage(AttributeTargets.Parameter)]
	public sealed class DefaultParameterValueAttribute : Attribute
	{
		public DefaultParameterValueAttribute(object value)
		{
			this.value = value;
		}

		public object Value
		{
			get
			{
				return this.value;
			}
		}

		private object value;
	}
}
