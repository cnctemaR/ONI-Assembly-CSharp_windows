using System;

namespace System.Runtime.InteropServices
{
	[AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Enum | AttributeTargets.Interface | AttributeTargets.Delegate, Inherited = false)]
	[ComVisible(true)]
	public sealed class GuidAttribute : Attribute
	{
		public GuidAttribute(string guid)
		{
			this.guidValue = guid;
		}

		public string Value
		{
			get
			{
				return this.guidValue;
			}
		}

		private string guidValue;
	}
}
