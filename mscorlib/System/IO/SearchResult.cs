using System;
using System.Security;
using Microsoft.Win32;

namespace System.IO
{
	internal sealed class SearchResult
	{
		[SecurityCritical]
		internal SearchResult(string fullPath, string userPath, Win32Native.WIN32_FIND_DATA findData)
		{
			this.fullPath = fullPath;
			this.userPath = userPath;
			this.findData = findData;
		}

		internal string FullPath
		{
			get
			{
				return this.fullPath;
			}
		}

		internal string UserPath
		{
			get
			{
				return this.userPath;
			}
		}

		internal Win32Native.WIN32_FIND_DATA FindData
		{
			[SecurityCritical]
			get
			{
				return this.findData;
			}
		}

		private string fullPath;

		private string userPath;

		[SecurityCritical]
		private Win32Native.WIN32_FIND_DATA findData;
	}
}
