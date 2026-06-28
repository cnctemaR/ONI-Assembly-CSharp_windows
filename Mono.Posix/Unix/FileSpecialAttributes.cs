using System;

namespace Mono.Unix
{
	[Flags]
	public enum FileSpecialAttributes
	{
		SetUserId = 2048,
		SetGroupId = 1024,
		Sticky = 512
	}
}
