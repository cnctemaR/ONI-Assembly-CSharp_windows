using System;
using System.Runtime.CompilerServices;

namespace MonoMod.Core.Platforms
{
	[NullableContext(1)]
	internal interface INativeExceptionHelper
	{
		GetExceptionSlot GetExceptionSlot { get; }

		[NullableContext(2)]
		IntPtr CreateNativeToManagedHelper(IntPtr target, out IDisposable handle);

		[NullableContext(2)]
		IntPtr CreateManagedToNativeHelper(IntPtr target, out IDisposable handle);
	}
}
