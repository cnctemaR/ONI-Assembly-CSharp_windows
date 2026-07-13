using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Unity.Multiplayer.PlayMode
{
	internal class CurrentPlayerApi
	{
		public virtual bool IsMainEditor
		{
			get
			{
				return false;
			}
		}

		protected void SetTags(IEnumerable<string> tags)
		{
			this.m_Tags.Clear();
			bool flag = tags != null;
			if (flag)
			{
				this.m_Tags.AddRange(tags);
			}
		}

		public virtual IReadOnlyList<string> ReadOnlyTags()
		{
			return this.m_Tags.AsReadOnly();
		}

		public virtual void ReportResult(bool condition, string message = "", [CallerFilePath] string callingFilePath = "", [CallerLineNumber] int lineNumber = 0)
		{
		}

		private List<string> m_Tags = new List<string>();
	}
}
