using System;
using UnityEngine.Bindings;
using UnityEngine.Internal;

namespace UnityEngine
{
	[ExcludeFromDocs]
	[NativeHeader("Modules/Marshalling/MarshallingTests.h")]
	internal struct StructWithStringIntAndFloat
	{
		public override bool Equals(object other)
		{
			bool flag = other == null;
			bool flag2;
			if (flag)
			{
				flag2 = false;
			}
			else
			{
				bool flag3 = other is StructWithStringIntAndFloat;
				if (flag3)
				{
					StructWithStringIntAndFloat structWithStringIntAndFloat = (StructWithStringIntAndFloat)other;
					flag2 = this.a.Equals(structWithStringIntAndFloat.a) && this.b == structWithStringIntAndFloat.b && this.c == structWithStringIntAndFloat.c;
				}
				else
				{
					flag2 = false;
				}
			}
			return flag2;
		}

		public override int GetHashCode()
		{
			return this.a.GetHashCode();
		}

		public string a;

		public int b;

		public float c;
	}
}
