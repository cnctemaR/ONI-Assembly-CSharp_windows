using System;

namespace Mono.Cecil
{
	internal enum AssemblyHashAlgorithm : uint
	{
		None,
		MD5 = 32771U,
		SHA1,
		SHA256 = 32780U,
		SHA384,
		SHA512,
		Reserved = 32771U
	}
}
