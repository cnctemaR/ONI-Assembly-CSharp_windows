using System;

namespace System.Runtime.InteropServices
{
	[AttributeUsage(AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Event, Inherited = false)]
	[ComVisible(true)]
	public sealed class DispIdAttribute : Attribute
	{
		public DispIdAttribute(int dispId)
		{
			this._val = dispId;
		}

		public int Value
		{
			get
			{
				return this._val;
			}
		}

		internal int _val;
	}
}
