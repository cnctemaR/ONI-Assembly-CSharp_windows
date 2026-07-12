using System;

namespace UnityEngine.UIElements.UIR
{
	internal class VectorImageRenderInfoPool : LinkedPool<VectorImageRenderInfo>
	{
		public VectorImageRenderInfoPool()
			: base(() => new VectorImageRenderInfo(), delegate(VectorImageRenderInfo vectorImageInfo)
			{
				vectorImageInfo.Reset();
			}, 10000)
		{
		}
	}
}
