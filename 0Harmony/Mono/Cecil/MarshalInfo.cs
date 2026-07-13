using System;

namespace Mono.Cecil
{
	internal class MarshalInfo
	{
		public NativeType NativeType
		{
			get
			{
				return this.native;
			}
			set
			{
				this.native = value;
			}
		}

		public MarshalInfo(NativeType native)
		{
			this.native = native;
		}

		internal NativeType native;
	}
}
