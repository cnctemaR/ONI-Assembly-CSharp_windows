using System;
using System.Collections.Generic;

namespace System.Diagnostics.Tracing
{
	public class EventCommandEventArgs : EventArgs
	{
		private EventCommandEventArgs()
		{
		}

		public IDictionary<string, string> Arguments
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public EventCommand Command
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public bool DisableEvent(int eventId)
		{
			return true;
		}

		public bool EnableEvent(int eventId)
		{
			return true;
		}
	}
}
