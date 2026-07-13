using System;

namespace UnityEngine.Rendering
{
	public interface IRenderPipelineGraphicsSettings
	{
		int version { get; }

		bool isAvailableInPlayerBuild
		{
			get
			{
				return false;
			}
		}

		void Reset()
		{
		}
	}
}
