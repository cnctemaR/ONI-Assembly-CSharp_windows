using System;
using System.Runtime.InteropServices;

namespace System.EnterpriseServices
{
	[ComVisible(false)]
	[AttributeUsage(AttributeTargets.Assembly)]
	public sealed class ApplicationQueuingAttribute : Attribute
	{
		public ApplicationQueuingAttribute()
		{
			this.enabled = true;
			this.queueListenerEnabled = false;
			this.maxListenerThreads = 0;
		}

		public bool Enabled
		{
			get
			{
				return this.enabled;
			}
			set
			{
				this.enabled = value;
			}
		}

		public int MaxListenerThreads
		{
			get
			{
				return this.maxListenerThreads;
			}
			set
			{
				this.maxListenerThreads = value;
			}
		}

		public bool QueueListenerEnabled
		{
			get
			{
				return this.queueListenerEnabled;
			}
			set
			{
				this.queueListenerEnabled = value;
			}
		}

		private bool enabled;

		private int maxListenerThreads;

		private bool queueListenerEnabled;
	}
}
