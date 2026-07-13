using System;
using System.Runtime.CompilerServices;

namespace MonoMod.Core.Platforms.Runtimes
{
	[NullableContext(1)]
	[Nullable(0)]
	internal sealed class FxCLR2Runtime : FxBaseRuntime
	{
		public FxCLR2Runtime(ISystem system)
		{
			this.system = system;
			Abi? abiCore = this.AbiCore;
			if (abiCore == null)
			{
				this.AbiCore = system.DefaultAbi;
			}
		}

		private readonly ISystem system;
	}
}
