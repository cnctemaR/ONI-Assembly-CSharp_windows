using System;
using System.Security;

namespace System.IO
{
	internal class DirectoryInfoResultHandler : SearchResultHandler<DirectoryInfo>
	{
		[SecurityCritical]
		internal override bool IsResultIncluded(SearchResult result)
		{
			return FileSystemEnumerableHelpers.IsDir(result.FindData);
		}

		[SecurityCritical]
		internal override DirectoryInfo CreateObject(SearchResult result)
		{
			DirectoryInfo directoryInfo = new DirectoryInfo(result.FullPath, false);
			directoryInfo.InitializeFrom(result.FindData);
			return directoryInfo;
		}
	}
}
