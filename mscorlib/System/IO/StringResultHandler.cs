using System;
using System.Security;

namespace System.IO
{
	internal class StringResultHandler : SearchResultHandler<string>
	{
		internal StringResultHandler(bool includeFiles, bool includeDirs)
		{
			this._includeFiles = includeFiles;
			this._includeDirs = includeDirs;
		}

		[SecurityCritical]
		internal override bool IsResultIncluded(SearchResult result)
		{
			bool flag = this._includeFiles && FileSystemEnumerableHelpers.IsFile(result.FindData);
			bool flag2 = this._includeDirs && FileSystemEnumerableHelpers.IsDir(result.FindData);
			return flag || flag2;
		}

		[SecurityCritical]
		internal override string CreateObject(SearchResult result)
		{
			return result.UserPath;
		}

		private bool _includeFiles;

		private bool _includeDirs;
	}
}
