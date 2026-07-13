using System;
using UnityEngine.Scripting;

namespace UnityEngine.TextCore.LowLevel
{
	[UsedByNativeCode]
	internal struct OTL_Tag
	{
		public unsafe override string ToString()
		{
			char* ptr = stackalloc char[(UIntPtr)8];
			*ptr = (char)this.c0;
			ptr[1] = (char)this.c1;
			ptr[2] = (char)this.c2;
			ptr[3] = (char)this.c3;
			return new string(ptr);
		}

		public byte c0;

		public byte c1;

		public byte c2;

		public byte c3;

		public byte c4;
	}
}
