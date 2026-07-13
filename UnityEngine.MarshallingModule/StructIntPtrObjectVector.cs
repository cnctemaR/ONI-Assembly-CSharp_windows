using System;
using UnityEngine.Bindings;
using UnityEngine.Internal;

namespace UnityEngine
{
	[NativeHeader("Modules/Marshalling/MarshallingTests.h")]
	[ExcludeFromDocs]
	internal struct StructIntPtrObjectVector
	{
		public MyIntPtrObject[] field;
	}
}
