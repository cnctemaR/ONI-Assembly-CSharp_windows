using System;

namespace System
{
	internal static class MonoUtil
	{
		static MonoUtil()
		{
			int platform = (int)Environment.OSVersion.Platform;
			MonoUtil.IsUnix = platform == 4 || platform == 128 || platform == 6;
		}

		public static readonly bool IsUnix;
	}
}
