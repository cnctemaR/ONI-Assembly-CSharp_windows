using System;

namespace System.Net
{
	internal class NetConfig : ICloneable
	{
		internal NetConfig()
		{
		}

		object ICloneable.Clone()
		{
			return base.MemberwiseClone();
		}

		internal bool ipv6Enabled;

		internal int MaxResponseHeadersLength = 64;
	}
}
