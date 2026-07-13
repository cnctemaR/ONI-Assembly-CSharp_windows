using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Internal;

namespace UnityEngine
{
	[NativeHeader("Modules/Marshalling/MarshallingTests.h")]
	[ExcludeFromDocs]
	internal struct StructUnityObject
	{
		public int InstanceMethod([NotNull] object o)
		{
			if (o == null)
			{
				ThrowHelper.ThrowArgumentNullException(o, "o");
			}
			return StructUnityObject.InstanceMethod_Injected(ref this, o);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int InstanceMethod_Injected(ref StructUnityObject _unity_self, object o);

		public MarshallingTestObject field;
	}
}
