using System;

namespace Mono.Cecil
{
	internal interface IMetadataScope : IMetadataTokenProvider
	{
		MetadataScopeType MetadataScopeType { get; }

		string Name { get; set; }
	}
}
