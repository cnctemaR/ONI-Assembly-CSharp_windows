using System;
using Unity.Collections;

namespace UnityEngine
{
	internal struct TypeDispatchData : IDisposable
	{
		public void Dispose()
		{
			this.changed = null;
			this.changedID.Dispose();
			this.destroyedID.Dispose();
		}

		public Object[] changed;

		public NativeArray<int> changedID;

		public NativeArray<int> destroyedID;
	}
}
