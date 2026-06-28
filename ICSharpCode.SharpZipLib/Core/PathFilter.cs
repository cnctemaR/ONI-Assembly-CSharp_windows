using System;
using System.IO;

namespace ICSharpCode.SharpZipLib.Core
{
	public class PathFilter : IScanFilter
	{
		public PathFilter(string filter)
		{
			this.nameFilter_ = new NameFilter(filter);
		}

		public virtual bool IsMatch(string name)
		{
			bool flag = false;
			if (name != null)
			{
				string text = ((name.Length <= 0) ? string.Empty : Path.GetFullPath(name));
				flag = this.nameFilter_.IsMatch(text);
			}
			return flag;
		}

		private NameFilter nameFilter_;
	}
}
