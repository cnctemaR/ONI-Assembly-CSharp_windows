using System;

namespace Mono.Cecil.Pdb
{
	internal enum CustomMetadataType : byte
	{
		UsingInfo,
		ForwardInfo,
		IteratorScopes = 3,
		ForwardIterator
	}
}
