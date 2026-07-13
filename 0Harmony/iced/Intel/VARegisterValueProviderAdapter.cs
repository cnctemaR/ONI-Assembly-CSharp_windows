using System;
using System.Runtime.CompilerServices;

namespace Iced.Intel
{
	internal sealed class VARegisterValueProviderAdapter : IVATryGetRegisterValueProvider
	{
		[NullableContext(1)]
		public VARegisterValueProviderAdapter(IVARegisterValueProvider provider)
		{
			this.provider = provider;
		}

		public bool TryGetRegisterValue(Register register, int elementIndex, int elementSize, out ulong value)
		{
			value = this.provider.GetRegisterValue(register, elementIndex, elementSize);
			return true;
		}

		private readonly IVARegisterValueProvider provider;
	}
}
