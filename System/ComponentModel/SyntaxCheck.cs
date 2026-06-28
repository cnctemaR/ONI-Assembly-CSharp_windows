using System;
using System.IO;

namespace System.ComponentModel
{
	public static class SyntaxCheck
	{
		public static bool CheckMachineName(string value)
		{
			return value != null && value.Trim().Length != 0 && value.IndexOf('\\') == -1;
		}

		public static bool CheckPath(string value)
		{
			return value != null && value.Trim().Length != 0 && value.StartsWith("\\\\");
		}

		public static bool CheckRootedPath(string value)
		{
			return value != null && value.Trim().Length != 0 && Path.IsPathRooted(value);
		}
	}
}
