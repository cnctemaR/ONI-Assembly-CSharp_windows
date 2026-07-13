using System;
using UnityEngine.Internal;

namespace UnityEngine
{
	[ExcludeFromDocs]
	internal struct StructWithNonBlittableArrayField
	{
		public StructWithStringIntAndFloat[] field;
	}
}
