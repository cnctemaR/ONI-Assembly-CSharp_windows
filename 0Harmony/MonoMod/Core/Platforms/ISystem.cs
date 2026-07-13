using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using MonoMod.Utils;

namespace MonoMod.Core.Platforms
{
	[NullableContext(1)]
	internal interface ISystem
	{
		OSKind Target { get; }

		SystemFeature Features { get; }

		Abi? DefaultAbi { get; }

		IMemoryAllocator MemoryAllocator { get; }

		[Nullable(2)]
		INativeExceptionHelper NativeExceptionHelper
		{
			[NullableContext(2)]
			get;
		}

		[return: Nullable(new byte[] { 1, 2 })]
		IEnumerable<string> EnumerateLoadedModuleFiles();

		[return: NativeInteger]
		IntPtr GetSizeOfReadableMemory(IntPtr start, [NativeInteger] IntPtr guess);

		[NullableContext(0)]
		void PatchData(PatchTargetKind targetKind, IntPtr patchTarget, global::System.ReadOnlySpan<byte> data, global::System.Span<byte> backup);

		IntPtr GetNativeJitHookConfig(int runtimeMajMin);
	}
}
