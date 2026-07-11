using System;

namespace System.Runtime.InteropServices
{
	[AttributeUsage(AttributeTargets.Field, Inherited = false)]
	[ComVisible(true)]
	public sealed class FieldOffsetAttribute : Attribute
	{
		public FieldOffsetAttribute(int offset)
		{
			this.val = offset;
		}

		public int Value
		{
			get
			{
				return this.val;
			}
		}

		private int val;
	}
}
