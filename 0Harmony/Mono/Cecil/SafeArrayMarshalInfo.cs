using System;

namespace Mono.Cecil
{
	internal sealed class SafeArrayMarshalInfo : MarshalInfo
	{
		public VariantType ElementType
		{
			get
			{
				return this.element_type;
			}
			set
			{
				this.element_type = value;
			}
		}

		public SafeArrayMarshalInfo()
			: base(NativeType.SafeArray)
		{
			this.element_type = VariantType.None;
		}

		internal VariantType element_type;
	}
}
