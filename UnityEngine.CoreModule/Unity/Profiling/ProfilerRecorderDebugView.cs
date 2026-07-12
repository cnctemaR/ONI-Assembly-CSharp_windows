using System;

namespace Unity.Profiling
{
	internal sealed class ProfilerRecorderDebugView
	{
		public ProfilerRecorderDebugView(ProfilerRecorder r)
		{
			this.m_Recorder = r;
		}

		public ProfilerRecorderSample[] Items
		{
			get
			{
				return this.m_Recorder.ToArray();
			}
		}

		private ProfilerRecorder m_Recorder;
	}
}
