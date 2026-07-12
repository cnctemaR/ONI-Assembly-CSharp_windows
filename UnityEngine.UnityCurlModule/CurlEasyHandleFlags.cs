using System;

namespace Unity.Curl
{
	[Flags]
	internal enum CurlEasyHandleFlags : uint
	{
		kSendBody = 1U,
		kReceiveHeaders = 2U,
		kReceiveBody = 4U,
		kFollowRedirects = 8U
	}
}
