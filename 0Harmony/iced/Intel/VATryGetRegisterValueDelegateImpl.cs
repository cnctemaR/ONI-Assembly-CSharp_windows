using System;
using System.Runtime.CompilerServices;

namespace Iced.Intel
{
	internal sealed class VATryGetRegisterValueDelegateImpl : IVATryGetRegisterValueProvider
	{
		[NullableContext(1)]
		public VATryGetRegisterValueDelegateImpl(VATryGetRegisterValue getRegisterValue)
		{
			this.getRegisterValue = getRegisterValue;
		}

		public bool TryGetRegisterValue(Register register, int elementIndex, int elementSize, out ulong value)
		{
			return this.getRegisterValue(register, elementIndex, elementSize, out value);
		}

		private readonly VATryGetRegisterValue getRegisterValue;
	}
}
