using System;
using Mono.Cecil;

namespace MonoMod.Utils
{
	public delegate IMetadataTokenProvider Relinker(IMetadataTokenProvider mtp, IGenericParameterProvider context);
}
