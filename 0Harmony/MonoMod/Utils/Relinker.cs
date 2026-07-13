using System;
using System.Runtime.CompilerServices;
using Mono.Cecil;

namespace MonoMod.Utils
{
	internal delegate IMetadataTokenProvider Relinker(IMetadataTokenProvider mtp, [Nullable(2)] IGenericParameterProvider context);
}
