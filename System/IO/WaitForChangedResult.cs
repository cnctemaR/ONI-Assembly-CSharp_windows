using System;

namespace System.IO
{
	public struct WaitForChangedResult
	{
		internal WaitForChangedResult(WatcherChangeTypes changeType, string name, string oldName, bool timedOut)
		{
			this.ChangeType = changeType;
			this.Name = name;
			this.OldName = oldName;
			this.TimedOut = timedOut;
		}

		public WatcherChangeTypes ChangeType { readonly get; set; }

		public string Name { readonly get; set; }

		public string OldName { readonly get; set; }

		public bool TimedOut { readonly get; set; }

		internal static readonly WaitForChangedResult TimedOutResult = new WaitForChangedResult((WatcherChangeTypes)0, null, null, true);
	}
}
