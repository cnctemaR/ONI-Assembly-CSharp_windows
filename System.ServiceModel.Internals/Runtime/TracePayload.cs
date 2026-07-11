using System;

namespace System.Runtime
{
	internal struct TracePayload
	{
		public TracePayload(string serializedException, string eventSource, string appDomainFriendlyName, string extendedData, string hostReference)
		{
			this.serializedException = serializedException;
			this.eventSource = eventSource;
			this.appDomainFriendlyName = appDomainFriendlyName;
			this.extendedData = extendedData;
			this.hostReference = hostReference;
		}

		public string SerializedException
		{
			get
			{
				return this.serializedException;
			}
		}

		public string EventSource
		{
			get
			{
				return this.eventSource;
			}
		}

		public string AppDomainFriendlyName
		{
			get
			{
				return this.appDomainFriendlyName;
			}
		}

		public string ExtendedData
		{
			get
			{
				return this.extendedData;
			}
		}

		public string HostReference
		{
			get
			{
				return this.hostReference;
			}
		}

		private string serializedException;

		private string eventSource;

		private string appDomainFriendlyName;

		private string extendedData;

		private string hostReference;
	}
}
