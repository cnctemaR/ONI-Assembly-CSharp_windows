using System;

namespace ICSharpCode.SharpZipLib.Core
{
	public abstract class WindowsPathUtils
	{
		internal WindowsPathUtils()
		{
		}

		public static string DropPathRoot(string path)
		{
			string text = path;
			if (path != null && path.Length > 0)
			{
				if (path[0] == '\\' || path[0] == '/')
				{
					if (path.Length > 1 && (path[1] == '\\' || path[1] == '/'))
					{
						int num = 2;
						int num2 = 2;
						while (num <= path.Length && ((path[num] != '\\' && path[num] != '/') || --num2 > 0))
						{
							num++;
						}
						num++;
						if (num < path.Length)
						{
							text = path.Substring(num);
						}
						else
						{
							text = string.Empty;
						}
					}
				}
				else if (path.Length > 1 && path[1] == ':')
				{
					int num3 = 2;
					if (path.Length > 2 && (path[2] == '\\' || path[2] == '/'))
					{
						num3 = 3;
					}
					text = text.Remove(0, num3);
				}
			}
			return text;
		}
	}
}
