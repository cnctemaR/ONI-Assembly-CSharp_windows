using System;
using System.Runtime.CompilerServices;

namespace MonoMod.Core.Platforms
{
	[NullableContext(2)]
	internal interface IAltEntryFactory
	{
		IntPtr CreateAlternateEntrypoint(IntPtr entrypoint, int minLength, out IDisposable handle);
	}
}
