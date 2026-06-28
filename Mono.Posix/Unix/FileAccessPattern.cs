using System;

namespace Mono.Unix
{
	public enum FileAccessPattern
	{
		Normal,
		Sequential = 2,
		Random = 1,
		NoReuse = 5,
		PreLoad = 3,
		FlushCache
	}
}
