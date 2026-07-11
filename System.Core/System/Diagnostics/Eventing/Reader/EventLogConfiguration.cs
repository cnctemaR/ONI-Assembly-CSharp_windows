using System;
using System.Collections.Generic;
using System.Security;
using System.Security.Permissions;
using Unity;

namespace System.Diagnostics.Eventing.Reader
{
	[HostProtection(SecurityAction.LinkDemand, MayLeakOnAbort = true)]
	public class EventLogConfiguration : IDisposable
	{
		public EventLogConfiguration(string logName)
		{
			global::Unity.ThrowStub.ThrowNotSupportedException();
		}

		[SecurityCritical]
		public EventLogConfiguration(string logName, EventLogSession session)
		{
			global::Unity.ThrowStub.ThrowNotSupportedException();
		}

		public bool IsClassicLog
		{
			get
			{
				global::Unity.ThrowStub.ThrowNotSupportedException();
				return default(bool);
			}
		}

		public bool IsEnabled
		{
			get
			{
				global::Unity.ThrowStub.ThrowNotSupportedException();
				return default(bool);
			}
			set
			{
				global::Unity.ThrowStub.ThrowNotSupportedException();
			}
		}

		public string LogFilePath
		{
			get
			{
				global::Unity.ThrowStub.ThrowNotSupportedException();
				return null;
			}
			set
			{
				global::Unity.ThrowStub.ThrowNotSupportedException();
			}
		}

		public EventLogIsolation LogIsolation
		{
			get
			{
				global::Unity.ThrowStub.ThrowNotSupportedException();
				return EventLogIsolation.Application;
			}
		}

		public EventLogMode LogMode
		{
			get
			{
				global::Unity.ThrowStub.ThrowNotSupportedException();
				return EventLogMode.Circular;
			}
			set
			{
				global::Unity.ThrowStub.ThrowNotSupportedException();
			}
		}

		public string LogName
		{
			get
			{
				global::Unity.ThrowStub.ThrowNotSupportedException();
				return null;
			}
		}

		public EventLogType LogType
		{
			get
			{
				global::Unity.ThrowStub.ThrowNotSupportedException();
				return EventLogType.Administrative;
			}
		}

		public long MaximumSizeInBytes
		{
			get
			{
				global::Unity.ThrowStub.ThrowNotSupportedException();
				return 0L;
			}
			set
			{
				global::Unity.ThrowStub.ThrowNotSupportedException();
			}
		}

		public string OwningProviderName
		{
			get
			{
				global::Unity.ThrowStub.ThrowNotSupportedException();
				return null;
			}
		}

		public int? ProviderBufferSize
		{
			get
			{
				global::Unity.ThrowStub.ThrowNotSupportedException();
				return null;
			}
		}

		public Guid? ProviderControlGuid
		{
			get
			{
				global::Unity.ThrowStub.ThrowNotSupportedException();
				return null;
			}
		}

		public long? ProviderKeywords
		{
			get
			{
				global::Unity.ThrowStub.ThrowNotSupportedException();
				return null;
			}
			set
			{
				global::Unity.ThrowStub.ThrowNotSupportedException();
			}
		}

		public int? ProviderLatency
		{
			get
			{
				global::Unity.ThrowStub.ThrowNotSupportedException();
				return null;
			}
		}

		public int? ProviderLevel
		{
			get
			{
				global::Unity.ThrowStub.ThrowNotSupportedException();
				return null;
			}
			set
			{
				global::Unity.ThrowStub.ThrowNotSupportedException();
			}
		}

		public int? ProviderMaximumNumberOfBuffers
		{
			get
			{
				global::Unity.ThrowStub.ThrowNotSupportedException();
				return null;
			}
		}

		public int? ProviderMinimumNumberOfBuffers
		{
			get
			{
				global::Unity.ThrowStub.ThrowNotSupportedException();
				return null;
			}
		}

		public IEnumerable<string> ProviderNames
		{
			get
			{
				global::Unity.ThrowStub.ThrowNotSupportedException();
				return 0;
			}
		}

		public string SecurityDescriptor
		{
			get
			{
				global::Unity.ThrowStub.ThrowNotSupportedException();
				return null;
			}
			set
			{
				global::Unity.ThrowStub.ThrowNotSupportedException();
			}
		}

		public void Dispose()
		{
			global::Unity.ThrowStub.ThrowNotSupportedException();
		}

		[SecuritySafeCritical]
		protected virtual void Dispose(bool disposing)
		{
			global::Unity.ThrowStub.ThrowNotSupportedException();
		}

		public void SaveChanges()
		{
			global::Unity.ThrowStub.ThrowNotSupportedException();
		}
	}
}
