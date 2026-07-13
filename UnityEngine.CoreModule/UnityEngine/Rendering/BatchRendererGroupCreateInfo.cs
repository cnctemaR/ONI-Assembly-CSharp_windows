using System;

namespace UnityEngine.Rendering
{
	public struct BatchRendererGroupCreateInfo
	{
		public BatchRendererGroup.OnPerformCulling cullingCallback;

		public BatchRendererGroup.OnFinishedCulling finishedCullingCallback;

		public IntPtr userContext;
	}
}
