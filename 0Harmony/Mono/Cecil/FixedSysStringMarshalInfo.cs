using System;

namespace Mono.Cecil
{
	internal sealed class FixedSysStringMarshalInfo : MarshalInfo
	{
		public int Size
		{
			get
			{
				return this.size;
			}
			set
			{
				this.size = value;
			}
		}

		public FixedSysStringMarshalInfo()
			: base(NativeType.FixedSysString)
		{
			this.size = -1;
		}

		internal int size;
	}
}
