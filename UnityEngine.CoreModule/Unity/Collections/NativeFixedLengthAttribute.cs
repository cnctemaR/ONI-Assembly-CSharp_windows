using System;
using UnityEngine.Scripting;

namespace Unity.Collections
{
	[AttributeUsage(AttributeTargets.Field)]
	[RequiredByNativeCode]
	public sealed class NativeFixedLengthAttribute : Attribute
	{
		public NativeFixedLengthAttribute(int fixedLength)
		{
			this.FixedLength = fixedLength;
		}

		public int FixedLength;
	}
}
