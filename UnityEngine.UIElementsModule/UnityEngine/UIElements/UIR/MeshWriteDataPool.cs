using System;

namespace UnityEngine.UIElements.UIR
{
	internal class MeshWriteDataPool : ImplicitPool<MeshWriteData>
	{
		public MeshWriteDataPool()
			: base(MeshWriteDataPool.k_CreateAction, null, 100, 1000)
		{
		}

		private static readonly Func<MeshWriteData> k_CreateAction = () => new MeshWriteData();
	}
}
