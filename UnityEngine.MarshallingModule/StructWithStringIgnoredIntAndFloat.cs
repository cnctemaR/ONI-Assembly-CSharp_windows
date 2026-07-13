using System;
using UnityEngine.Bindings;
using UnityEngine.Internal;

namespace UnityEngine
{
	[ExcludeFromDocs]
	internal struct StructWithStringIgnoredIntAndFloat
	{
		public string a;

		[Ignore]
		public int b;

		public float c;
	}
}
