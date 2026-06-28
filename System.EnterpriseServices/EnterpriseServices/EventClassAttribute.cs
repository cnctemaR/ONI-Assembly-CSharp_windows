using System;
using System.Runtime.InteropServices;

namespace System.EnterpriseServices
{
	[AttributeUsage(AttributeTargets.Class)]
	[ComVisible(false)]
	public sealed class EventClassAttribute : Attribute
	{
		public EventClassAttribute()
		{
			this.allowInProcSubscribers = true;
			this.fireInParallel = false;
			this.publisherFilter = null;
		}

		public bool AllowInprocSubscribers
		{
			get
			{
				return this.allowInProcSubscribers;
			}
			set
			{
				this.allowInProcSubscribers = value;
			}
		}

		public bool FireInParallel
		{
			get
			{
				return this.fireInParallel;
			}
			set
			{
				this.fireInParallel = value;
			}
		}

		public string PublisherFilter
		{
			get
			{
				return this.publisherFilter;
			}
			set
			{
				this.publisherFilter = value;
			}
		}

		private bool allowInProcSubscribers;

		private bool fireInParallel;

		private string publisherFilter;
	}
}
