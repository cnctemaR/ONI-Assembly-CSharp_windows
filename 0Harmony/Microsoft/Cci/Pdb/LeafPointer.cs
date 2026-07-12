using System;

namespace Microsoft.Cci.Pdb
{
	internal struct LeafPointer
	{
		internal struct LeafPointerBody
		{
			internal uint utype;

			internal LeafPointerAttr attr;
		}
	}
}
