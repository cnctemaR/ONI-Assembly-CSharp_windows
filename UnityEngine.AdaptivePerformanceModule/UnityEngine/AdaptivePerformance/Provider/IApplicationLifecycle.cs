using System;

namespace UnityEngine.AdaptivePerformance.Provider
{
	public interface IApplicationLifecycle
	{
		void ApplicationPause();

		void ApplicationResume();
	}
}
