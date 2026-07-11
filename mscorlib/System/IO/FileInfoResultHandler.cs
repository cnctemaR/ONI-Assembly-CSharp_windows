using System;
using System.Security;

namespace System.IO
{
	internal class FileInfoResultHandler : SearchResultHandler<FileInfo>
	{
		[SecurityCritical]
		internal override bool IsResultIncluded(SearchResult result)
		{
			return FileSystemEnumerableHelpers.IsFile(result.FindData);
		}

		[SecurityCritical]
		internal override FileInfo CreateObject(SearchResult result)
		{
			FileInfo fileInfo = new FileInfo(result.FullPath, false);
			fileInfo.InitializeFrom(result.FindData);
			return fileInfo;
		}
	}
}
