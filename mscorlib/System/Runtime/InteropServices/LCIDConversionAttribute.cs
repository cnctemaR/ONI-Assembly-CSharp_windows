using System;

namespace System.Runtime.InteropServices
{
	[AttributeUsage(AttributeTargets.Method, Inherited = false)]
	[ComVisible(true)]
	public sealed class LCIDConversionAttribute : Attribute
	{
		public LCIDConversionAttribute(int lcid)
		{
			this.id = lcid;
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
