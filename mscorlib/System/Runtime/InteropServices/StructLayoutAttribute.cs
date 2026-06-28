using System;

namespace System.Runtime.InteropServices
{
	[ComVisible(true)]
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, Inherited = false)]
	public sealed class StructLayoutAttribute : Attribute
	{
		public StructLayoutAttribute(short layoutKind)
		{
			this.lkind = (LayoutKind)layoutKind;
		}

		public StructLayoutAttribute(LayoutKind layoutKind)
		{
			this.lkind = layoutKind;
		}

		public LayoutKind Value
		{
			get
			{
				return this.lkind;
			}
		}

		public CharSet CharSet = CharSet.Auto;

		public int Pack = 8;

		public int Size;

		private LayoutKind lkind;
	}
}
