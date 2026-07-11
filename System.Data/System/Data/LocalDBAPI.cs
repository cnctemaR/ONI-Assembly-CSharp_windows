using System;
using System.Runtime.InteropServices;
using System.Text;

namespace System.Data
{
	internal static class LocalDBAPI
	{
		internal static string GetLocalDBMessage(int hrCode)
		{
			throw new PlatformNotSupportedException("LocalDB is not supported on this platform.");
		}

		internal static string GetLocalDbInstanceNameFromServerName(string serverName)
		{
			if (serverName == null)
			{
				return null;
			}
			serverName = serverName.TrimStart(Array.Empty<char>());
			if (!serverName.StartsWith("(localdb)\\", StringComparison.OrdinalIgnoreCase))
			{
				return null;
			}
			string text = serverName.Substring("(localdb)\\".Length).Trim();
			if (text.Length == 0)
			{
				return null;
			}
			return text;
		}

		private const string const_localDbPrefix = "(localdb)\\";

		[UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
		private delegate int LocalDBFormatMessageDelegate(int hrLocalDB, uint dwFlags, uint dwLanguageId, StringBuilder buffer, ref uint buflen);
	}
}
