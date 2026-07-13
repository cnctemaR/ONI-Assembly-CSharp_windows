using System;
using Unity.Collections;

namespace UnityEngine.Rendering
{
	public struct ShadowCastersCullingInfos
	{
		public NativeArray<ShadowSplitData> splitBuffer;

		public NativeArray<LightShadowCasterCullingInfo> perLightInfos;
	}
}
