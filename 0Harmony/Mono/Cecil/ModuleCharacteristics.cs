using System;

namespace Mono.Cecil
{
	[Flags]
	internal enum ModuleCharacteristics
	{
		HighEntropyVA = 32,
		DynamicBase = 64,
		NoSEH = 1024,
		NXCompat = 256,
		AppContainer = 4096,
		TerminalServerAware = 32768
	}
}
