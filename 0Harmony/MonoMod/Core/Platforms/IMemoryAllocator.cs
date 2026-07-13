using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace MonoMod.Core.Platforms
{
	[NullableContext(1)]
	internal interface IMemoryAllocator
	{
		int MaxSize { get; }

		bool TryAllocate(AllocationRequest request, [<24b3ba8a-00b7-40fc-a603-2711fa115297>MaybeNullWhen(false)] out IAllocatedMemory allocated);

		bool TryAllocateInRange(PositionedAllocationRequest request, [<24b3ba8a-00b7-40fc-a603-2711fa115297>MaybeNullWhen(false)] out IAllocatedMemory allocated);
	}
}
