using System;
using System.Security;

namespace System.IO
{
	internal class FileSystemInfoResultHandler : SearchResultHandler<FileSystemInfo>
	{
		[SecurityCritical]
		internal override bool IsResultIncluded(SearchResult result)
		{
			bool flag = FileSystemEnumerableHelpers.IsFile(result.FindData);
			return FileSystemEnumerableHelpers.IsDir(result.FindData) || flag;
		}

		[SecurityCritical]
		internal override FileSystemInfo CreateObject(SearchResult result)
		{
			FileSystemEnumerableHelpers.IsFile(result.FindData);
			if (FileSystemEnumerableHelpers.IsDir(result.FindData))
			{
				DirectoryInfo directoryInfo = new DirectoryInfo(result.FullPath, false);
				directoryInfo.InitializeFrom(result.FindData);
				return directoryInfo;
			}
			FileInfo fileInfo = new FileInfo(result.FullPath, false);
			fileInfo.InitializeFrom(result.FindData);
			return fileInfo;
		}
	}
}
