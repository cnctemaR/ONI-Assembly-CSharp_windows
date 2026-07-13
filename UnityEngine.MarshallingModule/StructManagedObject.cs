using System;
using UnityEngine.Bindings;
using UnityEngine.Internal;

namespace UnityEngine
{
	[ExcludeFromDocs]
	[NativeHeader("Modules/Marshalling/MarshallingTests.h")]
	internal struct StructManagedObject
	{
		public MyManagedObject field;
	}
}
