using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace MonoMod.Core.Platforms.Memory
{
	internal abstract class QueryingMemoryPageAllocatorBase
	{
		public abstract uint PageSize { get; }

		public abstract bool TryQueryPage(IntPtr pageAddr, out bool isFree, out IntPtr allocBase, [NativeInteger] out IntPtr allocSize);

		public abstract bool TryAllocatePage([NativeInteger] IntPtr size, bool executable, out IntPtr allocated);

		public abstract bool TryAllocatePage(IntPtr pageAddr, [NativeInteger] IntPtr size, bool executable, out IntPtr allocated);

		[NullableContext(2)]
		public abstract bool TryFreePage(IntPtr pageAddr, [<24b3ba8a-00b7-40fc-a603-2711fa115297>NotNullWhen(false)] out string errorMsg);
	}
}
