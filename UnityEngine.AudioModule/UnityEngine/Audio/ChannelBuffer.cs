using System;

namespace UnityEngine.Audio
{
	[Obsolete("Types with embedded references are not supported in this version of your compiler.", true)]
	public ref struct ChannelBuffer
	{
		public int channelCount
		{
			get
			{
				return this.m_ChannelCount;
			}
		}

		public int frameCount
		{
			get
			{
				return this.m_FrameCount;
			}
		}

		public unsafe float this[int channel, int frame]
		{
			get
			{
				return *this.Buffer[frame * this.m_ChannelCount + channel];
			}
			set
			{
				*this.Buffer[frame * this.m_ChannelCount + channel] = value;
			}
		}

		public void Clear()
		{
			this.Buffer.Clear();
		}

		public ChannelBuffer(Span<float> buffer, int channels)
		{
			bool flag = channels < 1;
			if (flag)
			{
				throw new ArgumentException("channels must be positive and non-zero");
			}
			this.Buffer = buffer;
			this.m_ChannelCount = channels;
			this.m_FrameCount = buffer.Length / channels;
		}

		internal Span<float> Buffer;

		private int m_ChannelCount;

		private int m_FrameCount;
	}
}
