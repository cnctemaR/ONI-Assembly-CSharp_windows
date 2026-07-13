using System;

namespace UnityEngine.Bindings
{
	[VisibleToOtherModules]
	internal ref struct OutArrayNativeData
	{
		public IntPtr arrayRef;

		public IntPtr createAndCallback;

		public IntPtr createArray;
	}
}
