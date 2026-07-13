using System;

namespace Mono.Cecil
{
	[Flags]
	internal enum ManifestResourceAttributes : uint
	{
		VisibilityMask = 7U,
		Public = 1U,
		Private = 2U
	}
}
