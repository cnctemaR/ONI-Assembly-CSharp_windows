using System;
using System.Runtime.CompilerServices;

namespace MonoMod.Core.Interop.Attributes
{
	[NullableContext(1)]
	[Nullable(0)]
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
	internal sealed class MultipurposeSlotOffsetTableAttribute : Attribute
	{
		public int Bits { get; }

		public Type HelperType { get; }

		public MultipurposeSlotOffsetTableAttribute(int bits, Type helperType)
		{
			this.Bits = bits;
			this.HelperType = helperType;
		}
	}
}
