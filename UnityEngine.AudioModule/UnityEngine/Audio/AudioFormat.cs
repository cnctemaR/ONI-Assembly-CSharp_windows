using System;

namespace UnityEngine.Audio
{
	public struct AudioFormat
	{
		public AudioFormat(AudioConfiguration config)
		{
			this.m_Config = config;
		}

		public AudioFormat(AudioSpeakerMode speakerMode, int sampleRate, int bufferSize)
		{
			this.m_Config = new AudioConfiguration
			{
				sampleRate = sampleRate,
				dspBufferSize = bufferSize,
				speakerMode = speakerMode
			};
		}

		public readonly int channelCount
		{
			get
			{
				return this.m_Config.speakerMode.ChannelCount();
			}
		}

		public readonly int bufferFrameCount
		{
			get
			{
				return this.m_Config.dspBufferSize;
			}
		}

		public readonly int sampleRate
		{
			get
			{
				return this.m_Config.sampleRate;
			}
		}

		public readonly AudioSpeakerMode speakerMode
		{
			get
			{
				return this.m_Config.speakerMode;
			}
		}

		internal readonly AudioConfiguration audioConfiguration
		{
			get
			{
				return this.m_Config;
			}
		}

		private AudioConfiguration m_Config;
	}
}
