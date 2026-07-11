using System;
using System.Runtime.InteropServices;

namespace System.Runtime.CompilerServices
{
	[ComVisible(true)]
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Parameter, Inherited = false)]
	[Serializable]
	public sealed class DateTimeConstantAttribute : CustomConstantAttribute
	{
		public DateTimeConstantAttribute(long ticks)
		{
			this.ticks = ticks;
		}

		internal long Ticks
		{
			get
			{
				return this.ticks;
			}
		}

		public override object Value
		{
			get
			{
				return this.ticks;
			}
		}

		private long ticks;
	}
}
