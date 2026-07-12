using System;

namespace FMODUnity
{
	[Serializable]
	public class CodecChannelCount
	{
		public CodecChannelCount()
		{
		}

		public CodecChannelCount(CodecChannelCount other)
		{
			this.format = other.format;
			this.channels = other.channels;
		}

		public CodecType format;

		public int channels;
	}
}
