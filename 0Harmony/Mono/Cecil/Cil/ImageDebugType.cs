using System;

namespace Mono.Cecil.Cil
{
	internal enum ImageDebugType
	{
		CodeView = 2,
		Deterministic = 16,
		EmbeddedPortablePdb,
		PdbChecksum = 19
	}
}
