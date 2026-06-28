using System;

namespace FileHelpers
{
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
	public sealed class FieldOrderAttribute : Attribute
	{
		public int Order { get; private set; }

		public FieldOrderAttribute(int order)
		{
			this.Order = order;
		}
	}
}
