using System;

namespace UnityEngine.AdaptivePerformance
{
	public interface IDevelopmentSettings
	{
		bool Logging { get; set; }

		int LoggingFrequencyInFrames { get; set; }
	}
}
