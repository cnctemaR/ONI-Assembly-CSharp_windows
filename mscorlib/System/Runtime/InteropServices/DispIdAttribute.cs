using System;

namespace System.Runtime.InteropServices
{
	[ComVisible(true)]
	[AttributeUsage(AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Event, Inherited = false)]
	public sealed class DispIdAttribute : Attribute
	{
		public DispIdAttribute(int dispId)
		{
			this.id = dispId;
		}

		public int Value
		{
			get
			{
				return this.id;
			}
		}

		private int id;
	}
}
