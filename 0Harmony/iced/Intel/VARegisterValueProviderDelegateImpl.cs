using System;
using System.Runtime.CompilerServices;

namespace Iced.Intel
{
	internal sealed class VARegisterValueProviderDelegateImpl : IVATryGetRegisterValueProvider
	{
		[NullableContext(1)]
		public VARegisterValueProviderDelegateImpl(VAGetRegisterValue getRegisterValue)
		{
			if (getRegisterValue == null)
			{
				throw new ArgumentNullException("getRegisterValue");
			}
			this.getRegisterValue = getRegisterValue;
		}

		public bool TryGetRegisterValue(Register register, int elementIndex, int elementSize, out ulong value)
		{
			value = this.getRegisterValue(register, elementIndex, elementSize);
			return true;
		}

		private readonly VAGetRegisterValue getRegisterValue;
	}
}
