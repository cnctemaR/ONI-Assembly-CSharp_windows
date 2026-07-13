using System;
using System.Runtime.CompilerServices;

namespace MonoMod.Core.Utils
{
	[NullableContext(1)]
	internal interface IInitialize<[Nullable(2)] T>
	{
		void Initialize(T value);
	}
}
