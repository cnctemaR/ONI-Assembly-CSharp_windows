using System;
using System.Runtime.Remoting.Channels;

namespace System.Runtime.Remoting
{
	[Serializable]
	internal class ChannelInfo : IChannelInfo
	{
		public ChannelInfo()
		{
			this.channelData = ChannelServices.GetCurrentChannelInfo();
		}

		public ChannelInfo(object remoteChannelData)
		{
			this.channelData = new object[] { remoteChannelData };
		}

		public object[] ChannelData
		{
			get
			{
				return this.channelData;
			}
			set
			{
				this.channelData = value;
			}
		}

		private object[] channelData;
	}
}
